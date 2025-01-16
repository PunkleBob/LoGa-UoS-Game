using MoreMountains.Feedbacks;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LoGaCulture.LUTE
{
    public class LocationMarker : MonoBehaviour, IPointerClickHandler
    {
        [Tooltip("The camera that will be used to render the marker canvas.")]
        private Camera markerCamera;
        [Tooltip("The canvas that will be used to render the marker.")]
        private Canvas markerCanvas;
        private bool showName = true;
        private BasicFlowEngine engine;
        public LUTELocationInfo locationInfo;

        [Tooltip("The image that will be used to render the marker.")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [Tooltip("The image that will be used to render the marker radius.")]
        [SerializeField] protected SpriteRenderer radiusSpriteRenderer;
        [Tooltip("The text mesh that will be used to render the marker label")]
        [SerializeField] protected TextMesh textMesh;
        [Tooltip("The feedback to play when the location is completed.")]
        [SerializeField] protected MMFeedbacks completeFeedback;

        public TextMesh TextMesh { get => textMesh; set => textMesh = value; }
        public SpriteRenderer RadiusRenderer { get => radiusSpriteRenderer; set => radiusSpriteRenderer = value; }
        public GameObject RadiusObject { get; set; }

        private SpriteRenderer markerRadius;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!locationInfo.Interactable)
                return;

            var locVar = engine.GetComponents<LocationVariable>().FirstOrDefault(x => x.Value.infoID == locationInfo.infoID);

            if (engine != null && !string.IsNullOrEmpty(locationInfo.ExecuteNode))
            {
                // First we must ensure that the player is a location
                if (locVar.Evaluate(ComparisonOperator.Equals, null))
                {
                    // We are at the right location so we can execute the node
                    if (locationInfo._LocationStatus != LUTELocationInfo.LocationStatus.Completed)
                    {
                        engine.ExecuteNode(locationInfo.ExecuteNode);
                    }
                }
            }

            if (locationInfo.AllowClickWithoutLocation && !string.IsNullOrEmpty(locationInfo.ExecuteNode))
            {
                if (locVar == null)
                    return;

                // We can click on the location without requiring the player to be at the location
                // This will trigger the execute node if it is set
                engine.ExecuteNode(locationInfo.ExecuteNode);


                /// Old code used for alberto demo - must ensure we reinstate at some point ///
                // If the location is completed or we click without being at the location
                //if (locationInfo._LocationStatus == LUTELocationInfo.LocationStatus.Completed || !locVar.Evaluate(ComparisonOperator.Equals, null))
                //{
                //    LocationInfoPanel newPanel = LocationInfoPanel.GetLocationInfoPanel();
                //    if (newPanel != null)
                //    {
                //        newPanel.SetLocationInfo(locationInfo);
                //        newPanel.ToggleMenu();
                //    }
                //}
            }

            LocationServiceSignals.DoLocationClicked(locVar);
        }

        protected void OnEnable()
        {
            LocationServiceSignals.OnLocationComplete += OnLocationComplete;
            NodeSignals.OnNodeEnd += OnNodeEnd;
        }

        protected void OnDestroy()
        {
            LocationServiceSignals.OnLocationComplete -= OnLocationComplete;
            NodeSignals.OnNodeEnd -= OnNodeEnd;
        }

        protected void Start()
        {
            markerCanvas = GetComponentInChildren<Canvas>();

            markerRadius = RadiusObject.GetComponent<SpriteRenderer>();
            markerRadius.color = locationInfo.defaultRadiusColour;
        }

        protected void Update()
        {
            if (markerCamera != null)
            {
                transform.LookAt(transform.position + markerCamera.transform.rotation * Vector3.forward, markerCamera.transform.rotation * Vector3.up);
            }

            if (engine != null)
            {
                var node = engine.FindNode(locationInfo.NodeComplete);
                if (node != null)
                {
                    if (node.NodeComplete)
                    {
                        locationInfo._LocationStatus = LUTELocationInfo.LocationStatus.Completed;
                    }
                }
                if (!string.IsNullOrEmpty(locationInfo.ExecuteNode) || locationInfo.IndependentMarkerUpdating)
                {
                    var locVar = engine.GetComponents<LocationVariable>().FirstOrDefault(x => x.Value.infoID == locationInfo.infoID);
                    if (locVar.Evaluate(ComparisonOperator.Equals, null))
                    {
                        OnLocationComplete(locVar);
                    }
                }
            }

            switch (locationInfo._LocationStatus)
            {
                case LUTELocationInfo.LocationStatus.Unvisited:
                    SetMarkerSprite(locationInfo.Sprite);
                    SetRadiusColour(locationInfo.defaultRadiusColour);
                    break;
                case LUTELocationInfo.LocationStatus.Visited:
                    SetMarkerSprite(locationInfo.InProgressSprite);
                    SetRadiusColour(locationInfo.visitedRadiusColour);
                    break;
                case LUTELocationInfo.LocationStatus.Completed:
                    SetMarkerSprite(locationInfo.CompletedSprite);
                    SetRadiusColour(locationInfo.completedRadiusColour);
                    break;
            }
        }

        private void OnLocationComplete(LocationVariable location)
        {
            if (location == null || locationInfo == null)
                return;

            if (location.Value.infoID == locationInfo.infoID)
            {
                if (locationInfo._LocationStatus != LUTELocationInfo.LocationStatus.Completed)
                {
                    locationInfo._LocationStatus = LUTELocationInfo.LocationStatus.Visited;
                }
                // Everytime we visit a location we should save the info
                if (locationInfo.SaveInfo)
                {
                    var locVar = engine.GetComponents<LocationVariable>().FirstOrDefault(x => x.Value.infoID == locationInfo.infoID);
                    if (locVar != null)
                    {
                        if (locVar.Value._LocationStatus != locationInfo._LocationStatus)
                        {
                            locVar.Value._LocationStatus = locationInfo._LocationStatus;

                            var saveManager = LogaManager.Instance.SaveManager;
                            saveManager.AddSavePoint("ObjectInfo" + locationInfo.Name, "A list of location info to be stored " + System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy"), false);
                        }
                    }
                }
            }
        }

        private void OnNodeEnd(Node node)
        {
            if (locationInfo == null || string.IsNullOrEmpty(locationInfo.NodeComplete))
                return;

            if (locationInfo.NodeComplete == node._NodeName)
            {
                completeFeedback?.PlayFeedbacks();
                locationInfo._LocationStatus = LUTELocationInfo.LocationStatus.Completed;
            }
        }

        // Sets up the canvas to use the marker camera to render it
        public void SetCanvasCam(Camera cam)
        {
            markerCamera = cam;
            if (markerCanvas == null)
                markerCanvas = GetComponentInChildren<Canvas>();
            if (markerCanvas != null)
                markerCanvas.worldCamera = cam;
        }

        public void SetInfo(LUTELocationInfo info)
        {
            if (info == null)
                return;
            locationInfo = info;
        }

        public void SetEngine(BasicFlowEngine _engine)
        {
            if (_engine == null)
                return;
            engine = _engine;
        }

        public void SetMarkerText(string text)
        {
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            textMesh.text = text;
        }

        public void SetMarkerName(bool _showName)
        {
            showName = _showName;
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            textMesh.text = showName ? textMesh.text : "";
        }

        public void SetMarkerColor(Color color)
        {
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            textMesh.color = color;
        }

        public void SetMarkerSprite(Sprite sprite)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            spriteRenderer.sprite = sprite;
        }

        public void SetRadiusColour(Color color)
        {
            if (markerRadius == null)
                markerRadius = RadiusObject.GetComponent<SpriteRenderer>();
            markerRadius.color = color;
        }
    }
}