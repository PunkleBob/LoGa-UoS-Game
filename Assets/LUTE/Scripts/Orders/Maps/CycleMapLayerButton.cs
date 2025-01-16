using Mapbox.Unity.Map;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Map",
             "Cycle Layers",
             "Creates a button which will cycle and loop through the map layers defined.")]
    [AddComponentMenu("")]
    public class CycleMapLayerButton : GenericButton
    {
        [Tooltip("The layers to cycle through")]
        [SerializeField] protected ImageryLayer[] layers;
        [Tooltip("Custom icon to display for this menu")]

        public override void OnEnter()
        {
            if (layers == null || layers.Length == 0)
            {
                return;
            }

            var popupIcon = SetupButton();

            UnityEngine.Events.UnityAction buttonAction = () =>
            {
                LogaManager.Instance.MapLayerChanger.CycleLayers(layers);
            };

            SetAction(popupIcon, buttonAction.Invoke);

            Continue();
        }
    }
}
