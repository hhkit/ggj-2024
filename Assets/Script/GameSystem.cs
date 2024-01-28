using DG.Tweening;
#if UNITY_EDITOR
using EasyButtons;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameSystem : MonoBehaviour
{
    public Queue<Jester> m_JesterQueue { get; private set; } = new();
    public King m_King { get; private set; }
    public Jester m_CurrentJester { get; private set; }

    public int m_Submitted { get; private set; }
    public int m_Points { get; private set; }
    public int m_Quota { get; private set; }

    public static GameSystem instance;

    private JokeManager jokeManager;
    private DayManager dayManager;
    private JesterFactory jesterFactory;
    private DialogueManager dialogManager;
    private DialogueDirector director;

    private StartDayController startDayController;
    public GameUIController gameUI;
    private EndDayController endDayController;

    public GameObject coinShower;

    void Awake()
    {
        instance = this;
        jokeManager = FindObjectOfType<JokeManager>();
        dayManager = FindObjectOfType<DayManager>();
        jesterFactory = FindObjectOfType<JesterFactory>();
        dialogManager = FindObjectOfType<DialogueManager>();
        director = FindObjectOfType<DialogueDirector>();
        Debug.Assert(jokeManager != null, "No JokeManager in scene");
        Debug.Assert(dayManager != null, "No DayManager in scene");
        Debug.Assert(jesterFactory != null, "No JesterFactory in scene");

        startDayController  = FindObjectOfType<StartDayController>(true);
        startDayController.LetterClosed.AddListener(StartGame);

        endDayController = FindObjectOfType<EndDayController>(true);

       
    }

    bool hasStarted = false;
    void StartGame()
    {
        if (hasStarted)
            return;

        AudioManager.PlayBGM("Tax-Office-Day-PM-Music", 0.2f);
        AudioManager.PlayOneShot("DayStart");
        hasStarted = true;

        InitializeKing();
        PopulateJesters();
        m_Points = 0;
        m_Submitted = 0;
        m_Quota = dayManager.currentDay.quota;

        AdvanceJesterQueue()
            .OnComplete(StartJesterConversation);
        startDayController.LetterClosed.RemoveListener(StartGame);

    }

    void InitializeKing()
    {
        m_King = FindObjectOfType<King>();
        Debug.Assert(m_King != null);

        var preference = dayManager.currentDay.kingPreference.Trim();
        Debug.Log($"King prefers {preference} today");
        if (preference != "")
            m_King.SetJokePreference(preference);
    }

    void PopulateJesters()
    {
        var jokeQueue = jokeManager.CreateJokeQueue(m_King, dayManager.currentDay.jesters);
        var jesters = jokeQueue.Select(jesterFactory.CreateJester).ToArray();

        float z = 0;
        foreach (var jester in jesters)
        {
            z += 0.5f;
            jester.z = z;
        }

        m_JesterQueue = new(jesters);

        if (dayManager.currentDay.jesters.assassin)
        {
            // select one character as assassin
            var turnMe = Random.Range(0, jesters.Count());
            var assassin = jesters[turnMe];
            assassin.BecomeAssassin();
            Debug.Log($"{turnMe} am become death");
        }

        Debug.Log($"created {m_JesterQueue.Count()} jesters from {jokeQueue.Count()} jokes");
    }

    bool CheckJoke(Jester jester, out RejectionReason rejectReason)
    {
        m_Submitted += 1;
        Debug.Assert(jester != null, "Invalid jester");
        return m_King.ApproveJester(jester, out rejectReason);
    }

    Tween AdvanceJesterQueue()
    {

        if (m_JesterQueue.Count() == 0)
        {
            m_CurrentJester = null;
            return null;
        }

        
        // TODO: potential atom bomb waiting to go off
        if (m_CurrentJester != null)
            m_JesterQueue.Dequeue();

        m_CurrentJester = m_JesterQueue.FirstOrDefault();
        if (m_CurrentJester != null)
            dialogManager.OnJesterSpeakJoke += m_CurrentJester.PlayTalkAnimation;
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => AudioManager.PlayOneShot("CallNextSound"));
        seq.AppendInterval(0.25f);
        seq.Append(WaypointManager.instance.MoveJesters(new(m_JesterQueue)));
        return seq;
    }

    void AddScore()
    {
        m_Points++;
    }
    void DeductScore()
    {
        m_Points--;
    }

    // This begins the end of day sequence
    Tween EndDay()
    {
        var winFlag = m_Points >= m_Quota;
        return endDayController.PlayEndingSequence(m_Submitted, m_Quota, winFlag ? WinState.Win : WinState.Lose);
    }

#if UNITY_EDITOR
    [Button]
#endif
    public void AcceptJester()
    {
        StartCoroutine(AcceptJesterCoro());
    }
    // Player sends the Jester at the front of the line to the King
    private IEnumerator AcceptJesterCoro()
    {
        var jester = m_CurrentJester;
        jester.z = 0.5f;

        yield return director.PlaySendToKingDialog();

        yield return new WaitForTween(WaypointManager.instance.SendJesterToKing(jester));

        if (CheckJoke(jester, out RejectionReason reason))
        {

            Instantiate(coinShower);
            AddScore();
            yield return director.PlayKingDialog(true, reason);
            WaypointManager.instance.PlayKingAcceptJester(jester); // jester leave
        }
        else
        {
            if (reason == RejectionReason.Death)
            {
                endDayController.PlayEndingSequence(m_Submitted, m_Quota, WinState.Death);
                yield break;
            }

            DeductScore();
            yield return director.PlayKingDialog(false, reason);
            WaypointManager.instance.PlayKingRefuseJester(jester);
        }

        AdvanceJesterQueue()
            .OnComplete(StartJesterConversation);
    }

#if UNITY_EDITOR
    [Button]
#endif
    public void RejectJester()
    {
        AudioManager.PlayOneShot("DenyAudienceSound");
        StartCoroutine(RejectJesterCoro());
    }

    // Player refuses the Jester as audience to the King
    private IEnumerator RejectJesterCoro()
    {
        var jester = m_CurrentJester;
        jester.z = 0.5f;

        yield return director.PlayRejectDialog();
        
        WaypointManager.instance.RefuseJester(jester)
            .OnKill(() => Destroy(jester.gameObject));

        AdvanceJesterQueue()
            .OnComplete(StartJesterConversation);
    }

    public void StartJesterConversation()
    {
        
        if (m_JesterQueue.Count() == 0)
        {
            EndDay();
            return;
        }

        ReplayJesterConversation();
    }

    public void ReplayJesterConversation()
    {
        DialogueDirector.instance.StartJokeDialog(m_CurrentJester,
            () => // last
            {
                gameUI.Show();
            });
    }

}
