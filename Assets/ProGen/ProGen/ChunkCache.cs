using System;
using System.Threading;
using System.Collections.Generic;

namespace ProGen
{

    public interface IChunkCache
    {
        IEnumerable<ChunkKey> GetKeys();

        ChunkKey Set(Chunk chunk);

        bool Get(ChunkKey key, out Chunk value);

        bool Remove(ChunkKey key);
    }

    public class ChunkKey : IEquatable<ChunkKey>
    {
        public int[] Position { get; }

        public ChunkKey(int[] pos)
        {
            Position = pos;
        }

        public bool Equals(ChunkKey y)
        {
            if (Position.Length != y.Position.Length)
            {
                return false;
            }
            for (int i = 0; i < Position.Length; i++)
            {
                int posX = Position[i];
                int posY = y.Position[i];
                if (!posX.Equals(posY))
                {
                    return false;
                }
            }
            return true;
        }


        public override bool Equals(object y)
        {
            return Equals((ChunkKey)y);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (int i in Position)
            {
                hash = hash * 23 + i.GetHashCode();
            }
            return hash;
        }
    }

    public class InMemoryChunkCache : IChunkCache
    {
        private Mutex mutex = new Mutex();
        private Dictionary<ChunkKey, Chunk> chunks = new Dictionary<ChunkKey, Chunk>();

        public IEnumerable<ChunkKey> GetKeys()
        {
            mutex.WaitOne();
            foreach (ChunkKey key in chunks.Keys)
            {
                yield return key;
            }
            mutex.ReleaseMutex();
        }

        public ChunkKey Set(Chunk chunk)
        {
            mutex.WaitOne();
            ChunkKey key = new ChunkKey(chunk.Position);
            chunks.Add(key, chunk);
            mutex.ReleaseMutex();
            return key;
        }

        public bool Get(ChunkKey key, out Chunk value)
        {
            mutex.WaitOne();
            bool gotVal = chunks.TryGetValue(key, out value);
            mutex.ReleaseMutex();
            return gotVal;
        }

        public bool Remove(ChunkKey key)
        {
            mutex.WaitOne();
            bool removed = chunks.Remove(key);
            mutex.ReleaseMutex();
            return removed;
        }
    }
}