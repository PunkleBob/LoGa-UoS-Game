
using UnityEngine;

/// Draws a fullscreen texture over the scene to give a fade effect. Setting Target Alpha to 1 will obscure the screen, alpha 0 will reveal the screen.
/// If no Fade Texture is provided then a default flat color texture is used.
[OrderInfo("Camera",
             "Fade Screen",
             "Draws a fullscreen texture over the scene to give a fade effect. Setting Target Alpha to 1 will obscure the screen, alpha 0 will reveal the screen")]
[AddComponentMenu("")]
public class FadeScreen : Order
{
    [Tooltip("Time for fade effect to complete")]
    [SerializeField] protected float duration = 1f;
    [Tooltip("Current target alpha transparency value. The fade gradually adjusts the alpha to approach this target value.")]
    [SerializeField] protected float targetAlpha = 1f;
    [Tooltip("Wait until the fade has finished before executing next order")]
    [SerializeField] protected bool waitUntilFinished = true;
    [Tooltip("Colourr to render fullscreen fade texture with when screen is obscured")]
    [SerializeField] protected Color fadeColor = Color.black;
    [Tooltip("Optional texture to use when rendering the fullscreen fade effect.")]
    [SerializeField] protected Texture2D fadeTexture;
    [SerializeField] protected LeanTweenType fadeTweenType = LeanTweenType.easeInOutQuad;

    public override void OnEnter()
    {
        var cameraManager = LogaManager.Instance.CameraManager;

        if (fadeTexture)
        {
            cameraManager.ScreenFadeTexture = fadeTexture;
        }
        else
        {
            cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(fadeColor, 32, 32);
        }

        cameraManager.Fade(targetAlpha, duration, delegate
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }, fadeTweenType);

        if (!waitUntilFinished)
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        return "Fade to " + targetAlpha + " over " + duration + " seconds";
    }

    public override Color GetButtonColour()
    {
        return new Color32(216, 228, 170, 255);
    }
}
