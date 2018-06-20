using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class StaticStrings : MonoBehaviour
{
    private Vector3[] P=new Vector3[2];
   
    private float defualtStringLength;
    
    private float Thickness;
    private int index;


    public void ConnectString(Vector3  _p0, Vector3  _p1, float _thickness, int _index,string _type)
    {
        index = _index;
        P[0] = _p0;
        P[1] = _p1;
       
        Thickness = _thickness;

        var d = P[1] - P[0];
        var L = d.magnitude;
        defualtStringLength = L;

        var T = gameObject.transform;

        T.localScale = new Vector3(Thickness, (L) / 2, Thickness);
        T.localPosition = (P[0] + P[1]) * 0.5f;
        T.localRotation = Quaternion.FromToRotation(T.up, d);



        //Str1.UpdateLimitSpring(spring);

        gameObject.name = "String" + index+":"+_type;
    }

    public void updateString(Vector3 p0, Vector3 p1)
    {

        if (!float.IsNaN(p0.x) && !float.IsNaN(p1.x) && !float.IsNaN(p0.y) && !float.IsNaN(p1.y) &&
            !float.IsNaN(p0.z) && !float.IsNaN(p1.z))
        {
            P[0] = p0;
            P[1] = p1;

            var d = P[1] - P[0];
            var L = d.magnitude;
            defualtStringLength = L;

            var T = gameObject.transform;

            var fwd0 = Vector3.Cross(Vector3.down - Vector3.left, d);

            T.localPosition = (P[0] + P[1]) * 0.5f;
            T.localScale = new Vector3(Thickness, (L) / 2, Thickness);
            T.transform.localRotation = Quaternion.LookRotation(fwd0, d);
        }

    }

}
