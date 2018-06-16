using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using SpatialSlur.SlurData;
using UnityEngine;
using UnityEngine.UI;

public class HexigonGrowthManager : MonoBehaviour
{
    private SharedEdgeGraph HexigonGrid;
    private HexigonGraphManager GraphManager;
    private EdgeGraph _graph;
    List<TensileVertex> TenVertex;
    List<TensileEdge> TenEdge;

    [SerializeField] private ButtonImageHandler directBtn;

    private bool GraphDone;

    private Queue<int> _sourceQueue;
    private Queue<int> _queue;
    private PriorityQueue<float, int> _proQueue = new PriorityQueue<float, int>();
    private List<int> Sources;
    private bool GrowDirect = false;  

    private int Frame = 0;

    private int[] Depth;

    int S_Inx = 0;

    private int countX;
    private int countY;
    private int countZ;


    void  getShortestSourcePath()
    {
        if (GraphDone == true)
        {
            Queue<int> _tempQueue = new Queue<int>();
            _sourceQueue = new Queue<int>();

            List<int> _tempSources = new List<int>(Sources.Count);
            _tempSources.AddRange(Sources);

            var next = _tempSources[0];
            //_sourceQueue.Enqueue(Sources [0]);
            _tempQueue.Enqueue(next);
            _tempSources.Remove(next);



            for (int i = 0; i < Sources.Count - 1; i++)
            {

                int minIndex = 0;
                float minDistance = float.MaxValue;

                int v = _tempQueue.Dequeue();

                foreach (var src in _tempSources)
                {
                    var p0 = TenVertex[v].transform.localPosition;
                    var p1 = TenVertex[src].transform.localPosition;
                    var d = p1 - p0;
                    var _d = d.magnitude;

                    if (minDistance > _d && _d != 0)
                    {
                        minDistance = _d;
                        minIndex = src;
                    }
                }
                _tempQueue.Enqueue(minIndex);
                _sourceQueue.Enqueue(minIndex);
                _tempSources.Remove(minIndex);
            }

            _proQueue.Insert(0f, Sources[0]);
        }
    }

   

    // Use this for initialization
    void Start()
    {
        //initializeGrowther();
    }

    public void initializeGrowther()
    {
        GraphManager = transform.GetComponent<HexigonGraphManager>();

        GraphDone = GraphManager.GraphReady();

        displayed = false;

        HexigonGrid = GraphManager._hexGrid();
        _graph = HexigonGrid.edgeGraph;
        Depth = new int[_graph.VertexCount];
        TenVertex = HexigonGrid.TensileVertexObjects;
        TenEdge = HexigonGrid.TensileEdgeObjects;
        _queue = new Queue<int>();
        _sourceQueue = new Queue<int>();


        countX = GraphManager.GetCountX();
        countY = GraphManager.GetCountY();
        countZ = GraphManager.GetCountZ();
    }



    public void ResetGrowth()
    {
        _queue.Clear();

        foreach (var t in TenVertex)
        {
           t.SetAllState(0,0);
        }

        foreach (var e in TenEdge)
        {
          e.SetAllState(0,0);
        }

        GrowDirect = false;
        Frame = 0;
        if (Sources != null)
        {
            Sources.Clear();
        }

        if (_sourceQueue != null)
        {
            _sourceQueue.Clear();
        }
        SourceCreated = false;
        _proQueue = new PriorityQueue<float, int>();
        S_Inx = 0;
        displayed = false;
    }


