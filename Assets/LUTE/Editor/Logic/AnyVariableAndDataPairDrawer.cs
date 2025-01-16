using UnityEngine;
using UnityEditor;
using System.Linq;

// Custom drawer for the AnyVaraibleAndDataPair, shows only the matching data for the targeted variable scripts 
[CustomPropertyDrawer(typeof(AnyVariableAndDataPair))]
public class AnyVariableAndDataPairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;

        var varProp = property.FindPropertyRelative("variable");

        EditorGUI.PropertyField(position, varProp, label);

        position.y += EditorGUIUtility.singleLineHeight;

        if (varProp.objectReferenceValue != null)
        {
            var varPropType = varProp.objectReferenceValue.GetType();

            var typeActionsRes = AnyVariableAndDataPair.typeActionLookup[varPropType];

            if (typeActionsRes != null)
            {
                var targetName = "data." + typeActionsRes.DataPropName;
                var dataProp = property.FindPropertyRelative(targetName);
                //if we are using a node collection variable we want to compare the total count of the collection rather than another list
                if (varPropType == typeof(NodeCollectionVariable))
                {
                    var value = "data." + typeActionsRes.DataPropName + ".total";
                    dataProp = property.FindPropertyRelative(value);
                }

                if (dataProp != null)
                {
                    EditorGUI.PropertyField(position, dataProp, new GUIContent("Compare With", "Data to use in pair with the above variable."));
                }
                else
                {
                    EditorGUI.LabelField(position, "Cound not find property in AnyVariableData of name " + targetName);
                }
            }
            else
            {
                //no matching data type, oops
                EditorGUI.LabelField(position, "Cound not find property in AnyVariableData of type " + varPropType.Name);
            }
        }
        else
        {
            //no var selected
            EditorGUI.LabelField(position, "Must select a variable before setting data.");
        }

        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //changes in new Unity circa UIElements mean that some data that used to be single line
        //  are now multiple lines, so we have to ask the props individually how high they are
        var dataProp = GetDataProp(property);

        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("variable")) +
            (dataProp != null ?
                EditorGUI.GetPropertyHeight(dataProp) :
                EditorGUIUtility.singleLineHeight);
    }

    protected SerializedProperty GetDataProp(SerializedProperty property)
    {
        var varProp = property.FindPropertyRelative("variable");
        if (varProp.objectReferenceValue != null)
        {
            var varPropType = varProp.objectReferenceValue.GetType();

            var typeActionsRes = AnyVariableAndDataPair.typeActionLookup[varPropType];

            if (typeActionsRes != null)
            {
                var targetName = "data." + typeActionsRes.DataPropName;
                return property.FindPropertyRelative(targetName);
            }
        }
        return null;
    }
}