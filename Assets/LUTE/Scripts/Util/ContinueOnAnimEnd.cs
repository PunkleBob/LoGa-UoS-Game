using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script to set specific continue event which is called using animation events
public class ContinueOnAnimEnd : MonoBehaviour
{
    UnityEngine.Events.UnityAction cont = null;
    private void Continue()
    {
        if(cont != null)
        {
            cont.Invoke();
        }
    }

    public void SetEvent(UnityEngine.Events.UnityAction _cont)
    {
        if(_cont != null)
        {
            cont = _cont;
        }
    }
}
