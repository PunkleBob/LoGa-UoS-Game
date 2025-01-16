using UnityEngine;

namespace LoGaCulture.LUTE
{
    [EventHandlerInfo("Location",
                  "On Location",
                  "Executes when a specific location has been visited (with overload parameters).")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LocationClickEventHandler : EventHandler
    {
        [Tooltip("The location to check against.")]
        [SerializeField] protected LocationData location;
        [Tooltip("Whether the event should be triggered when relative location marker is pressed.")]
        [SerializeField] protected bool requiresPress;
        [Tooltip("Whether the event should be triggered when the player enters the location automatically.")]
        [SerializeField] protected bool autoTrigger = true;
        [Tooltip("If waiting for marker press, allow it to be pressed without the location satisfied.")]
        [SerializeField] protected bool requiresLocation;
        [Tooltip("If this node can be triggered by pressing a location marker, should we ensure that the player is outside the location or always trigger?")]
        [SerializeField] protected bool triggerWhenAtLocation;
        [Tooltip("Whether to update the location marker when the node is completed.")]
        [SerializeField] protected bool updateLocationMarkerOnComplete;

        public bool AutoTrigger
        {
            get { return autoTrigger; }
            set { autoTrigger = value; }
        }
        public bool RequiresPress
        {
            get { return requiresPress; }
            set { requiresPress = value; }
        }
        public bool RequiresLocation
        {
            get { return requiresLocation; }
            set { requiresLocation = value; }
        }

        public LocationData Location
        {
            get { return location; }
        }

        private void OnEnable()
        {
            LocationServiceSignals.OnLocationClicked += OnLocationClicked;
        }

        private void OnDisable()
        {
            LocationServiceSignals.OnLocationClicked -= OnLocationClicked;
        }

        private void Update()
        {
            autoTrigger = requiresPress ? false : true;
            requiresPress = autoTrigger ? false : requiresPress;
            requiresLocation = autoTrigger ? true : requiresLocation;

            if (autoTrigger)
            {
                if (location.Value != null)
                    parentNode.NodeLocation = location.locationRef;
            }
            else
            {
                parentNode.NodeLocation = null;
            }

            if (location.Value != null)
            {
                if (updateLocationMarkerOnComplete)
                {
                    location.Value.NodeComplete = parentNode._NodeName;
                }

                if (Application.isPlaying)
                {
                    if (autoTrigger)
                    {
                        bool locationMet = location.locationRef.Evaluate(ComparisonOperator.Equals, this.location.Value);
                        if (locationMet)
                        {
                            ExecuteNode();
                        }
                    }
                }
            }
        }

        protected void OnLocationClicked(LocationVariable location)
        {
            if (location == null || !requiresPress)
            {
                return;
            }

            bool locationMet;
            if (requiresLocation)
            {
                locationMet = location.Evaluate(ComparisonOperator.Equals, this.location.Value);
            }
            else
            {
                if (triggerWhenAtLocation)
                {
                    locationMet = true;
                }
                else
                {
                    locationMet = !location.Evaluate(ComparisonOperator.Equals, this.location.Value);
                }
            }

            if (location.Value.infoID == this.location.Value.infoID)
            {
                if (locationMet)
                {
                    ExecuteNode();
                }
            }
        }
    }
}