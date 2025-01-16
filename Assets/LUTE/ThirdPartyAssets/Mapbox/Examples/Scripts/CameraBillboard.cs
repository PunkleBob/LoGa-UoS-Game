namespace Mapbox.Examples
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CameraBillboard : MonoBehaviour, IPointerClickHandler
    {
        Camera _camera;
        Canvas canvas;
        Image image;
        public TextMesh textMesh;
        MeshRenderer meshRenderer;
        public SpriteRenderer spriteRenderer;

        private bool showName = true;
        void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
            image = GetComponentInChildren<Image>();
        }

        void Update()
        {
            if (_camera != null)
            {
                transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
            }
        }

        public void SetCanvasCam(Camera cam)
        {
            _camera = cam;
            if (canvas == null)
                canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvas.worldCamera = cam;
        }

        public Camera GetCurrentCam()
        {
            if (canvas == null)
                canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
                return canvas.worldCamera;

            return null;
        }

        public RectTransform GetImageTrans()
        {
            if (image == null)
                image = GetComponentInChildren<Image>();
            return image.GetComponent<RectTransform>();
        }

        public void SetText(string text)
        {
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            if (textMesh != null)
                textMesh.text = text;
        }

        public void SetColor(Color color)
        {
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            if (textMesh != null)
                textMesh.color = color;
        }

        public void SetName(bool show)
        {
            showName = show;
            if (textMesh == null)
                textMesh = GetComponentInChildren<TextMesh>();
            if (textMesh != null)
                textMesh.text = showName ? textMesh.text : "";
        }

        public void SetIcon(Sprite icon)
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sprite = icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // This is where we must handle the click event to trigger the node
        }
    }
}