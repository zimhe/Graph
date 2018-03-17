using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using SpatialSlur.SlurData;
using UnityEngine;

public class HexigonGrowthManager : MonoBehaviour
{
    private SharedEdgeGraph HexigonGrid;
    private HexigonGraphManager GraphManager;
    private EdgeGraph _graph;
    List<TensileVertex> TenVertex;
    List<TensileEdge> TenEdge;

    private Queue<int> _sourceQueue;
    private Queue<int> _queue;
    private PriorityQueue<float, int> _proQueue = new PriorityQueue<float, int>();
    private List<int> Sources;
    private bool Grow = false;  

    private int Frame = 0;

    private int[] Depth;

    int S_Inx = 0;

    private int countX;
    private int countY;
    private int countZ;


    void  getShortestSourcePath()
    {
        Queue<int> _tempQueue = new Queue<int>();
        _sourceQueue = new Queue<int>();

        List<int> _tempSources = new List<int>(Sources.ToArray());

        //_sourceQueue.Enqueue(Sources [0]);
        _tempQueue.Enqueue(_tempSources [0]);
        _tempSources.Remove(_tempSources[0]);

        for (int i = 0; i <Sources .Count -1; i++)
        { 
            
            int minIndex = 0;
            float minDistance = float.MaxValue;

            int v = _tempQueue.Dequeue();

            foreach (var src in _tempSources)
            {
                var p0 = TenVertex[v].transform.position;
                var p1 = TenVertex[src].transform.position;
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

   

    // Use this for initialization
    void Start()
    {
        //HexigonGrid = transform.GetComponent<ShiftedGraphManager>().HexigonGrid();
        //HexigonGrid = transform.GetComponent<GraphManager>().HexigonGrid();
        HexigonGrid = transform.GetComponent<HexigonGraphManager>()._hexGrid();
        GraphManager = transform.GetComponent<HexigonGraphManager>();
        //HexigonGrid = transform.GetComponent<HexigonGraphManager>().();

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

    void ResetGrowth()
    {
        _queue.Clear();

        foreach (var t in TenVertex)
        {
            t.SetState(0);
        }

        foreach (var e in TenEdge)
        {
            e.SetState(0);
        }

        Grow = false;
        Frame = 0;
        Sources.Clear();
        SourceCreated = false;
        _sourceQueue.Clear();
        _proQueue = new PriorityQueue<float, int>();
        S_Inx = 0;

    }


    void UpdateGrowth()
    {
        if (Grow == true)
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

            }
        }
    }

    void ChangeLook(int _subState)
    {
        foreach (var t in TenVertex)
        {
            if (t.State!=0)
            {
                t.SetState(2);
                t.SetSubState(_subState);
            }
        }

        foreach (var TE in TenEdge)
        {
            if (TE.State != 0)
            {
                TE.SetState(2);
                TE.SetSubState(_subState);
            }
        }
    }

    void getDepth()
    {
        GraphUtil.GetVertexEdgeDepths(_graph, SourceRO(), Depth);
    }



    void CreatSourceRdm()
    {
        Sources = new List<int>();
        System.Random Random = new System.Random();
        int SourceCount = (int)TenVertex.Count / 50;
        for (int i = 0; i < SourceCount; i++)
        {
            int _source = Random.Next(0, TenVertex.Count);

            Sources.Add(_source);
        }
        foreach (int src in Sources)
        {
            _queue.Enqueue(src);
            TenVertex[src].SetState(1);
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
        if (Grow == true&&_sourceQueue .Count !=0)
        {
          
           //for (int i = 0; i < _sourceQueue.Count; i++)
            {
                float key;
                int vertex;
                _proQueue.RemoveMin(out key, out vertex);
                var T = _sourceQueue.ToArray();
                foreach (var vi in _graph.GetConnectedVertices(vertex))
                {
                    var target = T[S_Inx];

                    if (vi == target)
                    {
                        print("reached");

                        _proQueue =new PriorityQueue<float, int>();
                        _proQueue.Insert(0f, target);

                  
                        if (S_Inx < T.Length-1)
                        {
                            S_Inx++;
                            target = T[S_Inx];
                        }
                        else
                        {
                            Grow = false;
                        }
                    }

                    if (Depth[vi] > 4)
                    {
                        //return;
                    }
                    var t = TenVertex[vi];
                    int nc = NeighborCount(vi);
                    if ( nc <4)
                    {
                        _proQueue.Insert(GetKey(vi, target), vi);
                        if (nc < 3)
                        {
                            if (t.State == 0)
                                t.SetState(2);
                        }
                    }
                }

                foreach (var E in HexigonGrid.TensileEdgeObjects)
                {
                    int St = E.Start;
                    int Ed = E.End;
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreatSourceRdm();

            //SourceCreated = true;
            getShortestSourcePath();
            getDepth();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreatSource();
            SourceCreated = true;
        }

        if (Input.GetKeyDown((KeyCode.Space)))
        {
            if (Grow == false)
            {
                Grow = true;
            }
            else
            {
                Grow = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetGrowth();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeLook(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeLook(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeLook(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeLook(0);
        }

        // UpdateGrowth();

        if (SourceCreated == true)
        {
            if (Frame < countY - 1)
            {
                CalculateSource();
                SaveSource();
                Frame++;
            }
            else
            {
                getShortestSourcePath();
                getDepth();
            }
        }
        UpdateDirectedGrowth();
    }
}

