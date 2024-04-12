using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollider : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider collider;

    public void Init()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        collider = GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;   
    }

    public float GetHeight(float x, float z)
    {
        // Get the vertices of the mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // Calculate the barycentric coordinates of the point (x, z) in the triangle
            Vector3 e0 = v1 - v0;
            Vector3 e1 = v2 - v0;
            Vector3 e2 = new Vector3(x - v0.x, 0, z - v0.z);

            float dot00 = Vector3.Dot(e0, e0);
            float dot01 = Vector3.Dot(e0, e1);
            float dot02 = Vector3.Dot(e0, e2);
            float dot11 = Vector3.Dot(e1, e1);
            float dot12 = Vector3.Dot(e1, e2);

            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if the point is inside the triangle
            if (u > 0 && v > 0 && u + v < 1)
            {
                // Interpolate the position using barycentric coordinates
                return (v0.y + u * (v1.y - v0.y) + v * (v2.y - v0.y))*transform.lossyScale.y;
            }
        }

        // If no triangle contains the point, return a default height
        return 0f;
    }

    public Vector3 SnapToSurface(Vector3 pos)
    {
        Ray ray = new Ray(pos+Vector3.up*100000f, -Vector3.up);
        RaycastHit hit;
        if (collider.Raycast(ray, out hit, 100000f)){
            return hit.point;
        }
        return pos;
    }
}
