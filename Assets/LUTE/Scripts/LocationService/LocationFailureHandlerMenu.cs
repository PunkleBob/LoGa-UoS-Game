using Mapbox.Utils;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoGaCulture.LUTE
{
    public class LocationFailureHandlerMenu : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup buttonMenuGroup;
        [Header("Custom Location Menu")]
        [SerializeField] protected CanvasGroup customLocationMenuGroup;
        [SerializeField] protected TMP_Dropdown locationChoiceDropdown;

        protected static bool locationFailureMenuActive = false;

        protected List<FailedLocatioNode> failedLocationNodes = new List<FailedLocatioNode>();
        protected LTDescr fadeTween; //Used for fading menu
        protected LocationFailureHandlerMenu instance;
        protected List<Button> nodeButtons = new List<Button>();
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            if (transform.parent == null)
            {
                GameObject.DontDestroyOnLoad(this);
            }
            else
            {
                Debug.LogError("LocationFailureHandlerMenu should be a root object in the scene hierarchy otherwise it cannot be preserved across scenes.");
            }
        }

        protected virtual void Start()
        {
            if (!locationFailureMenuActive)
            {
                buttonMenuGroup.alpha = 0.0f;
                buttonMenuGroup.interactable = false;
                buttonMenuGroup.blocksRaycasts = false;
            }
        }

        protected void OnEnable()
        {
            LocationServiceSignals.OnLocationFailed += OnLocationFailed;
        }

        protected void OnDisable()
        {
            LocationServiceSignals.OnLocationFailed -= OnLocationFailed;
        }

        protected virtual void OnLocationFailed(FailureMethod failureMethod, Node relatedNode)
        {
            if (failureMethod == null || relatedNode == null)
            {
                return;
            }

            var newFailedLocationNode = new FailedLocatioNode(failureMethod.QueriedLocation, relatedNode, "Location Not Found...");
            failedLocationNodes.Add(newFailedLocationNode);
            if (locationFailureMenuActive)
                CreateNodeButton(newFailedLocationNode);
        }

        public void ToggleMenu()
        {
            if (fadeTween != null)
            {
                LeanTween.cancel(fadeTween.id, true);
                fadeTween = null;
            }

            foreach (var failedLocationNode in failedLocationNodes)
            {
                var button = nodeButtons.Find(x => x.GetComponentInChildren<TextMeshProUGUI>().text.Contains(failedLocationNode.relatedNode._NodeName));
                if (button == null)
                {
                    CreateNodeButton(failedLocationNode);
                }
            }

            if (locationFailureMenuActive)
            {
                //Fade menu out
                LeanTween.value(buttonMenuGroup.gameObject, buttonMenuGroup.alpha, 0f, 0.4f)
        .setEase(LeanTweenType.easeOutQuint)
        .setOnUpdate((t) =>
        {
            buttonMenuGroup.alpha = t;
        }).setOnComplete(() =>
        {
            buttonMenuGroup.alpha = 0f;
            buttonMenuGroup.interactable = false;
            buttonMenuGroup.blocksRaycasts = false;
        });
            }
            else
            {
                //Fade menu in
                LeanTween.value(buttonMenuGroup.gameObject, buttonMenuGroup.alpha, 1f, 0.4f)
        .setEase(LeanTweenType.easeOutQuint)
        .setOnUpdate((t) =>
        {
            buttonMenuGroup.alpha = t;
        }).setOnComplete(() =>
        {
            buttonMenuGroup.alpha = 1f;
            buttonMenuGroup.interactable = true;
            buttonMenuGroup.blocksRaycasts = true;
        });
            }
            locationFailureMenuActive = !locationFailureMenuActive;
        }

        public void PlayNode(Node node)
        {
            if (node != null)
            {
                node.StartExecution();
                failedLocationNodes.RemoveAll(x => x.relatedNode == node);
                var nodeButton = nodeButtons.Find(x => x.GetComponentInChildren<TextMeshProUGUI>().text.Contains(node._NodeName));
                if (nodeButton != null)
                {
                    nodeButtons.Remove(nodeButton);
                    Destroy(nodeButton.gameObject);
                }
                ToggleMenu();
            }
        }

        protected void CreateNodeButton(FailedLocatioNode failedLocationNode)
        {
            var newButton = Resources.Load<GameObject>("Prefabs/FailedNodeButton");
            if (newButton != null)
            {
                var button = Instantiate(newButton, buttonMenuGroup.transform);
                button.GetComponentInChildren<TextMeshProUGUI>().text = failedLocationNode.relatedNode._NodeName + "@" + failedLocationNode.failedLocation.Key;
                button.GetComponent<Button>().onClick.AddListener(() => PlayNode(failedLocationNode.relatedNode));
                nodeButtons.Add(button.GetComponent<Button>());
            }
        }

        public void ToggleCustomLocationMenu()
        {
            if (customLocationMenuGroup)
            {
                customLocationMenuGroup.alpha = customLocationMenuGroup.alpha > 0 ? 0 : 1;

                if (locationChoiceDropdown == null)
                {
                    locationChoiceDropdown = customLocationMenuGroup.GetComponentInChildren<TMP_Dropdown>();
                    if (locationChoiceDropdown == null)
                    {
                        Debug.LogError("LocationChoiceDropdown is not found in the custom location menu");
                        return;
                    }
                }

                // If we can see the location selector then we fill the dropdown with the available locations
                if (customLocationMenuGroup.alpha > 0)
                {
                    //Fill the dropdown with the available locations
                    locationChoiceDropdown.ClearOptions();
                    //locationChoiceDropdown.placeholder.GetComponent<TextMeshProUGUI>().text = "Select a location";
                    var engine = BasicFlowEngine.CachedEngines[0];
                    if (engine == null)
                    {
                        engine = FindObjectOfType<BasicFlowEngine>();
                    }
                    if (engine == null)
                    {
                        Debug.LogError("BasicFlowEngine is not found in the scene");
                        return;
                    }
                    var handler = engine.GetComponent<LocationFailureHandler>();
                    if (handler == null)
                    {
                        Debug.LogError("LocationFailureHandler is not found in the scene");
                        return;
                    }
                    var locations = engine.GetComponents<LocationVariable>();
                    foreach (var location in locations)
                    {
                        // Find the location failure handler in handler
                        var failureLocation = handler.FailureMethods.Find(x => x.QueriedLocation.Value == location.Value);
                        // If the location is not handled then we handle it
                        if (failureLocation != null && !failureLocation.IsHandled)
                        {
                            locationChoiceDropdown.options.Add(new TMP_Dropdown.OptionData(location.Key));
                        }
                    }
                }
            }

        }

        public void RunLocationFailure()
        {
            // Get the input value from the dropdown and test to location failure handler based on this value
            var engine = BasicFlowEngine.CachedEngines[0];
            if (engine == null)
            {
                engine = FindObjectOfType<BasicFlowEngine>();
            }
            if (engine == null)
            {
                Debug.LogError("BasicFlowEngine is not found in the scene");
                return;
            }

            var locationKey = locationChoiceDropdown.options[locationChoiceDropdown.value].text;
            var location = engine.GetComponents<LocationVariable>().ToList().Find(x => x.Key == locationKey);
            if (location == null)
            {
                Debug.LogError("Location with key " + locationKey + " is not found in the scene");
                return;
            }
            LocationFailureHandler handler = engine.GetComponent<LocationFailureHandler>();
            if (handler == null)
            {
                Debug.LogError("LocationFailureHandler is not found in the scene");
                return;
            }
            Vector2d location2D = location.Value.LatLongString();
            handler.HandleFailure(location2D);
            customLocationMenuGroup.alpha = 0;
            locationChoiceDropdown.ClearOptions();
        }
    }

    [System.Serializable]
    public class FailedLocatioNode
    {
        public LocationVariable failedLocation;
        public Node relatedNode;
        public string failureMessage;

        public FailedLocatioNode(LocationVariable failedLocation, Node relatedNode, string failureMessage)
        {
            this.failedLocation = failedLocation;
            this.relatedNode = relatedNode;
            this.failureMessage = failureMessage;
        }
    }
}
