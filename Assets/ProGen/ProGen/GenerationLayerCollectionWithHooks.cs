using System;
using System.Collections.Generic;

namespace ProGen
{
    public class GenerationLayerCollectionWithHooks : IGenerationLayerCollection
    {
        private GenerationLayerCollection collection;
        private IList<Action> seedChangeListeners = new List<Action>();

        public GenerationLayerCollectionWithHooks(GenerationLayerCollection collection)
        {
            this.collection = collection;
        }

        public void AddSeedChangeListener(Action listener)
        {
            seedChangeListeners.Add(listener);
        }

        public void RemoveSeedChangeListener(Action listener)
        {
            seedChangeListeners.Remove(listener);
        }

        public void ClearLayers()
        {
            collection.ClearLayers();
        }

        public void AddLayer(IGenerationLayer layer)
        {
            collection.AddLayer(layer);
        }

        public void RemoveLayer(IGenerationLayer layer)
        {
            collection.RemoveLayer(layer);
        }

        public IList<IGenerationLayer> GetLayers()
        {
            return collection.GetLayers();
        }

        public int GetSeed()
        {
            return collection.GetSeed();
        }

        public void SetSeed(int seed)
        {
            if (collection.GetSeed() == seed)
            {
                return;
            }
            collection.SetSeed(seed);
            foreach (Action act in seedChangeListeners)
            {
                act();
            }
        }
    }
}