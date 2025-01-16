using Mapbox.Examples;
using UnityEngine;


namespace LoGaCulture.LUTE
{

    [OrderInfo("Map",
                 "Update Location Info",
                 "Updates the status of a given location.")]
    [AddComponentMenu("")]
    public class UpdateLocationInfo : Order
    {
        [Tooltip("The location to update.")]
        [SerializeField] protected LocationData location;
        [Tooltip("The status to update the location to.")]
        [SerializeField] protected LUTELocationInfo.LocationStatus status;

        public override void OnEnter()
        {
            if (location.locationRef == null || location.Value == null)
            {
                Continue();
                return;
            }
            location.Value._LocationStatus = status;

            SpawnOnMap spawnOnMap = GetEngine().GetComponentInChildren<SpawnOnMap>();
            if (spawnOnMap != null)
            {
                spawnOnMap.ProcessLocationInfo();
                spawnOnMap.CreateMarkers();
            }

            Continue();
        }

        public override string GetSummary()
        {
            return location.locationRef == null ? "Error: No location selected" : "Updates " + location.Value.name + " to " + status;
        }
    }
}
