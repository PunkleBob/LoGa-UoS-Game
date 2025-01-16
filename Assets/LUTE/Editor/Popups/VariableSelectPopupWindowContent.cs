using LoGaCulture.LUTE;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Show the variable selection window as a searchable popup
public class VariableSelectPopupWindowContent : BasePopupWindowContent
{
    static readonly int POPUP_WIDTH = 200, POPUP_HEIGHT = 200;
    static List<System.Type> _variableTypes;
    static List<System.Type> VariableTypes
    {
        get
        {
            if (_variableTypes == null || _variableTypes.Count == 0)
                CacheVariableTypes();

            return _variableTypes;
        }
    }

    static void CacheVariableTypes()
    {
        var derivedType = typeof(Variable);
        _variableTypes = EditorExtensions.FindDerivedTypes(derivedType)
            .Where(x => !x.IsAbstract && derivedType.IsAssignableFrom(x))
            .ToList();
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        CacheVariableTypes();
    }

    protected override void PrepareAllItems()
    {
        int i = 0;
        foreach (var item in VariableTypes)
        {
            VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(item);
            if (variableInfo != null)
            {
                allItems.Add(new FilteredListItem(i, (variableInfo.Category.Length > 0 ? variableInfo.Category + CATEGORY_CHAR : "") + variableInfo.VariableType));
            }

            i++;
        }
    }

    protected override void SelectByOrigIndex(int index)
    {
        AddVariable(VariableTypes[index]);
    }

    static public void DoAddVariable(Rect position, string currentHandlerName, BasicFlowEngine engine)
    {
        curEngine = engine;
        //new method
        VariableSelectPopupWindowContent win = new VariableSelectPopupWindowContent(currentHandlerName, POPUP_WIDTH, POPUP_HEIGHT);
        PopupWindow.Show(position, win);
        //old method
    }

    private static BasicFlowEngine curEngine;

    public VariableSelectPopupWindowContent(string currentHandlerName, int width, int height)
        : base(currentHandlerName, width, height)
    {
    }

    public static void AddVariable(object obj)
    {
        AddVariable(obj, string.Empty);
    }

    public static Variable AddVariable(object obj, string suggestedName)
    {
        System.Type t = obj as System.Type;
        if (t == null)
        {
            return null;
        }

        var engine = curEngine != null ? curEngine : GraphWindow.GetEngine();

        if (engine == null)
        {
            Debug.LogError("No engine found to add variable to");
            return null;
        }

        Undo.RecordObject(engine, "Add Variable");
        Variable newVariable = engine.gameObject.AddComponent(t) as Variable;

        newVariable.Key = engine.GetUniqueVariableKey(suggestedName);

        //if suggested exists, then insert, if not just add
        var existingVariable = engine.GetVariable(suggestedName);
        if (existingVariable != null)
        {
            engine.Variables.Insert(engine.Variables.IndexOf(existingVariable) + 1, newVariable);
        }
        else
        {
            engine.Variables.Add(newVariable);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(engine);

        return newVariable;
    }

    public static Variable AddVariable(object obj, string suggestedName, LUTELocationInfo location)
    {
        System.Type t = obj as System.Type;
        if (t == null)
        {
            return null;
        }

        var engine = curEngine != null ? curEngine : GraphWindow.GetEngine();
        Undo.RecordObject(engine, "Add Variable");
        Variable newVariable = engine.gameObject.AddComponent(t) as Variable;
        newVariable.Key = engine.GetUniqueVariableKey(suggestedName);

        //rather than only allowing location checks we should take in any struct and check against the variable type value - should also take in what scope you like
        if (newVariable.GetType() == typeof(LocationVariable) && location != null)
        {
            newVariable.Apply(SetOperator.Assign, location);
            newVariable.Scope = VariableScope.Public;
        }

        //if suggested exists, then insert, if not just add
        var existingVariable = engine.GetVariable(suggestedName);
        if (existingVariable != null)
        {
            engine.Variables.Insert(engine.Variables.IndexOf(existingVariable) + 1, newVariable);
        }
        else
        {
            engine.Variables.Add(newVariable);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(engine);

        return newVariable;
    }

    public static Variable AddVariable(object obj, string suggestedName, NodeCollection node)
    {
        System.Type t = obj as System.Type;
        if (t == null)
        {
            return null;
        }

        var engine = curEngine != null ? curEngine : GraphWindow.GetEngine();
        Undo.RecordObject(engine, "Add Variable");
        Variable newVariable = engine.gameObject.AddComponent(t) as Variable;
        newVariable.Key = engine.GetUniqueVariableKey(suggestedName);

        if (newVariable.GetType() == typeof(NodeCollectionVariable) && node != null)
        {
            newVariable.Apply(SetOperator.Assign, node);
            newVariable.Scope = VariableScope.Global;
        }

        //if suggested exists, then insert, if not just add
        var existingVariable = engine.GetVariable(suggestedName);
        if (existingVariable != null)
        {
            engine.Variables.Insert(engine.Variables.IndexOf(existingVariable) + 1, newVariable);
        }
        else
        {
            engine.Variables.Add(newVariable);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(engine);

        return newVariable;
    }

    public static Variable AddVariable(object obj, string suggestedName, Node node)
    {
        System.Type t = obj as System.Type;
        if (t == null)
        {
            return null;
        }

        var engine = curEngine != null ? curEngine : GraphWindow.GetEngine();
        Undo.RecordObject(engine, "Add Variable");
        Variable newVariable = engine.gameObject.AddComponent(t) as Variable;
        newVariable.Key = engine.GetUniqueVariableKey(suggestedName);

        if (newVariable.GetType() == typeof(NodeVariable) && node != null)
        {
            newVariable.Apply(SetOperator.Assign, node);
            newVariable.Scope = VariableScope.Global;
        }

        //if suggested exists, then insert, if not just add
        var existingVariable = engine.GetVariable(suggestedName);
        if (existingVariable != null)
        {
            engine.Variables.Insert(engine.Variables.IndexOf(existingVariable) + 1, newVariable);
        }
        else
        {
            engine.Variables.Add(newVariable);
        }

        // Because this is an async call, we need to force prefab instances to record changes
        PrefabUtility.RecordPrefabInstancePropertyModifications(engine);

        return newVariable;
    }
}
