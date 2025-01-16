using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class NarrativeItems
{
    [MenuItem("LUTE/Create/Narrative/Character", false, 50)]
    static void CreateCharacter()
    {
        GameObject go = EngineMenuItems.SpawnPrefab("Character");
        go.transform.position = Vector3.zero;
    }
}
