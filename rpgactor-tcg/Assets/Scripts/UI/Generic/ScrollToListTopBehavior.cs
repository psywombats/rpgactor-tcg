using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToListTopBehavior : MonoBehaviour
{
    [SerializeField, Required] private ListView list;
    
    private ScrollRect scrollRect;
    private ScrollRect ScrollRect => scrollRect ??= GetComponent<ScrollRect>();
    
    public void Start()
    {
        list.OnPopulate += ListOnOnPopulate;
        ScrollToTop();
    }

    private void ListOnOnPopulate(int dataSize)
    {
        ScrollToTop();
    }

    private void ScrollToTop()
    {
        ScrollRect.verticalNormalizedPosition = 0;
    }
}