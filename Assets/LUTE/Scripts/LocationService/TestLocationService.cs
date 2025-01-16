using UnityEngine;
using System.Collections;
using TMPro;

public class TestLocationService : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI horizontalAccuracyText;
    public TextMeshProUGUI timestampText;

    private void Start()
    {
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            statusText.text = "Timed out";
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            //access location data
            statusText.text = "Running GPS";
            InvokeRepeating("UpdateGPSData", 0.5f, 0.5f);
        }
    }

    private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            statusText.text = Input.location.status.ToString();
            latitudeText.text = Input.location.lastData.latitude.ToString();
            longitudeText.text = Input.location.lastData.longitude.ToString();
            altitudeText.text = Input.location.lastData.altitude.ToString();
            horizontalAccuracyText.text = Input.location.lastData.horizontalAccuracy.ToString();
            timestampText.text = Input.location.lastData.timestamp.ToString();
        }
        else
        {
            statusText.text = "Stopped";
        }
    }
}