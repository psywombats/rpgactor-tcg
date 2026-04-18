using UnityEngine;
using System.Collections;
using TMPro;

public class TextAutotyper : MonoBehaviour, IInputListener 
{
    [SerializeField] protected TMP_Text textbox;
    [SerializeField] private float charsPerSecond = 120f;
    [SerializeField] private float speedupMultiplier = 4f;
    [SerializeField] private GameObject advanceArrow;
    [SerializeField] private bool speedUpWhenHurried;

    private bool hurried;
    private bool confirmed;
    
    protected int TypingStartIndex { get; set; }

    protected virtual void Start()
    {
        textbox.text = "";
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) 
    {
        switch (eventType) 
        {
            case InputManager.Event.Hold:
                if (command == InputManager.Command.Primary) 
                {
                    hurried = true;
                }
                break;
            case InputManager.Event.Down:
                if (command == InputManager.Command.Primary) 
                {
                    confirmed = true;
                }
                break;
        }
        return true;
    }

    public IEnumerator TypeRoutine(string text, bool waitForConfirm = true) 
    {
        hurried = false;
        confirmed = false;
        var elapsed = 0f;
        var total = (text.Length - TypingStartIndex) / charsPerSecond;
        while (elapsed <= total && textbox != null) 
        {
            elapsed += Time.deltaTime;
            var charsToShow = Mathf.FloorToInt(elapsed * charsPerSecond) + TypingStartIndex;
            var cutoff = charsToShow > text.Length ? text.Length : charsToShow;
            textbox.text = text[..cutoff];
            textbox.text += "<color=#aa000000>";
            textbox.text += text[cutoff..];
            textbox.text += "</color>";
            yield return null;

            elapsed += Time.deltaTime;
            if (hurried) 
            {
                hurried = false;
                if (speedUpWhenHurried) 
                {
                    elapsed += Time.deltaTime * speedupMultiplier;
                }
            }
            if (confirmed) 
            {
                confirmed = false;
                if (!speedUpWhenHurried) 
                {
                    break;
                }
            }
        }
        textbox.text = text;

        if (waitForConfirm) 
        {
            confirmed = false;
            if (advanceArrow != null) advanceArrow.SetActive(true);
            while (!confirmed) 
            {
                yield return null;
            }
            if (advanceArrow != null) advanceArrow.SetActive(false);
        }
    }
}
