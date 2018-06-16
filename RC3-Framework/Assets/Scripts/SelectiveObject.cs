using UnityEngine;
using System.Collections;

public abstract class SelectiveObject : MonoBehaviour
{

    private bool over;

    private bool selected;

    private SelectionState State = SelectionState.Default;


 


    private void OnMouseEnter()
    {
        over = true;
    }



    private void OnMouseOver()
    {
        if(State!=SelectionState.Selected)
        Highlight();
    }

    private void OnMouseExit()
    {
        if (State != SelectionState.Selected)
            Restore();


        over = false;
    }

    public bool IsOver()
    {
        return over;
    }

    public enum SelectionState
    {
        Selected,
        Default
    }

    public void SetState(SelectionState _state)
    {
        State = _state;
        switch (_state)
        {
            case SelectionState.Default:
                Restore();
                break;
            case SelectionState.Selected:
                Select();
                break;
        }

    }

    public abstract void Highlight();


    public abstract void Restore();

    public abstract void Select();

}
