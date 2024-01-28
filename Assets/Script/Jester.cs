using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI.Table;

public class Jester : MonoBehaviour
{
    public Joke m_Joke;
    public bool isAssassin { get; private set; } = false;

    public GameObject m_AssassinSkin;
    public GameObject[] m_Skins;

    public JesterRace m_JokerRace;
    public static float JESTERSPEED = 2;
    private static float KINGSIDE_SCALE = 0.5f;
    private static float HALLWAY_SCALE = 1.0f;
    public Transform spritePuppet;

    private Tween currentTween;
    float idleTimer = 0;
    private AnimationDelegate newAniDelegate;
    private AnimationDelegate animationStateDelegate;

    public delegate void AnimationDelegate();

    void Start()
    {
        if (isAssassin == false)
        {
            foreach (var skin in m_Skins)
                skin.SetActive(false);
            m_AssassinSkin?.SetActive(false);

            var useSkinId = UnityEngine.Random.Range(0, m_Skins.Length);
            m_Skins[useSkinId].SetActive(true);
        }
    }

    public void BecomeAssassin()
    {
        foreach (var skin in m_Skins)
            skin.SetActive(false);
        m_AssassinSkin.SetActive(true);
        isAssassin = true;
    }

    void Update()
    {
        if (animationStateDelegate != null)
            animationStateDelegate();
    }

    public Tween GoToPosition(Vector3 _targetPos)
    {
        float distance = Vector3.Distance(_targetPos, transform.position);
        var tween = transform.DOMove(_targetPos, distance / JESTERSPEED);
        ChangeAnimation(MoveAnimation);
        tween.onComplete = GoToIdleAnimation;
        return tween;
    }

    public Tween GoToKing(Vector3[] _path, float duration)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOMove(_path[0], duration))
            .Append(transform.DOMove(_path[1], duration))
            .Join(transform.DOScale(KINGSIDE_SCALE, duration));

        ChangeAnimation(MoveAnimation);
        sequence.onComplete = GoToIdleAnimation;
        return sequence;
    }

    public Tween ResetSprite(float duration)
    {
        var seq = DOTween.Sequence();
        seq.Append(spritePuppet.DORotate(Vector3.zero, duration))
            .Join(spritePuppet.DOLocalMove(Vector3.zero, duration));
        return seq;
    }

    public void GoToIdleAnimation()
    {
        ChangeAnimation(IdleAnimation);
    }

    public void ChangeAnimation(AnimationDelegate _newAnimState)
    {
        if (animationStateDelegate == _newAnimState)
            return;

        newAniDelegate = _newAnimState;
        if (currentTween.IsActive())
            currentTween.Kill();


        //Reset sprite transform
        currentTween = ResetSprite(0.01f);
        currentTween.OnComplete(SetAnimationState);
    }

    //Play new animation after sprite reset
    public void SetAnimationState()
    {
        animationStateDelegate = newAniDelegate;
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

    void PunchlineAnimation()
    {
        if ((currentTween != null && !currentTween.IsActive()) || currentTween == null)
        {
            Vector3 rot = new Vector3(0, -180, 0);
            var seq = DOTween.Sequence();
            currentTween = seq.Append(spritePuppet.DOLocalRotate(rot, 0.2f))
                .Append(spritePuppet.DOLocalRotate(rot, 0.5f))
                .Append(spritePuppet.DOLocalRotate(Vector3.zero, 0.2f));
             currentTween.onComplete = GoToIdleAnimation;
        }

    }

    void HopAnimation()
    {
        if ((currentTween != null && !currentTween.IsActive()) || currentTween == null)
        {
            Vector3 jumpLoc = new Vector3(0, 0.4f, 0);
            currentTween = spritePuppet.DOPunchPosition(jumpLoc, 0.5f, 1);
            currentTween.onComplete = GoToIdleAnimation;
        }
    }

    void DoNothingAnimation()
    {

    }

    void DoNothingForAnimation()
    {
        ChangeAnimation(DoNothingAnimation);
    }
    
    public void PlayTalkAnimation()
    {
        ChangeAnimation(HopAnimation);
    }

    public void PlayPunchlineAnimation()
    {
        ChangeAnimation(PunchlineAnimation);
    }

}
