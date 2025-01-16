using UnityEditor;

public class SaveMenuItems
{
    [MenuItem("LUTE/Create/Save/Save Menu", false, 1100)]
    static void CreateSaveMenu()
    {
        EngineMenuItems.SpawnPrefab("SaveMenu");
    }

    [MenuItem("LUTE/Create/Save/Save Data", false, 1101)]
    static void CreateSaveMenu(MenuCommand menuCommand)
    {
        EngineMenuItems.SpawnPrefab("SaveData");
    }
}
    