using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensileVertex : MonoBehaviour
{
    [SerializeField] private GameObject VertexIndicatorPFB;
    [SerializeField] private GameObject ShapePFB;
    [SerializeField] private GameObject ShellPFB;
    [SerializeField] private GameObject BarsPFB;
    [SerializeField] private GameObject StringsPFB;

    List<TensileEdge> connectedEdgeObjs=new List<TensileEdge>(6);

    

    /*
   void Awake()
   {
       EdgeIndicatorPFB.SetActive(true);
       ShapePFB.SetActive(false);
       ShellPFB.SetActive(false);
       BarsPFB.SetActive(false);
       StringsPFB.SetActive(false);
   }
   */

    private GameObject[] ObjHolder = new GameObject[3];

    

    public int State { get; set; }

    public int FutureState { get; set; }

    public int SubState { get; set; }

    public int Index { get; set; }

    public int Depth { get; set; }

    private int Scale;

    private int Age;

    public void AddEdgeObjs(TensileEdge Eobj)
    {
        connectedEdgeObjs.Add(Eobj);
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
    }

    void UpdateState()
    {
        if (State == 0)//Show only center point
        {
            //VertexIndicatorPFB.SetActive(true);
            //ShapePFB.SetActive(false);
            //ShellPFB.SetActive(false);
            //BarsPFB.SetActive(false);
            //StringsPFB.SetActive(false);
            foreach (var o in ObjHolder)
            {
                if (o != null)
                    DestroyImmediate(o);
            }
            ObjHolder[0] = Instantiate(VertexIndicatorPFB, transform);
            ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.white;
        }

        if (State == 1)//Show source red cube
        {
            //VertexIndicatorPFB.SetActive(false);
            //ShapePFB.SetActive(true);
            //ShellPFB.SetActive(false);
            //BarsPFB.SetActive(false);
            //StringsPFB.SetActive(false);
            //ShapePFB.GetComponent<MeshRenderer>().material.color = Color.red;
            foreach (var o in ObjHolder)
            {
                if (o != null)
                    DestroyImmediate(o);
            }
            ObjHolder[0] = Instantiate(ShapePFB, transform);
            ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (State == 2)
        {
            if (SubState == 0)//Show growed white cube
            {
                //VertexIndicatorPFB.SetActive(false);
                //ShapePFB.SetActive(true);
                //ShellPFB.SetActive(false);
                //BarsPFB.SetActive(false);
                //StringsPFB.SetActive(false);
                //ShapePFB.GetComponent<MeshRenderer>().material.color = Color.white;
                foreach (var o in ObjHolder)
                {
                    if (o != null)
                        DestroyImmediate(o);
                }
                ObjHolder[0] = Instantiate(ShapePFB, transform);
                ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.white;
            }

            if (SubState == 1)//Show all structure
            {
                //VertexIndicatorPFB.SetActive(false);
                //ShapePFB.SetActive(false);
                //ShellPFB.SetActive(false);
                //BarsPFB.SetActive(true);
                //StringsPFB.SetActive(true);
                foreach (var o in ObjHolder)
                {
                    if (o != null)
                        DestroyImmediate(o);
                }
                ObjHolder[0] = Instantiate(BarsPFB, transform);
                ObjHolder[1] = Instantiate(ShellPFB, transform);
                ObjHolder[2] = Instantiate(StringsPFB, transform);
            }

            if (SubState == 2)//Show bars & shell
            {
                //VertexIndicatorPFB.SetActive(false);
                //ShapePFB.SetActive(false);
                //ShellPFB.SetActive(true);
                //BarsPFB.SetActive(true);
                //StringsPFB.SetActive(false);
                foreach (var o in ObjHolder)
                {
                    if (o != null)
                        DestroyImmediate(o);
                }
                ObjHolder[0] = Instantiate(BarsPFB, transform);
                ObjHolder[1] = Instantiate(ShellPFB, transform);
            }

            if (SubState == 3)//Show Bars
            {
                //VertexIndicatorPFB.SetActive(false);
                //ShapePFB.SetActive(false);
                //ShellPFB.SetActive(false);
                //BarsPFB.SetActive(true);
                //StringsPFB.SetActive(false);
                foreach (var o in ObjHolder)
                {
                    if (o != null)
                        DestroyImmediate(o);
                }
                ObjHolder[0] = Instantiate(BarsPFB, transform);
            }
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


}
