using System;

namespace ProGen
{
    public interface IGenerationLayer : ICloneable
    {
        int GetSeed();

        void SetSeed(int seed);

        void Generate(Chunk chunk);
    }

    public abstract class BaseGenerationLayer : IGenerationLayer
    {
        protected int seed = 1337;

        public virtual int GetSeed()
        {
            return seed;
        }

        public virtual void SetSeed(int seed)
        {
            this.seed = seed;
        }

        public abstract object Clone();

        public abstract void Generate(Chunk chunk);
    }
}
