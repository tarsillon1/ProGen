using UnityEngine;

using System.Collections.Generic;

namespace ProGen.Unity
{
    public class MaterialCollectionEntry
    {
        public short ID { get; }

        public Material Material { get; }

        public MaterialCollectionEntry(short id, Material material)
        {
            ID = id;
            Material = material;
        }
    }

    public class MaterialCollectionComponent : MonoBehaviour
    {
        private int maxID;

        private Dictionary<int, MaterialCollectionEntry> materials = new Dictionary<int, MaterialCollectionEntry>();

        public Material Placeholder;

        public void AddMaterial(MaterialCollectionEntry entry)
        {
            if (maxID == 0 || maxID < entry.ID)
            {
                maxID = entry.ID;
            }
            if (materials.ContainsKey(entry.ID))
            {
                materials.Remove(entry.ID);
            }
            materials.Add(entry.ID, entry);
        }

        public void RemoveMaterial(MaterialCollectionEntry entry)
        {
            bool removed = materials.Remove(entry.ID);
            if (removed && entry.ID == maxID)
            {
                int newMax = 0;
                foreach (int i in materials.Keys)
                {
                    if (newMax == 0 || newMax < i)
                    {
                        newMax = i;
                    }
                }
                maxID = newMax;
            }
        }

        public Material[] ToArray()
        {
            int len = maxID + 1;
            Material[] arr = new Material[len];
            for (int i = 0; i < len; i++)
            {
                MaterialCollectionEntry entry;
                bool hasEntry = materials.TryGetValue(i, out entry);
                if (hasEntry)
                {
                    arr[i] = entry.Material;
                }
                else
                {
                    arr[i] = Placeholder;
                }
            }
            return arr;
        }
    }
}