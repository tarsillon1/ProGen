using UnityEngine;

namespace ProGen.Unity
{
    public class SupplementalMaterialCollection : MaterialCollectionComponent
    {
        public Material GrassMaterial;

        public Material DirtMaterial;

        public Material StoneMaterial;

        void set()
        {
            AddMaterial(new MaterialCollectionEntry((short)SupplementalBlock.Grass, GrassMaterial));
            AddMaterial(new MaterialCollectionEntry((short)SupplementalBlock.Stone, StoneMaterial));
            AddMaterial(new MaterialCollectionEntry((short)SupplementalBlock.Dirt, DirtMaterial));
        }

        void Start()
        {
            set();
        }

        void OnValidate()
        {
            set();
        }
    }
}