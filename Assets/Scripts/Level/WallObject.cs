using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WallObject : MonoBehaviour
{
    public Mesh Mesh;
    private List<Vector3> Vertices;
    private List<int> Triangles;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = GetComponent<MeshFilter>().mesh;
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
    }

    //could be made more efficient
    public void AddTriangle(Vector3 first, Vector3 second, Vector3 third)
    {
        Triangles.Add(Vertices.Count);
        Vertices.Add(first);
        Triangles.Add(Vertices.Count);
        Vertices.Add(second);
        Triangles.Add(Vertices.Count);
        Vertices.Add(third);
    }

    public void UpdateMesh()
    {
        Mesh.Clear();
        Mesh.vertices = Vertices.ToArray();
        Mesh.triangles = Triangles.ToArray();
    }
}
