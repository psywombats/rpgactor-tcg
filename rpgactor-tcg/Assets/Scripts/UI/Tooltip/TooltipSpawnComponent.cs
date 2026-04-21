using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipSpawnComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string tooltipMessage;
    [SerializeField] private TextTooltipComponent textTooltip;

    private string message;
    public string Message
    {
        get => message ?? tooltipMessage;
        set => message = value;
    }
    
    public TextTooltipComponent Tooltip
    {
        get
        {
            if (textTooltip == null)
            {
                textTooltip = TextTooltipComponent.FindTooltip();
            }
            return textTooltip;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(Message))
        {
            Tooltip.SignalMouseEnter(Message);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.SignalMouseExit();
    }
}
