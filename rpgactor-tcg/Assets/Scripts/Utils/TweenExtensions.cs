using System.Threading.Tasks;
using DG.Tweening;

public static class TweenExtensions
{
    public static Task AsTask(this Tween tween)
    {
        var tcs = new TaskCompletionSource<bool>();
        tween.onComplete += () => tcs.SetResult(true);
        tween.Play();
        return tcs.Task;
    }
}