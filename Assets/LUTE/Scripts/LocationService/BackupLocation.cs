using UnityEngine;

/// <summary>
/// A location class that takes in a specific location and a list of backup locations
/// </summary>
public class BackupLocation : MonoBehaviour
{
    [SerializeField] protected LocationVariable location;
    [SerializeField] protected LocationVariable[] backupLocations;

    public LocationVariable Location => location;
    public LocationVariable[] BackupLocations => backupLocations;
}
