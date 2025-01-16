using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Linq;
using UnityEngine;


namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Changes the current abstract map layer based on what has been provided.
    /// </summary>
    public class MapLayerChanger : MonoBehaviour
    {
        [SerializeField] protected ImageryLayer defaultImageryLayer;

        public virtual void ChangeLayer(ImageryLayer imageryLayer)
        {
            if (imageryLayer != null)
            {
                defaultImageryLayer = imageryLayer;
            }
            if (defaultImageryLayer == null)
            {
                return;
            }

            var map = LocationProviderFactory.Instance.mapManager;

            map.ImageLayer = defaultImageryLayer;
            var loc2D = Conversions.StringToLatLon(map.Options.locationOptions.latitudeLongitude);
            map.Initialize(loc2D, map.AbsoluteZoom);
        }

        public virtual void CycleLayers(ImageryLayer[] layers)
        {
            if (layers == null || layers.Length == 0)
            {
                return;
            }

            var currentIndex = layers.ToList().IndexOf(defaultImageryLayer);
            if (currentIndex == -1)
            {
                currentIndex = 0;
            }

            currentIndex = (currentIndex + 1) % layers.Length;
            ChangeLayer(layers[currentIndex]);
        }
    }
}