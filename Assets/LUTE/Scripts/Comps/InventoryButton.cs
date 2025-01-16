using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public static InventoryButton ActiveButton { get; set; }

    private Button button;
    private Inventory inventory;
    private Image buttonIconImage;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        buttonIconImage = GetComponentInChildren<Image>();
        if (button == null)
        {
            Debug.LogError("InventoryButton requires a Button component on a child object");
            return;
        }
        button.onClick.AddListener(OnClick);
    }

    public static InventoryButton GetInventoryButton()
    {
        if (ActiveButton == null)
        {
            var ib = GameObject.FindObjectOfType<InventoryButton>();
            if (ib != null)
            {
                ActiveButton = ib;
            }

            if (ActiveButton == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/InventoryButton");
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.SetActive(false);
                    go.name = "InventoryButton";
                    ActiveButton = go.GetComponent<InventoryButton>();
                }
            }
        }
        return ActiveButton;
    }

    public void SetIcon(Sprite icon)
    {
        if (buttonIconImage != null)
        {
            buttonIconImage.sprite = icon;
        }
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void OnClick()
    {
        if (inventory != null)
        {
            InventoryInputManager inventoryInputManager = inventory.GetComponentInChildren<InventoryInputManager>();
            if (inventoryInputManager != null)
            {
                inventoryInputManager.ToggleInventory();
            }
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
