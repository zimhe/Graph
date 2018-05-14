using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class TensileEdge : MonoBehaviour {

    [SerializeField] private GameObject EdgeIndicatorPFB;
    [SerializeField] private GameObject ShapePFB;

    [SerializeField] private Color XCol;
    [SerializeField] private Color YCol;
    [SerializeField] private Color ZCol;

    private Transform BarObjHolder;

    private Transform StringObjHolder;

    public void SetBarHolder(Transform bhd)
    {
        BarObjHolder = bhd;
    }
    public void SetStrHolder(Transform shd)
    {
        StringObjHolder = shd;
    }

    public int StringCount()
    {
        return strings.Count;
    }

    public int BarCount()
    {
        return AllBars.Count;
    }


    [SerializeField] private StaticBar BarPrefab;
    [SerializeField] private StaticStrings StringPrefab;
    [SerializeField] private Material meshMat;

    private GameObject _mesh;


    private List<StaticBar> AllBars = new List<StaticBar>(6);
    private StaticBar[] ToVertexBars = new StaticBar[2];
    private StaticBar[] ToTiangleBars = new StaticBar[2];
    private StaticBar[] ToLayerBars = new StaticBar[2];

   

    Vector3[] ToVertex=new Vector3[2];
    Vector3[] ToTriangle=new Vector3[2];
    Vector3[] ToLayer=new Vector3[2];

    GameObject[] toVertexDot=new GameObject[2];
    GameObject[] toTriangleDot = new GameObject[2];
    GameObject[] toLayerDot = new GameObject[2];

    private List<Vector3[]> pointCloud = new List<Vector3[]>(6);


    private Vector3[] BarEnds0 = new Vector3[2];
    private Vector3[] BarEnds1 = new Vector3[2];
    private Vector3[] BarEnds2 = new Vector3[2];
    private Vector3[] BarEnds3 = new Vector3[2];
    private Vector3[] BarEnds4 = new Vector3[2];
    private Vector3[] BarEnds5 = new Vector3[2];


    List<StaticStrings> strings=new List<StaticStrings>(24);

    private List<TensileVertex> ConnectedVertexObjs = new List<TensileVertex>(2);
    private List<TensileVertex> DiagonalVertexObjs = new List<TensileVertex>(2);
    private List<TensileTriangle> ConnectedTriangles=new List<TensileTriangle>(2);


    List<GameObject> ObjHolder=new List<GameObject>(30);



    private GameObject GraphHolder;

    public int State { get; set; }

    public int FutureState { get; set; }

    public int SubState { get; set; }

    public int StartVertice { get; set; }

    public int EndVertice { get; set; }
 
    public int Depth { get; set; }

    public TensileVertex NeighborVertexStart { get; set; }
    public TensileVertex NeighborVertexEnd { get; set; }
    public TensileEdge NeighborEdgeAbove { get; set; }
    public TensileEdge NeighborEdgeBelow { get; set; }


    private List <Vector3> PositionToGrow=new List<Vector3>(2);

    public void AddPositionToGrow(Vector3 _p)
    {
       PositionToGrow.Add(_p);
    }

    public List<Vector3> GetPositionToGrow()
    {
        return PositionToGrow;
    }



    public Vector3 PosToGrow(int index)
    {
        Vector3 ptg = PositionToGrow[index];
        PositionToGrow.Remove(PositionToGrow[index]);

        Destroy(ToGrowIndicators[index]);

        ToGrowIndicators.Remove(ToGrowIndicators[index]);


        return ptg;
    }

    private List<GameObject> ToGrowIndicators = new List<GameObject>(2);

    public void addIndicator(GameObject _indicator)
    {
        ToGrowIndicators.Add(_indicator);
    }

    public List<GameObject> GetIndicators()
    {
        return ToGrowIndicators;
    }

    public void ToggleIndicators(bool _toggle)
    {
        foreach (var ind in ToGrowIndicators)
        {
            ind.GetComponent<MeshRenderer>().enabled = _toggle;
        }
    }

    public GameObject GetGraphHolder()
    {
        return GraphHolder;
    }


    private float Scale;

    private float thickness = 0.005f;

    private bool ShowGraph = true;

    void Start()
    {
        setPoint();

    }

    void clean()
    {
        foreach (var o in ObjHolder)
        {
            if (o != null)
            {
                DestroyImmediate(o);
            }

        }
        ObjHolder.Clear();
        foreach (var b in AllBars)
        {
            if (b != null)
            {
                DestroyImmediate(b.gameObject);
            }
        }
        AllBars.Clear();
        foreach (var s in strings)
        {
            if (s != null)
            {
                DestroyImmediate(s.gameObject);
            }
        }
        strings.Clear();
    }
    public void GraphToggle()
    {
        if (ShowGraph == true)
        {
            ShowGraph = false;

            GraphHolder.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            ShowGraph = true;
            GraphHolder.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    public void GraphToggle(bool toggle)
    {
     
        
            ShowGraph = toggle;

            GraphHolder.GetComponent<MeshRenderer>().enabled = toggle;
        
       
    }


    void updateStructure()
    {
        updateTransform();

        updateBalls();
      
        if (StartVertice != EndVertice && ConnectedTriangles.Count == 2)
        {
            updatePoint();

            if (AllBars.Count != 0)
            {
                updateBars();
                if (strings.Count != 0)
                {
                    updateStrings();
                }
            }
        }
    }


    private float DefThickness = 0.005f;
    private float SelectedThickness = 0.02f;
    private float GraphThickness = 0.005f;

    public void GraphSelect()
    {
        GraphThickness = SelectedThickness;
    }

    public void resetGraphThickness()
    {
        GraphThickness = DefThickness;

    }

    void updateTransform()
    {
        if (ConnectedVertexObjs.Count != 0)
        {
            var p0 = ConnectedVertexObjs[0].transform.localPosition;
            var p1 = ConnectedVertexObjs[1].transform.localPosition;

            var D = p1 - p0;
            var Pos = (p0 + p1) * 0.5f;

            transform.localPosition = Pos;

            var up = Vector3.Cross(D, Vector3.down);
            transform.localRotation = Quaternion.LookRotation(D, up);
            GraphHolder.transform.localScale = new Vector3(GraphThickness, 0.5f * D.magnitude, GraphThickness);
        }
    }

    void setPoint()
    {
        pointCloud.Add(BarEnds2);
        pointCloud.Add(BarEnds3);
        pointCloud.Add(BarEnds1);
        pointCloud.Add(BarEnds0);
        pointCloud.Add(BarEnds5);
        pointCloud.Add(BarEnds4);
    }
    void updatePoint()
    {
        ToVertex[0] = ConnectedVertexObjs[0].transform.localPosition;
        ToVertex[1] = ConnectedVertexObjs[1].transform.localPosition;
        ToTriangle[0] = ConnectedTriangles[0].Circumcenter();
        ToTriangle[1] = ConnectedTriangles[1].Circumcenter();
        ToLayer[0] = DiagonalVertexObjs[0].transform.localPosition;
        ToLayer[1] = DiagonalVertexObjs[1].transform.localPosition;

        var Dv = ToVertex[1] - ToVertex[0];

        BarEnds0[0] = (ToVertex[1] - ToVertex[0]) * 0.72f + ToVertex[0] + Vector3.up * 0.132f * Scale;
        BarEnds0[1] = (ToVertex[0] - ToVertex[1]) * 0.72f + ToVertex[1] + Vector3.up * 0.132f * Scale;
        BarEnds1[0] = (ToVertex[1] - ToVertex[0]) * 0.72f + ToVertex[0] + Vector3.down * 0.132f * Scale;
        BarEnds1[1] = (ToVertex[0] - ToVertex[1]) * 0.72f + ToVertex[1] + Vector3.down * 0.132f * Scale;


        float s = 0.2623f;
        float L = s * Dv.magnitude;
        float Lt0 = (ToVertex[0] - ToTriangle[0]).magnitude * s;
        float R0 = Mathf.Sqrt(Mathf.Pow(Lt0, 2f) - Mathf.Pow(L / 2f, 2f));
        float Lt1 = (ToVertex[1] - ToTriangle[1]).magnitude * s;
        float R1 = Mathf.Sqrt(Mathf.Pow(Lt1, 2f) - Mathf.Pow(L / 2f, 2f));

        var Dt = ToTriangle[1] - ToTriangle[0];
        float DL = Dt.magnitude;

        if (float.IsNaN(DL) )
        {
            print("shit");
            DL = Mathf.Pow(0.001f, 3f);
        }

        float t0 = Mathf.Abs((DL - R0) / DL);
        float t1 = Mathf.Abs((DL - R1) / DL);

        var DvP = Vector3.ProjectOnPlane(Dv, Vector3.up);

        var b20= Dt * t1 + ToTriangle[0] + Dv * s / 2;
        var b21 = -Dt * t0 + ToTriangle[1] + Dv * s / 2;
        var b30 = Dt * t1 + ToTriangle[0] - Dv * s / 2;
        var b31 = -Dt * t0 + ToTriangle[1] - Dv * s / 2;

        var p1 = ConnectedTriangles[1].TriPlane();
        var p0 = ConnectedTriangles[0].TriPlane();


        BarEnds2[0] = p1.ClosestPointOnPlane(b20);
        BarEnds2[1] = p0.ClosestPointOnPlane(b21);
        BarEnds3[0] = p1.ClosestPointOnPlane(b30);
        BarEnds3[1] = p0.ClosestPointOnPlane(b31);


        //BarEnds2[0] = Dt * t1 + ToTriangle[0] + Dv * s / 2;
        //BarEnds2[1] = -Dt * t0 + ToTriangle[1] + Dv * s / 2;
        //BarEnds3[0] = Dt * t1 + ToTriangle[0] - Dv * s / 2;
        //BarEnds3[1] = -Dt * t0 + ToTriangle[1] - Dv * s / 2;


        var Db = DiagonalVertexObjs[0].transform.localPosition - DiagonalVertexObjs[1].transform.localPosition;
        var Da = DiagonalVertexObjs[0].VertAbovePosition() - DiagonalVertexObjs[1].VertAbovePosition();

        BarEnds4[0] = Db * 0.57f + DiagonalVertexObjs[1].transform.localPosition + (0.42552f / 2f) * Scale * Vector3.down;
        BarEnds4[1] = Da * 0.57f + DiagonalVertexObjs[1].VertAbovePosition() + (0.42552f / 2f) * Scale * Vector3.down;
        BarEnds5[0] = -Db * 0.57f + DiagonalVertexObjs[0].transform.localPosition + (0.42552f / 2f) * Scale * Vector3.down;
        BarEnds5[1] = -Da * 0.57f + DiagonalVertexObjs[0].VertAbovePosition() + (0.42552f / 2f) * Scale * Vector3.down;

        pointCloud[0] = BarEnds2;
        pointCloud[1] = BarEnds3;
        pointCloud[2] = BarEnds1;
        pointCloud[3] = BarEnds0;
        pointCloud[4] = BarEnds5;
        pointCloud[5] = BarEnds4;
    }


    void SetBars()
    {
        string edgeType = "Edge:"+StartVertice+","+EndVertice;
        ToVertexBars[0] = Instantiate(BarPrefab,BarObjHolder);
        ToVertexBars[0].SetupBar(BarEnds0[0],BarEnds0[1],thickness,0,edgeType);


        ToVertexBars[1] = Instantiate(BarPrefab, BarObjHolder);
        ToVertexBars[1].SetupBar(BarEnds1[0], BarEnds1[1], thickness, 1,edgeType);

        ToTiangleBars[0] = Instantiate(BarPrefab, BarObjHolder);
        ToTiangleBars[0].SetupBar(BarEnds2[0],BarEnds2[1],thickness*1.3f,2,edgeType);

        ToTiangleBars[1] = Instantiate(BarPrefab, BarObjHolder);
        ToTiangleBars[1].SetupBar(BarEnds3[0], BarEnds3[1], thickness*1.3f, 3,edgeType);    

        ToLayerBars[0] = Instantiate(BarPrefab, BarObjHolder);
        ToLayerBars[0].SetupBar(BarEnds4[0],BarEnds4[1],thickness,4,edgeType);

        ToLayerBars[1] = Instantiate(BarPrefab, BarObjHolder);
        ToLayerBars[1].SetupBar(BarEnds5[0], BarEnds5[1], thickness, 5,edgeType);

        AllBars.AddRange(ToVertexBars);
        AllBars.AddRange(ToTiangleBars);
        AllBars.AddRange(ToLayerBars);
        

    }

    void updateBars()
    {
        ToVertexBars[0].UpdateBar(BarEnds0[0], BarEnds0[1]);
        ToVertexBars[1].UpdateBar(BarEnds1[0], BarEnds1[1]);
        ToTiangleBars[0].UpdateBar(BarEnds2[0], BarEnds2[1]);
        ToTiangleBars[1].UpdateBar(BarEnds3[0], BarEnds3[1]);
        ToLayerBars[0].UpdateBar(BarEnds4[0], BarEnds4[1]);
        ToLayerBars[1].UpdateBar(BarEnds5[0], BarEnds5[1]);
    }


    void setString()
    {
        if (pointCloud.Count != 0)
        {
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int ToPoint;
                    int ToIndex0;
                    int ToIndex1;

                    Vector3 _start;
                    Vector3 _end0;
                    Vector3 _end1;

                    if (i % 2 == 0)
                    {
                        ToPoint = 0;
                        ToIndex0 = 2;
                        ToIndex1 = 3;
                    }
                    else
                    {
                        ToPoint = 1;
                        ToIndex0 = 1;
                        ToIndex1 = 2;
                    }
                    _start = pointCloud[i][j];
                    if (i < 4)
                    {
                        _end0 = pointCloud[i + ToIndex0][ToPoint];
                        _end1 = pointCloud[i + ToIndex1][ToPoint];
                    }
                    else
                    {
                        _end0 = pointCloud[0][ToPoint];
                        _end1 = pointCloud[1][ToPoint];
                    }

                    StaticStrings str0 = Instantiate(StringPrefab, StringObjHolder);
                    StaticStrings str1 = Instantiate(StringPrefab, StringObjHolder);
                    str0.ConnectString(_start, _end0, thickness * 0.3f, index, "edge");
                    str1.ConnectString(_start, _end1, thickness * 0.3f, index + 1, "edge");
                    strings.Add(str0);
                    strings.Add(str1);
                    index += 2;
                }
            }
        }
       
    }

    void updateStrings()
    {
        if (pointCloud.Count != 0)
        {
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int ToPoint;
                    int ToIndex0;
                    int ToIndex1;

                    Vector3 _start;
                    Vector3 _end0;
                    Vector3 _end1;

                    if (i % 2 == 0)
                    {
                        ToPoint = 0;
                        ToIndex0 = 2;
                        ToIndex1 = 3;
                    }
                    else
                    {
                        ToPoint = 1;
                        ToIndex0 = 1;
                        ToIndex1 = 2;
                    }
                    _start = pointCloud[i][j];
                    if (i < 4)
                    {
                        _end0 = pointCloud[i + ToIndex0][ToPoint];
                        _end1 = pointCloud[i + ToIndex1][ToPoint];
                    }
                    else
                    {
                        _end0 = pointCloud[0][ToPoint];
                        _end1 = pointCloud[1][ToPoint];
                    }

                    strings[index].updateString(_start, _end0);
                    strings[index + 1].updateString(_start, _end1);

                    index += 2;
                }
            }
        }
          
    }


    public void SetState(int _state)
    {
        State = _state;
        UpdateState();
    }
    public void SetAllState(int _state,int _substate)
    {
        State = _state;
        SubState = _substate;
        UpdateState();
    }

    public void AddConnectedVertex(TensileVertex V)
    {
        if(!ConnectedVertexObjs.Contains(V))
        ConnectedVertexObjs.Add(V);
    }

    public void AddDiagonalVertex(TensileVertex V)
    {
        if(!DiagonalVertexObjs.Contains(V))
        DiagonalVertexObjs.Add(V);
    }
    public void AddTriangle(TensileTriangle _tt)
    {
        if(!ConnectedTriangles.Contains(_tt))
        ConnectedTriangles.Add(_tt);
    }


    public IEnumerable<TensileVertex> GetConnectedVertexObj()
    {
        return ConnectedVertexObjs;
    }
    public IEnumerable<TensileTriangle> GetConnecetedTriangles()
    {
        return ConnectedTriangles;
    }

    public void SetupStructure(float _scale, int _state, int _start,int _end, IEnumerable<Vector3> _position)
    {
        var p = _position.ToArray();
        Scale = _scale;
      
        SetState(_state);
        StartVertice  = _start;
        EndVertice = _end;
        var D = p[_end] - p[_start];
        var Pos = (p[_start] + p[_end]) * 0.5f;
        transform.localScale *= Scale;
        transform.localPosition = Pos;

        var up = Vector3.Cross(D, Vector3.down);
        transform.localRotation = Quaternion.LookRotation(D,up);
        GraphHolder = Instantiate(EdgeIndicatorPFB, transform);
       GraphHolder.transform.localScale=new Vector3(0.005f,0.5f*D.magnitude,0.005f);
    }

    void UpdateState() 
    {
        
        if (State == 0)//Show only center point
        {
           clean();
        }

        if (State == 2)
        {
            if (SubState == 0)//Show growed white cube
            {
                clean();

                var Obj = Instantiate(ShapePFB, transform);
                var Obj1 = Instantiate(ShapePFB, transform);
                var Obj2 = Instantiate(ShapePFB, transform);
                // Obj.transform.localScale = new Vector3(0.05f, GraphHolder.transform.localScale.y, 0.05f);

                Obj1.transform.localPosition = Vector3.forward * 0.5f * GraphHolder.transform.localScale.y;

                Obj2.transform.localPosition = Vector3.back * 0.5f * GraphHolder.transform.localScale.y;

                ObjHolder.Add(Obj);
                ObjHolder.Add(Obj1);
                ObjHolder.Add(Obj2);
            }

            if (SubState == 1)//Show all structure
            {

                clean();
                if (ConnectedTriangles.Count == 2)
                {
                    SetBars();
                    setString();

                    foreach (var b in AllBars)
                    {
                        ObjHolder.Add(b.gameObject);
                    }
                    foreach (var s in strings)
                    {
                        ObjHolder.Add(s.gameObject);
                    }
                }
            }
            if (SubState == 2)
            {
                clean();
                if (ConnectedTriangles.Count == 2)
                {
                    int ti = 0;
                    foreach (var p in ToTriangle)
                    {
                        var tt = Instantiate(ShapePFB,transform.parent);
                        tt.transform.localPosition = p;
                        tt.GetComponent<MeshRenderer>().material.color = XCol;
                        toTriangleDot[ti] = tt;
                        ti++;
                    }
                    int vi = 0;
                    foreach (var p in ToVertex)
                    {
                     
                        var tv = Instantiate(ShapePFB,transform.parent);
                        tv.transform.localPosition = p;
                        tv.GetComponent<MeshRenderer>().material.color = YCol;
                        toVertexDot[vi] = tv;
                        vi++;
                    }
                    int li = 0;
                    foreach (var p in ToLayer)
                    {
                        var tl = Instantiate(ShapePFB,transform.parent);
                        tl.transform.localPosition = p;
                        tl.GetComponent<MeshRenderer>().material.color = ZCol;
                        toLayerDot[li] = tl;
                        li++;
                    }

                    ObjHolder.AddRange(toTriangleDot);
                    ObjHolder.AddRange(toVertexDot);
                    ObjHolder.AddRange(toLayerDot);
                    SetBars();
                    foreach (var tt in ToTiangleBars)
                    {
                        tt.GetComponent<MeshRenderer>().material.color = XCol;
                    }
                    foreach (var tv in ToVertexBars)
                    {
                        tv.GetComponent<MeshRenderer>().material.color = YCol;
                    }
                    foreach (var tl in ToLayerBars)
                    {
                        tl.GetComponent<MeshRenderer>().material.color = ZCol;
                    }

                    foreach (var b in AllBars)
                    {
                        ObjHolder.Add(b.gameObject);
                    }
                }
            }
        }
    }

    void SetMesh()
    {
        _mesh=new GameObject();
        _mesh.transform.parent = transform;
       MeshFilter mf= _mesh.AddComponent<MeshFilter>();
        MeshRenderer mr = _mesh.AddComponent<MeshRenderer>();
        Mesh m=new Mesh();

        mf.sharedMesh = m;
        mr.material = meshMat;

        List<Vector3> meshVertices=new List<Vector3>();
       int[] meshTriangles=new int[]{0,8,2,0,2,9,0,4,8,0,6,4,0,9,6,2,7,9,2,8,5,8,4,10,8,10,5,5,10,3,3,7,5,7,3,11,7,11,9,9,11,6,6,11,1,1,11,3,1,3,10,1,10,4,4,6,1};


        foreach (var va in pointCloud)
        {
            meshVertices.AddRange(va);
        }

        m.vertices = meshVertices.ToArray();

        m.triangles = meshTriangles;

        m.RecalculateTangents();
        m.RecalculateNormals();

    }

    void updateBalls()
    {
        if (State==2&&SubState == 0)
        {
            ObjHolder[1].transform.localPosition = Vector3.forward * 0.5f * GraphHolder.transform.localScale.y;
            ObjHolder[2].transform.localPosition = Vector3.back * 0.5f * GraphHolder.transform.localScale.y;
        }
        if (State == 2 && SubState == 2&&ConnectedTriangles.Count==2)
        {
            toTriangleDot[0].transform.localPosition = ToTriangle[0];
            toTriangleDot[1].transform.localPosition = ToTriangle[1];
            toVertexDot[0].transform.localPosition = ToVertex[0];
            toVertexDot[1].transform.localPosition = ToVertex[1];
            toLayerDot[0].transform.localPosition = ToLayer[0];
            toLayerDot[1].transform.localPosition = ToLayer[1];
        }
    }

    public void ResetGame()
    {
        State = 0;
        UpdateState();
    }
    public void SetSubState(int _subState)
    {
        SubState = _subState;
        UpdateState();
    }

    void Update()
    {
      updateStructure();
    }
}
