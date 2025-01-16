using Mapbox.Map;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceRollerCard : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected TMP_InputField modifierField;
    [SerializeField] protected MMFeedbacks rollFeedback;

    protected bool closeOnUse = true;
    protected bool continueOnUse = false;
    protected bool setRollValue = false;
    //The dice variable that will be set if the roll value is set
    protected DiceVariable diceVariable;
    //Sides of Di
    protected int sides = 6;
    protected UnityEngine.Events.UnityAction action;

    protected static List<DiceRollerCard> activeCards = new List<DiceRollerCard>();
    public static DiceRollerCard ActiveRollerCard { get; set; }

    protected virtual void Awake()
    {
        if (!activeCards.Contains(this))
        {
            activeCards.Add(this);
        }

        if (modifierField != null)
        {
            modifierField.onValueChanged.AddListener(SetDiceVarModifier);
        }
    }

    protected virtual void OnDestroy()
    {
        activeCards.Remove(this);
        modifierField.onValueChanged.RemoveListener(SetDiceVarModifier);
    }

    private void SetDiceVarModifier(string val)
    {
        if (diceVariable != null)
        {
            diceVariable.SetModifier(int.Parse(val));
        }
    }

    public static DiceRollerCard GetContainerCard(bool _closeOnUse, bool _continueOnUse, bool _setRollValue, DiceVariable _diceVar, int _sides, UnityEngine.Events.UnityAction postAnimEvent = null)
    {
        if (ActiveRollerCard == null)
        {
            DiceRollerCard rollerCard = null;
            if (activeCards.Count > 0)
            {
                rollerCard = activeCards[0];
            }
            if (rollerCard != null)
            {
                ActiveRollerCard = rollerCard;
            }
            if (ActiveRollerCard == null)
            {
                GameObject containerObj = Resources.Load<GameObject>("Prefabs/DiceRollerCard");
                if (containerObj != null)
                {
                    GameObject go = Instantiate(containerObj) as GameObject;
                    go.name = "DiceRollerCard";
                    ActiveRollerCard = go.GetComponent<DiceRollerCard>();
                }
            }
        }

        ActiveRollerCard.closeOnUse = _closeOnUse;
        ActiveRollerCard.continueOnUse = _continueOnUse;
        ActiveRollerCard.setRollValue = _setRollValue;
        ActiveRollerCard.diceVariable = _diceVar;
        ActiveRollerCard.sides = _sides;
        //if event is not null then ensure that when animation is finished (using script get) then we will call the event
        ActiveRollerCard.action = postAnimEvent;

        return ActiveRollerCard;
    }

    public void RollDice()
    {
        var continueOnAnim = GetComponent<ContinueOnAnimEnd>();
        if(continueOnAnim != null)
        {
            continueOnAnim.SetEvent(action);
        }
        if (continueOnUse)
        {
            //Ensure that animator will call continue event when the animation is finished
            animator.SetBool("Continue", true);
        }
        if (closeOnUse)
        {
            //Ensure that the animator is listening for the close event and then we will call the close event
            animator.SetBool("FadeDice", true);
        }
        int roll = Random.Range(1, sides);
        if (setRollValue && diceVariable != null)
        {
            roll = diceVariable.RollDice();
        }
        rollFeedback?.PlayFeedbacks();
        animator.SetTrigger("Roll");
        animator.SetInteger("RollValue", roll);
    }

    public void CloseCard()
    {
        if(closeOnUse)
        { }
    }
}
