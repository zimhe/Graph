using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GumBall : MonoBehaviour
{
    [SerializeField] private Color OnAxisColor;

    [SerializeField] private Transform AxisX;
    [SerializeField] private Transform AxisY;
    [SerializeField] private Transform AxisZ;

    [SerializeField] private Transform XZ;
    [SerializeField] private Transform XY;
    [SerializeField] private Transform YZ;

    [SerializeField] private Transform ArcX;
    [SerializeField] private Transform ArcY;
    [SerializeField] private Transform ArcZ;

    [SerializeField] private Transform ScaleX;
    [SerializeField] private Transform ScaleY;
    [SerializeField] private Transform ScaleZ;

    [SerializeField] private Transform O;

    private Transform Target;

    public Transform overAxis { get; set; }

    private List<Transform> Axises = new List<Transform>(6);


    Dictionary<Transform,Color> DefColor= new Dictionary<Transform, Color>();


    // Use this for initialization
    void Start ()
    {
        Axises.AddRange(new [] {AxisX, AxisY, AxisZ,XZ,XY,YZ,O,ArcX,ArcY,ArcZ,ScaleX,ScaleY,ScaleZ});

        Axises.ForEach(a =>
        {
            if (a.childCount == 0)
            {
                DefColor.Add(a, a.GetComponent<MeshRenderer>().material.color);
            }
            else
            {
                DefColor.Add(a,a.GetChild(0).GetComponent<MeshRenderer>().material.color);
            }
        });

      
    }
	
	// Update is called once per frame
	void Update ()
	{

	    if (Target != null)
	    {
            UpdateScalePosition(Target.localScale);
	    }
	    else
	    {
	        UpdateScalePosition(transform.localScale*3f);
	    }
	}


    public void HIghlightAxis()
    {
        Axises.ForEach(a =>
        {
            if (overAxis != a)
            {
             
                if (a.childCount > 0)
                {
                    for (int i = 0; i < a.childCount; i++)
                    {
                        a.GetChild(i).GetComponent<MeshRenderer>().material.color = DefColor[a];
                    }
                }
                else
                {
                    a.GetComponent<MeshRenderer>().material.color = DefColor[a];
                }
                    

            }
        });


        if (overAxis != null)
        {
            
            if (overAxis.childCount>0)
            {
                for (int i = 0; i < overAxis.childCount; i++)
                {
                    overAxis.GetChild(i).GetComponent<MeshRenderer>().material.color = OnAxisColor;
                }
            }
            else
            {
                overAxis.GetComponent<MeshRenderer>().material.color = OnAxisColor;
            }
        }
    }

    public void Restore()
    {
        overAxis = null;
        ResetColor();
    }

    public void ResetColor()
    {
        Axises.ForEach(a => {  if(a.childCount>0)
            {
                for (int i = 0; i < a.childCount; i++)
                {
                    a.GetChild(i).GetComponent<MeshRenderer>().material.color = DefColor[a];
                }
            }
            else
            {
                a.GetComponent<MeshRenderer>().material.color = DefColor[a];
            }
        });
    }

    public void Localize(Transform _target)
    {
        transform.position = _target.position;
        transform.localRotation = _target.localRotation;

        Target = _target;

       UpdateScalePosition(_target.localScale);
    }

    public void UpdateScalePosition(Vector3 _scale)
    {
        ScaleX.localPosition = new Vector3(-_scale.x/(transform.localScale.x*2f)-0.2f, 0f, 0f);
        ScaleY.localPosition = new Vector3(0f, -_scale.y/(transform.localScale.y*2f)-0.2f, 0f);
        ScaleZ.localPosition = new Vector3(0f, 0f, -_scale.z/(transform.localScale.z*2)-0.2f);
    }


}
