using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfSight : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3 origin = Vector3.zero;
        float fov = 90.0f;
        int rayCount = 30;
        float angle = 40.0f;
        float angleIncrease = fov / rayCount;
        float distance = 50.0f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;
        int verticeIndex = 1;
        int triangleIndex = 0;

        for (int index = 0; index <= rayCount; ++index)
        {
            float angleRad = angle * (Mathf.PI / 180.0f);
            Vector3 vertex = origin + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * distance;
            vertices[verticeIndex] = vertex;

            if (index > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = verticeIndex - 1;
                triangles[triangleIndex + 2] = verticeIndex;
                triangleIndex += 3;
            }
            ++verticeIndex;
            angle -= angleIncrease;
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}
