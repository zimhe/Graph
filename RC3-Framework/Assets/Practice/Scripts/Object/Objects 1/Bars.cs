using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bars : MonoBehaviour
{
    private Vector3 [] Vertices = new Vector3[2];

    private FixedJoint[] Joints = new FixedJoint[2];

    Transform [] _Joints=new Transform [2];

    private GameObject pivot;

    float  Length;

    private int index;

    public void SetupBar(Transform  J0, Transform  J1,float Thickness,int _index,int _j0,int _j1)
    {
        Vertices [0] = J0.localPosition  ;
        Vertices [1] = J1.localPosition  ;

        _Joints[0] = J0;
        _Joints[1] = J1;

        var T = gameObject.GetComponent<Transform>();
        var d = Vertices[1] - Vertices[0];

        var L = d.magnitude;

        T.localScale = new Vector3(Thickness, (L+1)/2, Thickness);
        T.localPosition  = (Vertices [0]+Vertices [1]) * 0.5f;
        T.localRotation  =Quaternion.FromToRotation(T.up , d);


        Joints[0] = gameObject.AddComponent<FixedJoint>();
        Joints[1] = gameObject.AddComponent<FixedJoint>();
       //Joints[0].enableCollision  = true;
       //Joints[1].enableCollision  = true;

        Joints[0].connectedBody = J0.GetComponent<Rigidbody>();
        Joints[1].connectedBody = J1.GetComponent<Rigidbody>();
       
        J0.GetComponent<TJoint>().SaveConnectedBar(gameObject);
        J1.GetComponent<TJoint>().SaveConnectedBar(gameObject);





        Length = L;

        index = _index;
        gameObject.name = "Bar" + index;
    }


    public float  GetLength()
    {
        return Length;
    }

    public int GetIndex()
    {
        return index;
    }

    public Vector3 GetJoint(int _index)
    {
        return Vertices [_index ];
    }

    public Transform GetAttachedJoint(int _index)
    {
        return _Joints[_index];
    }

    private void Update()
    {
        //updateBarPositon();
    }
   public void updateBarPositon()
    {
        Vertices[0] = _Joints[0].localPosition;
        Vertices[1] = _Joints[1].localPosition;
        var T = gameObject.GetComponent<Transform>();
        var d = Vertices[1] - Vertices[0];
        var FWD = Vector3.Cross(Vector3.down, d);
        T.localPosition = (Vertices[0] + Vertices[1]) * 0.5f;
        T.localRotation = Quaternion.LookRotation(FWD, d);
    }

}
