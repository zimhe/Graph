using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;
using UnityEngine.UI;

public class TriangleBoundryGraphManager : MonoBehaviour
{

    [SerializeField] private SharedEdgeGraph _edgeGraph;
    [SerializeField] private TensileVertex TGVertexPrefab;
    [SerializeField] private TensileEdge TGEdgePrefab;
    private int SegmentCount = 2;
  
    private int LayerCount = 1;

    [SerializeField] private float Scale = 1f;


    public float getScale()
    {
        return Scale;
    }
    [SerializeField] int DefState;

    [SerializeField] private Button btn;
    [SerializeField] private Button btn2;

    private TensileEdge[,] EdgeObj2d;

    private Transform BarsHolder;
    private Transform StringHolder;

    List<TensileEdge> verticalEdges = new List<TensileEdge>();

    private TensileTriangle[] Triangle;
    private int[] TriangleVerts;

    private List<Vector3> P = new List<Vector3>();

    public SharedEdgeGraph Grid()
    {
        return _edgeGraph;
    }

    public int GetSegmentCount()
    {
        return SegmentCount;
    }
    public int GetLayerCount()
    {
        return LayerCount;
    }
   


    private List<Vector3> currentPositions;
    private List<Vector3> savedPositions;

    public List<Vector3> getSavedPositions()
    {
        return savedPositions;
    }

    public void addPosition(Vector3 _p)
    {
        savedPositions.Add(_p);
        currentPositions.Add(_p);
    }

    public void AddVertexEdgeTriangle(TensileEdge SE, int index)
    {
        int V0 = SE.StartVertice;
        int V1 = SE.EndVertice;

        if (SE.GetConnecetedTriangles().Count() == 1)
        {
            Vector3 _p = SE.PosToGrow(index);

            _edgeGraph.edgeGraph.AddVertex();

            int V = _edgeGraph.edgeGraph.VertexCount-1;

            _edgeGraph.edgeGraph.AddEdge(V, V0);
            _edgeGraph.edgeGraph.AddEdge(V, V1);

              
            addPosition(_p);
            var tObj = Instantiate(TGVertexPrefab, transform);
            tObj.SetBarHolder(BarsHolder);
            tObj.SetStrHolder(StringHolder);
            tObj.transform.localPosition = _p;

            tObj.SetupStructure(Scale, DefState, V);

            _edgeGraph.TensileVertexObjects[V0].AddConnectedVertObjInLayer(tObj);
            _edgeGraph.TensileVertexObjects[V1].AddConnectedVertObjInLayer(tObj);

            _edgeGraph.TensileVertexObjects.Add(tObj);

            SE.AddDiagonalVertex(tObj);
            SE.NeighborVertexEnd = tObj;


            var eObj0 = Instantiate(TGEdgePrefab, transform);
            eObj0.SetBarHolder(BarsHolder);
            eObj0.SetStrHolder(StringHolder);
            eObj0.SetupStructure(Scale, DefState, V, V0, savedPositions);

            _edgeGraph.TensileEdgeObjects.Add(eObj0);

            var eObj1 = Instantiate(TGEdgePrefab, transform);
            eObj1.SetBarHolder(BarsHolder);
            eObj1.SetStrHolder(StringHolder);
            eObj1.SetupStructure(Scale, DefState, V, V1, savedPositions);

            _edgeGraph.TensileEdgeObjects.Add(eObj1);

            var tri =new TensileTriangle();

            tri.SetupTriangle(V,V0,V1, savedPositions);

            _edgeGraph.TensileTriangle.Add(tri);

            tri.addTensileEdgeObjs(SE);
            tri.addTensileEdgeObjs(eObj0);
            tri.addTensileEdgeObjs(eObj1);

            tri.addTensileVertexObjs(tObj);
            tri.addTensileVertexObjs(_edgeGraph.TensileVertexObjects[V0]);
            tri.addTensileVertexObjs(_edgeGraph.TensileVertexObjects[V1]);

            SE.AddTriangle(tri);

            eObj0.AddTriangle(tri);

            eObj0.AddConnectedVertex(tObj);
            eObj0.AddConnectedVertex(_edgeGraph.TensileVertexObjects[V0]);

            eObj0.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[V1]);
          
            tObj.AddEdgeObjs(eObj0);
            _edgeGraph.TensileVertexObjects[V0].AddEdgeObjs(eObj0);

          



            eObj1.AddTriangle(tri);

            eObj1.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[V0]);

