namespace Mapbox.Examples
{
    using Mapbox.Unity.Map;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class QuadTreeCameraMovement : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 20)]
        public float _panSpeed = 1.0f;

        [SerializeField]
        float _zoomSpeed = 0.25f;

        [SerializeField]
        public Camera _referenceCamera;
        [SerializeField]
        public Camera _referenceCameraGame;

        [SerializeField]
        public AbstractMap _mapManager;

        [SerializeField]
        bool _useDegreeMethod;

        [SerializeField] protected bool allowPanning;
        [SerializeField] protected bool allowZooming;
        [SerializeField] protected bool allowTilting;

        [Range(0, 21)]
        [SerializeField] protected float minZoomLevel = 0.0f;
        [Range(0, 21)]
        [SerializeField] protected float maxZoomLevel = 21.0f;

        [SerializeField] float sensitivityZ = 2f;   // Horizontal sensitivity

        [HideInInspector]
        public bool _dragStartedOnUI = false;

        private Vector3 _origin;
        private Vector3 _mousePosition;
        private Vector3 _mousePositionPrevious;
        private bool _shouldDrag;
        private bool _isInitialized = false;
        private Plane _groundPlane = new Plane(Vector3.up, 0);
        // Store the current rotation of the camera
        private float rotationZ = 0f;  // Vertical rotation
        private Vector3 defaultRotation;

        public static QuadTreeCameraMovement _instance;

        void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            if (null == _referenceCamera)
            {
                _referenceCamera = GetComponent<Camera>();
            }
            _mapManager.OnInitialized += () =>
            {
                _isInitialized = true;
            };

        }

        void Start()
        {
            if (_referenceCameraGame != null)
                _referenceCamera = _referenceCameraGame;

            defaultRotation = _referenceCameraGame.transform.localEulerAngles;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
            {
                _dragStartedOnUI = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _dragStartedOnUI = false;
            }
        }

        private void LateUpdate()
        {
            if (!_isInitialized) { return; }

            if (!_dragStartedOnUI)
            {

                if (Input.touchSupported && Input.touchCount > 0)
                {
                    HandleTouch();
                }
                else
                {
                    HandleMouseAndKeyBoard();
                }
            }
        }

        void HandleMouseAndKeyBoard()
        {
            // zoom
            float scrollDelta = 0.0f;
            scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            ZoomMapUsingTouchOrMouse(scrollDelta);


            //pan keyboard
            float xMove = Input.GetAxis("Horizontal");
            //Returns true if any key was pressed.
            float zMove = Input.GetAxis("Vertical");

            if (allowPanning)
                PanMapUsingKeyBoard(xMove, zMove);

            // If right mouse button is held, rotate/pan the camera
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y"); // useful if you want up/down panning

                // Use the unified method to handle camera rotation and zoom
                if (allowTilting)
                    PanOrRotateCamera(mouseX);
            }


            //pan mouse
            if (allowPanning)
                PanMapUsingTouchOrMouse();
        }

        void HandleTouch()
        {
            float zoomFactor = 0.0f;
            //pinch to zoom.
            switch (Input.touchCount)
            {
                case 1:
                    {
                        if (allowPanning)
                            PanMapUsingTouchOrMouse();

                        Touch touch = Input.GetTouch(0);
                        float touchX = touch.deltaPosition.x;
                        float touchY = touch.deltaPosition.y;

                        // Use the unified method to handle camera rotation with touch input
                        if (allowTilting)
                            PanOrRotateCamera(touchX);  // No zoom for single touch
                    }
                    break;
                case 2:
                    {
                        // Store both touches.
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        // Find the position in the previous frame of each touch.
                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        // Find the magnitude of the vector (the distance) between the touches in each frame.
                        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                        // Find the difference in the distances between each frame.
                        zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
                    }
                    if (allowZooming)
                        ZoomMapUsingTouchOrMouse(zoomFactor);
                    break;
                default:
                    break;
            }
        }

        void PanOrRotateCamera(float zInput)
        {
            // Adjust the camera rotation based on the input
            rotationZ += zInput * sensitivityZ;  // Horizontal movement (pan or rotate)

            // Apply the new rotation to the camera
            _referenceCameraGame.transform.localEulerAngles = new Vector3(defaultRotation.x, defaultRotation.y, rotationZ);
        }

        public void ZoomMapUsingTouchOrMouse(float zoomFactor)
        {
            var zoom = Mathf.Max(minZoomLevel, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, maxZoomLevel));
            if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
            {
                _mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
            }
        }
        public void PanMapUsingKeyBoard(float xMove, float zMove)
        {
            if (Math.Abs(xMove) > 0.0f || Math.Abs(zMove) > 0.0f)
            {
                // Get the number of degrees in a tile at the current zoom level.
                // Divide it by the tile width in pixels ( 256 in our case)
                // to get degrees represented by each pixel.
                // Keyboard offset is in pixels, therefore multiply the factor with the offset to move the center.
                float factor = _panSpeed * (Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom));

                var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + zMove * factor * 2.0f, _mapManager.CenterLatitudeLongitude.y + xMove * factor * 4.0f);

                _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
            }
        }

        public void PanMapUsingTouchOrMouse()
        {
            if (_useDegreeMethod)
            {
                UseDegreeConversion();
            }
            else
            {
                UseMeterConversion();
            }
        }

        public void PanMapUsingTouchOrMouseEditor(Event e)
        {
            UseMeterConversionEditor(e);
        }

        void UseMeterConversion()
        {
            if (Input.GetMouseButtonUp(1))
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                var pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                var latlongDelta = _mapManager.WorldToGeoPosition(pos);
                // Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y);
            }

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
                            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        void UseMeterConversionEditor(Event e)
        {
            if (_dragStartedOnUI)
            {
                var mousePosScreen = e.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                var newLocVec = new Vector3(mousePosScreen.x, mousePosScreen.y, _referenceCamera.transform.localPosition.y);
                _mousePosition = _referenceCamera.ScreenToWorldPoint(newLocVec);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(newLocVec);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
                            var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, -offset.z * factor));
                            var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

                            _mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        void UseDegreeConversion()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var mousePosScreen = Input.mousePosition;
                //assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
                //http://answers.unity3d.com/answers/599100/view.html
                mousePosScreen.z = _referenceCamera.transform.localPosition.y;
                _mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

                if (_shouldDrag == false)
                {
                    _shouldDrag = true;
                    _origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
                }
            }
            else
            {
                _shouldDrag = false;
            }

            if (_shouldDrag == true)
            {
                var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
                if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
                {
                    _mousePositionPrevious = _mousePosition;
                    var offset = _origin - _mousePosition;

                    if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
                    {
                        if (null != _mapManager)
                        {
                            // Get the number of degrees in a tile at the current zoom level.
                            // Divide it by the tile width in pixels ( 256 in our case)
                            // to get degrees represented by each pixel.
                            // Mouse offset is in pixels, therefore multiply the factor with the offset to move the center.
                            float factor = _panSpeed * Conversions.GetTileScaleInDegrees((float)_mapManager.CenterLatitudeLongitude.x, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;

                            var latitudeLongitude = new Vector2d(_mapManager.CenterLatitudeLongitude.x + offset.z * factor, _mapManager.CenterLatitudeLongitude.y + offset.x * factor);
                            _mapManager.UpdateMap(latitudeLongitude, _mapManager.Zoom);
                        }
                    }
                    _origin = _mousePosition;
                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    _mousePositionPrevious = _mousePosition;
                    _origin = _mousePosition;
                }
            }
        }

        private Vector3 getGroundPlaneHitPoint(Ray ray)
        {
            float distance;
            if (!_groundPlane.Raycast(ray, out distance)) { return Vector3.zero; }
            return ray.GetPoint(distance);
        }
    }
}