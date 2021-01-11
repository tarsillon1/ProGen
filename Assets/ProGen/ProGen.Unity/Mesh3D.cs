using UnityEngine;

namespace ProGen.Unity
{
    public class Mesh3DDrawInput
    {
        public int X;

        public int Y;

        public int Z;

        public short Material;

        public bool IsTopFaceShowing;

        public bool IsBottomFaceShowing;

        public bool IsWestFaceShowing;

        public bool IsEastFaceShowing;

        public bool IsNorthFaceShowing;

        public bool IsSouthFaceShowing;
    }

    public class Mesh3DEraseInput
    {
        public int FromX;
        public int ToX;
        public int FromY;
        public int ToY;
        public int FromZ;
        public int ToZ;
    }


    [RequireComponent(typeof(MeshFilter))]
    public class Mesh3D : MonoBehaviour
    {
        private Mesh mesh;

        private DrawHelper3D helper;

        public Material[] Materials { get; set; }

        public bool IsDestroyed { get; private set; }

        public bool IsStarted { get; private set; }

        void Start()
        {
            helper = new DrawHelper3D(Materials.Length);

            mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            GetComponent<MeshFilter>().mesh = mesh;

            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider != null)
            {
                collider.sharedMesh = mesh;
            }

            MeshRenderer render = GetComponent<MeshRenderer>();
            render.materials = Materials;

            IsStarted = true;
        }

        void OnDestroy()
        {
            IsDestroyed = true;
        }

        public void Draw(Mesh3DDrawInput input)
        {
            int x = input.X;
            int y = input.Y;
            int z = input.Z;
            short mat = input.Material;

            if (input.IsWestFaceShowing)
            {
                helper.CreateWestFace(x, y, z, mat);
            }

            if (input.IsEastFaceShowing)
            {
                helper.CreateEastFace(x, y, z, mat);
            }

            if (input.IsBottomFaceShowing)
            {
                helper.CreateBottomFace(x, y, z, mat);
            }

            if (input.IsTopFaceShowing)
            {
                helper.CreateTopFace(x, y, z, mat);
            }

            if (input.IsSouthFaceShowing)
            {
                helper.CreateSouthFace(x, y, z, mat);
            }

            if (input.IsNorthFaceShowing)
            {
                helper.CreateNorthFace(x, y, z, mat);
            }
        }

        public void Erase(Mesh3DEraseInput input)
        {
            helper.Erase(input.FromX, input.ToX, input.FromY, input.ToY, input.FromZ, input.ToZ);
        }

        public void Render()
        {
            if (IsDestroyed)
            {
                return;
            }
            mesh.Clear();

            mesh.subMeshCount = Materials.Length;
            mesh.vertices = helper.Vertices.ToArray();
            for (int i = 0; i < Materials.Length; i++)
            {
                mesh.SetTriangles(helper.Triangles[i].ToArray(), i);
            }

            OptimizeMesh();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
        }

        void OptimizeMesh()
        {
            mesh.Optimize();
            mesh.OptimizeIndexBuffers();
            mesh.OptimizeReorderVertexBuffer();
        }
    }
}