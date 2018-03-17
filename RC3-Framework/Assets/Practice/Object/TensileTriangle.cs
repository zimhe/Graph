using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensileTriangle : MonoBehaviour
{
    private Vector3 A;
    private Vector3 B;
    private Vector3 C;
    private Vector3 CC;

    private List<int> vertices=new List<int>(3);
    List<List<int>> edges=new List<List<int>>(3);
    List< TensileEdge> EdgeObjects=new List<TensileEdge>(3);

    public void SetupTriangle(int v0, int v1, int v2, List<Vector3> _positions)
    {
        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        edges.Add(new List<int>());
        edges.Add(new List<int>());
        edges.Add(new List<int>());
        edges[0].Add(v1);
        edges[0].Add(v0);
        edges[1].Add(v2);
        edges[1].Add(v1);
        edges[2].Add(v2);
        edges[2].Add(v0);


        A = _positions[v0];
        B = _positions[v1];
        C = _positions[v2];

        GetCircumCenter(A,B,C);
    }



    void GetCircumCenter(Vector3 a, Vector3 b, Vector3 c)
    {

        float x1 = a.x;
        float z1 = a.z;
        float x2 = b.x;
        float z2 = b.z;
        float x3 = c.x;
        float z3 = c.z;

        float A1 = 2f * (x2 - x1);
        float B1 = 2f * (z2 - z1);
        float C1 = x2 * x2 + z2 * z2 - x1 * x1 - z1 * z1;

        float A2 = 2f * (x3 - x2);
        float B2 = 2f * (z3 - z2);
        float C2 = x3 * x3 + z3 * z3 - x2 * x2 - z2 * z2;

        float CenterX = ((C1 * B2) - (C2 * B1)) / ((A1 * B2) - (A2 * B1));
        float CenterZ = ((A1 * C2) - (A2 * C1)) / ((A1 * B2) - (A2 * B1));

        float CenterY = (a.y + b.y + c.y) / 3f;

        CC = new Vector3(CenterX, CenterY, CenterZ);

    }

    public Vector3 Circumcenter()
    {
        return CC;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
