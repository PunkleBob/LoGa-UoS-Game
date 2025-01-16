using UnityEngine;
using UnityEngine.Serialization;

/// Fades a sprite to a target color over a period of time.
[OrderInfo("Sprite",
             "Fade Sprite",
             "Fades a sprite to a target colour over a period of time")]
[AddComponentMenu("")]
[ExecuteInEditMode]
public class FadeSprite : Order
{
    [Tooltip("Sprite object to be faded")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [Tooltip("Length of time to perform the fade")]
    [SerializeField] protected float _duration = 1f;
    [Tooltip("Target color to fade to. To only fade transparency level, set the color to white and set the alpha to required transparency.")]
    [SerializeField] protected Color32 _targetColor = Color.white;
    [Tooltip("Wait until the fade has finished before executing the next order")]
    [SerializeField] protected bool waitUntilFinished = true;

    public override void OnEnter()
    {
        if (spriteRenderer == null)
        {
            Continue();
            return;
        }

        //Custom class to handle sprite fading to avoid bloating this order out and to allow for reuse (and avoid putting update in here!)
        SpriteFader.FadeSprite(spriteRenderer, _targetColor, _duration, Vector2.zero, delegate
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        });

        if (!waitUntilFinished)
        {
            Continue();
        }
    }

    public override string GetSummary()
    {
        if (spriteRenderer == null)
        {
            return "Error: No sprite renderer selected";
        }

        return spriteRenderer.name + " to " + _targetColor.ToString();
    }

    // public override Color GetButtonColor() //when custom styling is implemented
    // {
    //     return new Color32(221, 184, 169, 255);
    // }


    #region Backwards compatibility

    [HideInInspector][FormerlySerializedAs("duration")] public float durationOLD;
    [HideInInspector][FormerlySerializedAs("targetColor")] public Color targetColorOLD;

    protected virtual void OnEnable()
    {
        if (durationOLD != default(float))
        {
            _duration = durationOLD;
            durationOLD = default(float);
        }
        if (targetColorOLD != default(Color))
        {
            _targetColor = targetColorOLD;
            targetColorOLD = default(Color);
        }
    }

    #endregion
}

