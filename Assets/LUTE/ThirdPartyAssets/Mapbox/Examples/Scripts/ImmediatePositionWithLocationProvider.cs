namespace Mapbox.Examples
{
    using Mapbox.Unity.Location;
    using UnityEngine;

    public class ImmediatePositionWithLocationProvider : MonoBehaviour
    {
        [SerializeField] protected bool moveWithMap;
        bool _isInitialized;

        ILocationProvider _locationProvider;
        ILocationProvider LocationProvider
        {
            get
            {
                if (_locationProvider == null)
                {
                    _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
                }

                return _locationProvider;
            }
        }

        Vector3 _targetPosition;

        void Start()
        {
            LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
        }

        void LateUpdate()
        {
            if (_isInitialized)
            {
                var map = LocationProviderFactory.Instance.mapManager;
                transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
                if (moveWithMap)
                    map.UpdateMap(LocationProvider.CurrentLocation.LatitudeLongitude);

            }
        }

        //upates the map preview to the player location
        public void UpdateMapToPlayer()
        {
            if (_isInitialized)
            {
                var map = LocationProviderFactory.Instance.mapManager;
                map.UpdateMap(LocationProvider.CurrentLocation.LatitudeLongitude);
            }
        }
    }
}