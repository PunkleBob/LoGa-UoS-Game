using UnityEngine;
using UnityEditor;
using MoreMountains.InventoryEngine;
using System.Linq;

[CustomEditor(typeof(AddItem))]
public class AddItemEditor : OrderEditor
{
    protected SerializedProperty itemProp;
    protected SerializedProperty amountProp;
    protected SerializedProperty feedbackProp;
    protected SerializedProperty persistentProp;
    protected SerializedProperty itemAlreadyInventoryProp;
    protected int itemIndex = 0;

    public override void OnEnable()
    {
        base.OnEnable();
        itemProp = serializedObject.FindProperty("item");
        amountProp = serializedObject.FindProperty("amount");
        feedbackProp = serializedObject.FindProperty("feedback");
        persistentProp = serializedObject.FindProperty("persistentItem");
        itemAlreadyInventoryProp = serializedObject.FindProperty("addIfAlreadyInInventory");
    }

    public override void OnInspectorGUI()
    {
        DrawOrderGUI();
    }

    public override void DrawOrderGUI()
    {
        serializedObject.Update();
        var items = ContainerCardEditor.GetAllInstances<InventoryItem>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemProp.objectReferenceValue as InventoryItem)
            {
                itemIndex = i;
            }
        }
        itemIndex = EditorGUILayout.Popup("Item to Add", itemIndex, items.Select(x => x.name).ToArray());
        itemProp.objectReferenceValue = items[itemIndex];

        EditorGUILayout.PropertyField(amountProp);
        EditorGUILayout.PropertyField(feedbackProp);
        EditorGUILayout.PropertyField(persistentProp);
        EditorGUILayout.PropertyField(itemAlreadyInventoryProp);
        serializedObject.ApplyModifiedProperties();
    }
}
