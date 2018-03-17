using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TensegrityObject : MonoBehaviour
{
    [SerializeField] private GameObject BarPrefab;
    [SerializeField] private GameObject StringPrefab;
    [SerializeField] private GameObject CenterPrefab;
    [SerializeField] private GameObject BoundryPrefab;

    private GameObject CentreObj;
    private GameObject BarObj;
    private GameObject StringObj;
    private GameObject Boundry;

    private GameObject[] ObjHolder = new GameObject[3];

    public bool ShowStructure;

    public int State { get; set; }

    public int FutureState { get; set; }

    public int SubState { get; set; }

    public int Index { get; set; }

    public int Depth { get; set; }

    private int Scale;

   public  void SetState(int _state)
    {
        State = _state;
        UpdateState();
    }

    public void UpdateToFuture()
    {
        SetState(FutureState);
    }


    public void SetupStructure(int _scale, int _state, int _index)
    {
        Scale = _scale;
        transform.localScale *= Scale;
        Index = _index;
        SetState(_state);
    }


    void SetObj()
    {
        ObjHolder [0] = Instantiate(CenterPrefab, transform);
    }


    void UpdateState()
    {
        if (State == 0)//Show only center point
        {
            DestroyImmediate(ObjHolder[0]);
            DestroyImmediate(ObjHolder[1]);
            DestroyImmediate(ObjHolder[2]);
            ObjHolder[0] = Instantiate(CenterPrefab, transform);
            ObjHolder [0].GetComponent<MeshRenderer>().material.color = Color.white ;
        }

        if (State == 1)//Show source red cube
        {
            DestroyImmediate(ObjHolder[0]);
            DestroyImmediate(ObjHolder[1]);
            DestroyImmediate(ObjHolder[2]);
            ObjHolder[0] = Instantiate(BoundryPrefab, transform);
            ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.red ;
        }

        if (State == 2)
        {
            if (SubState == 0)//Show growed white cube
            {
                DestroyImmediate(ObjHolder[0]);
                DestroyImmediate(ObjHolder[1]);
                DestroyImmediate(ObjHolder[2]);
                ObjHolder[0] = Instantiate(BoundryPrefab, transform);
                ObjHolder[0].GetComponent<MeshRenderer>().material.color = Color.white;
            }

            if (SubState == 1)//Show all structure
            {
                DestroyImmediate(ObjHolder[0]);
                DestroyImmediate(ObjHolder[1]);
                DestroyImmediate(ObjHolder[2]);
                ObjHolder[1] = Instantiate(BarPrefab, transform);
                ObjHolder[2] = Instantiate(StringPrefab, transform);
            }

            if (SubState == 2)//Show only bars
            {
                DestroyImmediate(ObjHolder[0]);
                DestroyImmediate(ObjHolder[1]);
                DestroyImmediate(ObjHolder[2]);
                ObjHolder[1] = Instantiate(BarPrefab, transform);
            }

            if (SubState == 3)//Show only strings
            {
                DestroyImmediate(ObjHolder[0]);
                DestroyImmediate(ObjHolder[1]);
                DestroyImmediate(ObjHolder[2]);
                ObjHolder[2] = Instantiate(StringPrefab, transform);
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
   