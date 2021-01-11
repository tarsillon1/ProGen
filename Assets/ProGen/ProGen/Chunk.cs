using System;
using System.Collections.Generic;

namespace ProGen
{
    public class ChunkBlock
    {
        public int[] Position { get; }

        public short Material { get; }

        public ChunkBlock(int[] pos, short material)
        {
            Position = pos;
            Material = material;
        }
    }

    public class Chunk
    {
        public int[] Position { get; }

        public short[] Size { get; }

        private object[] map;

        private Dictionary<int, object> indexes;

        private int lastDimIndex;

        private int lastArrayIndex;

        public Chunk(int[] chunkPos, short[] dim)
        {
            Position = chunkPos;
            Size = dim;
            if (Position.Length != Size.Length)
            {
                throw new System.Exception("chunk position array length must equal dimension array length");
            }
            indexes = new Dictionary<int, object>();
            map = new object[Size[0]];
            lastDimIndex = Size.Length - 1;
            lastArrayIndex = Size.Length - 2;
        }

        public void Set(short[] blockPos, short material)
        {
            object[] current = map;
            bool isEmpty = material == 0;
            for (int dim = 0; dim < lastDimIndex; dim++)
            {
                int index = blockPos[dim];
                bool isNextLastArray = dim == lastArrayIndex;
                if (isNextLastArray)
                {
                    short[] found = (short[])current[index];
                    if (found == null)
                    {
                        found = new short[Size[lastDimIndex]];
                        current[index] = found;
                    }
                    found[blockPos[lastDimIndex]] = material;
                }
                else
                {
                    object[] next = (object[])current[index];
                    if (next == null)
                    {
                        next = new object[Size[dim + 1]];
                        current[index] = next;
                    }
                    current = next;
                }
            }
        }

        public short Get(short[] blockPos)
        {
            object[] current = map;
            for (int dim = 0; dim < lastDimIndex; dim++)
            {
                int index = blockPos[dim];
                bool isNextLastArray = dim == lastArrayIndex;
                if (isNextLastArray)
                {
                    short[] next = (short[])current[index];
                    if (next == null)
                    {
                        return 0;
                    }
                    return next[blockPos[lastDimIndex]];
                }
                else
                {
                    current = (object[])current[index];
                    if (current == null)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }
    }
}