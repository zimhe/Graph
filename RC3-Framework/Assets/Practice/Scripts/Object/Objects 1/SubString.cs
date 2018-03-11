using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubString : MonoBehaviour {

    private GameObject [] Segments;
    private GameObject[] Wire;
    private ConfigurableJoint[] CFJ;
    private ConfigurableJoint FromJnt;
    private SubString Str0;
    private SubString Str1;

    private Transform From;
    private Transform To;

    private int Level;

    private float tkns;

    private Vector3 p0 = new Vector3();
    private Vector3 p1 = new Vector3();

    private Vector3 Direction = new Vector3();

    SoftJointLimit Limit = new SoftJointLimit();
    private float LimitFloat;
    private SoftJointLimitSpring Spring;


    public void SetupSubSting(Transform _from,Transform _to, int _SdvLvl,float _Thickness)
    {
       
        if (_SdvLvl >= 1)
        {
            Level = _SdvLvl;
        }
        else
        {
            Level = 1;
        }
        Segments = new GameObject[Level-1];
        Wire = new GameObject[Level];
        CFJ = new ConfigurableJoint[Level-1];
        tkns = _Thickness;
        Direction = _to.localPosition - _from .localPosition;
        From = _from;
        To = _to;

        Limit.limit =Mathf.Abs ((To.localPosition - From.localPosition).magnitude )/ Level;
        FromJnt = From.gameObject.AddComponent<ConfigurableJoint>();
        FromJnt.autoConfigureConnectedAnchor = false;
        FromJnt.anchor = FromJnt.connectedAnchor = new Vector3(0, 0, 0);
        FromJnt.xMotion = FromJnt.yMotion = FromJnt.zMotion = ConfigurableJointMotion.Limited;
        //FromJnt.angularXMotion = FromJnt.angularYMotion = FromJnt.angularZMotion = ConfigurableJointMotion.Limited;
        FromJnt.linearLimit = Limit;
       
        for (int i = 0; i < Level; i++)
        {
            SetupSegment(i);
        }
        for (int i = 0; i < Level-1; i++)
        {
            ConnectBody(i);
        }
        if (Level == 1)
        {
           FromJnt.connectedBody = To.GetComponent<Rigidbody>();
        }
    }

    void SetupSegment(int _index)
    {
        Vector3 _fromV = From.localPosition;
        Vector3 _toV = To.localPosition;
        GameObject wire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wire.GetComponent<CapsuleCollider>().enabled = false;
        wire.transform.parent = From.parent;
        Wire[_index] = wire;
        var _p = Direction * (_index + 1) / Level;

        if (Level > 1)
        {
            if (_index < Level-1)
            {
                GameObject sgm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sgm.GetComponent<SphereCollider>().enabled = false;
                Segments[_index] = sgm;
                sgm.transform.parent = From.parent;
                sgm.transform.localPosition = _p + _fromV;
                sgm.transform.localScale = new Vector3(tkns, tkns, tkns);
                Rigidbody _body = sgm.AddComponent<Rigidbody>();
                //_body.isKinematic = true;
                _body.drag = 5f;
                _body.angularDrag = 10f;
                CFJ[_index] = sgm.AddComponent<ConfigurableJoint>();
                sgm.name = "S" + _index;
            }
        }
        UpdateWire(_index);
    }

    void UpdateWire(int _index)
    {
        GameObject wire = Wire[_index];
        if (Level > 1)
        {
            Vector3 fwd = new Vector3();
            Vector3 _dUp = new Vector3();
            Vector3 _dDn = new Vector3();
            if (_index == 0)
            {
                var d = Segments[_index].transform.localPosition - From.localPosition;
                fwd = Vector3.Cross(Vector3.down, d);
                _dUp = d;
                _dDn = From.localPosition;
            }
            else if (_index > 0 && _index < Level - 1)
            {
                var d = Segments[_index].transform.localPosition - Segments[_index - 1].transform.localPosition;
                fwd = Vector3.Cross(Vector3.down, d);
                _dUp = d;
                _dDn = Segments[_index - 1].transform.localPosition;
            }
            else if (_index == Level - 1)
            {
                var d = Segments[_index - 1].transform.localPosition - To.localPosition;
                fwd = Vector3.Cross(Vector3.down, d);
                _dUp = d;
                _dDn = To.localPosition;
            }
            wire.transform.localRotation = Quaternion.LookRotation(fwd, _dUp);
            wire.transform.localScale = new Vector3(tkns, _dUp.magnitude * 0.5f, tkns);
            wire.transform.localPosition = _dUp * 0.5f + _dDn;
            wire.name = "W" + _index;
        }
        else
        {
            Vector3 fwd = Vector3.Cross(Vector3.down, Direction);
            wire.transform.localRotation = Quaternion.LookRotation(fwd, Direction);
            wire.transform.localPosition = Direction * 0.5f + From.localPosition;
            wire.transform.localScale = new Vector3(tkns, Direction.magnitude * 0.5f, tkns);
        }
    }

    public  void UpdateAllWire()
    {
        foreach (GameObject wire in Wire)
        {
            int _index = Array.IndexOf(Wire, wire);
            if (Level > 1)
            {
                Vector3 fwd = new Vector3();
                Vector3 _dUp = new Vector3();
                Vector3 _dDn = new Vector3();
                if (_index == 0)
                {
                    var d = Segments[_index].transform.localPosition - From.localPosition;
                    fwd = Vector3.Cross(Vector3.left, d);
                    _dUp = d;
                    _dDn = From.localPosition;
                }
                else if (_index > 0 && _index < Level - 1)
                {
                    var d = Segments[_index].transform.localPosition - Segments[_index - 1].transform.localPosition;
                    fwd = Vector3.Cross(Vector3.left, d);
                    _dUp = d;
                    _dDn = Segments[_index - 1].transform.localPosition;
                }
                else if (_index == Level - 1)
                {
                    var d = Segments[_index - 1].transform.localPosition - To.localPosition;
                    fwd = Vector3.Cross(Vector3.left, d);
                    _dUp = d;
                    _dDn = To.localPosition;
                }
                wire.transform.localRotation = Quaternion.LookRotation(fwd, _dUp);
                wire.transform.localScale = new Vector3(tkns, _dUp.magnitude * 0.5f, tkns);
                wire.transform.localPosition = _dUp * 0.5f + _dDn;
                wire.name = "W" + _index;
            }
            else
            {
                var dir = To.localPosition - From.localPosition;
                Vector3 fwd = Vector3.Cross(Vector3.down, dir);
                wire.transform.localRotation = Quaternion.LookRotation(fwd, dir);
                wire.transform.localPosition = dir * 0.5f + From.localPosition;
                wire.transform.localScale = new Vector3(tkns, dir.magnitude * 0.5f, tkns);
            }
        }
        UpdateColor();
        //updateSegmentRotation();
    }

    void UpdateColor()
    {
        float Threshold = 1000f;
        foreach (GameObject wire in Wire)
        {
            int _index = Array.IndexOf(Wire, wire);
            ConfigurableJoint JFrom = From.GetComponent<ConfigurableJoint>();
            float force = 0f;
            
            if (_index == 0)
            {
                force = From.GetComponent<ConfigurableJoint>().currentForce.magnitude;
                
            }
            else
            {
                force = CFJ[_index - 1].currentForce.magnitude;
            }
            wire.GetComponent<MeshRenderer>().material.color = WireForceColor(force, Threshold);
        }
        foreach (GameObject sgm in Segments)
        {
            int _index = Array.IndexOf(Segments, sgm);
            float force = CFJ[_index].currentForce.magnitude;
            sgm.GetComponent<MeshRenderer>().material.color = WireForceColor(force, Threshold);
        }
    }

    Color WireForceColor(float force,  float _threshold)
    {
        Color wc = new Color();
        float t;
        if (force > _threshold)
        {
            t = (force - _threshold) / _threshold;
            wc = Color.Lerp(Color.green, Color.red, t);
        }
        if (force <= _threshold)
        {
            t = (_threshold - force) / _threshold;
            wc= Color.Lerp(Color.green, Color.cyan , t);
        }
        return wc;
    }

    void ConnectBody(int _index)
    {
        if (Level > 1)
        {
            if (_index == Level - 2)
            {
                CFJ[_index].connectedBody = To.GetComponent<Rigidbody>();
            }
            else
            {
                CFJ[_index].connectedBody = Segments[_index + 1].GetComponent<Rigidbody>();
            }
            if (_index == 0)
            {
                FromJnt.connectedBody = Segments[_index].GetComponent<Rigidbody>();
            }

            CFJ[_index].autoConfigureConnectedAnchor = false;
            CFJ[_index].xMotion = CFJ[_index].yMotion = CFJ[_index].zMotion = ConfigurableJointMotion.Limited;
            //CFJ[_index].angularXMotion = CFJ[_index].angularYMotion = CFJ[_index].angularZMotion = ConfigurableJointMotion.Limited;
            CFJ[_index].anchor = CFJ[_index].connectedAnchor = new Vector3(0, 0, 0);
            CFJ[_index].linearLimit = Limit;
            CFJ[_index].linearLimitSpring = From.GetComponent<ConfigurableJoint>().linearLimitSpring;
        }
    }

    public void UpdateLimitFloat(float _Length)
    {
        LimitFloat = _Length;
    }

    public void UpdateLImit()
    {
        float LMT = 0f;
        Limit.limit = LimitFloat / Level;
        foreach (ConfigurableJoint cfj in CFJ)
        {
            SoftJointLimit CurrentLimit = cfj.linearLimit;

            float currentForce = cfj.currentForce.magnitude;
            if (currentForce  < 2700f && currentForce  > 100f)
            {
                CurrentLimit.limit = LimitFloat / Level;
            }
            else if(currentForce  >=2700f)
            {
                CurrentLimit.limit += 0.1f;
            }
            else if (currentForce <= 100f)
            {
                CurrentLimit.limit -= 0.1f;
            }

            CurrentLimit.contactDistance = CurrentLimit.limit * 0.3f;
           // CurrentLimit.bounciness = 0.1f;
            cfj.linearLimit = CurrentLimit;
            //print(cfj.currentForce.magnitude);
        }

        SoftJointLimit CurrentFrmLmt = FromJnt.linearLimit;
        float currentFrmForce = FromJnt.currentForce.magnitude;
        if (currentFrmForce  < 2700f && currentFrmForce  > 100f)
        {
            CurrentFrmLmt.limit = LimitFloat / Level;
        }else if (currentFrmForce > 2700f)
        {
            CurrentFrmLmt.limit += 0.1f;
        }
        else if (currentFrmForce <= 100f)
        {
            CurrentFrmLmt.limit -= 0.1f;
        }

        CurrentFrmLmt.contactDistance = CurrentFrmLmt.limit * 0.3f;

        FromJnt.linearLimit = CurrentFrmLmt;
        //print(currentFrmForce);
    }

    public void UpdateLimitSpring(SoftJointLimitSpring _spring)
    {
       Spring = new SoftJointLimitSpring();
        foreach (ConfigurableJoint cfj in CFJ)
        {
            cfj.linearLimitSpring= _spring;
        }
        FromJnt.linearLimitSpring = _spring;
    }
     
    void updateSegmentRotation()
    {

        foreach (var sgm in Segments)
        {
            int _index = Array.IndexOf(Segments, sgm);
            Vector3 _q = new Vector3();
            if(Level !=2)
            {
                if (_index == 0)
                {
                    _q = Segments[_index + 1].transform.localPosition - From.localPosition;
                }
                else if (_index == Level - 2)
                {
                    _q = To.transform.localPosition - Segments[_index - 1].transform.localPosition;
                }
                else
                {
                    _q = Segments[_index + 1].transform.localPosition - Segments[_index - 1].transform.localPosition;
                }
            }
            else
            {
                _q = To.transform.localPosition - From.localPosition;
            }
            var fwd = Vector3.Cross(Vector3.down, _q);

            sgm.transform.localRotation = Quaternion.LookRotation(fwd, _q);

        }
    }


    // Update is called once per frame
    void Update ()
    {
       
    }
}
