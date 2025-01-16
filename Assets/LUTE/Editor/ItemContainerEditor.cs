using UnityEditor;

[CustomEditor(typeof(LocationPickups))]
public class ItemContainerEditor : OrderEditor
{
    protected SerializedProperty feedbackProp;
    protected SerializedProperty showPromptProp;
    protected SerializedProperty showCardProp;
    protected SerializedProperty itemLocProp;
    protected SerializedProperty itemProp;
    protected SerializedProperty itemQuantProp;

    protected int locationVarIndex = 0;
    protected int itemIndex = 0;
    public override void OnEnable()
    {
        base.OnEnable();
        feedbackProp = serializedObject.FindProperty("pickupFeedback");
        showPromptProp = serializedObject.FindProperty("showPrompt");
        showCardProp = serializedObject.FindProperty("showPickupCard");
        itemLocProp = serializedObject.FindProperty("itemLocation");
        itemProp = serializedObject.FindProperty("item");
        itemQuantProp = serializedObject.FindProperty("itemsQuantitiy");
    }

    public override void OnInspectorGUI()
    {
        DrawOrderGUI();
    }

    public override void DrawOrderGUI()
    {
        LocationPickups t = target as LocationPickups;
        var engine = (BasicFlowEngine)t.GetEngine();

        EditorGUILayout.PropertyField(feedbackProp);
        EditorGUILayout.PropertyField(showPromptProp);
        EditorGUILayout.PropertyField(showCardProp);
        EditorGUILayout.PropertyField(itemLocProp);

        //var locationVars = engine.GetComponents<LocationVariable>();
        //for (int i = 0; i < locationVars.Length; i++)
        //{
        //    if (locationVars[i] == itemLocProp.objectReferenceValue as LocationVariable)
        //    {
        //        locationVarIndex = i;
        //    }
        //}

        //locationVarIndex = EditorGUILayout.Popup("Location", locationVarIndex, locationVars.Select(x => x.Key).ToArray());
        //if (locationVars.Length > 0)
        //    itemLocProp.objectReferenceValue = locationVars[locationVarIndex];

        //var items = ContainerCardEditor.GetAllInstances<InventoryItem>();
        //for (int j = 0; j < items.Length; j++)
        //{
        //    if (items[j] == itemProp.objectReferenceValue as InventoryItem)
        //    {
        //        itemIndex = j;
        //    }
        //}

        //Create a drop down list based on the items in the project
        //itemIndex = EditorGUILayout.Popup("Item to Add", itemIndex, items.Select(x => x.name).ToArray());
        //itemProp.objectReferenceValue = items[itemIndex];

        EditorGUILayout.PropertyField(itemQuantProp);

        serializedObject.ApplyModifiedProperties();
    }
}