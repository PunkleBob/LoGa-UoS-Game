#pragma warning disable 0649

using UnityEngine;

public partial class LogaEditorResources : ScriptableObject
{
    [SerializeField] private EditorTexture add;
    [SerializeField] private EditorTexture add20;
    [SerializeField] private EditorTexture cross;
    [SerializeField] private EditorTexture customLUTEUI;
    [SerializeField] private EditorTexture customLUTEUIText;
    [SerializeField] private EditorTexture default100;
    [SerializeField] private EditorTexture diamond;
    [SerializeField] private EditorTexture duplicate;
    [SerializeField] private EditorTexture hamburgerMenu;
    [SerializeField] private EditorTexture keyIcon;
    [SerializeField] private EditorTexture lockIcon;
    [SerializeField] private EditorTexture maximise;
    [SerializeField] private EditorTexture minimise;
    [SerializeField] private EditorTexture progress100;
    [SerializeField] private EditorTexture progress100_new;
    [SerializeField] private EditorTexture remove;
    [SerializeField] private EditorTexture undiscovered100;
    [SerializeField] private EditorTexture undiscovered100_new;
    [SerializeField] private EditorTexture logaFavicon;
    [SerializeField] private EditorTexture logaLogo;
    [SerializeField] private EditorTexture logaLogoSquare;
    [SerializeField] private EditorTexture choice_node_off;
    [SerializeField] private EditorTexture choice_node_on;
    [SerializeField] private EditorTexture connection_point;
    [SerializeField] private EditorTexture cutePlayOrder;
    [SerializeField] private EditorTexture dottedBox;
    [SerializeField] private EditorTexture event_node_off;
    [SerializeField] private EditorTexture event_node_on;
    [SerializeField] private EditorTexture mapIcon;
    [SerializeField] private EditorTexture order_background;
    [SerializeField] private EditorTexture play_big;
    [SerializeField] private EditorTexture process_node_off;
    [SerializeField] private EditorTexture process_node_on;
    [SerializeField] private EditorTexture stretchedNode;

    public static Texture2D Add { get { return Instance.add.Texture2D; } }
    public static Texture2D Add20 { get { return Instance.add20.Texture2D; } }
    public static Texture2D Cross { get { return Instance.cross.Texture2D; } }
    public static Texture2D CustomLUTEUI { get { return Instance.customLUTEUI.Texture2D; } }
    public static Texture2D CustomLUTEUIText { get { return Instance.customLUTEUIText.Texture2D; } }
    public static Texture2D Default100 { get { return Instance.default100.Texture2D; } }
    public static Texture2D Diamond { get { return Instance.diamond.Texture2D; } }
    public static Texture2D Duplicate { get { return Instance.duplicate.Texture2D; } }
    public static Texture2D HamburgerMenu { get { return Instance.hamburgerMenu.Texture2D; } }
    public static Texture2D KeyIcon { get { return Instance.keyIcon.Texture2D; } }
    public static Texture2D LockIcon { get { return Instance.lockIcon.Texture2D; } }
    public static Texture2D Maximise { get { return Instance.maximise.Texture2D; } }
    public static Texture2D Minimise { get { return Instance.minimise.Texture2D; } }
    public static Texture2D Progress100 { get { return Instance.progress100.Texture2D; } }
    public static Texture2D Progress100New { get { return Instance.progress100_new.Texture2D; } }
    public static Texture2D Remove { get { return Instance.remove.Texture2D; } }
    public static Texture2D Undiscovered100 { get { return Instance.undiscovered100.Texture2D; } }
    public static Texture2D Undiscovered100New { get { return Instance.undiscovered100_new.Texture2D; } }
    public static Texture2D LogaFavicon { get { return Instance.logaFavicon.Texture2D; } }
    public static Texture2D LogaLogo { get { return Instance.logaLogo.Texture2D; } }
    public static Texture2D LogaLogoSquare { get { return Instance.logaLogoSquare.Texture2D; } }
    public static Texture2D ChoiceNodeOff { get { return Instance.choice_node_off.Texture2D; } }
    public static Texture2D ChoiceNodeOn { get { return Instance.choice_node_on.Texture2D; } }
    public static Texture2D ConnectionPoint { get { return Instance.connection_point.Texture2D; } }
    public static Texture2D CutePlayOrder { get { return Instance.cutePlayOrder.Texture2D; } }
    public static Texture2D DottedBox { get { return Instance.dottedBox.Texture2D; } }
    public static Texture2D EventNodeOff { get { return Instance.event_node_off.Texture2D; } }
    public static Texture2D EventNodeOn { get { return Instance.event_node_on.Texture2D; } }
    public static Texture2D MapIcon { get { return Instance.mapIcon.Texture2D; } }
    public static Texture2D OrderBackground { get { return Instance.order_background.Texture2D; } }
    public static Texture2D PlayBig { get { return Instance.play_big.Texture2D; } }
    public static Texture2D ProcessNodeOff { get { return Instance.process_node_off.Texture2D; } }
    public static Texture2D ProcessNodeOn { get { return Instance.process_node_on.Texture2D; } }
    public static Texture2D StretchedNode { get { return Instance.stretchedNode.Texture2D; } }
}
