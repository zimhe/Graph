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

public class TriangleBoundryGrowthManager : MonoBehaviour
{
    private SharedEdgeGraph HexigonGrid;
    private TriangleBoundryGraphManager GraphManager;
    private EdgeGraph _graph;
    List<TensileVertex> TenVertex;
    List<TensileEdge> TenEdge;


    private bool GraphDone;

    private int SegCount;
    private int LayerCount;

    private float Scale;

    bool displayed = false;

 


    [SerializeField] Button turnOnAllBtn;

    [SerializeField] private Transform SelectorPrefab;

    private Transform Selector;

    // Use this for initialization
    void Start()
    {
        Selector = Instantiate(SelectorPrefab, transform);
        Selector.parent = transform;
        Selector.localPosition=Vector3.zero;

        //initializeGrowther();
    }

    public void initializeGrowther()
    {
        GraphManager = transform.GetComponent<TriangleBoundryGraphManager>();

        Scale = GraphManager.getScale();

        GraphDone = GraphManager.GraphReady();

        displayed = false;

        HexigonGrid = GraphManager.Grid();
        _graph = HexigonGrid.edgeGraph;
     
        TenVertex = HexigonGrid.TensileVertexObjects;
        TenEdge = HexigonGrid.TensileEdgeObjects;

        SegCount = GraphManager.GetSegmentCount();
        LayerCount = GraphManager.GetLayerCount();
        CheckEdges();
     }

    public void ResetGrowth()
    {
       
    }

    void CheckEdges()
    {
        foreach (var e in HexigonGrid.TensileEdgeObjects)
        {

            int S = e.StartVertice;
            int E = e.EndVertice;

            var sp = GraphManager.getSavedPositions();
            var T = e.GetConnecetedTriangles().ToArray();



            if (T.Length == 1)
            {
                if (e.GetPositionToGrow().Count == 0)
                {
                    int V = T[0].GiveDiagonalVertice(S, E);

                    e.NeighborVertexStart = TenVertex[V];

                    if (e.NeighborVertexEnd == null)
                    {
                        var Pv = sp[V];
                        var Px = e.transform.localPosition * 2 - Pv;
                        e.AddPositionToGrow(Px);
                    }
                }
            }
            else if(T.Length==2)
            {
                if (e.GetPositionToGrow().Count == 0)
                {
                    var pe = (sp[S] + sp[E]) * 0.5f;

                    if (e.NeighborEdgeAbove == null)
                    {
                        var Px0 = new Vector3(pe.x, pe.y + (0.42552f / 1f) * Scale, pe.z);
                        e.AddPositionToGrow(Px0);
                    }

                    if (e.NeighborEdgeBelow == null)
                    {
                        var Px1 = new Vector3(pe.x, pe.y - (0.42552f / 1f) * Scale, pe.z);
                        e.AddPositionToGrow(Px1);
                    }
                }
            }

            if (e.GetPositionToGrow().Count == 0)
            {
                continue;
            }
            else
            {
                if (e.GetIndicators().Count == 0)
                {
                    foreach (var p in e.GetPositionToGrow())
                    {
                        var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        o.GetComponent<MeshRenderer>().material.color = Color.white;
                        o.transform.parent = transform;
                        o.transform.localScale = 0.05f * Vector3.one;
                        o.transform.localPosition = p;

                        e.addIndicator(o);

                        o.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
        }
    }

    void Select()
    {
        float d = float.MaxValue;

        int index = 0;
        foreach (var e in TenEdge)
        {
            var dis = (e.transform.localPosition - Selector.transform.localPosition).magnitude;
            if (dis < d)
            {
                d = dis;
                index = Array.IndexOf(TenEdge.ToArray(),e);
            }
        }

        SelectionIndex = index;
        selectEdge(index);
    }

    void selectEdge(int index)
    {
        foreach (var e in TenEdge)
        {
            e.resetGraphThickness();
            e.ToggleIndicators(false);
        }

        TenEdge[index].GraphSelect();
        TenEdge[index].ToggleIndicators(true);

        Selector.transform.localPosition = TenEdge[index].transform.localPosition;
    }


    int SelectionIndex = 0;
    void SelectionInput()
    {
        var ec = TenEdge.Count;
        if (Input.GetKeyDown(KeyCode.D))
        {
            Selector.Translate(Vector3.right*Scale*0.7f);
       
            Select();
          
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Selector.Translate(Vector3.left * Scale*0.7f);

            Select();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Selector.Translate(Vector3.forward * Scale*0.7f);

            Select();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Selector.Translate(Vector3.back * Scale*0.7f);

            Select();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            GraphManager.AddVertexEdgeTriangle(TenEdge[SelectionIndex],0);

           {
                CheckEdges();
            }
         

        }
    }

    void updateEdge()
    {
        foreach (var e in HexigonGrid.TensileEdgeObjects)
        {

            if (e.GetConnecetedTriangles().Count() == 2)
            {
                var sc = e.StringCount();
                var bc = e.BarCount();
                if (e.State != 2 || e.SubState != 1 || sc != 24 || bc != 6)
                {
                    e.SetAllState(2, 1);
                }
            }
        }

        foreach (var v in HexigonGrid.TensileVertexObjects)
        {
            if (v.GetConnectedEdgeObjs().Count() == 6)
            {
                v.EnableVertex();
                if (v.State != 2 || v.SubState != 1)
                {
                  
                    v.SetAllState(2,1);
                }
            }
        }


    }

   

    void Update()
    {
     
        SelectionInput();

        updateEdge();
    }

    void FixedUpdate()
    {
      
    }

 
}

