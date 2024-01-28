using DG.Tweening;
using UnityEngine;

public class WaitForTween : CustomYieldInstruction
{
    Tween tween;
    public override bool keepWaiting => tween.IsPlaying();

    public WaitForTween(Tween _tween)
    {
        tween = _tween;
    }


}