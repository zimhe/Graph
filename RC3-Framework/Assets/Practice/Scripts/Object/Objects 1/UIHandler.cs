using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Slider LengthChanger;

    [SerializeField] private Text LengthText;

    private float Defualt = 11f;

    private void Awake()
    {
        
    }


    // Use this for initialization
	void Start ()
	{
	   
	    LengthChanger.value = Defualt;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    LengthText.text = LengthChanger.value.ToString();
	    LengthText.color = Color.white;
	}

    public float SliderValue()
    {
        return LengthChanger.value;
    }
}
