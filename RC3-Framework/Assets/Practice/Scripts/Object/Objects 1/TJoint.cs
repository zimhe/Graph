using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TJoint : MonoBehaviour
{
    private int index;
    private int BarIndex;
    private int StartEndIndex;
    private FixedJoint FJ = new FixedJoint();
    private GameObject ConnectedBar;

    public void SetIndex(int _index)
    {
        index = _index;
    }

    public void SetupJoint(Vector3 _position)
    {
        transform.localPosition = _position;
        gameObject.name = "Joint" + index + ", " + "Bar_" + BarIndex + ", " + "ID_" + StartEndIndex;
    }

    public void SetFixJointToBar(GameObject _Bar)
    {
        FJ = gameObject.AddComponent<FixedJoint>();
        FJ.connectedBody = _Bar.GetComponent< Rigidbody>();
    }
    public void SetIndex2d(int _Bi, int _Si)
    {
        BarIndex = _Bi;
        StartEndIndex = _Si;
    }

    public void SaveConnectedBar(GameObject _bar)
    {
        ConnectedBar = _bar;
    }

    public GameObject getConnectedBar()
    {
        return ConnectedBar;
    }

    public int getConnectedBarID()
    {
        return BarIndex;
    }

    public int getStartEndID()
    {
        return StartEndIndex;
    }

    public int GetIndex()
    {
        return index;
    }

    public int GetBarIndex()
    {
        return BarIndex;
    }

    public int GetStartEndIndex()
    {
        return StartEndIndex;
    }
}
