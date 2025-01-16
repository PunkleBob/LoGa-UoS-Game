using System.Collections;
using TMPro;  // If using TextMeshPro
using UnityEngine;
using UnityEngine.Networking;

namespace LoGaCulture.LUTE.Weather
{
    public class GetApiData : MonoBehaviour
    {
        public string URL;

        // Text components for displaying the values
        public TextMeshProUGUI temperatureText; // For TextMeshPro
        public TextMeshProUGUI cloudCoverText;  // For TextMeshPro

        protected virtual void Start()
        {
            GetData();
        }

        public void GetData()
        {
            StartCoroutine(FetchData());
        }

        public IEnumerator FetchData()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(URL))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    // Parse the JSON response
                    WeatherApiResponse apiResponse = JsonUtility.FromJson<WeatherApiResponse>(request.downloadHandler.text);

                    // Display temperature and cloud cover in UI text
                    //temperatureText.text = "Temperature: " + apiResponse.current.temperature_2m + "°C";
                    //cloudCoverText.text = "Cloud Cover: " + apiResponse.current.cloud_cover + "%";

                    // Debug log to confirm values
                    Debug.Log("Temperature: " + apiResponse.current.temperature_2m + "°C");
                    Debug.Log("Cloud Cover: " + apiResponse.current.cloud_cover + "%");
                }
            }
        }

        // Class structure matching the JSON response
        [System.Serializable]
        public class CurrentWeather
        {
            public float temperature_2m;
            public int cloud_cover;
        }

        [System.Serializable]
        public class WeatherApiResponse
        {
            public CurrentWeather current;
        }
    }
}
