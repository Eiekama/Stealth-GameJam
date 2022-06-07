using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMesh : MonoBehaviour
{
    public static Mesh Cone(float range, float angle, int triangleCount)
    {
        Vector3[] vertices;
        Vector2[] uv;
        int[] triangles;

        Vector3 origin = Vector3.zero;
        float currentAngle = 0;
        float angleIncrease;

        vertices = new Vector3[2 * (triangleCount + 2)];
        uv = new Vector2[vertices.Length];
        triangles = new int[triangleCount * 12 + 12];

        angleIncrease = angle / triangleCount;

        //top plane
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i < triangleCount + 1; i++)
        {
            Vector3 vertex = origin + GetVectorFromAngle(currentAngle, 90 + angle / 2) * range; //offset to GetVectorFromAngle added so that cone faces correct direction
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            currentAngle -= angleIncrease;
        }

        //bottom plane
        for (int i = 0; i < triangleCount + 2; i++)
        {
            vertices[vertexIndex] = vertices[i] - Vector3.up;

            if (i > 1)
            {
                triangles[triangleIndex] = triangleCount + 2;
                triangles[triangleIndex + 1] = vertexIndex;
                triangles[triangleIndex + 2] = vertexIndex - 1;

                triangleIndex += 3;
            }
            vertexIndex++;
        }

        //side planes
        for (int i = 0; i < vertices.Length / 2 - 1; i++)
        {
            if (i == 0 && angle == 360f)
            {
                triangleIndex += 6;
                continue;
            }
            triangles[triangleIndex] = i;
            triangles[triangleIndex + 1] = i + triangleCount + 2;
            triangles[triangleIndex + 2] = i + 1;

            triangles[triangleIndex + 3] = i + 1;
            triangles[triangleIndex + 4] = i + triangleCount + 2;
            triangles[triangleIndex + 5] = i + 1 + triangleCount + 2;

            triangleIndex += 6;
        }

        //final side plane
        if (angle != 360f)
        {
            triangles[triangleIndex] = vertices.Length / 2 - 1;
            triangles[triangleIndex + 1] = triangleCount + 2;
            triangles[triangleIndex + 2] = 0;

            triangles[triangleIndex + 3] = triangleCount + 2;
            triangles[triangleIndex + 4] = vertices.Length / 2 - 1;
            triangles[triangleIndex + 5] = vertices.Length / 2 - 1 + triangleCount + 2;
        }

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };

        return mesh;
    }

    static Vector3 GetVectorFromAngle(float angle, float offset=0)
    {
        float angleRad = (angle + offset) * (Mathf.PI / 180);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
}
