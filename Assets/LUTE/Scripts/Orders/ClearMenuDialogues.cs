using UnityEngine;

namespace LoGaCulture.LUTE
{
    [OrderInfo("Narrative",
             "Clear Menu Dialogues",
             "Clears all menu dialogues in the scene.")]
    [AddComponentMenu("")]
    public class ClearMenuDialogues : Order
    {
        public override void OnEnter()
        {
            var menuDialogues = FindObjectsOfType<MenuDialogue>();
            foreach (var menuDialogue in menuDialogues)
            {
                menuDialogue.Clear();
                menuDialogue.SetActive(false);
            }
            Continue();
        }

        public override string GetSummary()
        {
            return "Clears all menu dialogues in the scene.";
        }
    }
}
