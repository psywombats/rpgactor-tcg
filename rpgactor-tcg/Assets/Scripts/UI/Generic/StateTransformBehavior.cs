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
        EditorUtility.SetDirty(this);
    }

    protected abstract Vector2 Get();
    protected abstract void Set(Vector2 val);

    public Task TweenToStateAsync(bool useSecondaryState, float duration, bool snapping = false)
        => TweenToLerpAsync(duration, useSecondaryState ? 1f : 0f, snapping);

    public async Task TweenToLerpAsync(float duration, float t, bool snapping = false)
    {
        var target = t * posB + (1f - t) * posA;
        if ((target - Get()).sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }
        var tween = DOTween.To(Get, Set, target, duration);
        tween.SetOptions(snapping).SetTarget(Trans);
        await tween.AsTask();
    }

    public void JumpToState(bool usesSecondaryState) => JumpToLerp(usesSecondaryState ? 1f : 0f);

    public void JumpToLerp(float t)
    {
        Set(t * posB + (1f - t) * posA);
        if (!Application.IsPlaying(this))
        {
            EditorUtility.SetDirty(this);
        }
    }
}