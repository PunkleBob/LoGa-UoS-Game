using UnityEngine;

[OrderInfo("Generic", "Create Prefab", "Spawns a given prefab gameobject at a specific location with a given size and rotation")]
[AddComponentMenu("")]
public class GenericPrefab : Order
{
    [Tooltip("The object to create")]
    [SerializeField] protected GameObject prefabToCreate;
    [Tooltip("The object name")]
    [SerializeField] protected string prefabName;
    [Tooltip("What scale to create the object at (default is 1)")]
    [SerializeField] protected Vector3 spawnScale = Vector3.one;
    [Tooltip("Where to spawn the object, default is at centre")]
    [SerializeField] protected Vector3 spawnPosition = Vector3.zero;
    [Tooltip("What rotation to spawn the object at")]
    [SerializeField] protected Quaternion spawnRotation = Quaternion.identity;

    public override void OnEnter()
    {
        if (prefabToCreate == null)
            return;
        var newObj = Instantiate(prefabToCreate);
        if (!string.IsNullOrEmpty(prefabName))
        {
            newObj.name = prefabName;
        }
        newObj.transform.position = spawnPosition;
        newObj.transform.localScale = spawnScale;
        newObj.transform.rotation = spawnRotation;

        Continue();
    }

    public override string GetSummary()
    {
        if (prefabToCreate == null)
            return "Error: no prefab to spwan!";

        return "Spawning: " + prefabToCreate.name + " at " + spawnPosition;
    }
}
