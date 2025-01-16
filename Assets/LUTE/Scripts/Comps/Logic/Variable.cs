using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class PropertyReference
{
    [SerializeField]
    private GameObject targetObject;
    [SerializeField]
    private string propertyName;
    private PropertyInfo cachedProperty;

    public GameObject TargetObject => targetObject;
    public string PropertyName => propertyName;

    public PropertyReference(GameObject target, string propertyName)
    {
        this.targetObject = target;
        this.propertyName = propertyName;

        // Search through all components for the property
        FindPropertyInComponents();
    }

    private object FindPropertyInComponents()
    {
        if (targetObject == null) return null;

        // Get all components on the GameObject
        Component[] components = targetObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component == null) continue;

            // Get the type of the component
            System.Type componentType = component.GetType();

            // Search for the field with the given name
            FieldInfo field = componentType.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                // Field found, return its value
                return field.GetValue(component);
            }
        }

        Debug.LogWarning($"Field '{propertyName}' not found on GameObject '{targetObject.name}'.");
        return null;
    }

    public void SetValue(object value)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is null. Aborting SetValue operation.");
            return;
        }

        try
        {
            // Get all components on the GameObject
            Component[] components = targetObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                try
                {
                    // Get the type of the component
                    System.Type componentType = component.GetType();

                    // Search for the field with the given name
                    FieldInfo field = componentType.GetField(propertyName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    if (field != null)
                    {
                        // Ensure value is compatible with the field type
                        if (value == null || field.FieldType.IsAssignableFrom(value.GetType()))
                        {
                            field.SetValue(component, value);
                        }
                        else
                        {
                            Debug.LogWarning($"Type mismatch for field '{propertyName}'. Expected '{field.FieldType}', got '{value?.GetType()}'.");
                        }
                    }
                }
                catch (Exception fieldEx)
                {
                    Debug.LogError($"Error setting field '{propertyName}' in component '{component?.GetType().Name}': {fieldEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while setting value for property '{propertyName}' on target object '{targetObject.name}': {ex.Message}");
        }
    }


    public object GetValue(string propertyName)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is null. Aborting GetValue operation.");
            return null;
        }

        try
        {
            // Get all components on the GameObject
            Component[] components = targetObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                try
                {
                    // Get the type of the component
                    System.Type componentType = component.GetType();

                    // Search for the field with the given name
                    FieldInfo field = componentType.GetField(propertyName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    if (field != null)
                    {
                        return field.GetValue(component); // Return the field value
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error retrieving field '{propertyName}' in component '{component?.GetType().Name}': {ex.Message}");
                }
            }

            Debug.LogWarning($"Field '{propertyName}' not found on any components of GameObject '{targetObject.name}'.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while getting the value of '{propertyName}' on target object '{targetObject.name}': {ex.Message}");
        }

        return null; // Return null if the field was not found or an error occurred
    }


    // Helper method to get the type of the property
    public Type GetPropertyType()
    {
        return cachedProperty?.PropertyType;
    }

    // Helper method to get the component type that contains the property
    public System.Type GetComponentType(string propertyName)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is null. Aborting GetComponentType operation.");
            return null;
        }

        try
        {
            // Get all components on the GameObject
            Component[] components = targetObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                try
                {
                    // Get the type of the component
                    System.Type componentType = component.GetType();

                    // Search for the field with the given name
                    FieldInfo field = componentType.GetField(propertyName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    if (field != null)
                    {
                        return componentType; // Return the type of the component
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error finding field '{propertyName}' in component '{component?.GetType().Name}': {ex.Message}");
                }
            }

            Debug.LogWarning($"No component containing field '{propertyName}' was found on GameObject '{targetObject.name}'.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred while getting the component type for '{propertyName}' on target object '{targetObject.name}': {ex.Message}");
        }

        return null; // Return null if no component was found
    }

}

// Standard comparison operators.
public enum ComparisonOperator
{
    /// <summary> == mathematical operator.</summary>
    Equals,
    /// <summary> != mathematical operator.</summary>
    NotEquals,
    /// <summary> < mathematical operator.</summary>
    LessThan,
    /// <summary> > mathematical operator.</summary>
    GreaterThan,
    /// <summary> <= mathematical operator.</summary>
    LessThanOrEquals,
    /// <summary> >= mathematical operator.</summary>
    GreaterThanOrEquals
}

