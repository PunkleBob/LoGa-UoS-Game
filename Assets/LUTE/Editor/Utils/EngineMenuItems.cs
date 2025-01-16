using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//Class to handle menu items for creating new engines, variables etc.
public class EngineMenuItems
{
    [MenuItem("LUTE/Create/Engine", false, 0)]
    static void CreateEngine()
    {
        GameObject go = SpawnPrefab("BasicFlowEngine");
        go.transform.position = Vector3.zero;

        //Update latest version of our new engine
        var engine = go.GetComponent<BasicFlowEngine>();
        if(engine != null)
        {
            engine.Version = LogaConstants.CurrentVersion;
        }

        //The most recently created engine should start with a default game started node
        if(GameObject.FindObjectsOfType<BasicFlowEngine>().Length > 1 )
        {
            var node = go.GetComponent<Node>();
            GameObject.DestroyImmediate(node._EventHandler);
            node._EventHandler = null;
        }
        GraphWindow.ShowWindow();
    }

    [MenuItem("LUTE/Create/Blueprints/DemoExample", false, 100)]
    static void AddBlueprint_DemoExample()
    {
        AddBlueprint("DemoExample");
    }

    public static GameObject SpawnPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
        if (prefab == null)
        {
            Debug.LogError("No prefab to create with name of: " + prefabName + "...Please ensure the prefab is in the prefab folder!");
            return null;
        }

        GameObject go = GameObject.Instantiate(prefab) as GameObject;
        go.name = prefab.name;

        SceneView view = SceneView.lastActiveSceneView;
        if (view != null)
        {
            Camera sceneCam = view.camera;
            Vector3 pos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            pos.z = 0f;
            go.transform.position = pos;
        }

        Selection.activeGameObject = go;

        Undo.RegisterCreatedObjectUndo(go, "Create Object");

        return go;
    }

    public static void AddBlueprint(string blueprintName)
    {
        var blueprintEngine = Resources.Load<GameObject>("Prefabs/" + blueprintName).GetComponent<BasicFlowEngine>();
        //Adds a blueprint to the first engine found in the scene
        var engineInstance = GameObject.FindObjectOfType<BasicFlowEngine>();
        var engine = GraphWindow.GetInstance();

        //as long as we find an engine to copy to and the window is open
        if (engine != null && engineInstance != null)
        {
            //Get all nodes on the blueprint engine
            var nodes = blueprintEngine.GetComponents<Node>();
            var groups = blueprintEngine.GetComponents<Group>();
            if (nodes.Length > 0)
                engine.AddBlueprint(nodes.ToList(), groups.ToList(), engineInstance);
        }
        else
        {
            //use the name of actual blueprint above
            GameObject go = SpawnPrefab(blueprintName + " (Engine)");
            go.transform.position = Vector3.zero;
            var newEngine = go.GetComponent<BasicFlowEngine>();
            if (newEngine != null)
            {
                newEngine.Version = LogaConstants.CurrentVersion;
            }
        }

        GraphWindow.ShowWindow();
    }
}
