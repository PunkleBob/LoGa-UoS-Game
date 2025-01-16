using Mapbox.Unity.Map;
using UnityEngine;


namespace LoGaCulture.LUTE
{
    [OrderInfo("Map",
             "Update Map Image Layer",
             "Updates the map layer to one specified")]
    [AddComponentMenu("")]
    public class UpdateMapLayerImage : Order
    {
        [Tooltip("The map layer to update to")]
        [SerializeField] protected ImageryLayer _layer;

        public override void OnEnter()
        {
            if (_layer == null)
            {
                Continue();
            }

            LogaManager.Instance.MapLayerChanger.ChangeLayer(_layer);

            Continue();
        }

        public override string GetSummary()
        {
            if (_layer != null)
                return "Updates map layer to: " + _layer.LayerSource;

            return "Error: No layer specified";
        }

        public override Color GetButtonColour()
        {
            return new Color32(216, 228, 170, 255);
        }
    }
}
