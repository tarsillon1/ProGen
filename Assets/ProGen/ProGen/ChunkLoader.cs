using System.Threading;
using System.Collections.Generic;

namespace ProGen
{
    public class ChunkLoadResponse
    {
        public bool Loaded { get; }

        public int CountGenerated { get; }

        public int CountDestroyed { get; }

        public ChunkLoadResponse(bool loaded, int countGenerated, int countDestroyed)
        {
            this.Loaded = loaded;
            this.CountGenerated = countGenerated;
            this.CountDestroyed = countDestroyed;
        }
    }

    public class ChunkLoader
    {

        private short[] count;

        public short[] Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                UpdatePrecalculatedValues();
            }
        }

        public short[] Size { get; set; }

        public short MaxPendingTasks { get; set; }

        public IChunkCache Loaded { set; get; }

        public IGenerationLayerCollection Collection { get; set; }

        private ManualResetEvent ev = new ManualResetEvent(false);

        private int currentPendingTasks = 0;

        private int currentLoadID = 0;

        private bool isLoading = false;

        private int[] dimFactors;

        private int countTotal;

        public ChunkLoader()
        {
            Loaded = new InMemoryChunkCache();
            MaxPendingTasks = 32;
        }

        private int GetLoadID()
        {
            return Interlocked.Increment(ref currentLoadID);
        }

        private void WaitForPendingTasks()
        {
            ev.WaitOne();
            ev = new ManualResetEvent(false);
        }

        private async void Generate(Chunk chunk)
        {
            await new WaitForBackgroundThread();
            ChunkGenerator generator = new ChunkGenerator();
            foreach (IGenerationLayer layer in Collection.GetLayers())
            {
                generator.AddLayer((IGenerationLayer)layer.Clone());
            }
            generator.Generate(chunk);
            Loaded.Set(chunk);
            if (Interlocked.Decrement(ref currentPendingTasks) == 0)
            {
                ev.Set();
            }
        }

        private void UpdatePrecalculatedValues()
        {
            countTotal = 1;
            dimFactors = new int[Count.Length];
            for (int dim = 0; dim < Count.Length; dim++)
            {
                int[] minMax = new int[2];
                minMax[1] = (Count[dim] / 2) + 1;
                minMax[0] = -minMax[1];
                countTotal *= (Count[dim] * 2) + 1;
                int dimFactor = 0;
                for (int otherDim = dim + 1; otherDim < Count.Length; otherDim++)
                {
                    if (dimFactor == 0)
                    {
                        dimFactor = (Count[otherDim] * 2) + 1;
                    }
                    else
                    {
                        dimFactor *= (Count[otherDim] * 2) + 1;
                    }
                }
                dimFactors[dim] = dimFactor;
            }
        }

        public ChunkLoadResponse Load(int[] pos)
        {
            isLoading = true;

            int loadID = GetLoadID();

            if (currentPendingTasks > 0)
            {
                WaitForPendingTasks();
            }

            int countGenerated = 0;

            HashSet<ChunkKey> destroyKeys = new HashSet<ChunkKey>();
            foreach (ChunkKey key in Loaded.GetKeys())
            {
                destroyKeys.Add(key);
            }

            for (int i = 0; i < countTotal; i++)
            {
                int[] chunkPos = new int[Count.Length];
                for (int dim = 0; dim < Count.Length; dim++)
                {
                    chunkPos[dim] = i;
                    for (int otherDim = dim - 1; otherDim >= 0; otherDim--)
                    {
                        chunkPos[dim] -= chunkPos[otherDim] * dimFactors[otherDim];
                    }
                    int factor = dimFactors[dim];
                    if (factor != 0)
                    {
                        chunkPos[dim] /= factor;
                    }
                }

                for (int dim = 0; dim < Count.Length; dim++)
                {
                    chunkPos[dim] -= Count[dim];
                    chunkPos[dim] += pos[dim];
                }

                Chunk chunk;
                ChunkKey key = new ChunkKey(chunkPos);
                bool loaded = Loaded.Get(key, out chunk);
                if (!loaded)
                {
                    if (loadID != currentLoadID)
                    {
                        return new ChunkLoadResponse(false, countGenerated, 0);
                    }
                    countGenerated++;
                    Interlocked.Increment(ref currentPendingTasks);
                    Generate(new Chunk(chunkPos, Size));
                }
                else
                {
                    destroyKeys.Remove(key);
                }
                if (currentPendingTasks > MaxPendingTasks)
                {
                    WaitForPendingTasks();
                }
            }

            if (currentPendingTasks > 0)
            {
                WaitForPendingTasks();
            }

            int countDestroyed = 0;
            foreach (ChunkKey key in destroyKeys)
            {
                bool removed = Loaded.Remove(key);
                if (!removed)
                {
                    Chunk c;
                    bool gotIt = Loaded.Get(key, out c);
                    if (gotIt)
                    {
                        throw new System.Exception("wtf2");

                    }
                    foreach (ChunkKey k in Loaded.GetKeys())
                    {
                        if (k.Equals(key))
                        {
                            throw new System.Exception("wtf");
                        }
                    }
                    throw new System.Exception("failed to remove chunk from loaded cache: " + Utils.EncodePosition(key.Position));
                }
                countDestroyed++;
            }

            isLoading = false;
            return new ChunkLoadResponse(true, countGenerated, countDestroyed);
        }

        public bool IsLoading()
        {
            return isLoading;
        }
    }
}