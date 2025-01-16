using UnityEngine;
using System;
using MoreMountains.InventoryEngine;

[CreateAssetMenu(fileName = "BasicLocationItem", menuName = "LUTE/Inventory/BasicLocationItem", order = 0)]
[Serializable]
public class BasicLocationItem : InventoryItem
{
    public override bool Use(string playerID)
    {
        Debug.Log("Basic Location Item Used");
        return false;
    }
}