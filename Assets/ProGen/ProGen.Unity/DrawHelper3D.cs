using UnityEngine;
using System.Collections.Generic;

namespace ProGen.Unity
{
    class DrawHelper3D
    {
        public List<int>[] Triangles;

        public List<Vector3> Vertices = new List<Vector3>();

        public Dictionary<Vector3, int> VetexBuffer = new Dictionary<Vector3, int>();

        public DrawHelper3D(int subMeshCount)
        {
            Triangles = new List<int>[subMeshCount];
            for (int i = 0; i < subMeshCount; i++)
            {
                Triangles[i] = new List<int>();
            }
        }

        public void Erase(int fromX, int toX, int fromY, int toY, int fromZ, int toZ)
        {
            HashSet<int> removeVerts = new HashSet<int>();
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector3 vert = Vertices[i];
                if (fromX <= vert.x && toX >= vert.x &&
                    fromY <= vert.y && toY >= vert.y &&
                    fromZ <= vert.z && toZ >= vert.z)
                {
                    removeVerts.Add(i);
                }
            }

            List<Vector3> newVerts = new List<Vector3>();
            Dictionary<Vector3, int> newVertexBuffer = new Dictionary<Vector3, int>();
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (!removeVerts.Contains(i))
                {
                    Vector3 vert = Vertices[i];
                    int index = newVerts.Count;
                    newVertexBuffer.Add(vert, index);
                    newVerts.Add(vert);
                }
            }

            for (int i = 0; i < Triangles.Length; i++)
            {
                List<int> triangles = Triangles[i];
                HashSet<int> removeTriangles = new HashSet<int>();
                for (int j = 0; j < triangles.Count / 3; j += 3)
                {
                    int vert1 = triangles[j];
                    int vert2 = triangles[j + 1];
                    int vert3 = triangles[j + 2];
                    if (removeVerts.Contains(vert1) ||
                        removeVerts.Contains(vert2) ||
                        removeVerts.Contains(vert3))
                    {
                        removeTriangles.Add(j);
                        removeTriangles.Add(j + 1);
                        removeTriangles.Add(j + 2);
                    }
                }
                List<int> newTriangles = new List<int>();
                for (int j = 0; j < triangles.Count; j++)
                {
                    if (!removeTriangles.Contains(j))
                    {
                        int vertIndex = triangles[j];
                        Vector3 vert = Vertices[vertIndex];
                        int newVertIndex;
                        newVertexBuffer.TryGetValue(vert, out newVertIndex);
                        newTriangles.Add(newVertIndex);
                    }
                }
                Triangles[i] = newTriangles;
            }
            Vertices = newVerts;
            VetexBuffer = newVertexBuffer;
        }

        public void CreateBottomFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x, y, z);
            int vectorIndex2 = CreateVertex(x, y, z + 1);
            int vectorIndex3 = CreateVertex(x + 1, y, z);
            int vectorIndex4 = CreateVertex(x + 1, y, z + 1);

            CreateFace(vectorIndex1, vectorIndex3, vectorIndex2, vectorIndex2, vectorIndex3, vectorIndex4, mat);
        }

        public void CreateTopFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x, y + 1, z);
            int vectorIndex2 = CreateVertex(x + 1, y + 1, z);
            int vectorIndex3 = CreateVertex(x, y + 1, z + 1);
            int vectorIndex4 = CreateVertex(x + 1, y + 1, z + 1);

            CreateFace(vectorIndex1, vectorIndex3, vectorIndex2, vectorIndex2, vectorIndex3, vectorIndex4, mat);
        }

        public void CreateWestFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x, y, z);
            int vectorIndex2 = CreateVertex(x, y + 1, z);
            int vectorIndex3 = CreateVertex(x, y, z + 1);
            int vectorIndex4 = CreateVertex(x, y + 1, z + 1);

            CreateFace(vectorIndex1, vectorIndex3, vectorIndex2, vectorIndex2, vectorIndex3, vectorIndex4, mat);
        }

        public void CreateEastFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x + 1, y, z);
            int vectorIndex2 = CreateVertex(x + 1, y + 1, z);
            int vectorIndex3 = CreateVertex(x + 1, y, z + 1);
            int vectorIndex4 = CreateVertex(x + 1, y + 1, z + 1);

            CreateFace(vectorIndex1, vectorIndex2, vectorIndex3, vectorIndex2, vectorIndex4, vectorIndex3, mat);
        }

        public void CreateNorthFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x, y, z + 1);
            int vectorIndex2 = CreateVertex(x, y + 1, z + 1);
            int vectorIndex3 = CreateVertex(x + 1, y, z + 1);
            int vectorIndex4 = CreateVertex(x + 1, y + 1, z + 1);

            CreateFace(vectorIndex1, vectorIndex3, vectorIndex2, vectorIndex2, vectorIndex3, vectorIndex4, mat);
        }

        public void CreateSouthFace(int x, int y, int z, int mat)
        {
            int vectorIndex1 = CreateVertex(x, y, z);
            int vectorIndex2 = CreateVertex(x, y + 1, z);
            int vectorIndex3 = CreateVertex(x + 1, y, z);
            int vectorIndex4 = CreateVertex(x + 1, y + 1, z);

            CreateFace(vectorIndex1, vectorIndex2, vectorIndex3, vectorIndex2, vectorIndex4, vectorIndex3, mat);
        }


        public void CreateFace(int vectorIndex1, int vectorIndex2, int vectorIndex3, int vectorIndex4, int vectorIndex5, int vectorIndex6, int subMeshId)
        {
            List<int> triangles = Triangles[subMeshId];
            triangles.Add(vectorIndex1);
            triangles.Add(vectorIndex2);
            triangles.Add(vectorIndex3);
            triangles.Add(vectorIndex4);
            triangles.Add(vectorIndex5);
            triangles.Add(vectorIndex6);
        }


        public int CreateVertex(int x, int y, int z)
        {
            int index;
            Vector3 vector = new Vector3(x, y, z);
            if (VetexBuffer.TryGetValue(vector, out index))
            {
                return index;
            }
            index = Vertices.Count;
            Vertices.Add(vector);
            VetexBuffer.Add(vector, index);
            return index;
        }
    }
}