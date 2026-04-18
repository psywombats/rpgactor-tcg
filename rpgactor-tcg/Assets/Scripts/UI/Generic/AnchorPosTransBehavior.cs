using UnityEngine;

public class AnchorPosTransBehavior : StateTransformBehavior
{
    protected override Vector2 Get() => Trans.anchoredPosition;
    protected override void Set(Vector2 val) => Trans.anchoredPosition = val;
}