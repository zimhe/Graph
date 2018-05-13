using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSlur.SlurCore;

public class TensileTriangle
{
    private Vector3 A;
    private Vector3 B;
    private Vector3 C;
    private Vector3 CC;

    private List<int> vertices=new List<int>(3);
    List<List<int>> edges=new List<List<int>>(3);
    List< TensileEdge> EdgeObjects=new List<TensileEdge>(3);
    List<TensileVertex> VertexObjects=new List<TensileVertex>();


    public IEnumerable<Vector3> Points()
    {
        List<Vector3> P=new List<Vector3>(3);
        P.Add(A);
        P.Add(B);
        P.Add(C);
        return P;
    }

    public void SetupTriangle(int v0, int v1, int v2, List<Vector3> _positions)
    {
        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        edges.Add(new List<int>());
        edges.Add(new List<int>());
        edges.Add(new List<int>());

        if (v0 > v1)
        {
            edges[0].Add(v0);
            edges[0].Add(v1);
        }
        else
        {
            edges[0].Add(v1);
            edges[0].Add(v0);
            
        }
        if (v1 > v2)
        {
            edges[1].Add(v1);
            edges[1].Add(v2);
        }
        else
        {
            edges[1].Add(v2);
            edges[1].Add(v1);
        }

        if (v0 > v2)
        {
            edges[2].Add(v0);
            edges[2].Add(v2);
        }
        else
        {
            edges[2].Add(v2);
            edges[2].Add(v0);
        }
      

        A = _positions[v0];
        B = _positions[v1];
        C = _positions[v2];

      
      GetCircumCenterUsingSpatialSlur();

        //GetCircumCenter(A,B,C);
    }

    public void updateTriangle(List<Vector3> _positions)
    {
        int _a = vertices[0];
        int _b = vertices[1];
        int _c = vertices[2];
        A = _positions[_a];
        B = _positions[_b];
        C = _positions[_c];

       GetCircumCenterUsingSpatialSlur();
       setPlane();
    }

    void GetCircumCenterUsingSpatialSlur()
    {
        Vec3d _a = new Vec3d(A.x, A.y, A.z);
        Vec3d _b = new Vec3d(B.x, B.y, B.z);
        Vec3d _c = new Vec3d(C.x, C.y, C.z);

        Vec3d _cc = GeometryUtil.GetCircumcenter(_a, _b, _c);
        

        CC = new Vector3((float)_cc.X, (float)_cc.Y, (float)_cc.Z);
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

    public IEnumerable<List<int>> GetTriEdges()
    {
        return edges;
    }

    public IEnumerable<int> GetTriVertices()
    {
        return vertices;
    }

    public void addTensileEdgeObjs(TensileEdge _te)
    {
        if(!EdgeObjects.Contains(_te))
        EdgeObjects.Add(_te);
    }

    public IEnumerable<TensileEdge> GetConnectEdgesObjs()
    {
        return EdgeObjects;
    }
    public IEnumerable<TensileVertex> GetConnectVertexObjs()
    {
        return VertexObjects;
    }


    public void addTensileVertexObjs(TensileVertex _tv)
    {
        if(!VertexObjects.Contains(_tv))
        VertexObjects.Add(_tv);
    }

    public int GiveDiagonalVertice(int v0, int v1)
    {
        int other =0;
        foreach (int v in vertices)
        {
            if (v != v0 && v != v1)
            {
                other = v;
            }
            else
            {
                continue;
            }
        }

        return other;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Vector3 _normal;


    private Plane _plane=new Plane();



    void calculateNormal()
    {
        _normal = Vector3.Cross(B - A, C - A);
    }

    void setPlane()
    {
        _plane.Set3Points(A,B,C);

       
    }


    public  Vector3 Circumcenter()
    {
        return CC;
    }

    public Vector3 Normal()
    {
        return _plane.normal;
    }

    public Plane TriPlane()
    {
        return _plane;
    }


}
