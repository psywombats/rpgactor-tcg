using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextTooltipComponent : MonoBehaviour
{
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private RectTransform myTrans;
    [Space] 
    [SerializeField] private float inMult = 2.5f;
    [SerializeField] private float outMult = 5f;
    [Space]
    [SerializeField] private LayoutElement layout;
    [SerializeField] private int cutoff = 50;

    private bool mouseIn;
    private float state = -1f;

    public static TextTooltipComponent FindTooltip()
    {
        return FindFirstObjectByType<TextTooltipComponent>();
    }
    
    public void Populate(string message)
    {
        bodyText.text = message;
    }

    public void SignalMouseEnter(string message)
    {
        Populate(message);
        layout.enabled = message.Length > cutoff;
        mouseIn = true;
    }

    public void SignalMouseExit()
    {
        mouseIn = false;
    }
    
    protected void Update()
    {
        if (mouseIn) state += Time.deltaTime * inMult;
        else state -= Time.deltaTime * outMult;

        if (state > 1f) state = 1f;
        if (state < -1f) state = -1f;

        fader.alpha = Mathf.Clamp(state, 0f, 1f);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, 
            InputManager.Instance.GetMouse(), 
            canvas.worldCamera, 
            out var targetPos);
        myTrans.anchoredPosition = targetPos;
    }
}