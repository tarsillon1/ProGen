using UnityEngine;

using System.Diagnostics;
using System.Collections.Generic;

namespace ProGen.Unity
{
    public class ChunkLoader3D : MonoBehaviour
    {
        public short numChunkX;

        public short numChunkY;

        public short numChunkZ;

        public short chunkWidth;

        public short chunkLength;

        public short chunkHeight;

        public GameObject chunkPrefab;

        public MaterialCollectionComponent materials;

        public GenerationLayerCollectionComponent layers;

        private Mesh3D mesh;

        private int[] lastCenterChunk;

        private ChunkLoader chunkLoader = new ChunkLoader();

        private HashSet<ChunkKey> drawnChunks;

        void Start()
        {
            chunkLoader.Collection = layers.Collection;
            chunkLoader.Count = new short[] { numChunkX, numChunkY, numChunkZ };
            chunkLoader.Size = new short[] { chunkWidth, chunkHeight, chunkLength };

            drawnChunks = new HashSet<ChunkKey>();

            Vector3 pos = new Vector3(0, 0, 0);
            GameObject chunkObj = Instantiate(chunkPrefab, pos, Quaternion.identity);
            mesh = chunkObj.GetComponent<Mesh3D>();
            mesh.Materials = materials.ToArray();
        }

        private int[] GetCenterChunk()
        {
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;
            int z = (int)transform.position.z;

            int chunkX = x / chunkWidth;
            int chunkY = y / chunkHeight;
            int chunkZ = z / chunkLength;

            return new int[] { chunkX, chunkY, chunkZ };
        }

        async void Load()
        {
            await new WaitForBackgroundThread();
            ConsoleLogger.Debug("loading new chunks...");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ChunkLoadResponse res = chunkLoader.Load(lastCenterChunk);
            watch.Stop();
            if (res.Loaded)
            {
                ConsoleLogger.Debug("destroying " + res.CountDestroyed + " and generating " + res.CountGenerated + " chunks took " + watch.ElapsedMilliseconds + "ms");
                EraseChunks();
                DrawChunks();
                await new WaitForUpdate();
                mesh.Render();
            }
        }

        void Update()
        {
            int[] currentCenterChunk = GetCenterChunk();
            if (lastCenterChunk == null ||
                lastCenterChunk[0] != currentCenterChunk[0] ||
                lastCenterChunk[1] != currentCenterChunk[1] ||
                lastCenterChunk[2] != currentCenterChunk[2])
            {
                lastCenterChunk = currentCenterChunk;
                Load();
            }
        }

        void EraseChunks()
        {
            foreach (ChunkKey key in drawnChunks)
            {
                Chunk chunk;
                if (!chunkLoader.Loaded.Get(key, out chunk))
                {
                    EraseChunk(key.Position[0], key.Position[1], key.Position[2]);
                }
            }
        }

        bool DrawChunks()
        {
            foreach (ChunkKey key in chunkLoader.Loaded.GetKeys())
            {
                if (!drawnChunks.Contains(key))
                {
                    drawnChunks.Add(key);
                    DrawChunk(key.Position);
                }
            }
            return true;
        }

