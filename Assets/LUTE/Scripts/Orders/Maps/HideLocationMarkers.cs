using Mapbox.Examples;
using UnityEngine;

[OrderInfo("Map",
             "Hide Locations",
             "Hides a series of location markers based on the locations provided - can be revaled again using the 'Reveal Location Marker(s)' Order")]
[AddComponentMenu("")]
public class HideLocationMarkers : Order
{
    [Tooltip("The locations of the markers to hide.")]
    [SerializeField] protected LocationData[] locations;

    private SpawnOnMap map;

    public override void OnEnter()
    {
        var engine = GetEngine();

        if (engine == null)
        {
            Continue();
            return;
        }

        map = engine.GetMap();

        if (map == null)
        {
            Continue();
            return;
        }

        if (locations == null || locations.Length <= 0)
        {
            Continue();
            return;
        }

        //Wait a moment before hiding the locations to ensure the map has been loaded correctly
        Invoke("HideLocations", 0.45f);

        Continue();
    }

    private void HideLocations()
    {
        foreach (LocationData location in locations)
        {
            if (location.locationRef != null)
                map.HideLocationMarker(location.locationRef);
        }
    }

    public override string GetSummary()
    {
        if (locations != null)
            return "Hides location markers at: " + locations.Length + " locations";

        return "Error: No locations provided.";
    }

    public override bool HasReference(Variable variable)
    {
        bool hasReference = false;

        foreach (LocationData location in locations)
        {
            hasReference = location.locationRef == variable || hasReference;
        }
        return hasReference;
    }

#if UNITY_EDITOR
    protected override void RefreshVariableCache()
    {
        base.RefreshVariableCache();

        if (locations != null)
        {
            foreach (LocationData location in locations)
            {
                if (location.locationRef != null)
                    GetEngine().DetermineSubstituteVariables(location.locationRef.Key, referencedVariables);
            }
        }
    }
#endif
}
