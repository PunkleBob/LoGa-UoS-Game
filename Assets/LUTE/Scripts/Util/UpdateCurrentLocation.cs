using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    /// <summary>
    /// Helper class to update the current location a given lat long.
    /// </summary>
    public class UpdateCurrentLocation : MonoBehaviour
    {
        [SerializeField] private Vector2d coordinates;
        [SerializeField] private AbstractMap currentMap;

        public virtual void UpdateLocation()
        {
            if (coordinates == null)
                return;

            if (currentMap == null)
                currentMap = FindObjectOfType<AbstractMap>();
            if (currentMap == null)
                return;

            currentMap.UpdateMap(coordinates);
        }

    }
}
