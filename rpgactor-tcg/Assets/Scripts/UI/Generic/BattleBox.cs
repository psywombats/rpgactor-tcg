using UnityEngine;
using System.Collections;
using System.Text;
using EditorAttributes;

public class BattleBox : TextAutotyper 
{
    public const string Tab = "  ";

    [SerializeField] private int lineCount = 4;

    private int fullLines;
    private string[] lines;

    private RectTransform textTrans;
    private RectTransform TextTrans => textTrans ??= textbox.GetComponent<RectTransform>();
    
    private float LineWidth => TextTrans.rect.width;

    protected override void Start() 
    {
        base.Start();
        Clear();
    }

    public void Clear() 
    {
        if (lines == null) 
        {
            lines = new string[lineCount];
        } 
        else 
        {
            fullLines = 0;
            for (var i = 0; i < lineCount; i += 1) 
            {
                lines[i] = "";
            }

        }
        textbox.text = "";
    }

    [Button(nameof(CanTest), ConditionResult.EnableDisable)]
    public void Test() => StartCoroutine(TestRoutine());
    public bool CanTest => Application.isPlaying;
    private IEnumerator TestRoutine() 
    {
        yield return WriteLineRoutine("The combat begins!!");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
            "eiusmod tempor incididunt ut labore et dolore magna aliqua.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("BobHuman dealt 10 damages!");
        yield return WriteLineRoutine("BubHuman deflected by ESP.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("This is a run-on sentence which is composed of multiple sentences in sequence" +
            " strung together in a nonsense fashion. My hope is that this stress the multiline capabilities of the" +
            " text area, and hopefully illustrates that even when the text exceeds one box length, everything is still" +
            " handled properly.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("Now wasn't that fun?");
    }

    public IEnumerator WriteLineRoutine(string messageIn) 
    {
        InputManager.Instance.PushListener(this);

        var words = messageIn.Split(' ');
        var at = 0;
        var thisLine = new StringBuilder();
        while (at < words.Length) 
        {
            var word = words[at];
            var nextString = thisLine.ToString();
            if (at > 0) 
            {
                nextString += " ";
            }
            nextString +=  word;
            if (ExceedsLineWidth(nextString)) 
            {
                break;
            }

            if (at > 0) 
            {
                thisLine.Append(" ");
            }
            thisLine.Append(word);
            at += 1;
        }

        if (fullLines < lineCount) 
        {
            lines[fullLines] = thisLine.ToString();
            fullLines += 1;
        } 
        else 
        {
            for (var i = 0; i < lineCount - 1; i += 1) 
            {
                lines[i] = lines[i + 1];
            }
            lines[lineCount - 1] = thisLine.ToString();
        }

        TypingStartIndex = 0;
        var fullMessage = new StringBuilder();
        for (var i = 0; i < lineCount; i += 1) 
        {
            if (i < fullLines - 1) 
            {
                TypingStartIndex += lines[i].Length + 2; // +2 for \r\n
            }
            fullMessage.AppendLine(lines[i]);
        }
            
        yield return TypeRoutine(fullMessage.ToString(), false);
        InputManager.Instance.RemoveListener(this);

        if (at < words.Length) 
        {
            var rest = new StringBuilder();
            var first = true;
            for (; at < words.Length; at += 1) 
            {
                if (!first) 
                {
                    rest.Append(" ");
                } 
                else 
                {
                    first = false;
                }
                rest.Append(words[at]);
            }
            yield return WriteLineRoutine(rest.ToString());
        }
    }

    private bool ExceedsLineWidth(string line) 
    {
        var preferred = textbox.GetPreferredValues(line);
        return preferred.x > LineWidth;
    }
}
