using System;
using System.Threading.Tasks;
using DG.Tweening;
using EditorAttributes;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class StateTransformBehavior : MonoBehaviour
{
    [SerializeField] private Vector2 posA;
    [SerializeField] private Vector2 posB;
    [SerializeField] private bool inSecondaryState;
    [SerializeField] private bool startAtPrimary;

    private RectTransform trans;
    protected RectTransform Trans => trans ??= GetComponent<RectTransform>();

    [Button] private void MemorizePrimary() => MemorizePosition(false);
    [Button] private void MemorizeSecondary() => MemorizePosition(true);
    
    [Button] private void JumpToPrimary() => JumpToState(false);
    [Button] private void JumpToSecondary() => JumpToState(true);

    protected void Start()
    {
        if (startAtPrimary)
        {
            JumpToPrimary();
        }
    }

    private void MemorizePosition(bool isSecondaryState)
    {
        var val = Get();
        if (isSecondaryState)
        {
            posB = val;
        }
        else
        {
            posA = val;
        }
        inSecondaryState = isSecondaryState;
        EditorUtility.SetDirty(this);
    }

    protected abstract Vector2 Get();
    protected abstract void Set(Vector2 val);

    public async Task TweenToStateAsync(bool useSecondaryState, float duration, bool snapping = false)
    {
        if (useSecondaryState == inSecondaryState)
        {
            return;
        }
        var tween = DOTween.To(Get, Set, useSecondaryState ? posB : posA, duration);
        tween.SetOptions(snapping).SetTarget(Trans);
        await tween.AsTask();
        inSecondaryState = useSecondaryState;
    }

    public void JumpToState(bool usesSecondaryState)
    {
        Set(usesSecondaryState ?  posB : posA);
        inSecondaryState = usesSecondaryState;
        if (!Application.IsPlaying(this))
        {
            EditorUtility.SetDirty(this);
        }
    }
}