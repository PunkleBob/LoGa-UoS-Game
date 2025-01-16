
using UnityEngine;
[OrderInfo("Application",
             "Quit/Exit",
             "Quit the game. Does not work in the editor or webplayer - be careful with mobile builds")]
[AddComponentMenu("")]
public class Quit : Order
{
    public override void OnEnter()
    {
        Application.Quit();

        // On platforms that don't support Quit we just continue onto the next command
        Continue();
    }

    // public override Color GetButtonColor()
    // {
    //     return new Color32(235, 191, 217, 255);
    // }
}
