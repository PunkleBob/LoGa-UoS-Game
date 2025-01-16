//#if UNITY_EDITOR
//#endif

//using UnityEditor;
//using UnityEngine;

//namespace LoGaCulture.LUTE
//{
//    /// <summary>
//    /// Utility class for creating default implementations of LUTE UI controls.
//    /// </summary>
//    public static class LUTEMenuControls
//    {
//        public static GameObject CreateText(Resources resources)
//        {
//            GameObject go = null;

//#if UNITY_EDITOR
//            go = ObjectFactory.CreateGameObject("LUTEText (TMP)");
//            ObjectFactory.AddComponent<VariableTMProText>(go);
//#else
//                go = CreateUIElementRoot("LUTEText (TMP)", s_TextElementSize);
//                go.AddComponent<VariableTMProText>();
//#endif

//            return go;
//        }
//    }
//}
