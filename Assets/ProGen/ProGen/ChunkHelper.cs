using System;

namespace ProGen
{
    public class ChunkBlockHelper
    {
        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public bool HasZ { get; }

        public short Material { get; }


        private ChunkBlock block;

        public ChunkBlockHelper(ChunkBlock block, bool hasZ)
        {
            this.block = block;
            X = block.Position[0];
            Y = block.Position[1];
            if (hasZ)
            {
                HasZ = hasZ;
                Z = block.Position[2];
            }
            Material = block.Material;
        }
    }

    public class ChunkHelper
    {
        private Chunk chunk;

        public int X { get; }

        public int Y { get; }

        public int Z { get; }

        public bool HasZ { get; }

        public short Width { get; }

        public short Height { get; }

        public short Length { get; }

        private short[] pos;

        public ChunkHelper(Chunk chunk)
        {
            this.chunk = chunk;
            X = chunk.Position[0];
            Y = chunk.Position[1];
            Width = chunk.Size[0];
            Height = chunk.Size[1];
            HasZ = chunk.Position.Length > 2;
            if (HasZ)
            {
                Z = chunk.Position[2];
                Length = chunk.Size[2];
            }
            pos = new short[chunk.Size.Length];
        }

        public void Set(short x, short y, short z, short material)
        {
            if (!HasZ)
            {
                throw new System.Exception("chunk has less than three dimensions");
            }
            pos[0] = x;
            pos[1] = y;
            pos[2] = z;
            chunk.Set(pos, material);
        }

        public void Set(short x, short y, short material)
        {
            pos[0] = x;
            pos[1] = y;
            chunk.Set(pos, material);
        }

        public short Get(short x, short y)
        {
            pos[0] = x;
            pos[1] = y;
            return chunk.Get(pos);
        }

        public short Get(short x, short y, short z)
        {
            if (!HasZ)
            {
                throw new System.Exception("chunk has less than three dimensions");
            }
            pos[0] = x;
            pos[1] = y;
            pos[2] = z;
            return chunk.Get(pos);
        }
    }
}