using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    private BasicFlowEngine engine;
    private Dictionary<string, Variable> globalVariables = new Dictionary<string, Variable>();

    private void Awake()
    {
        engine = new GameObject("GlobalVariablesEngine").AddComponent<BasicFlowEngine>();
        engine.transform.parent = transform;
    }

    public Variable GetGlobalVariable(string key)
    {
        Variable v = null;
        globalVariables.TryGetValue(key, out v);
        return v;
    }

    public BaseVariable<T> GetOrAddVariable<T>(string variableKey, T defaultvalue, Type type)
    {
        Variable v = null;
        BaseVariable<T> vAsT = null;
        var res = globalVariables.TryGetValue(variableKey, out v);

        if (res && v != null)
        {
            vAsT = v as BaseVariable<T>;

            if (vAsT != null)
            {
                return vAsT;
            }
            else
            {
                Debug.LogError("A variable of name " + variableKey + " already exists, but of a different type");
            }
        }
        else
        {
            //create the variable
            vAsT = engine.gameObject.AddComponent(type) as BaseVariable<T>;
            vAsT.Value = defaultvalue;
            vAsT.Key = variableKey;
            vAsT.Scope = VariableScope.Public;
            globalVariables[variableKey] = vAsT;
            engine.Variables.Add(vAsT);
        }

        return vAsT;
    }
}
