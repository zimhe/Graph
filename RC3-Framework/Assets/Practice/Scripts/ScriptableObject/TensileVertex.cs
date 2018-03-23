using System;
using System.Collections;
using System.Collections.Generic;
using SpatialSlur.SlurCore;
using SpatialSlur.SlurData;
using UnityEngine;
using Object = UnityEngine.Object;

public class TensileVertex : MonoBehaviour
{
    [SerializeField] private GameObject VertexIndicatorPFB;
    [SerializeField] private GameObject ShapePFB;

    [SerializeField] private Color col;

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

    List<Vector3> Positions = new List<Vector3>(6);
    private Vector3[] bar0pos = new Vector3[2];
    private Vector3[] bar1pos = new Vector3[2];
    private Vector3[] bar2pos = new Vector3[2];
    private Vector3[] bar3pos = new Vector3[2];
    private Vector3[] bar4pos = new Vector3[2];
    private Vector3[] bar5pos = new Vector3[2];

    private GameObject GraphHolder;

    List<GameObject> ObjHolder = new List<GameObject>();

    public int State { get; set; }

    public int FutureState { get; set; }

    public int SubState { get; set; }

    public int Index { get; set; }

    public int Depth { get; set; }

    private int Scale;

    private int Age;

    private List<TensileVertex> VertAbove = new List<TensileVertex>(1);

    private List<TensileVertex> VertBelow = new List<TensileVertex>(1);

    List<TensileVertex> connectedVertInLayerDefOrder = new List<TensileVertex>(6);

    List<TensileVertex> connectedVertexsInCounterClockwise = new List<TensileVertex>(6);

    [SerializeField] private StaticBar barPrefab;
    [SerializeField] private StaticStrings stringPrefab;

    StaticBar [] allBars=new StaticBar[6];
    private float thickness = 0.005f;

    List<TensileEdge> connectedEdgeObjs=new List<TensileEdge>(6);

    private bool Enabled;

    private bool ShowGraph = true;





    void Start()
    {
        //EnableVertex();
    }

