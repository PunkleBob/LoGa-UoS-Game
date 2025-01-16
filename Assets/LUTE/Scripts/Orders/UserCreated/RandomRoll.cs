using Mapbox.Map;
using UnityEngine;

[OrderInfo("Ludic",
              "RandomRoll",
              "If there is a random dice in the scene then it will set its value to between 1 or max sides allowed")]
[AddComponentMenu("")]
public class RandomRoll : Order
{
    [Tooltip("If the dice roll should just be visual or if it should be set to the value of the roll")]
    [SerializeField] protected bool setRollValue = true;
    [Tooltip("If the card will close on finishing roll")]
    [SerializeField] protected bool closeOnFinish = true;
    [Tooltip("Should the card continue to the next order upon finishing a roll or should we allow infinite rolls?")]
    [SerializeField] protected bool continueOnFinish = true;

    public override void OnEnter()
    {
        UnityEngine.Events.UnityAction action = () =>
        {
            Continue();
        };
        var diceVar = GetEngine().GetRandomDice();
        DiceRollerCard.GetContainerCard(closeOnFinish, continueOnFinish, setRollValue, diceVar, GetEngine().SidesOfDie, action);
    }

    public override string GetSummary()
  {
 //you can use this to return a summary of the order which is displayed in the inspector of the order
      return "";
  }
}