// Mathematical operations that can be performed on variables.
public enum SetOperator
{
    /// <summary> = operator. </summary>
    Assign,
    /// <summary> =! operator. </summary>
    Negate,
    /// <summary> += operator. </summary>
    Add,
    /// <summary> -= operator. </summary>
    Subtract,
    /// <summary> *= operator. </summary>
    Multiply,
    /// <summary> /= operator. </summary>
    Divide
}

public enum VariableScope
{
    /// <summary> Can only be accessed by commands in the same Engine. </summary>
    Private,
    /// <summary> Can be accessed from any command in any Engine. </summary>
    Public,
    /// <summary> Creates and/or references a global variable of that name, all variables of this name and scope share the same underlying variable and exist for the duration of the instance of Unity.</summary>
    Global,
}

// Helper class for variable info attribute - used to display variable info in the inspector
// Sealed class means it cannot be inherited from
public sealed class VariableInfoAttribute : System.Attribute
{
    //Note do not use "isPreviewedOnly:true", it causes the script to fail to load without errors shown
    public VariableInfoAttribute(string category, string variableType, int order = 0, bool isPreviewedOnly = false)
    {
        this.Category = category;
        this.VariableType = variableType;
        this.Order = order;
        this.IsPreviewedOnly = isPreviewedOnly;
    }

    public string Category { get; set; }
    public string VariableType { get; set; }
    public int Order { get; set; }
    public bool IsPreviewedOnly { get; set; }
}

// Attribute class for variable properties - used to display variable properties in the inspector
public sealed class VariablePropertyAttribute : PropertyAttribute
{
    public VariablePropertyAttribute(params System.Type[] variableTypes)
    {
        this.VariableTypes = variableTypes;
    }

    public VariablePropertyAttribute(AllVariableTypes.VariableAny any)
    {
        VariableTypes = AllVariableTypes.AllLogaVarTypes;
    }

    public VariablePropertyAttribute(string defaultText, params System.Type[] variableTypes)
    {
        this.defaultText = defaultText;
        this.VariableTypes = variableTypes;
    }

    public string defaultText = "<None>";
    public string compatibleVariableName = string.Empty;

    public System.Type[] VariableTypes { get; set; }
}

[RequireComponent(typeof(BasicFlowEngine))]
[System.Serializable]
public abstract class Variable : MonoBehaviour
{
    [SerializeField] protected VariableScope scope;
    [SerializeField] protected string key = "";
    [SerializeField] protected List<PropertyReference> propertyReferences = new List<PropertyReference>();

    public virtual VariableScope Scope
    {
        get { return scope; }
        set { scope = value; }
    }

    public virtual string Key
    {
        get { return key; }
        set { key = value; }
    }

    // If the engine is reset then this callback is used to reset the variable to its default value
    public abstract void OnReset();

    // Used by SetVariable, child classes required to declare and implement operators
    public abstract void Apply(SetOperator setOperator, object value);

    // Used by ifs, while etc. - child classes required to declare and implement comparisons
    public abstract bool Evaluate(ComparisonOperator comparisonOperator, object value);

    // Does the underlying type provide support for +-*/ operations?
    public virtual bool SupportsArithmetic(SetOperator setOperator) { return false; }

    // Does the underlying type provide support for ==/!=/< etc. operations?
    public virtual bool SupportsComparison() { return false; }

    // Referenced value of type defined within child classes; intended for editor code
    public abstract object GetValue();

    // Variables are required to be on an engine object so this method is used as a helper
    public virtual BasicFlowEngine GetEngine() { return GetComponent<BasicFlowEngine>(); }

    public virtual object ReturnValue() { return null; }

    // Find by target object and property name
    // Could use dictionary for faster lookup
    public PropertyReference FindPropertyReference(GameObject target, string propertyName)
    {
        if (target == null || string.IsNullOrEmpty(propertyName))
            return null;

        return propertyReferences.FirstOrDefault(p =>
            p.TargetObject == target && p.PropertyName == propertyName);
    }

    public PropertyReference AddPropertyReference(GameObject target, string propertyName)
    {
        var propertyReference = FindPropertyReference(target, propertyName);

        if (propertyReference == null)
        {
            propertyReference = new PropertyReference(target, propertyName);
            propertyReferences.Add(propertyReference);
        }
        return propertyReference;
    }

