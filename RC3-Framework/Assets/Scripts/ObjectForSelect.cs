using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// specify behaviors of objects on Highlighting and Selecting
/// </summary>
public class ObjectForSelect : SelectiveObject
{
  
    [SerializeField] private Color HighlightColor;
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color DefaultColor;


    public override void Highlight()
    {
        transform.GetComponent<MeshRenderer>().material.color = HighlightColor;
    }

    public override void Restore()
    {
        transform.GetComponent<MeshRenderer>().material.color = DefaultColor;
    }

    public override void Select()
    {
        transform.GetComponent<MeshRenderer>().material.color = SelectedColor;
    }
}
