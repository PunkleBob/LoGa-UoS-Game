namespace Mapbox.Unity.MeshGeneration.Factories
{
    using Data;
    using Mapbox.Directions;
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using Modifiers;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class DirectionsFactory : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        MeshModifier[] MeshModifiers;
        [SerializeField]
        Material _material;

        [SerializeField]
        public Transform[] _waypoints;
        private List<Vector3> _cachedWaypoints;

        [SerializeField]
        [Range(0.05f, 10)]
        private float UpdateFrequency = 2;



        private Directions _directions;
        private int _counter;

        GameObject _directionsGO;
        private bool _recalculateNext;

        protected virtual void Awake()
        {
            if (_map == null)
            {
                _map = FindObjectOfType<AbstractMap>();
            }
            _directions = MapboxAccess.Instance.Directions;
            // _map.OnInitialized += Query;
            // _map.OnUpdated += Query;
        }

        public void Start()
        {
            _cachedWaypoints = new List<Vector3>(_waypoints.Length);
            foreach (var item in _waypoints)
            {
                if (item != null)
                    _cachedWaypoints.Add(item.position);
            }
            _recalculateNext = false;

            foreach (var modifier in MeshModifiers)
            {
                modifier.Initialize();
            }

            StartCoroutine(QueryTimer());
        }

        protected virtual void OnDestroy()
        {
            _map.OnInitialized -= Query;
            _map.OnUpdated -= Query;
        }

        void Query()
        {
            var count = _waypoints.Length;
            var wp = new Vector2d[count];
            for (int i = 0; i < count; i++)
            {
                if (_waypoints[i] != null)
                {
                    wp[i] = _waypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
                }
            }
            var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
            _directionResource.Steps = true;
            _directions.Query(_directionResource, HandleDirectionsResponse);
        }

        public IEnumerator QueryTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(UpdateFrequency);
                for (int i = 0; i < _waypoints.Length; i++)
                {
                    if (_waypoints[i].position != _cachedWaypoints[i])
                    {
                        _recalculateNext = true;
                        _cachedWaypoints[i] = _waypoints[i].position;
                    }
                }

                if (_recalculateNext)
                {
                    Query();
                    _recalculateNext = false;
                }
            }
        }

        void HandleDirectionsResponse(DirectionsResponse response)
        {
            if (response == null || null == response.Routes || response.Routes.Count < 1)
            {
                return;
            }

            var meshData = new MeshData();
            var dat = new List<Vector3>();
            foreach (var point in response.Routes[0].Geometry)
            {
                dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
            }

            var feat = new VectorFeatureUnity();
            feat.Points.Add(dat);

            foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
            {
                mod.Run(feat, meshData, _map.WorldRelativeScale);
            }

            CreateGameObject(meshData);
        }

        GameObject CreateGameObject(MeshData data)
        {
            if (_directionsGO != null)
            {
                _directionsGO.Destroy();
            }
            _directionsGO = new GameObject("direction waypoint " + " entity");
            var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
            mesh.subMeshCount = data.Triangles.Count;

            mesh.SetVertices(data.Vertices);
            _counter = data.Triangles.Count;
            for (int i = 0; i < _counter; i++)
            {
                var triangle = data.Triangles[i];
                mesh.SetTriangles(triangle, i);
            }

            _counter = data.UV.Count;
            for (int i = 0; i < _counter; i++)
            {
                var uv = data.UV[i];
                mesh.SetUVs(i, uv);
            }

            mesh.RecalculateNormals();
            _directionsGO.AddComponent<MeshRenderer>().material = _material;
            _directionsGO.layer = LayerMask.NameToLayer("Map");
            return _directionsGO;
        }
        public void SetWaypoints(Transform[] waypoints, AbstractMap map)
        {
            _map = map;
            _waypoints = waypoints;
            this.enabled = true;
        }

        public void SetInitialPosition(Vector3 location)
        {
            if (_waypoints.Length < 2)
            {
                Debug.LogError("Cannot set initial waypoint as there is no predefined waypoint");
                return;
            }

            _waypoints[0].position = location;
        }

        public void SetTargetPosition(Transform location)
        {
            if (_waypoints.Length < 2)
            {
                Debug.LogError("Cannot set target waypoint as there is no predefined waypoint");
                return;
            }
            _waypoints[1] = location;
        }
    }
}