    void UpdateGrowth()
    {
        if (GrowSpread == true)
        {
            if (_queue.Count == 0)
            {
                return;
            }

            foreach (int v in _graph.GetConnectedVertices(_queue.Dequeue()))
            {
                var t = TenVertex[v];
                if (t.State != 0)
                {
                    continue;
                }

                var depth = Depth[v];

                if (depth > 3)
                {
                    return;
                }

                int nc = NeighborCount(v);
                /*  if (t.State !=0)
                  {
                      if (nc >= 1 && nc <= 2)
                      {
                          t.SetState(2);
                          _queue.Enqueue(v);
                      }
                      if (nc > 4)
                      {
                          t.SetState(0);
                      }
                  }*/
                if (t.State == 0)
                {
                    if (nc >= 1 && nc <= 2)
                    {
                        t.SetState(2);
                        _queue.Enqueue(v);
                    }
                }

                //t.UpdateToFuture();
                Frame++;
                foreach (var E in HexigonGrid.TensileEdgeObjects)
                {
                    int St = E.StartVertice;
                    int Ed = E.EndVertice;
                    int SState = HexigonGrid.TensileVertexObjects[St].State;
                    int EState = HexigonGrid.TensileVertexObjects[Ed].State;

                    if (SState != 0 && EState != 0)
                    {
                        E.SetState(2);
                    }
                }

            }
        }
    }

    public void ChangeLook(int _subState)
    {
        if (GraphDone == true)
        {
            foreach (var t in TenVertex)
            {
                if (t.State != 0)
                {
                    if (t.State != 2)
                    {
                        t.SetAllState(2, _subState);
                    }
                    else
                    {
                        t.SetSubState(_subState);
                    }

                }
            }
            foreach (var TE in TenEdge)
            {
                if (TE.State != 0)
                {
                    if (TE.State != 2)
                    {
                        TE.SetAllState(2, _subState);
                    }
                    else
                    {
                        TE.SetSubState(_subState);
                    }
                }
            }
        }
    }

    void getDepth()
    {
        if (GraphDone == true)
        {
            GraphUtil.GetVertexEdgeDepths(_graph, SourceRO(), Depth);
        }
    }

    void CreatSourceRdm(int _percentage)
    {
        if (GraphDone == true)
        {
            System.Random Random = new System.Random();
            int SourceCount = Mathf.FloorToInt(TenVertex.Count * (_percentage * 0.01f));


            if (Sources != null && Sources.Count != 0)
            {

                foreach (var tv in TenVertex)
                {
                    if (tv.State != 0)
                    {
                       tv.SetAllState(0,0);
                    }
                }
                foreach (var te in TenEdge)
                {
                    if (te.State != 0)
                    {
                        te.SetAllState(0,0);
                    }
                }

                Sources = new List<int>(SourceCount);
            }
            else
            {
                Sources = new List<int>(SourceCount);
            }

            for (int i = 0; i <= SourceCount; i++)
            {

                int _source = Random.Next(0, TenVertex.Count);

                Sources.Add(_source);
                TenVertex[_source].SetState(1);
                _queue.Enqueue(_source);
            }
        }
    }


    void CreatSource()
    {
        Sources = new List<int>();
        System.Random Random = new System.Random();
        //int SourceCount = (int)TenVertex.Count / 200;

        

        var Vobj = GraphManager.Vert3D();

        int Frame = 0;

        for (int z = 0; z < countZ; z++)
        {
            for (int x = 0; x < countX; x++)
            {
                int vert = GraphManager.remap3D()[x, z, 0];

                var state = Random.Next(0, 2);

                TensileVertex CurObj = Vobj[x, z, 0];

                CurObj.SetState(state);
            }
        }
    }
    void CalculateSource()
    {
        var Vobj = GraphManager.Vert3D();
        for (int z = 0; z < countZ; z++)
        {
            for (int x = 0; x < countX; x++)
            {
                int vert = GraphManager.remap3D()[x, z, 0];

                TensileVertex CurObj = Vobj[x, z, 0];

                int NBCount = 0;
                foreach (int v in HexigonGrid.edgeGraph.GetConnectedVertices(vert))
                {
                    int State = TenVertex[v].State;
                    NBCount += State;
                }

                int a = 1;
                int b = 2;
                int c = 1;
                int d = 1;

                if (CurObj.State == 1)
                {
                    if (NBCount < a)
                    {
                        CurObj.FutureState = 0;
                    }

                    if (NBCount >= a && NBCount <= b)
                    {
                        CurObj.FutureState = 1;
                    }
                }
                else if (CurObj.State == 0)
                {
                    if (NBCount >= c && NBCount <= d)
                    {
                        CurObj.FutureState = 1;
                    }
                }

                if (CurObj.GetAge() > 0)
                {
                    CurObj.FutureState = 0;
                }
            }
        }
    }

