//Script derived from Fungus - released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Select the UI element when the gameobject is enabled.
/// </summary>
[RequireComponent(typeof(Selectable))]
public class SelectOnEnable : MonoBehaviour
{
    protected Selectable selectable;

    protected void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    protected void OnEnable()
    {
        selectable.Select();
    }
}
