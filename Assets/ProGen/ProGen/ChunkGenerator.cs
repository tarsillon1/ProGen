using System;
using System.Collections.Concurrent;

namespace ProGen
{

    class LayerEntry
    {
        public int InsertionIndex { get; set; }
        public IGenerationLayer Layer { get; set; }
    }

    public class ChunkGenerator
    {
        private ConcurrentBag<LayerEntry> layers;

        public ChunkGenerator()
        {
            layers = new ConcurrentBag<LayerEntry>();
        }

        public void AddLayer(IGenerationLayer layer)
        {
            LayerEntry entry = new LayerEntry();
            entry.InsertionIndex = layers.Count;
            entry.Layer = layer;
            layers.Add(entry);
        }

        public void RemoveLayer(IGenerationLayer layer)
        {
            LayerEntry foundEntry = null;
            foreach (LayerEntry entry in layers)
            {
                if (entry.Layer == layer)
                {
                    foundEntry = entry;
                    break;
                }
            }
            if (foundEntry != null)
            {
                int tries = 0;
                while (!layers.TryTake(out foundEntry))
                {
                    if (tries > 3)
                    {
                        throw new System.Exception("failed to remove layer");
                    }
                    tries++;
                }
            }
        }

        public void ClearLayers()
        {
            layers = new ConcurrentBag<LayerEntry>();
        }

        private IGenerationLayer[] GetOrderedLayers()
        {
            LayerEntry[] entries = layers.ToArray();
            Array.Sort(entries, (obj1, obj2) =>
            {
                if (obj1.InsertionIndex > obj2.InsertionIndex)
                {
                    return 1;
                }
                else if (obj1.InsertionIndex == obj2.InsertionIndex)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });
            IGenerationLayer[] orderedLayers = new IGenerationLayer[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                orderedLayers[i] = entries[i].Layer;
            }
            return orderedLayers;
        }

        public void Generate(Chunk chunk)
        {
            foreach (IGenerationLayer layer in GetOrderedLayers())
            {
                layer.Generate(chunk);
            }
        }
    }
}