    public void RemovePropertyReference(GameObject target, string propertyName)
    {
        var propertyReference = FindPropertyReference(target, propertyName);

        if (propertyReference != null)
        {
            propertyReferences.Remove(propertyReference);
        }
    }

    protected virtual void Update()
    {

        foreach (var pr in propertyReferences)
        {
            object value = pr.GetValue(pr.PropertyName);
            object returnValue = ReturnValue();

            // If the value is null or different to the return value then set the value
            if (value == null || value != returnValue)
            {
                // If the return value is a string then apply text variations
                // We don't save the varied text return value as the variable's value as this will disrupt the variation system
                // This will not use rich text tags (like Dialogue Systems) as this is really intended behaviours
                if (returnValue.GetType() == typeof(string))
                {
                    string s = returnValue as string;
                    string variedText = TextVariationHandler.SelectVariations(s);
                    Apply(SetOperator.Assign, variedText);
                }
                pr.SetValue(ReturnValue());
            }
        }
    }
}

// Base class for all variable types - provides a common interface for accessing variable properties which is inherented from above
public abstract class BaseVariable<T> : Variable
{
    //cache mechanism for global static variables
    private BaseVariable<T> _globalStaticRef;
    private BaseVariable<T> globalStaticRef
    {
        get
        {
            if (_globalStaticRef != null)
            {
                return _globalStaticRef;
            }
            else if (Application.isPlaying)
            {
                return _globalStaticRef = LogaManager.Instance.GlobalVariables.GetOrAddVariable(Key, value, this.GetType());
            }
            else
            {
                return null;
            }
        }
    }

    [SerializeField] protected T value;

    public virtual T Value
    {
        get
        {
            if (scope != VariableScope.Global || !Application.isPlaying)
            {
                return this.value;
            }
            else
            {
                return globalStaticRef.value;
            }
        }
        set
        {
            if (scope != VariableScope.Global || !Application.isPlaying)
            {
                this.value = value;
            }
            else
            {
                globalStaticRef.Value = value;
            }
        }
    }

    public override object GetValue()
    {
        return value;
    }

    protected T startValue;

    public override void OnReset()
    {
        Value = startValue;
    }

    public override string ToString()
    {
        if (Value != null)
            return Value.ToString();
        else
            return "null";
    }

    protected virtual void Start()
    {
        //cache the start value to use later on
        startValue = Value;
    }

    // Apply to get from base system.object to T
    public override void Apply(SetOperator setOperator, object value)
    {
        if (value is T || value == null)
            Apply(setOperator, (T)value);
        else if (value is BaseVariable<T>)
        {
            var bv = value as BaseVariable<T>;
            Apply(setOperator, bv.value);
        }
        else
        {
            Debug.LogError("Cannot do Apply on variable, as object type: " + value.GetType().Name + " is incompatible with " + typeof(T).Name);
        }
    }

    public virtual void Apply(SetOperator setOperator, T value)
    {
        switch (setOperator)
        {
            case SetOperator.Assign:
                Value = value;
                break;
            default:
                Debug.LogError("Cannot do Apply on variable, as operator: " + setOperator.ToString() + " is not supported");
                break;
        }
    }

    // Evaluate to get from base system.object to T
    public override bool Evaluate(ComparisonOperator comparisonOperator, object value)
    {
        if (value is T || value == null)
        {
            return Evaluate(comparisonOperator, (T)value);
        }
        else if (value is BaseVariable<T>)
        {
            var vbg = value as BaseVariable<T>;
            return Evaluate(comparisonOperator, vbg.Value);
        }
        else
        {
            Debug.LogError("Cannot do Evaluate on variable, as object type: " + value.GetType().Name + " is incompatible with " + typeof(T).Name);
        }

        return false;
    }

    public virtual bool Evaluate(ComparisonOperator comparisonOperator, T value)
    {
        bool condition = false;
        switch (comparisonOperator)
        {
            case ComparisonOperator.Equals:
                condition = Equals(Value, value);
                break;
            case ComparisonOperator.NotEquals:
                condition = !Equals(Value, value);
                break;
            default:
                Debug.LogError("Cannot do Evaluate on variable, as operator: " + comparisonOperator.ToString() + " is not supported");
                break;
        }
        return condition;
    }

    public override bool SupportsArithmetic(SetOperator setOperator)
    {
        return setOperator == SetOperator.Assign || base.SupportsArithmetic(setOperator);
    }

    public override object ReturnValue()
    {
        return Value;
    }
}