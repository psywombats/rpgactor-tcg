using System.Collections;
using System.Threading.Tasks;
using Effekseer;
using UnityEngine;

public class BattleAnimPlayer : MonoBehaviour
{
    [SerializeField] private EffekseerEmitter emitter;
    [SerializeField] private Camera sceneCam;
    [Space]
    [SerializeField] private float zRange = 10f;

    public void PlayEffect(RectTransform canvasSpaceAnchor, EffekseerEffectAsset effect)
    {
        var point = new Vector3(canvasSpaceAnchor.position.x, canvasSpaceAnchor.position.y, zRange);
        var worldPoint = sceneCam.ScreenToWorldPoint(point);
        transform.position = worldPoint;
        emitter.Play(effect);
    }
    
    public async Task PlayEffectAsync(RectTransform canvasSpaceAnchor, EffekseerEffectAsset effect)
    {
        PlayEffect(canvasSpaceAnchor, effect);
        await AwaitAnimEndRoutine();
    }

    private IEnumerator AwaitAnimEndRoutine()
    {
        while (emitter.handles.Count > 0)
        {
            yield return null;
        }
    }
}