using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TensileEdge : MonoBehaviour {

    [SerializeField] private GameObject EdgeIndicatorPFB;
    [SerializeField] private GameObject ShapePFB;
    [SerializeField] private GameObject ShellPFB;
    [SerializeField] private GameObject BarsPFB;
    [SerializeField] private GameObject StringsPFB;

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

    public bool ShowStructure;

    public int State { get; set; }

    public int FutureState { get; set; }

    public int SubState { get; set; }

    public int Start { get; set; }

    public int End { get; set; }

    public int Depth { get; set; }

    private int Scale;

    public void SetState(int _state)
    {
        State = _state;
        UpdateState();
    }


    public void SetupStructure(int _scale, int _state, int _start,int _end, IEnumerable<Vector3> _position)
    {
        var p = _position.ToArray();
        Scale = _scale;
      
        SetState(_state);
        Start  = _start;
        End = _end;
        var D = p[_end] - p[_start];
        var Pos = (p[_start] + p[_end]) * 0.5f;
        transform.localScale *= Scale;
        transform.localPosition = Pos;
        transform .localRotation=Quaternion.FromToRotation(transform .forward,D);
       
    }

    void UpdateState()
    {
        if (State == 0)//Show only center point
        {
            //EdgeIndicatorPFB.SetActive(true);
            //ShapePFB.SetActive(false); 
            //ShellPFB.SetActive(false);
            //BarsPFB.SetActive(false);
            //StringsPFB.SetActive(false);
            foreach (var o in ObjHolder)
            {
                if(o!=null)
                DestroyImmediate(o);
            }

            ObjHolder[0] = Instantiate(EdgeIndicatorPFB, transform);

            ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.white;
        }

        if (State == 1)//Show source red cube
        {
            //EdgeIndicatorPFB.SetActive(false);
            //ShapePFB.SetActive(true);
            //ShellPFB.SetActive(false);
            //BarsPFB.SetActive(false);
            //StringsPFB.SetActive(false);
            foreach (var o in ObjHolder)
            {
                if (o != null)
                    DestroyImmediate(o);
            }

            ObjHolder[0] = Instantiate(ShapePFB, transform);
            ObjHolder[0].GetComponent<MeshRenderer>().material.color=Color.red;
        }

        if (State == 2)
        {
            if (SubState == 0)//Show growed white cube
            {
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
                //EdgeIndicatorPFB.SetActive(false);
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
                ObjHolder[1] = Instantiate(StringsPFB, transform);
                ObjHolder[2] = Instantiate(ShellPFB, transform);
            }

            if (SubState == 2)//Show bars & shell
            {
                //EdgeIndicatorPFB.SetActive(false);
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
                //EdgeIndicatorPFB.SetActive(false);
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