    void SaveSource()
    {
        var Vobj = GraphManager.Vert3D();

        for (int z = 0; z < countZ; z++)
        {
            for (int x = 0; x < countX; x++)
            {
                if (Frame < countY - 1)
                {
                    Vobj[x, z, 0].UpdateToFuture();
                    Vobj[x, z, 0].FutureState = 0;

                    int St = Vobj[x, z, 0].State;

                    TensileVertex Sobj = Vobj[x, z, Frame + 1];

                     Sobj.SetState(St);

                    if (St == 1)
                    {
                        int S = GraphManager.remap3D()[x, z, Frame + 1];

                        Sources.Add(S);
                    }
                }
            }
        }
        foreach (int src in Sources)
        {
            _queue.Enqueue(src);
             //TenVertex[src].SetState(1);
        }
    }

    bool displayed = false;


    [SerializeField] Button  turnOnAllBtn;
    public void TurnOnAll()
    {

        if (GraphDone == true&&GrowSpread==false&&GrowDirect==false)
        {
            if (displayed == false)
            {
                foreach (var tv in TenVertex)
                {
                    tv.SetAllState(2, 1);
                }

                foreach (var te in TenEdge)
                {
                    te.SetAllState(2, 1);
                }
                displayed = true;
               turnOnAllBtn.GetComponent<ButtonImageHandler>().SetTexture(true);
            }
            else
            {
                foreach (var tv in TenVertex)
                {
                    tv.SetAllState(0, 0);
                }

                foreach (var te in TenEdge)
                {
                    te.SetAllState(0, 0);
                }
                displayed = false;
                turnOnAllBtn.GetComponent<ButtonImageHandler>().SetTexture(false);

            }
        }
    }
    public void TurnOnAllPre()
    {
        if (GraphDone == true && GrowSpread == false && GrowDirect == false)
        {
            if (displayed == false)
            {
                foreach (var tv in TenVertex)
                {
                    tv.SetAllState(2, 2);
                }

                foreach (var te in TenEdge)
                {
                    te.SetAllState(2, 2);
                }
                displayed = true;
            }
            else
            {
                foreach (var tv in TenVertex)
                {
                    tv.SetAllState(0, 0);
                }

                foreach (var te in TenEdge)
                {
                    te.SetAllState(0, 0);
                }
                displayed = false;
            }
        }
    }





    IEnumerable<int> SourceRO()
    {
        return Sources;
    }


    int NeighborCount(int vertex)
    {
        int count = 0;
        foreach (var v in _graph.GetConnectedVertices(vertex))
        {
            if (TenVertex[v].State != 0)
            {
                count++;
            }
        }
        return count;
    }

    float GetKey(int vertex, int target)
    {
        float _key = 0f;

        var p0 = TenVertex[vertex].transform.position;
        var p1 = TenVertex[target].transform.position;

        _key = Mathf .Abs(( p1 - p0).magnitude);

        return _key;
    }

