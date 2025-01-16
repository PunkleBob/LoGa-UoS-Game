using UnityEngine;
using UnityEngine.Serialization;

/// Fades a sprite to a target color over a period of time.
[OrderInfo("Sprite",
             "Fade Sprite Custom",
             "Fades a sprite to a target color over a period of time")]
[AddComponentMenu("")]
[ExecuteInEditMode]
public class CustomFadeSprite : FadeSprite
{
    public override void OnEnter()
    {
        base.OnEnter();
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

    protected override void OnEnable()
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

