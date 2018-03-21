using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageHandler : MonoBehaviour
{

    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;

    private bool toggle = true;

	// Use this for initialization
	void Start ()
	{
	    GetComponent<Image>().sprite = on;

    }

    public void SwithTexture()
    {
        if (toggle == true)
        {
            toggle = false;
            GetComponent<Image>().sprite = off;
        }
        else
        {
            toggle = true;
            GetComponent<Image>().sprite = on;
        }
    }

    public void SetTexture(bool state)
    {
        if (state == true)
        {
            toggle = true;
            GetComponent<Image>().sprite = on;
        }
        else
        {
            toggle = false;
            GetComponent<Image>().sprite = off;
        }
    }

	// Update is called once per frame
	void Update ()
	{
	   
	}
}
