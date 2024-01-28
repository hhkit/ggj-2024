using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class WaypointManager : Manager
{
    public static WaypointManager instance;

    [SerializeField] private GameObject m_mainWaypoint;
    [SerializeField] private GameObject m_WaypointHolder;
    [SerializeField] private GameObject m_KingStart;
    [SerializeField] private GameObject m_KingEnd;
    [SerializeField] private GameObject m_ExitStageLeft;
    [SerializeField] private GameObject m_ExitStageRight;
    [SerializeField] private GameObject m_ThroneRoomExitWaypoint;
    private List<Transform> m_WayPointList;
    private List<Vector3> m_KingPathVector;
    [SerializeField] private float m_QueueGap = 1.4f;

    private static int WAYPOINTCOUNT = 20;

    [SerializeField] private GameObject debugJester;

    public override void ManagerInit()
    {
        instance = this;

        m_WayPointList = new(CreateQueueWaypoints().Select(pos =>
        {
            GameObject tmp = new GameObject();
            tmp.transform.SetParent(m_WaypointHolder.transform);
            tmp.transform.position = pos;
            return tmp.transform;
        }));
        // Debug.Log("waypoints: " + string.Join(',', m_WayPointList.Select(wp => wp.transform.position)));
    }

    void Start()
    {
        m_KingPathVector = (new GameObject[]{ m_KingStart, m_KingEnd }).Select(wp => wp.transform.position).ToList();
    }

#if UNITY_EDITOR
    public bool debug = true;

    void OnDrawGizmos()
    {
        if (debug == false)
            return;

        var wps = CreateQueueWaypoints();
        var front = wps.First();
        var back = wps.Last();
        Debug.DrawLine(front, back, Color.yellow);

        const float length = 0.1f;
        foreach (var wp in wps)
        {
            Debug.DrawLine(wp + Vector3.up * length, wp + Vector3.down * length, Color.yellow);
        }

        Debug.DrawLine(front, m_ExitStageLeft.transform.position, Color.red);

        Debug.DrawLine(front, m_KingStart.transform.position, Color.green);
        Debug.DrawLine(m_KingStart.transform.position, m_KingEnd.transform.position, Color.green);
        Debug.DrawLine(m_KingEnd.transform.position, m_ThroneRoomExitWaypoint.transform.position, Color.green);
        Debug.DrawLine(m_ThroneRoomExitWaypoint.transform.position, m_ExitStageRight.transform.position, Color.green);
    }
#endif


    public Tween MoveJesters(Queue<Jester> _jesters)
    {
        var tween = DOTween.Sequence();
        Debug.Log($"queue {_jesters.Count} wps: {m_WayPointList.Count} ");
        _jesters.Zip(m_WayPointList, (jester, waypoint) =>
        {
            tween.Join(jester.GoToPosition(waypoint.position));
            return jester;
        }).ToArray();

        return tween;
    }

    public Tween SendJesterToKing(Jester _jester)
    {
       return _jester.GoToKing(m_KingPathVector.ToArray(), 1.0f);
    }

    public Tween RefuseJester(Jester _jester)
    {
        _jester.FlipAround();
       return _jester.GoToPosition(m_ExitStageLeft.transform.position);
    }

    public Tween PlayKingAcceptJester(Jester _jester)
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(_jester.GoToPosition(m_ThroneRoomExitWaypoint.transform.position))
           .Join(_jester.ResetSprite(1.0f,true));
        seq.Append(_jester.GoToPosition(m_ExitStageRight.transform.position));
        return seq;
    }

    public Tween PlayKingRefuseJester(Jester _jester)
    {
        return PlayKingAcceptJester(_jester); // same path la
        //return _jester.GoToPosition(m_ThroneRoomExitWaypoint.transform.position);
    }

    IEnumerable<Vector3> CreateQueueWaypoints()
    {
        Vector3 origin = m_mainWaypoint.transform.position;
        return Enumerable.Range(0, WAYPOINTCOUNT).Select(i => origin + Vector3.left * m_QueueGap * i);
    }
}

