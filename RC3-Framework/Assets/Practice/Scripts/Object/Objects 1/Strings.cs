using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Strings : MonoBehaviour
{
    private Transform Point0;
    private Transform Point1;

    private GameObject String0;
    private GameObject String1;

    private SubString Str0 = new SubString();
    private SubString Str1=new SubString();

    private GameObject Pivot;


    [SerializeField]private GameObject _CanvasPrefab;
    [SerializeField] private Text IndexTextPrefab;
    private  Transform LengthSlider;

    private Vector3 _Joint0 = new Vector3();
    private Vector3 _Joint1 = new Vector3();

    private ConfigurableJoint  S0 = new ConfigurableJoint();
    private ConfigurableJoint S1 = new ConfigurableJoint();

    SoftJointLimit Limit= new SoftJointLimit();

    private GameObject _Canvas;
    private Text IndexTxt;

    private float defualtStringLength;
    private float updateStringLength;
    private float Elasticity = 0.01f;
    private float Force0 = 0f;
    private float Force1 = 0f;
    private float Thickness;
    private int index;

    void SetString()
    {
        
    }
    public void ConnectString(Transform  J0, Transform  J1, float _thickness, int _index, int _SubdiveideLevel)
    {
        index = _index;
        Point0 = J0;
        Point1 = J1;
        _Joint0 = Point0.localPosition;
        _Joint1 = Point1.localPosition;
        Thickness = _thickness;

        var d = _Joint1 - _Joint0;
        var L = d.magnitude;
        defualtStringLength = L;

        Pivot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Pivot.GetComponent<MeshRenderer>().material.color = Color.black;


        Pivot.transform.parent = transform;

        _Canvas = Instantiate(_CanvasPrefab, Pivot .transform);


        var T = gameObject.GetComponent<Transform>();

        var pvForm = Pivot.transform;

        LengthSlider = _Canvas.GetComponent<RectTransform>().GetChild(0);
        LengthSlider.GetComponent<Slider>().value =updateStringLength = defualtStringLength;

        var CvsForm = _Canvas.GetComponent<RectTransform>();

        IndexTxt = Instantiate(IndexTextPrefab, CvsForm);

        IndexTxt.text = index.ToString();

        pvForm.localPosition  = (_Joint0 +_Joint1) * 0.5f;
        pvForm .localRotation  = Quaternion.FromToRotation(pvForm.up, d);
        pvForm.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Pivot.AddComponent<Rigidbody>();
        //Pivot.GetComponent<Rigidbody>().useGravity = false;
        Pivot.GetComponent<Rigidbody>().drag  = 5f;
        Pivot.GetComponent<Rigidbody>().angularDrag = 10f;
        Pivot.GetComponent<SphereCollider>().enabled = false;
        // Pivot.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);


        SoftJointLimitSpring spring = new SoftJointLimitSpring();
        spring.spring = 3000f;
        spring.damper = 100f;

        Str0.SetupSubSting(Pivot.transform, Point0, _SubdiveideLevel, Thickness);
        Str0.UpdateLimitSpring(spring);
        Str1.SetupSubSting(Pivot.transform , Point1, _SubdiveideLevel, Thickness);
        //Str1.UpdateLimitSpring(spring);

        gameObject.name = "String" + index;
    }

    public void ChangeSliderValue(float _length)
    {
       
        LengthSlider.GetComponent<Slider>().value = _length;
    }

    public void UpdateStringLength()
    {
       var _L = LengthSlider.GetComponent<Slider>().value;
        updateStringLength = _L;
    }

    public  void UpdateStringPosition()
    {
        _Joint0 = Point0.localPosition ;
        _Joint1 = Point1.localPosition ;

        var d = _Joint1 - _Joint0;
        var L = d.magnitude;
        var T = gameObject.GetComponent<Transform>();
        var Lc = Pivot.transform;
        var CvsForm = _Canvas.GetComponent<RectTransform>();

        var q = Vector3.Cross(Vector3.down, d);
       Lc.localRotation = Quaternion.LookRotation(q, d);
        //Lc.localPosition = (_Joint0 + _Joint1) * 0.5f;

        CvsForm.localRotation = Quaternion.identity;
        CvsForm.localScale = new Vector3(1f, 1f, 1f);
    }

    public void DestroyThisString()
    {
        DestroyImmediate( LengthSlider.gameObject);
        DestroyImmediate( _Canvas);
        DestroyImmediate( Pivot);
        DestroyImmediate(Str0);
        DestroyImmediate(Str1);
    }

    private void Update()
    {
        
    }

    public void UpdateAllString()
    {
        UpdateStringPosition();
        UpdateStringLength();
        Str0.UpdateAllWire();
        Str1.UpdateAllWire();
        Str0.UpdateLimitFloat(updateStringLength / 2);
        Str0.UpdateLImit();
        Str1.UpdateLimitFloat(updateStringLength / 2);
        Str1.UpdateLImit();
    }

    public void printForce()
    {
        var f0 = S0.currentForce.magnitude;
        var f1 = S1.currentForce.magnitude;
        Force0 = f0;
        Force1 = f1;
    }

    public int GetVertexIndex0()
    {
        return Point0.GetComponent<TJoint>().GetIndex();
    }
    public float GetDefStringLength()
    {
        return defualtStringLength;
    }

    public float GetUpdateStringLength()
    {
        return updateStringLength;
    }

    public void SetUpdateStringLength(float _length)
    {
        updateStringLength =( _length);
    }

    public void ExtendUpdateLength(float _amount)
    {
        updateStringLength += _amount;
    }
    public void TrimUpdateLength(float _amount)
    {
        updateStringLength -= _amount;
    }

}
