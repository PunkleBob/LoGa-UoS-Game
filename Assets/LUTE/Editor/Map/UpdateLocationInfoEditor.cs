using UnityEditor;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(UpdateLocationInfo))]

    public class UpdateLocationInfoEditor : OrderEditor
    {
        protected SerializedProperty locationProp;
        protected SerializedProperty statusProp;

        private int locationVarIndex = 0;

        public override void OnEnable()
        {
            base.OnEnable();
            locationProp = serializedObject.FindProperty("location");
            statusProp = serializedObject.FindProperty("status");
        }

        public override void OnInspectorGUI()
        {
            DrawOrderGUI();
        }

        public override void DrawOrderGUI()
        {
            serializedObject.Update();

            //UpdateLocationInfo t = target as UpdateLocationInfo;
            //var engine = (BasicFlowEngine)t.GetEngine();

            //var locationVars = engine.GetComponents<LocationVariable>();
            //for (int i = 0; i < locationVars.Length; i++)
            //{
            //    if (locationVars[i] == locationProp.objectReferenceValue as LocationVariable)
            //    {
            //        locationVarIndex = i;
            //    }
            //}

            //locationVarIndex = EditorGUILayout.Popup("Location", locationVarIndex, locationVars.Select(x => x.Key).ToArray());
            //if (locationVars.Length > 0)
            //    locationProp.objectReferenceValue = locationVars[locationVarIndex];

            EditorGUILayout.PropertyField(locationProp);
            EditorGUILayout.PropertyField(statusProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}