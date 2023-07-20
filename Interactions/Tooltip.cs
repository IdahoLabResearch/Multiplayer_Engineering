using UnityEngine;

public class Tooltip : MonoBehaviour
{
    /// README
    /// <summary>The Tooltip script renders tooltips on top of gameobjects</summary>
    /// <param name="message">Enter the message you want the tooltip to say for this gameobject</param>
    /// <remarks>You must attach this script, and write a message, for each gameobject that you want to have a tooltip.</remarks>
    /// <remarks>To instantiate a Tooltip, you must also follow the instructions and create the gameobject hierarchy for a TooltipManager.</remarks>

    public string message;

    void OnMouseEnter()
    {
        TooltipManager._instance.SetAndShowTooltip(message);
    }

    void OnMouseDown()
    {
        TooltipManager._instance.HideTooltip();
    }

    void OnMouseExit()
    {
        TooltipManager._instance.HideTooltip();
    }
}