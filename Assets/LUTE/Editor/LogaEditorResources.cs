using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_5_0 || UNITY_5_1
using System.Reflection;
#endif

[CustomEditor(typeof(LogaEditorResources))]
public class LogaEditorResourcesInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (serializedObject.FindProperty("updateOnReloadScripts").boolValue)
        {
            GUILayout.Label("Updating Assets...");
        }
        else
        {
            if (GUILayout.Button("Sync with EditorResources folder"))
            {
                LogaEditorResources.GenerateResourcesScript();
            }

            DrawDefaultInspector();
        }
    }
}

////Reimport all assets

//public class LogaEditorResourcesPostprocessor : AssetPostprocessor
//{
//    private static void OnPostprocessAllAssets(string[] importedAssets, string[] _, string[] __, string[] ___)
//    {
//        foreach (var path in importedAssets)
//        {
//            if (path.EndsWith("LogaEditorResources.asset"))
//            {
//                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(LogaEditorResources)) as LogaEditorResources;
//                if (asset != null)
//                {
//                    LogaEditorResources.UpdateTextureReferences(asset);
//                    AssetDatabase.SaveAssets();
//                    return;
//                }
//            }
//        }
//    }
//}

public partial class LogaEditorResources : ScriptableObject
{
    [Serializable]
    public class EditorTexture
    {
        [SerializeField] private Texture2D normal;

        public Texture2D Texture2D
        {
            get { return normal; }
        }

        public EditorTexture(Texture2D normal)
        {
            this.normal = normal;
        }
    }

    private static LogaEditorResources instance;
    private static readonly string editorResourcesFolderName = "\"EditorResources\"";
    private static readonly string editorResourcesPath = System.IO.Path.Combine("LUTE", "EditorResources");
    [SerializeField][HideInInspector] private bool updateOnReloadScripts = false;

    public static LogaEditorResources Instance
    {
        get
        {
            if (instance == null)
            {
                var guids = AssetDatabase.FindAssets("LogaEditorResources t:LogaEditorResources");

                if (guids.Length == 0)
                {
                    instance = ScriptableObject.CreateInstance(typeof(LogaEditorResources)) as LogaEditorResources;
                    AssetDatabase.CreateAsset(instance, GetRootFolder() + "/LogaEditorResources.asset");
                }
                else
                {
                    if (guids.Length > 1)
                    {
                        Debug.LogError("Multiple Resource assets found!");
                    }

                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = AssetDatabase.LoadAssetAtPath(path, typeof(LogaEditorResources)) as LogaEditorResources;
                }
            }
            return instance;
        }
    }

    private static string GetRootFolder()
    {
        var res = AssetDatabase.FindAssets(editorResourcesFolderName);

        foreach (var item in res)
        {
            var path = AssetDatabase.GUIDToAssetPath(item);
            var safePath = System.IO.Path.GetFullPath(path);
            if (safePath.IndexOf(editorResourcesPath) != -1)
                return path;
        }

        Debug.LogError("EditorResources folder not found!");
        return string.Empty;
    }

    public static void GenerateResourcesScript()
    {
        //get all unique filenames
        var textureNames = new HashSet<string>();
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { GetRootFolder() });
        var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid));

        foreach (var path in paths)
        {
            textureNames.Add(Path.GetFileNameWithoutExtension(path));
        }

        //generate script
        var scriptGuid = AssetDatabase.FindAssets("LogaEditorResources t:MonoScript")[0];
        var relativePath = AssetDatabase.GUIDToAssetPath(scriptGuid).Replace("LogaEditorResources.cs", "LogaEditorResourcesGenerated.cs");
        var absolutePath = Application.dataPath + relativePath.Substring("Assets".Length);

        using (var writer = new StreamWriter(absolutePath))
        {
            writer.WriteLine("#pragma warning disable 0649");
            writer.WriteLine("");
            writer.WriteLine("using UnityEngine;");

            writer.WriteLine("");
            writer.WriteLine("    public partial class LogaEditorResources : ScriptableObject");
            writer.WriteLine("    {");

            foreach (var name in textureNames)
            {
                writer.WriteLine("        [SerializeField] private EditorTexture " + name + ";");
            }

            writer.WriteLine("");

            foreach (var name in textureNames)
            {
                var pascalCase = string.Join("", name.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(
                    s => s.Substring(0, 1).ToUpper() + s.Substring(1)).ToArray()
                );
                writer.WriteLine("        public static Texture2D " + pascalCase + " { get { return Instance." + name + ".Texture2D; } }");
            }

            writer.WriteLine("    }");
        }

        Instance.updateOnReloadScripts = true;
        AssetDatabase.ImportAsset(relativePath);
    }

    //[DidReloadScripts]
    //private static void OnDidReloadScripts()
    //{
    //    if (Instance.updateOnReloadScripts)
    //    {
    //        UpdateTextureReferences(Instance);
    //    }
    //}

    public static void UpdateTextureReferences(LogaEditorResources instance)
    {
        var serializedObject = new SerializedObject(instance);
        var prop = serializedObject.GetIterator();
        var rootFolder = new[] { GetRootFolder() };

        prop.NextVisible(true);
        while (prop.NextVisible(false))
        {
            if (prop.propertyType == SerializedPropertyType.Generic)
            {
                var guids = AssetDatabase.FindAssets(prop.name + "t:Texture2D", rootFolder);
                var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).Where(
                       path => path.Contains(prop.name + ".")
                   );

                foreach (var path in paths)
                {
                    var texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                    prop.FindPropertyRelative("normal").objectReferenceValue = texture;

                }
            }
        }

        serializedObject.FindProperty("updateOnReloadScripts").boolValue = false;

        // The ApplyModifiedPropertiesWithoutUndo() function wasn't documented until Unity 5.2
#if UNITY_5_0 || UNITY_5_1
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var applyMethod = typeof(SerializedObject).GetMethod("ApplyModifiedPropertiesWithoutUndo", flags);
            applyMethod.Invoke(serializedObject, null);
#else
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
#endif
    }
}