            eObj1.AddConnectedVertex(tObj);
            eObj1.AddConnectedVertex(_edgeGraph.TensileVertexObjects[V1]);
            tObj.AddEdgeObjs(eObj1);
            _edgeGraph.TensileVertexObjects[V1].AddEdgeObjs(eObj1);

            List<TensileEdge > SharedPointEdges=new List<TensileEdge>();

            foreach (var e in _edgeGraph.TensileEdgeObjects)
            {
                if (e.GetPositionToGrow().Count != 0)
                {
                    var P = e.GetPositionToGrow()[index];

                    if (P == _p && e != SE)
                    {
                        SharedPointEdges.Add(e);
                        var tp = e.PosToGrow(index);
                    }
                }
            }

            if (SharedPointEdges.Count != 0)
            {
                foreach (var e in SharedPointEdges)
                {
                    var _v0 = e.StartVertice;
                    var _v1 = e.EndVertice;
                    int _v2 = V;

                    e.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[V]);
                    TensileTriangle _tri = new TensileTriangle();

                    _tri.SetupTriangle(_v0, _v1, _v2, savedPositions);

                    e.AddTriangle(_tri);
                    _tri.addTensileEdgeObjs(e);

                    _tri.addTensileVertexObjs(_edgeGraph.TensileVertexObjects[_v0]);
                    _tri.addTensileVertexObjs(_edgeGraph.TensileVertexObjects[_v1]);
                    _tri.addTensileVertexObjs(_edgeGraph.TensileVertexObjects[_v2]);

                    TensileEdge ne;

                    List<TensileEdge>NearEdges=new List<TensileEdge>(2);

                    int NearEdgeCount = 0;

                    foreach (var ve in _edgeGraph.TensileVertexObjects[V].GetConnectedEdgeObjs())
                    {
                        if (ve.StartVertice == _v0 || ve.EndVertice == _v0)
                        {
                            NearEdgeCount++;
                            NearEdges.Add(ve);
                        }
                        else if (ve.StartVertice == _v1 || ve.EndVertice == _v1)
                        {
                            NearEdgeCount++;
                            NearEdges.Add(ve);
                        }
                    }

                    if (NearEdgeCount == 1)
                    {
                        foreach (var ve in _edgeGraph.TensileVertexObjects[V].GetConnectedEdgeObjs())
                        {
                            if (ve.StartVertice == _v0 || ve.EndVertice == _v0)
                            {
                                //NearEdgeCount++;
                                ne = ve;
                                ne.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v1]);
                                ne.AddTriangle(_tri);
                                _tri.addTensileEdgeObjs(ne);

                                var eObj2 = Instantiate(TGEdgePrefab, transform);
                                eObj2.SetBarHolder(BarsHolder);
                                eObj2.SetStrHolder(StringHolder);

                                eObj2.SetupStructure(Scale, DefState, _v1, _v2, savedPositions);
                                eObj2.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v0]);
                                _edgeGraph.edgeGraph.AddEdge(_v1, _v2);

