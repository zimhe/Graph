using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTips : MonoBehaviour
{


    [SerializeField] private string toolTips;
	// Use this for initialization

    private Text Txt;

    private Button btn;

	void Start ()
	{
	    Txt.text = toolTips;
	    Txt.transform.parent = transform;

        Txt.transform.localPosition=Vector3.up;

	    btn = transform.GetComponent<Button>();



	}


    // Update is called once per frame
    void Update ()
    {
  

		
	}
}
