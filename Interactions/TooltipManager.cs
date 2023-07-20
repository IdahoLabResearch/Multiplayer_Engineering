using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{

    /// README
    /// <summary>The Tooltip Manager is a supervisory script that controls whether a tooltip is visible</summary>
    /// <param name="TextComponent">Drag and drop the Text - TextMeshPro (TMP) here</param>
    /// <remarks>To render a tooltip, create a Canvas component, with an Image child component. The TooltipManager script is attached to the Image component. Further, the Image component should have a TextMeshPro child component. That TextMeshPro is used as the parameter for this script.</remarks>
    /// <remarks>This script manages instances of the Tooltip script. The Tooltip script must be attached to the gameobjects that should render a tooltip on hover.</remarks>

    public static TooltipManager _instance;
    public TextMeshProUGUI TextComponent;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowTooltip(string message)
    {
        gameObject.SetActive(true);
        TextComponent.text = message;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        TextComponent.text = string.Empty;
    }
}