                                eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v1]);
                                eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v2]);

                                eObj2.AddTriangle(_tri);
                                _tri.addTensileEdgeObjs(eObj2);

                                _edgeGraph.TensileEdgeObjects.Add(eObj2);

                                _edgeGraph.TensileVertexObjects[_v2].AddEdgeObjs(eObj2);
                                _edgeGraph.TensileVertexObjects[_v1].AddEdgeObjs(eObj2);
                                _edgeGraph.TensileVertexObjects[_v1].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);


                                break;
                            }
                            else if (ve.StartVertice == _v1 || ve.EndVertice == _v1)
                            {
                                // NearEdgeCount++;
                                ne = ve;
                                ne.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v0]);
                                ne.AddTriangle(_tri);
                                _tri.addTensileEdgeObjs(ne);


                                var eObj2 = Instantiate(TGEdgePrefab, transform);
                                eObj2.SetBarHolder(BarsHolder);
                                eObj2.SetStrHolder(StringHolder);

                                eObj2.SetupStructure(Scale, DefState, _v0, _v2, savedPositions);
                                eObj2.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v1]);
                                _edgeGraph.edgeGraph.AddEdge(_v0, _v2);

                                eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v0]);
                                eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v2]);

                                eObj2.AddTriangle(_tri);
                                _tri.addTensileEdgeObjs(eObj2);

                                _edgeGraph.TensileEdgeObjects.Add(eObj2);
                                _edgeGraph.TensileVertexObjects[_v2].AddEdgeObjs(eObj2);
                                _edgeGraph.TensileVertexObjects[_v0].AddEdgeObjs(eObj2);
                                _edgeGraph.TensileVertexObjects[_v0].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);

                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }


                    if (NearEdgeCount == 0)
                    {
                        ne = Instantiate(TGEdgePrefab, transform); ;
                        ne.SetBarHolder(BarsHolder);
                        ne.SetStrHolder(StringHolder);

                        ne.SetupStructure(Scale, DefState, _v1, _v2, savedPositions);
                        ne.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v0]);
                        _edgeGraph.edgeGraph.AddEdge(_v1,_v2);

                        ne.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v1]);
                        ne.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v2]);

                        ne.AddTriangle(_tri);
                        _tri.addTensileEdgeObjs(ne);

                        _edgeGraph.TensileEdgeObjects.Add(ne);
                        _edgeGraph.TensileVertexObjects[_v2].AddEdgeObjs(ne);
                        _edgeGraph.TensileVertexObjects[_v1].AddEdgeObjs(ne);


                        var eObj2 = Instantiate(TGEdgePrefab, transform);
                        eObj2.SetBarHolder(BarsHolder);
                        eObj2.SetStrHolder(StringHolder);

                        eObj2.SetupStructure(Scale, DefState, _v0, _v2, savedPositions);
                        eObj2.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v1]);
                        _edgeGraph.edgeGraph.AddEdge(_v0, _v2);

                        eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v0]);
                        eObj2.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_v2]);

                        eObj2.AddTriangle(_tri);
                        _tri.addTensileEdgeObjs(eObj2);

                        _edgeGraph.TensileEdgeObjects.Add(eObj2);
                        _edgeGraph.TensileVertexObjects[_v2].AddEdgeObjs(eObj2);
                        _edgeGraph.TensileVertexObjects[_v0].AddEdgeObjs(eObj2);

                        _edgeGraph.TensileVertexObjects[_v0].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);
                        _edgeGraph.TensileVertexObjects[_v1].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);


                    }

                    if (NearEdgeCount == 2)
                    {
                        NearEdges[0].AddTriangle(_tri);
                        NearEdges[1].AddTriangle(_tri);

                        NearEdges[0].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v1]);
                        NearEdges[1].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_v0]);

                        _tri.addTensileEdgeObjs(NearEdges[0]);
                        _tri.addTensileEdgeObjs(NearEdges[1]);

                        _edgeGraph.TensileVertexObjects[_v0].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);
                        _edgeGraph.TensileVertexObjects[_v1].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_v2]);

                    }
                }
            }
            if (_edgeGraph.TensileVertexObjects[V].GetConnectedEdgeObjs().Count() == 6)
            {
                List<TensileEdge> ToTriEdges = new List<TensileEdge>(2);
                foreach (var ce in _edgeGraph.TensileVertexObjects[V].GetConnectedEdgeObjs())
                {
                    if (ce.GetConnecetedTriangles().Count() == 1)
                    {
                        ToTriEdges.Add(ce);
                    }
                }

                int _vt0;
                int _vt1;

                if (ToTriEdges[0].StartVertice == V)
                {
                    _vt0 = ToTriEdges[0].EndVertice;
                }
                else
                {
                    _vt0 = ToTriEdges[0].StartVertice;
                }
                if (ToTriEdges[1].StartVertice == V)
                {
                    _vt1 = ToTriEdges[1].EndVertice;
                }
                else
                {
                    _vt1 = ToTriEdges[1].StartVertice;
                }

                TensileTriangle TT = new TensileTriangle();

                TT.SetupTriangle(_vt0, _vt1, V, savedPositions);

                ToTriEdges[0].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_vt1]);
                ToTriEdges[1].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[_vt0]);

                ToTriEdges[0].AddTriangle(TT);
                ToTriEdges[1].AddTriangle(TT);

                var te = Instantiate(TGEdgePrefab, transform);
                te.SetBarHolder(BarsHolder);
                te.SetStrHolder(StringHolder);

                te.SetupStructure(Scale, DefState, _vt0, _vt1, savedPositions);
                te.AddDiagonalVertex(_edgeGraph.TensileVertexObjects[V]);
                _edgeGraph.edgeGraph.AddEdge(_vt0, _vt1);

                te.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_vt0]);
                te.AddConnectedVertex(_edgeGraph.TensileVertexObjects[_vt1]);

                te.AddTriangle(TT);
                TT.addTensileEdgeObjs(te);

                _edgeGraph.TensileEdgeObjects.Add(te);
                _edgeGraph.TensileVertexObjects[_vt1].AddEdgeObjs(te);
                _edgeGraph.TensileVertexObjects[_vt0].AddEdgeObjs(te);

                _edgeGraph.TensileVertexObjects[_vt0].AddConnectedVertObjInLayer(_edgeGraph.TensileVertexObjects[_vt1]);

            }

        }
    }


    private bool awake;

    private int sizeHolderX = 2;
    private int sizeHolderY = 1;
    private int sizeHolderZ = 5;


    void PresentationMode(int _x, int _z, int _y)
    {
        SetXSize(_x.ToString());
        SetYSize(_y.ToString());
        SetZSize(_z.ToString());
        initializeGraph();
    }

    public void OneUnit()
    {
        PresentationMode(2, 2, 1);
    }

    public void ThreeLayer()
    {
        PresentationMode(2, 2, 3);
    }

    public void OneLayer()
    {
        PresentationMode(6, 6, 1);
    }


    public void HexagonGroup()
    {
        PresentationMode(3, 3, 1);

        _edgeGraph.TensileVertexObjects[0].GraphToggle(false);
        _edgeGraph.TensileVertexObjects[6].GraphToggle(false);


        List<TensileEdge> tempEdge = new List<TensileEdge>();
        foreach (var te in _edgeGraph.TensileEdgeObjects)
        {
            tempEdge.Add(te);
        }

        foreach (var te in _edgeGraph.TensileVertexObjects[0].GetConnectedEdgeObjs())
        {
            te.GraphToggle(false);

            var t = te.GetConnecetedTriangles();
            foreach (var tt in t)
            {
                foreach (var ee in tt.GetConnectEdgesObjs())
                {
                    tempEdge.Remove(ee);

                }
                break;
            }
        }
        foreach (var te in _edgeGraph.TensileVertexObjects[6].GetConnectedEdgeObjs())
        {
            te.GraphToggle(false);
            var t = te.GetConnecetedTriangles();
            foreach (var tt in t)
            {
                foreach (var ee in tt.GetConnectEdgesObjs())
                {
                    tempEdge.Remove(ee);
                }
                break;
            }
        }



        foreach (var te in tempEdge)
        {
            te.SetAllState(2, 2);
        }
        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.SetAllState(2, 2);
        }

    }

    public void SetXSize(string _x)
    {

        int IntX;

        int.TryParse(_x, out IntX);

        if (IntX != 0)
        {
            int xValue = Mathf.Clamp(IntX, 2, 15);
            sizeHolderX = xValue;
        }
    }
    public void SetYSize(string _y)
    {
        int IntY;

        int.TryParse(_y, out IntY);



        if (IntY != 0)
        {
            int yValue = Mathf.Clamp(IntY, 1, 15);
            sizeHolderY = yValue;
        }

    }
    public void SetZSize(string _z)
    {
        int IntZ;

        int.TryParse(_z, out IntZ);

        if (IntZ != 0)
        {
            int zValue = Mathf.Clamp(IntZ, 2, 15);
            sizeHolderZ = zValue;
        }
    }

    private void UpdateSize()
    {
        SegmentCount = sizeHolderX;
        LayerCount = sizeHolderY;
    }


    private bool displayGraph = true;
    public void GraphToggleAll()
    {
        if (displayGraph == false)
        {
            displayGraph = true;
        }
        else
        {
            displayGraph = false;
        }

        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.GraphToggle(displayGraph);
        }

        foreach (var te in _edgeGraph.TensileEdgeObjects)
        {
            te.GraphToggle(displayGraph);
        }

        foreach (var ve in verticalEdges)
        {
            ve.GraphToggle(displayGraph);
        }
    }


    private void Awake()
    {
        BarsHolder = new GameObject().transform;
        BarsHolder.parent = transform;

        BarsHolder.name = "Bars";

        StringHolder = new GameObject().transform;

        StringHolder.parent = transform;
        StringHolder.name = "Strings";

        initializeGraph();


        //btn.interactable = false;
        // btn2.interactable = false;
    }

    public bool GraphReady()
    {
        return awake;
    }

    public void initializeGraph()
    {
        if (awake == false)
        {
            UpdateSize();

          
            _edgeGraph = new SharedEdgeGraph();
            _edgeGraph.Initialize(EdgeGraph.Factory.CreateDecreasingTriangleGrid(SegmentCount, LayerCount));

            currentPositions = new List<Vector3>(_edgeGraph.edgeGraph.VertexCount);
            savedPositions = new List<Vector3>(_edgeGraph.edgeGraph.VertexCount);

            savedPositions.AddRange(SetVertexPosition());
            currentPositions.AddRange(savedPositions);
            _edgeGraph.TensileVertexObjects.AddRange(CreatVertexObjects());
            _edgeGraph.TensileEdgeObjects.AddRange(CreatEdgeObjects());
            _edgeGraph.TensileTriangle.AddRange(CreateTriangle());
            ConnectEdgeToVert();
            ConnectVertexs();
            Triangle = _edgeGraph.TensileTriangle.ToArray();

            enableVertexes();

            getNoise();

            transform.position = new Vector3(-SegmentCount * 0.5f, -LayerCount * 0.5f, -SegmentCount * 0.5f);
            awake = true;

            GetComponent<TriangleBoundryGrowthManager>().initializeGrowther();
            btn.interactable = true;
            btn2.interactable = true;
        }
        else
        {
            ResetGraph();

            initializeGraph();
        }

    }

    void enableVertexes()
    {
        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.EnableVertex();
        }
    }


    void Start()
    {

    }


    private List<float> NoiseX;
    private List<float> NoiseY;
    private List<float> NoiseZ;
    private float Degree = 0f;
    private bool LerpOnOff;

    public void LerpToggle()
    {


        if (LerpOnOff == false)
        {
            LerpOnOff = true;
        }
        else
        {
            LerpOnOff = false;
        }
    }

    private int frame = 0;
    public void LerpPosition()
    {

        if (LerpOnOff == true)
        {
            if (frame < currentPositions.Count - 1)
            {
                List<Vector3> toLerpPosition = new List<Vector3>(currentPositions.Count);

                for (int i = 0; i < currentPositions.Count; i++)
                {
                    var p = savedPositions[i];

                    float PDx = NoiseX[i] * Degree;
                    float PDy = NoiseY[i] * Degree;
                    float PDz = NoiseZ[i] * Degree;

                    float x = p.x + PDx;
                    float y = p.y + PDy;
                    float z = p.z + PDz;

                    p = new Vector3(x, y, z);
                    toLerpPosition.Add(p);
                }

                for (int i = 0; i < currentPositions.Count; i++)
                {
                    currentPositions[i] = Vector3.Lerp(currentPositions[i], toLerpPosition[i], Time.deltaTime * 1.5f);

                    var d = currentPositions[i] - toLerpPosition[i];

                    if (d.magnitude < 0.003f)
                    {
                        LerpOnOff = false;
                        updateNoise();
                        LerpOnOff = true;
                    }
                }
            }
            else
            {
                //updateNoise();
            }

        }
    }

    void getNoise()
    {
        NoiseX = new List<float>(currentPositions.Count);
        NoiseY = new List<float>(currentPositions.Count);
        NoiseZ = new List<float>(currentPositions.Count);
        for (int i = 0; i < currentPositions.Count; i++)
        {

            float valueX = Random.Range(-0.3f, 0.3f);
            float valueY = Random.Range(-0.3f, 0.3f);
            float valueZ = Random.Range(-0.3f, 0.3f);

            NoiseX.Add(valueX);
            NoiseY.Add(valueY);
            NoiseZ.Add(valueZ);


        }
    }

    void updateNoise()
    {
        for (int i = 0; i < currentPositions.Count; i++)
        {

            float valueX = Random.Range(-0.3f, 0.3f);
            float valueY = Random.Range(-0.3f, 0.3f);
            float valueZ = Random.Range(-0.3f, 0.3f);

            NoiseX[i] = valueX;
            NoiseY[i] = valueY;
            NoiseZ[i] = valueZ;
        }
    }

    public void adjustPosition(float _degree)
    {
        Degree = _degree;
        if (awake == true && LerpOnOff == false)
        {
            for (int i = 0; i < currentPositions.Count; i++)
            {
                var p = savedPositions[i];



                float PDx = NoiseX[i] * _degree;
                float PDy = NoiseY[i] * _degree;
                float PDz = NoiseZ[i] * _degree;

                float x = p.x + PDx;
                float y = p.y + PDy;
                float z = p.z + PDz;

                p = new Vector3(x, y, z);

                currentPositions[i] = p;
            }
        }
    }



    IEnumerable<TensileVertex> CreatVertexObjects()
    {
        List<TensileVertex> Obj = new List<TensileVertex>();
        int index = 0;

        foreach (var p in currentPositions)
        {
            var tObj = Instantiate(TGVertexPrefab, transform);
            tObj.SetBarHolder(BarsHolder);
            tObj.SetStrHolder(StringHolder);
            tObj.transform.localPosition = p;

            tObj.SetupStructure(Scale, DefState, index);
            Obj.Add(tObj);
            index++;
        }
        return Obj;
    }

    IEnumerable<TensileEdge> CreatEdgeObjects()
    {
        List<TensileEdge> EObj = new List<TensileEdge>();

        var vc = _edgeGraph.edgeGraph.VertexCount;

        EdgeObj2d = new TensileEdge[3 * vc, 3 * vc];

        var graph = _edgeGraph.edgeGraph;

        for (int i = 0; i < graph.EdgeCount; i++)
        {
            int S = graph.GetStartVertex(i);
            int E = graph.GetEndVertex(i);

            int V = S - SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount);

            if (E != V)
            {
                var eObj = Instantiate(TGEdgePrefab, transform);

                eObj.SetBarHolder(BarsHolder);
                eObj.SetStrHolder(StringHolder);
                eObj.SetupStructure(Scale, DefState, S, E, savedPositions);
                EObj.Add(eObj);
                EdgeObj2d[S, E] = eObj;
            }
            else
            {
                var eObj = Instantiate(TGEdgePrefab, transform);
                eObj.SetBarHolder(BarsHolder);
                eObj.SetStrHolder(StringHolder);
                eObj.SetupStructure(Scale, DefState, S, E, savedPositions);
                verticalEdges.Add(eObj);
            }
        }
        return EObj;
    }

    IEnumerable<TensileTriangle> CreateTriangle()
    {
        List<TensileTriangle> T = new List<TensileTriangle>();


        int xSize = SegmentCount - 1;
        int zSize = SegmentCount - 1;
        int[] triangleVerts = new int[xSize * zSize * 3 * LayerCount];
        for (int y = 0, ti = 0, vi = 0; y < LayerCount; y++, vi += 1)
        {
            for (int z = 0; z < zSize; z++, vi++)
            {
                for (int x = 0; x < xSize-z; x++, vi++)
                {          
                    {
                        if (x < xSize - z - 1)
                        {
                            triangleVerts[ti] = vi;
                            triangleVerts[ti + 1] = vi + xSize - z + 1;
                            triangleVerts[ti + 2] = vi + 1;
                            triangleVerts[ti + 3] = vi + 1;
                            triangleVerts[ti + 4] = vi + xSize - z + 1;
                            triangleVerts[ti + 5] = vi + xSize - z + 2;
                            ti += 6;
                        }
                        else
                        {
                            triangleVerts[ti] = vi;
                            triangleVerts[ti + 1] = vi + xSize - z + 1;
                            triangleVerts[ti + 2] = vi + 1;
                            ti += 3;
                        }
                   
                    }
                }
            }
        }

        TriangleVerts = triangleVerts;
        for (int i = 0; i < triangleVerts.Length; i += 3)
        {
            TensileTriangle Tri = new TensileTriangle();
            int v0 = triangleVerts[i];
            int v1 = triangleVerts[i + 1];
            int v2 = triangleVerts[i + 2];

            //print(v0 + "," + v1 + "," + v2);

            Tri.SetupTriangle(v0, v1, v2, savedPositions);
            foreach (var te in Tri.GetTriEdges())
            {
                int S = te[0];
                int E = te[1];

                int D = Tri.GiveDiagonalVertice(S, E);

                Tri.addTensileEdgeObjs(EdgeObj2d[S, E]);
                EdgeObj2d[S, E].AddTriangle(Tri);
                EdgeObj2d[S, E].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[D]);

            }
            T.Add(Tri);
        }
        return T;
    }

    void ConnectEdgeToVert()
    {
        var Verts = _edgeGraph.TensileVertexObjects;

        foreach (var Eg in _edgeGraph.TensileEdgeObjects)
        {
            int S = Eg.StartVertice;
            int E = Eg.EndVertice;

            Verts[S].AddEdgeObjs(Eg);
            Verts[E].AddEdgeObjs(Eg);
            Eg.AddConnectedVertex(Verts[S]);
            Eg.AddConnectedVertex(Verts[E]);
        }

        foreach (var ve in verticalEdges)
        {
            int S = ve.StartVertice;
            int E = ve.EndVertice;

            ve.AddConnectedVertex(Verts[S]);
            ve.AddConnectedVertex(Verts[E]);
        }
    }

    void ConnectVertexs()
    {
        var Verts = _edgeGraph.TensileVertexObjects;

        foreach (var V in Verts)
        {
            int i = V.Index;
            if (i >= SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount))
            {
                int blw = i - SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount);
                V.SetVertBelow(Verts[blw]);
            }


            if (i < _edgeGraph.edgeGraph.VertexCount - SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount))
            {
                int abv = i + SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount);
                V.SetVertAbove(Verts[abv]);
            }

            var cv = _edgeGraph.edgeGraph.GetConnectedVertices(i);

            foreach (var v in cv)
            {
                if (v == i - SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount) || v == i + SegmentCount * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount))
                {
                    continue;
                }
                else
                {
                    V.AddConnectedVertObjInLayer(Verts[v]);
                }
            }
        }
    }


    IEnumerable<Vector3> SetVertexPosition()
    {
        List<Vector3> _positions = new List<Vector3>();
        for (int y = 0; y < LayerCount; y++)
        {
            // add even row position
            for (int z = 0; z < SegmentCount; z++)
            {
                for (int x = 0; x < SegmentCount-z; x++)
                {
                    float w = x * Scale + 0.5f * Scale * z;
                    float h = z * Mathf.Sqrt(Mathf.Pow(Scale, 2) - Mathf.Pow(Scale * 0.5f, 2));
                    float t = y * (0.42552f / 1f) * Scale;
                    _positions.Add(new Vector3(w, t, h));
                }
            }
        }

        return _positions;
    }

    public int[,,] remap3D()
    {
        int[,,] remap = new int[SegmentCount, SegmentCount, LayerCount];

        int index = 0;
        for (int y = 0; y < LayerCount; y++)
        {
            for (int z = 0; z < SegmentCount; z++)
            {
                for (int x = 0; x < SegmentCount-z; x++)
                {
                    index++;
                    remap[x, z, y] = index-1;
                }
            }
        }
        return remap;
    }

    public TensileVertex[,,] Vert3D()
    {
        TensileVertex[,,] v3d = new TensileVertex[SegmentCount, SegmentCount, LayerCount];
        var VObj = Grid().TensileVertexObjects;

        for (int y = 0; y < LayerCount; y++)
        {
            for (int z = 0; z < SegmentCount; z++)
            {
                for (int x = 0; x < SegmentCount-z; x++)
                {
                    int id = remap3D()[x, z, y];
                    v3d[x, z, y] = VObj[id];
                }
            }
        }

        return v3d;
    }

    // Use this for initialization




    // Update is called once per frame
    void Update()
    {
        if (awake == true)
        {
            updateVertexPosition();
            updateTriangle();
            LerpPosition();
        }
    }

    void updateVertexPosition()
    {
        //foreach (var tv in _edgeGraph.TensileVertexObjects)
        //{
        //    int v = tv.Index;
        //    tv.transform.localPosition = currentPositions[v];
        //}

        for (int i = 0; i < _edgeGraph.edgeGraph.VertexCount; i++)
        {
            _edgeGraph.TensileVertexObjects[i].transform.localPosition = currentPositions[i];
        }
    }

    void updateTriangle()
    {
        foreach (var tt in _edgeGraph.TensileTriangle)
        {
            tt.updateTriangle(currentPositions);
        }
    }

    public void ResetGraph()
    {
        if (awake == true)
        {
            foreach (var tv in _edgeGraph.TensileVertexObjects)
            {
                tv.SetState(0);
                DestroyImmediate(tv.gameObject);
            }

            foreach (var te in _edgeGraph.TensileEdgeObjects)
            {
                te.SetState(0);
                DestroyImmediate(te.gameObject);
            }

            foreach (var ve in verticalEdges)
            {
                ve.SetState(0);
                DestroyImmediate(ve.gameObject);
            }


            _edgeGraph.TensileVertexObjects.Clear();
            _edgeGraph.TensileEdgeObjects.Clear();
            _edgeGraph.TensileTriangle.Clear();
            verticalEdges.Clear();

            btn.interactable = false;
            btn2.interactable = false;
            LerpOnOff = false;
            btn.GetComponent<ButtonImageHandler>().SetTexture(true);
            btn2.GetComponent<ButtonImageHandler>().SetTexture(true);
        }

        awake = false;
    }
}

