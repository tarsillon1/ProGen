using System.Collections.Generic;

namespace ProGen
{
    public interface IGenerationLayerCollection
    {
        void ClearLayers();
        void AddLayer(IGenerationLayer layer);
        void RemoveLayer(IGenerationLayer layer);

        IList<IGenerationLayer> GetLayers();
        void SetSeed(int seed);
        int GetSeed();
    }

    /// <summary>
    /// Represents a collection of layers that should be used toghther for procedural generation.
    /// The collection contains methods for applying common settings to all of the layers, such as setting the seed.
    /// </summary>
    public class GenerationLayerCollection : IGenerationLayerCollection
    {
        protected int seed;

        protected IList<IGenerationLayer> layers = new List<IGenerationLayer>();

        public void ClearLayers()
        {
            layers.Clear();
        }

        public void AddLayer(IGenerationLayer layer)
        {
            layer.SetSeed(seed);
            layers.Add(layer);
        }

        public void RemoveLayer(IGenerationLayer layer)
        {
            layers.Remove(layer);
        }

        public IList<IGenerationLayer> GetLayers()
        {
            return layers;
        }

        public int GetSeed()
        {
            return seed;
        }

        public void SetSeed(int seed)
        {
            this.seed = seed;
            foreach (IGenerationLayer layer in layers)
            {
                layer.SetSeed(seed);
            }
        }
    }
}