    public void EnableVertex()
    {
        reoderConnectedVertex();

        setBarPosition();

        Enabled = true;
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

    private string VertexType = "vertex";
    void ContstructBar()
    {
        if (connectedVertInLayerDefOrder.Count == 6)
        {
            StaticBar b0 = Instantiate(barPrefab, BarObjHolder);
            b0.SetupBar(bar0pos[0], bar0pos[1], thickness, 0,VertexType);
            allBars[0] = b0;

            StaticBar b1 = Instantiate(barPrefab, BarObjHolder);
            b1.SetupBar(bar1pos[0], bar1pos[1], thickness, 1,VertexType);
            allBars[1] = b1;

            StaticBar b2 = Instantiate(barPrefab, BarObjHolder);
            b2.SetupBar(bar2pos[0], bar2pos[1], thickness, 2,VertexType);
            allBars[2] = b2;

            StaticBar b3 = Instantiate(barPrefab, BarObjHolder);
            b3.SetupBar(bar3pos[0], bar3pos[1], thickness, 3,VertexType);
            allBars[3] = b3;

            StaticBar b4 = Instantiate(barPrefab, BarObjHolder);
            b4.SetupBar(bar4pos[0], bar4pos[1], thickness, 4,VertexType);
            allBars[4] = b4;

            StaticBar b5 = Instantiate(barPrefab, BarObjHolder);
            b5.SetupBar(bar5pos[0], bar5pos[1], thickness, 5,VertexType);
            allBars[5] = b5;
        }
    }
    void UpdateBar()
    {
        if (connectedVertInLayerDefOrder.Count == 6)
        {

            if (allBars[0] != null)
            {
                allBars[0].UpdateBar(bar0pos[0], bar0pos[1]);
               
            }

            if (allBars[1] != null)
            {
                allBars[1].UpdateBar(bar1pos[0], bar1pos[1]);
               
            }

            if (allBars[2] != null)
            {
                allBars[2].UpdateBar(bar2pos[0], bar2pos[1]);
               
            }

            if (allBars[3] != null)
            {
                allBars[3].UpdateBar(bar3pos[0], bar3pos[1]);
               
            }

            if (allBars[4] != null)
            {
                allBars[4].UpdateBar(bar4pos[0], bar4pos[1]);
              
            }

            if (allBars[5] != null)
            {
                allBars[5].UpdateBar(bar5pos[0],bar5pos[1]);
               
            }

            if (allStrings.Count != 0)
            {
                updateString();
            }

        }
    }

    void reoderConnectedVertex()
    {
        PriorityQueue<int , int> _queue=new PriorityQueue<int, int>();

        List<TensileVertex>reorderedByGraph=new List<TensileVertex>(6);

        foreach (var tv in connectedVertInLayerDefOrder)
        {
            int localID = Array.IndexOf(connectedVertInLayerDefOrder.ToArray(), tv);
            _queue.Insert(tv.Index,localID);
        }

        for (int i = 0; i < connectedVertInLayerDefOrder.Count; i++)
        {
            int localId;
            int vIndex;
            _queue.RemoveMin(out vIndex, out localId);

           reorderedByGraph.Add(connectedVertInLayerDefOrder[localId]);
        }

        if (reorderedByGraph.Count == 6)
        {
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[0]);
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[1]);
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[3]);
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[5]);
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[4]);
            connectedVertexsInCounterClockwise.Add(reorderedByGraph[2]);
        }
    }

  
   

    void setBarPosition()
    {
        if (connectedVertexsInCounterClockwise.Count == 6)
        {
            foreach (var v in connectedVertexsInCounterClockwise)
            {
                var d = v.transform.localPosition - transform.localPosition;

                Vector3 p = 0.28f * d + transform.localPosition;
                Positions.Add(p);
            }

            bar0pos[0] = Positions[0] + Vector3.up * 0.132f * Scale;
            bar0pos[1] = Positions[2] + Vector3.down * 0.132f * Scale;
            bar1pos[0] = Positions[1] + Vector3.up * 0.132f * Scale;
            bar1pos[1] = Positions[3] + Vector3.down * 0.132f * Scale;
            bar2pos[0] = Positions[2] + Vector3.up * 0.132f * Scale;
            bar2pos[1] = Positions[4] + Vector3.down * 0.132f * Scale;
            bar3pos[0] = Positions[3] + Vector3.up * 0.132f * Scale;
            bar3pos[1] = Positions[5] + Vector3.down * 0.132f * Scale;
            bar4pos[0] = Positions[4] + Vector3.up * 0.132f * Scale;
            bar4pos[1] = Positions[0] + Vector3.down * 0.132f * Scale;
            bar5pos[0] = Positions[5] + Vector3.up * 0.132f * Scale;
            bar5pos[1] = Positions[1] + Vector3.down * 0.132f * Scale;
        }

    }

    void updateBarPosition()
    {
        if (connectedVertexsInCounterClockwise.Count == 6)
        {
            for (int i = 0; i < connectedVertexsInCounterClockwise.Count; i++)
            {
                var d = connectedVertexsInCounterClockwise[i].transform.localPosition - transform.localPosition;

                Vector3 p = 0.28f * d + transform.localPosition;

                Positions[i] = p;
               
            }

            bar0pos[0] = Positions[0] + Vector3.up * 0.132f * Scale;
            bar0pos[1] = Positions[2] + Vector3.down * 0.132f * Scale;
            bar1pos[0] = Positions[1] + Vector3.up * 0.132f * Scale;
            bar1pos[1] = Positions[3] + Vector3.down * 0.132f * Scale;
            bar2pos[0] = Positions[2] + Vector3.up * 0.132f * Scale;
            bar2pos[1] = Positions[4] + Vector3.down * 0.132f * Scale;
            bar3pos[0] = Positions[3] + Vector3.up * 0.132f * Scale;
            bar3pos[1] = Positions[5] + Vector3.down * 0.132f * Scale;
            bar4pos[0] = Positions[4] + Vector3.up * 0.132f * Scale;
            bar4pos[1] = Positions[0] + Vector3.down * 0.132f * Scale;
            bar5pos[0] = Positions[5] + Vector3.up * 0.132f * Scale;
            bar5pos[1] = Positions[1] + Vector3.down * 0.132f * Scale;

        }
    }

    List<Vector3[]> stringPos = new List<Vector3[]>(6);
    List<StaticStrings> allStrings=new List<StaticStrings>(24);
    void setString()
    {
      
        stringPos.Add(bar0pos);
        stringPos.Add(bar1pos);
        stringPos.Add(bar2pos);
        stringPos.Add(bar3pos);
        stringPos.Add(bar4pos);
        stringPos.Add(bar5pos);
        for (int i = 0; i < 6; i++)
        {
            int fromStart;
            int fromEnd;
            if (i == 0)
            {
                fromEnd = 5;
            }
            else
            {
                fromEnd = i - 1;
            }

            if (i == 5)
            {
                fromStart = 0;
            }
            else
            {
                fromStart = i + 1;
            }

            StaticStrings s0 = Instantiate(stringPrefab, StringObjHolder);
            StaticStrings s1 = Instantiate(stringPrefab,StringObjHolder);
            StaticStrings s2 = Instantiate(stringPrefab, StringObjHolder);
            StaticStrings s3 = Instantiate(stringPrefab, StringObjHolder);

            s0.ConnectString(stringPos[i][1],stringPos[fromStart][0],thickness*0.3f,i*4,VertexType);
            s1.ConnectString(stringPos[i][1], stringPos[fromStart][1], thickness * 0.3f, i * 4+1,VertexType);
            s2.ConnectString(stringPos[i][0], stringPos[fromEnd][0], thickness * 0.3f, i * 4+2,VertexType);
            s3.ConnectString(stringPos[i][0], stringPos[fromEnd][1], thickness * 0.3f, i * 4+3,VertexType);

            allStrings.Add(s0);
            allStrings.Add(s1);
            allStrings.Add(s2);
            allStrings.Add(s3);

        }
    }

    void updateString()
    {

        stringPos[0] = bar0pos;
        stringPos[1] = bar1pos;
        stringPos[2] = bar2pos;
        stringPos[3] = bar3pos;
        stringPos[4] = bar4pos;
        stringPos[5] = bar5pos;

        for (int i = 0; i < 6; i++)
        {
            int fromStart;
            int fromEnd;
            if (i == 0)
            {
                fromEnd = 5;
            }
            else
            {
                fromEnd = i - 1;
            }

            if (i == 5)
            {
                fromStart = 0;
            }
            else
            {
                fromStart = i + 1;
            }

            allStrings[i * 4].updateString(stringPos[i][1], stringPos[fromStart][0]);
            allStrings[i * 4 + 1].updateString(stringPos[i][1], stringPos[fromStart][1]);
            allStrings[i * 4 + 2].updateString(stringPos[i][0], stringPos[fromEnd][0]);
            allStrings[i * 4 + 3].updateString(stringPos[i][0], stringPos[fromEnd][1]);

        }
    }

    public void SetVertAbove(TensileVertex _va)
    {
        VertAbove.Add(_va);
    }
    public void SetVertBelow(TensileVertex _vb)
    {
        VertBelow.Add(_vb);
    }

    public Vector3 VertAbovePosition()
    {
        
        if (VertAbove.Count !=0)
        {
            return VertAbove[0].transform.localPosition;
        }
        else if(VertBelow.Count!=0)
        {
            return (transform.localPosition-VertBelow[0].transform.localPosition)+transform.localPosition;
        }
        else
        {
            return (transform.localPosition + (0.42552f / 1f) * Scale * Vector3.up);
        }
    }

    public Vector3 VertBelowPosition()
    {
        if (VertBelow.Count != 0)
        {
            return VertBelow[0].transform.localPosition;
        }
        else if(VertAbove.Count!=0)
        {
            return (transform.localPosition-VertAbove[0].transform.localPosition)+transform.localPosition;
        }
        else
        {
            return (transform.localPosition + (0.42552f / 1f) * Scale * Vector3.down);
        }
    }



    public void AddEdgeObjs(TensileEdge Eobj)
    {
        connectedEdgeObjs.Add(Eobj);
    }

    public void AddConnectedVertObjInLayer(TensileVertex v)
    {

        connectedVertInLayerDefOrder.Add(v);

    }

    public IEnumerable<TensileVertex> GetConnectedVertexObj()
    {
        return connectedVertInLayerDefOrder;
    }

    public IEnumerable<TensileEdge> GetConnectedEdgeObjs()
    {
        return connectedEdgeObjs;
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


    public void UpdateToFuture()
    {
        SetState(FutureState);
        if (State == 1)
        {
            Age++;
        }

        if (State == 0)
        {
            Age = 0;
        }
    }

    public int GetAge()
    {
        return Age;
    }



    public void SetupStructure(int _scale, int _state, int _index)
    {
        Scale = _scale;
        transform.localScale *= Scale;
        Index = _index;
        SetState(_state);

        GraphHolder = Instantiate(VertexIndicatorPFB, transform);
    }

    void UpdateState()
    {
        if (State == 0)//Show only center point
        {
          clean();
        }

        if (State == 1)//Show source red cube
        {
           clean();

            GameObject vObj = Instantiate(ShapePFB, transform);

            vObj.transform.localScale *= 1.5f;
            ObjHolder.Add(vObj);
        }

        if (State == 2)
        {
            if (SubState == 0)//Show growed white cube
            {
                clean();
              
                GameObject vObj = Instantiate(ShapePFB, transform);
              
                ObjHolder.Add(vObj);
            }

            if (SubState == 1)//Show all structure
            {
                clean();
                if (connectedVertexsInCounterClockwise.Count == 6)
                {
                    ContstructBar();
                    setString();
                }
            }
            if (SubState == 2)//Show all structure
            {
                clean();
                if (connectedVertexsInCounterClockwise.Count == 6)
                {
                    ContstructBar();
                    foreach (var b in allBars)
                    {
                        b.GetComponent<MeshRenderer>().material.color = col;
                    }

                }

            }
        }
    }

    void clean()
    {
        foreach (var o in ObjHolder)
        {
            if (o != null)
                DestroyImmediate(o);
        }
        foreach (var b in allBars)
        {
            if (b != null)
            {
                DestroyImmediate(b.gameObject);
            }
        }

        foreach (var s in allStrings)
        {
            DestroyImmediate(s.gameObject);
        }
        ObjHolder.Clear();
        allBars.Clear();
        allStrings.Clear();
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
        if (Enabled == true)
        {
            updateBarPosition();
            UpdateBar();
        }
    }

}
