using System.Collections;
using System.Collections.Generic;
using RC3.Unity;
using UnityEngine;

public class CellEdge : EdgeObject
{
    public void SetEdge(Vector3 _start, Vector3 _end)
    {
        var d = _end - _start;

        var p = 0.5f * (_end + _start);

        var L = d.magnitude;

        transform.localPosition = p;
        transform.localRotation=Quaternion.FromToRotation(transform.up,d);
        transform.localScale =new Vector3(transform.localScale.x,0.5f*L,transform.localScale.z);
    }
}

