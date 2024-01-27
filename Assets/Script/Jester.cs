using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Jester : MonoBehaviour
{
    public Joke m_Joke;
    public JesterRace m_JokerRace;
    public static float JESTERSPEED = 2;
    private static float KINGSIDE_SCALE = 0.5f;
    private static float HALLWAY_SCALE = 1.0f;
    public Transform spritePuppet;

    private Tweener currentTween;
    private int newAnimState;
    public int m_AnimationState;
    float idleTimer = 0;
    //0 - Idle
    //1 - Moving
    //2 - Talking
    //3 - Move to King

    

    void Update()
    {
        PlayAnimation();
    }

    public Tween GoToPosition(Vector3 _targetPos)
    {
        float distance = Vector3.Distance(_targetPos, transform.position);
        var tween = transform.DOMove(_targetPos, distance / JESTERSPEED);
        ChangeAnimation(1);
        return tween;
    }

    public Tween GoToKing(Vector3[] _path, float duration)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOMove(_path[0], duration))
            .Append(transform.DOMove(_path[1], duration))
            .Join(spritePuppet.DOScale(KINGSIDE_SCALE, duration));
        return sequence;
    }

    public Tween ResetSprite(float duration)
    {
        var seq = DOTween.Sequence();
        seq.Append(spritePuppet.DORotate(Vector3.zero, duration))
            .Join(spritePuppet.DOScale(HALLWAY_SCALE, duration))
            .Join(spritePuppet.DOLocalMove(Vector3.zero, duration));
        return seq;
    }

    void PlayAnimation()
    {
        switch (m_AnimationState)
        {
            case 0:
                IdleAnimation();
                break;
            case 1:
                MoveAnimation();
                break;
            default:
                break;
        }
    }

    public void StopMoveAnimation()
    {
        if (m_AnimationState == 1)
            ChangeAnimation(0);
    }

    public void ChangeAnimation(int _newState)
    {
        if (_newState == m_AnimationState)
            return;

        //Reset sprite transform
        newAnimState = _newState;
        if (currentTween.IsActive())
            currentTween.Kill();

        ResetSprite(0.1f)
            .OnComplete(SetAnimationState);
    }

    //Play new animation after sprite reset
    public void SetAnimationState()
    {
        m_AnimationState = newAnimState;
    }

    void IdleAnimation()
    {
        if ((currentTween != null && !currentTween.IsActive()) || currentTween == null)
        {
            if (idleTimer > 0)
                idleTimer -= Time.deltaTime;
            else
            {
                idleTimer = UnityEngine.Random.Range(0, 0.2f);
                Vector3 rotate = new Vector3(0, 0, UnityEngine.Random.Range(-5, 5));
                currentTween = spritePuppet.DOPunchRotation(rotate, 0.5f, 1, 1).OnKill(() => currentTween = null);
            }
        }
    }

    void MoveAnimation()
    {

        if (currentTween != null && !currentTween.IsActive() || currentTween == null)
        {
            Vector3 jumpLoc = new Vector3(0, 0.1f, 0);
            currentTween = spritePuppet.DOPunchPosition(jumpLoc, 0.5f, 1, 1);
        }
    }

    
}
