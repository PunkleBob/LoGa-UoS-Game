//Class derived from Fungus base - released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Static class that hooks into the hierachy changed and item gui callbacks to put
/// an icon infront of all GOs that have a basic flow engine on them
/// </summary>
[InitializeOnLoad]
public class HierarchyIcons
{
    // the icon to display
    static Texture2D TextureIcon { get { return LogaEditorResources.LogaFavicon; } }

    //sorted list of the GO instance IDs that have flowcharts on them
    static List<int> engineIDs = new List<int>();

    static bool initalHierarchyCheckFlag = true;

    static HierarchyIcons()
    {
        initalHierarchyCheckFlag = true;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyIconCallback;
#if UNITY_2018_1_OR_NEWER
        EditorApplication.hierarchyChanged += HierarchyChanged;
#else
            EditorApplication.hierarchyWindowChanged += HierarchyChanged;
#endif
    }

    //track all gameobjectIds that have flowcharts on them
    static void HierarchyChanged()
    {
        engineIDs.Clear();

        if (LogaEditorPreferences.hideIconInHierarchy)
            return;

        var engines = GameObject.FindObjectsOfType<BasicFlowEngine>();

        engineIDs = engines.Select(x => x.gameObject.GetInstanceID()).Distinct().ToList();
        engineIDs.Sort();
    }

    //Draw icon if the isntance id is in our cached list
    static void HierarchyIconCallback(int instanceID, Rect selectionRect)
    {
        if (initalHierarchyCheckFlag)
        {
            HierarchyChanged();
            initalHierarchyCheckFlag = false;
        }

        if (LogaEditorPreferences.hideIconInHierarchy)
            return;

        // place the icon to the left of the element
        Rect r = new Rect(selectionRect);
#if UNITY_2019_1_OR_NEWER
        r.x -= 28;  //this would make sense as singleLineHeight *2 but it isn't as that includes padding
#else
            r.x = 0;
#endif
        r.width = r.height;

        //GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        //binary search as it is much faster to cache and int bin search than GetComponent
        //  should be less GC too
        if (engineIDs.BinarySearch(instanceID) >= 0)
            GUI.Label(r, TextureIcon);
    }
}