        void DrawChunk(int[] chunkPos)
        {
            Chunk chunk;
            Chunk eastChunk;
            Chunk westChunk;
            Chunk northChunk;
            Chunk southChunk;
            Chunk topChunk;
            Chunk bottomChunk;

            int chunkX = chunkPos[0];
            int chunkY = chunkPos[1];
            int chunkZ = chunkPos[2];

            int offsetX = chunkX * chunkWidth;
            int offsetY = chunkY * chunkHeight;
            int offsetZ = chunkZ * chunkLength;

            ChunkKey key = new ChunkKey(chunkPos);
            short[] chunkBlockPos = new short[3];
            bool isLoaded = chunkLoader.Loaded.Get(key, out chunk);
            if (!isLoaded)
            {
                return;
            }
            chunkPos[0] = chunkX + 1;
            chunkLoader.Loaded.Get(key, out eastChunk);
            chunkPos[0] = chunkX - 1;
            chunkLoader.Loaded.Get(key, out westChunk);
            chunkPos[0] = chunkX;
            chunkPos[1] = chunkY + 1;
            chunkLoader.Loaded.Get(key, out topChunk);
            chunkPos[1] = chunkY - 1;
            chunkLoader.Loaded.Get(key, out bottomChunk);
            chunkPos[1] = chunkY;
            chunkPos[2] = chunkZ + 1;
            chunkLoader.Loaded.Get(key, out northChunk);
            chunkPos[2] = chunkZ - 1;
            chunkLoader.Loaded.Get(key, out southChunk);

            Mesh3DDrawInput input = new Mesh3DDrawInput();
            for (short posX = 0; posX < chunkWidth; posX++)
            {
                for (short posY = 0; posY < chunkHeight; posY++)
                {
                    for (short posZ = 0; posZ < chunkLength; posZ++)
                    {
                        chunkBlockPos[0] = posX;
                        chunkBlockPos[1] = posY;
                        chunkBlockPos[2] = posZ;
                        input.Material = chunk.Get(chunkBlockPos);
                        if (input.Material == (short)SupplementalBlock.Air)
                        {
                            continue;
                        }

                        bool draw = false;

                        short westValue = -1;
                        if (posX > 0)
                        {
                            chunkBlockPos[0] = (short)(posX - 1);
                            westValue = chunk.Get(chunkBlockPos);
                        }
                        else if (westChunk != null)
                        {
                            chunkBlockPos[0] = (short)(chunkWidth - 1);
                            westValue = westChunk.Get(chunkBlockPos);
                        }
                        if (westValue == (int)SupplementalBlock.Air)
                        {
                            input.IsWestFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsWestFaceShowing = false;
                        }

                        short eastValue = -1;
                        if (posX + 1 < chunkWidth)
                        {
                            chunkBlockPos[0] = (short)(posX + 1);
                            eastValue = chunk.Get(chunkBlockPos);
                        }
                        else if (eastChunk != null)
                        {
                            chunkBlockPos[0] = 0;
                            eastValue = eastChunk.Get(chunkBlockPos);
                        }
                        if (eastValue == (short)SupplementalBlock.Air)
                        {
                            input.IsEastFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsEastFaceShowing = false;
                        }

                        chunkBlockPos[0] = posX;
                        short bottomValue = -1;
                        if (posY > 0)
                        {
                            chunkBlockPos[1] = (short)(posY - 1);
                            bottomValue = chunk.Get(chunkBlockPos);
                        }
                        else if (bottomChunk != null)
                        {
                            chunkBlockPos[1] = (short)(chunkHeight - 1);
                            bottomValue = bottomChunk.Get(chunkBlockPos);
                        }
                        if (bottomValue == (short)SupplementalBlock.Air)
                        {
                            input.IsBottomFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsBottomFaceShowing = false;
                        }

                        short topValue = -1;
                        if (posY + 1 < chunkHeight)
                        {
                            chunkBlockPos[1] = (short)(posY + 1);
                            topValue = chunk.Get(chunkBlockPos);
                        }
                        else if (topChunk != null)
                        {
                            chunkBlockPos[1] = 0;
                            topValue = topChunk.Get(chunkBlockPos);
                        }
                        if (topValue == (short)SupplementalBlock.Air)
                        {
                            input.IsTopFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsTopFaceShowing = false;
                        }

                        chunkBlockPos[1] = posY;
                        short southValue = -1;
                        if (posZ > 0)
                        {
                            chunkBlockPos[2] = (short)(posZ - 1);
                            southValue = chunk.Get(chunkBlockPos);
                        }
                        else if (southChunk != null)
                        {
                            chunkBlockPos[2] = (short)(chunkLength - 1);
                            southValue = southChunk.Get(chunkBlockPos);
                        }
                        if (southValue == (short)SupplementalBlock.Air)
                        {
                            input.IsSouthFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsSouthFaceShowing = false;
                        }

                        short northValue = -1;
                        if (posZ + 1 < chunkLength)
                        {
                            chunkBlockPos[2] = (short)(posZ + 1);
                            northValue = chunk.Get(chunkBlockPos);
                        }
                        else if (northChunk != null)
                        {
                            chunkBlockPos[2] = 0;
                            northValue = northChunk.Get(chunkBlockPos);
                        }
                        if (northValue == (short)SupplementalBlock.Air)
                        {
                            input.IsNorthFaceShowing = true;
                            draw = true;
                        }
                        else
                        {
                            input.IsNorthFaceShowing = false;
                        }

                        if (draw)
                        {
                            input.X = posX + offsetX;
                            input.Y = posY + offsetY;
                            input.Z = posZ + offsetZ;
                            mesh.Draw(input);
                        }
                    }
                }
            }
        }

        void EraseChunk(int x, int y, int z)
        {
            Mesh3DEraseInput input = new Mesh3DEraseInput();
            input.FromX = x * chunkWidth;
            input.ToX = input.FromX + chunkWidth;
            input.FromY = y * chunkHeight;
            input.ToY = input.FromY + chunkHeight;
            input.FromZ = z * chunkLength;
            input.ToZ = input.FromZ + chunkLength;
            mesh.Erase(input);
        }
    }
}