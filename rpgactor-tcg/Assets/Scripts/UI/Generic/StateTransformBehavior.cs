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
    [SerializeField] private bool startAtPrimary;

    public event Action<float> OnTransition;
    
    private RectTransform trans;
    protected RectTransform Trans
    {
        get
        {
            if (trans == null)
            {
                trans = GetComponent<RectTransform>();
            }
            return trans;
        }
    }

    [Button] private void MemorizePrimary() => MemorizePosition(false);
    [Button] private void MemorizeSecondary() => MemorizePosition(true);
    
    [Button] private void JumpToPrimary() => JumpToState(false);
    [Button] private void JumpToSecondary() => JumpToState(true);

    private bool started;
    private bool startChecked;

    protected void Start()
    {
        CheckStart();
    }

    private void CheckStart()
    {
        if (startAtPrimary && !started)
        {
            started = true;
            JumpToPrimary();
        }
        startChecked = true;
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
#if UNITY_EDITOR
        if (!Application.IsPlaying(this))
        {
            EditorUtility.SetDirty(this);
        }
#endif          
    }

    protected abstract Vector2 Get();
    protected abstract void Set(Vector2 val);

    public Task TweenToStateAsync(bool useSecondaryState, float duration, bool snapping = false)
        => TweenToLerpAsync(duration, useSecondaryState ? 1f : 0f, snapping);

    public async Task TweenToLerpAsync(float duration, float t, bool snapping = false)
    {
        CheckStart();
        var target = t * posB + (1f - t) * posA;
        if ((target - Get()).sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }
        var tween = DOTween.To(Get, Set, target, duration);
        tween.SetOptions(snapping).SetTarget(Trans);
        await tween.AsTask();
        OnTransition?.Invoke(t);
    }

    public void JumpToState(bool usesSecondaryState) => JumpToLerp(usesSecondaryState ? 1f : 0f);

    public void JumpToLerp(float t)
    {
        CheckStart();
        Set(t * posB + (1f - t) * posA);
        if (startChecked)
        {
            OnTransition?.Invoke(t);
        }
        
#if UNITY_EDITOR        
        if (!Application.IsPlaying(this))
        {
            EditorUtility.SetDirty(this);
        }
#endif        
    }
}