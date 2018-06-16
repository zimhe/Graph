using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Notes
 */


/// <summary>
/// attach this script to the camera
/// </summary>
public class CamObjectSelector : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private float _sensitivity = 1.0f;

    [SerializeField] private float _stiffness = 10.0f;

    [SerializeField] private GumBall gumBall;

    private int mask;

    [SerializeField] private float GumballSize = 0.1f;

    private RaycastHit RcHit;

    private RaycastHit GumHit;

    private Transform Target;

    private Vector3 _position;
    private Vector3 _scale;
    private Quaternion _rotation;


    private bool OnAction;

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        _camera = GetComponent<Camera>();

        gumBall.gameObject.SetActive(false);


    }


    /// <summary>
    /// 
    /// </summary>
    void Update()
    {

        SelectionOperator();

        updateGumBall();
    }

    void updateGumBall()
    {
        var z = Mathf.Clamp(Mathf.Abs(transform.localPosition.z), 6f, 40f);
        gumBall.transform.localScale = z * GumballSize * Vector3.one*0.1f;



        if (!SomethingOnRadius())
        {
            gumBall.ResetColor();
        }
    }

    void SelectionOperator()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RcHit))
        {
            if (RcHit.transform.GetComponent<SelectiveObject>())
            {
                if (Input.GetMouseButtonDown(0))
                {

                    if (Target != RcHit.transform&&Target!=null)
                    {
                        Target.GetComponent<SelectiveObject>().SetState(SelectiveObject.SelectionState.Default);
                        Target = RcHit.transform;
                        _scale = Target.localScale;
                        _rotation = Target.rotation;
                    }
                    else
                    {
                        Target = RcHit.transform;
                    }
                       

                    Target.GetComponent<SelectiveObject>() .SetState(SelectiveObject.SelectionState.Selected);

                    gumBall.gameObject.SetActive(true);

                    gumBall.Localize(Target);
                    _position = Target.position;
                    _scale = Target.localScale;
                    _rotation = Target.rotation;
                }
            }
        }

        mask = LayerMask.GetMask("Gumball");
        

        if (Physics.Raycast(ray, out GumHit,Mathf.Infinity,mask))
        {


            if (GumHit.transform.parent == gumBall.transform && !OnAction)
            {
                gumBall.overAxis = GumHit.transform;
                gumBall.HIghlightAxis();
            }
        }


        if (Input.GetMouseButton(0))
        {
            if (gumBall.overAxis != null)
            {
                var t = _sensitivity * transform.localPosition.z * 0.1f;

                var dx = Input.GetAxis("Mouse X") * t;
                var dy = Input.GetAxis("Mouse Y") * t;

                if (dx != 0.0f || dy != 0.0f)
                {
                    OnAction = true;
                }

                var v = transform.TransformVector(dx, dy, 0f);

                var tv = Target.InverseTransformVector(v);

                float px = 0f;
                float py = 0f;
                float pz = 0f;

                float sx = 0f;
                float sy = 0f;
                float sz = 0f;
                float sa = 0f;

                float angles = 0f;

                Vector3 Axis=Vector3.zero;
                Vector3 Direction = Vector3.zero;


                if (gumBall.overAxis.name == "X Axis")
                {
                    px -= tv.x;
                    Direction = gumBall.transform.TransformDirection(Vector3.right);
                }

                if (gumBall.overAxis.name == "Y Axis")
                {
                    py -= tv.y;
                    Direction = gumBall.transform.TransformDirection(Vector3.up);
                }

                if (gumBall.overAxis.name == "Z Axis")
                {
                    pz -= tv.z;
                    Direction = gumBall.transform.TransformDirection(Vector3.forward);
                }

                if (gumBall.overAxis.name == "XZ")
                {
                    px -= tv.x;
                    pz -= tv.z;
                }

                if (gumBall.overAxis.name == "XY")
                {
                    px -= tv.x;
                    py -= tv.y;
                }

                if (gumBall.overAxis.name == "YZ")
                {
                    py -= tv.y;
                    pz -= tv.z;
                }

                if (gumBall.overAxis.name == "Scale X")
                {
                    sx += tv.x+tv.y+tv.z;
                }
                if (gumBall.overAxis.name == "Scale Y")
                {
                    sy += tv.x+tv.y+tv.z;
                }
                if (gumBall.overAxis.name == "Scale Z")
                {
                    sz += tv.x + tv.y + tv.z;
                }

                if (gumBall.overAxis.name == "O")
                {
                    float s = 0f;

                    s+=tv.x-tv.y+tv.z;
                    sx = sy = sz = s;
                }

                if (gumBall.overAxis.name == "Arc X")
                {

                    angles += dx-dy;

                    Axis = gumBall.transform.TransformDirection(Vector3.right);
                }
                if (gumBall.overAxis.name == "Arc Y")
                {
                    angles += dx-dy;
                 
                    Axis = gumBall.transform.TransformDirection(Vector3.up);
                }
                if (gumBall.overAxis.name == "Arc Z")
                {
                    angles += dx-dy;
                   
                    Axis = gumBall.transform.TransformDirection(Vector3.forward);
                }

                var p = Target.transform.TransformVector(px/Target.transform.localScale.x, py / Target.transform.localScale.y, pz / Target.transform.localScale.z);

                _position += p;
                _scale+=new Vector3(_scale.x*sx*0.1f,_scale.y*sy*0.1f,_scale.z*sz*0.1f);

                gumBall.transform.position =
                    Vector3.Lerp(gumBall.transform.position, _position, Time.deltaTime * _stiffness);

                if (angles != 0f)
                {
                    float Agls = Mathf.LerpAngle(0f, angles, Time.deltaTime * _stiffness*5f);
                    gumBall.transform.RotateAround(gumBall.transform.position, Axis, Agls * 3f);
                }

               
                   
                Target.position = gumBall.transform.position;
                Target.rotation = gumBall.transform.rotation;

                Target.localScale= Vector3.Lerp(Target.localScale, _scale, Time.deltaTime * _stiffness);
            }
        }
        else
        {
            _position = gumBall.transform.position;
            _rotation = gumBall.transform.rotation;

            if (Target != null)
                _scale = Target.transform.localScale;

            OnAction = false;


        }

        if (Input.GetMouseButtonDown(0))
        {

            var radius = Mathf.Abs(transform.localPosition.z) * GumballSize;

            if (!SomethingOnRadius()&&Target!=null)
            {
                gumBall.Restore();
                gumBall.gameObject.SetActive(false);

                Target.GetComponent<SelectiveObject>().SetState(SelectiveObject.SelectionState.Default); 
                Target = null;
            }
        }
    }



    private bool SomethingOnRadius()
    {
        bool onRadius;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        var z = Mathf.Clamp(Mathf.Abs(transform.localPosition.z), 6f, 40f);

        var radius = z * GumballSize*0.1f;

        if (!Physics.SphereCast(ray, radius) && !OnAction)
        {
            onRadius = false;
        }
        else
        {
            onRadius = true;
        }


        return onRadius;
    }

    float eulerValue(float _v)
    {
        float e = _v;

        if (e > 180f)
        {
            e -= 360f;
        }

        if (e < -180f)
        {
            e += 360f;
        }

        return e;
    }

}

