using UnityEngine;

namespace ProGen.Unity
{
    public abstract class GenerationLayerCollectionComponent : MonoBehaviour
    {

        [SerializeField]
        private int Seed; // Developer set value in the inspector, for testing purposes.

        public GenerationLayerCollectionWithHooks Collection { get; private set; }

        public GenerationLayerCollectionComponent()
        {
            Collection = new GenerationLayerCollectionWithHooks(new ProGen.GenerationLayerCollection());
        }

        void OnValidate()
        {
            Collection.SetSeed(Seed);
        }
    }
}