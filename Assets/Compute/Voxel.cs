using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    [Range(1, 6)]
    public int faceIndex = 1;
    // Start is called before the first frame update

    //data
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateFace(new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateFace(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        vertices = new Vector3[4];
        vertices[0] = p1;
        vertices[1] = p2;  
        vertices[2] = p3;
        vertices[3] = p4;

        triangles = new int[]
        {
            0, 1, 2, 2, 3, 0 
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
