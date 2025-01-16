using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by the Engine window to serialize the currently active Engine object
/// so that the same Engine can be displayed while editing & playing.
/// </summary>
[AddComponentMenu("")]
public class LogaStates : MonoBehaviour
{
    [SerializeField] protected BasicFlowEngine selectedEngine;

    public virtual  BasicFlowEngine SelectedEngine { get { return selectedEngine; } set { selectedEngine = value; } }
}