    void UpdateDirectedGrowth()
    {
        if (GrowDirect == true&&_sourceQueue .Count !=0)
        {
          
           //for (int i = 0; i < _sourceQueue.Count; i++)
            {
                float key;
                int vertex;
                _proQueue.RemoveMin(out key, out vertex);
                var T = _sourceQueue.ToArray();


                foreach (var vi in _graph.GetConnectedVertices(vertex))
                {
                   
                    int target = T[S_Inx];

                
                    if (vi == target)
                    {
                        _proQueue = new PriorityQueue<float, int>();
                        _proQueue.Insert(0f, target);

                        if (S_Inx < T.Length - 1)
                        {
                            S_Inx++;
                            target = T[S_Inx];
                        }
                        else
                        {
                            GrowDirect = false;
                            directBtn.SetTexture(true);
                            print("done");
                        }
                    }

                    var t = TenVertex[vi];
                    int nc = NeighborCount(vi);
                    if ( nc <4)
                    {
                        if (nc < 3)
                        {
                            if (t.State == 0)
                                t.SetState(2);
                        }
                        _proQueue.Insert(GetKey(vi, target), vi);
                    }
                }

                foreach (var E in HexigonGrid.TensileEdgeObjects)
                {
                    int St = E.StartVertice;
                    int Ed = E.EndVertice;
                    int SState = HexigonGrid.TensileVertexObjects[St].State;
                    int EState = HexigonGrid.TensileVertexObjects[Ed].State;

                    if (SState != 0 && EState != 0)
                    {
                        E.SetState(2);
                    }
                }

                for (int i = 0; i < _graph.VertexCount; i++)
                {
                    int LayerCount = countX * countZ;

                    var EgsCur = TenVertex[i].GetConnectedEdgeObjs().ToArray();

                    int edgeCount = 0;


                    foreach (var edge in EgsCur)
                    {
                        edgeCount += edge.State;
                    }
                    if (i >=LayerCount)
                    {
                        int below = i - LayerCount;
                        if (TenVertex[i].State != 0 &&edgeCount<2)
                        {
                            var EgsBelow = TenVertex[below].GetConnectedEdgeObjs().ToArray();

                            foreach (var e in EgsBelow)
                            {
                                if (e.State != 0)
                                {
                                    int id = Array.IndexOf(EgsBelow, e);
                                    EgsCur[id].SetState(2);
                                }
                            }
                        }
                    }
                    if (i < _graph.VertexCount-LayerCount)
                    {
                        int up = i + LayerCount;
                        if (TenVertex[i].State != 0 &&edgeCount<2)
                        {
                            var EgsUp = TenVertex[up].GetConnectedEdgeObjs().ToArray();

                            foreach (var e in EgsUp)
                            {
                                if (e.State != 0)
                                {
                                    int id = Array.IndexOf(EgsUp, e);
                                    EgsCur[id].SetState(2);
                                }
                            }
                        }
                    }

                    if (i >= LayerCount && i < _graph.VertexCount - LayerCount)
                    {
                        int up = i + LayerCount;
                        int below = i - LayerCount;

                        if (TenVertex[up].State != 0 && TenVertex[below].State!=0)
                        {
                            var EgsUp = TenVertex[up].GetConnectedEdgeObjs().ToArray();

                            foreach (var e in EgsUp)
                            {
                                if (e.State != 0)
                                {
                                    int id = Array.IndexOf(EgsUp, e);
                                    EgsCur[id].SetState(2);
                                }
                            }

                            var EgsBelow = TenVertex[below].GetConnectedEdgeObjs().ToArray();
                            foreach (var e in EgsBelow)
                            {
                                if (e.State != 0)
                                {
                                    int id = Array.IndexOf(EgsBelow, e);
                                    EgsCur[id].SetState(2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame


    private bool SourceCreated;


    public void GenerateSource()
    {
        CreatSourceRdm(3);
        getShortestSourcePath();
        getDepth();
    }

    void Update()
    {
     

        if (GraphDone == true)
        {
            UpdateGrowth();
            UpdateDirectedGrowth();
        }
    }

    private bool GrowSpread;

    public void GrowthToggleDirect()
    {
        GrowSpread = false;
        if (GrowDirect == false)
        {
            GrowDirect = true;
        }
        else
        {
            GrowDirect = false;
        }
    }
    public void GrowthToggleSpread()
    {
        GrowDirect = false;
        if (GrowSpread == false)
        {
            GrowSpread = true;
           

        }
        else
        {
            GrowSpread = false;
        }
    }

}

