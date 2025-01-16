using MoreMountains.InventoryEngine;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StickerItem", menuName = "LUTE/Inventory/StickerItem", order = 0)]
[System.Serializable]
public class StickerItem : InventoryItem
{
    [Tooltip("The category that this sticker belongs to")]
    [SerializeField] protected StickerManager.StickerType stickerType;

    public StickerManager.StickerType StickerType { get { return stickerType; } }

    public override bool Use(string playerID)
    {
        var newPostcard = Postcard.GetPostcard(LogaConstants.DefaultPostcardName, LogaConstants.DefaultPostcardDesc, this, LogaConstants.DefaultPostcardAuthor);

        newPostcard.AddSticker(this);
        newPostcard.SetActive(true);

        return true;
    }
}