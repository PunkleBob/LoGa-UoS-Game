using UnityEngine;

[OrderInfo("Utility",
             "Comment",
             "Write Comments here to inform others of what you are up to.")]
[AddComponentMenu("")]

public class Comment : Order
{
    [Tooltip("Name of Commenter")]
    [SerializeField] protected string commenterName = "";

    [Tooltip("Text to display for this comment")]
    [TextArea(2, 4)]
    [SerializeField] protected string commentText = "";

    public override string GetSummary()
    {
        if (commenterName != "")
        {
            return commenterName + ": " + commentText;
        }
        return commentText;
    }
}
