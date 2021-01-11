namespace ProGen.Unity
{
    public class SupplementalGenerationLayerCollection : GenerationLayerCollectionComponent
    {
        public bool MountainsEnabled = true;

        public bool CavesEnabled = true;

        public SupplementalGenerationLayerCollection()
        {
            Collection.AddLayer(new SupplementalMountainLayer());
            Collection.AddLayer(new SupplementalCaveLayer());
        }
    }
}