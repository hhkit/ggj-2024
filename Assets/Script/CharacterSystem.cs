using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CharacterSystem : Manager
{
    public static CharacterSystem instance;

    [SerializeField] private GameObject m_mainWaypoint;
    [SerializeField] private GameObject m_WaypointHolder;
    [SerializeField] private GameObject m_OffscreenWaypoint;
    [SerializeField] private GameObject m_ThroneRoomExitWaypoint;
    [SerializeField] private List<GameObject> m_WayPointList;
    [SerializeField] private List<GameObject> m_KingPath;
    private List<Vector3> m_KingPathVector;

    private static int WAYPOINTCOUNT = 20;

    public override void ManagerInit()
    {
        instance = this;
        Vector3 currentPos = m_mainWaypoint.transform.position;
        float dist = 1.4f;

        for (int i = 0; i < WAYPOINTCOUNT; ++i)
        {
            GameObject tmp = new GameObject();
            tmp.name = $"{i}";
            tmp.transform.SetParent(m_WaypointHolder.transform);
            currentPos -= new Vector3(dist, 0, 0);
            tmp.transform.position += currentPos;
            m_WayPointList.Add(tmp);
        }

        // Debug.Log("waypoints: " + string.Join(',', m_WayPointList.Select(wp => wp.transform.position)));
    }

    void Start()
    {
        m_KingPathVector = new List<Vector3>();
        foreach (var item in m_KingPath)
            m_KingPathVector.Add(item.transform.position);
    }


    void Update()
    {
    
    }

    public Tween MoveJesters(Queue<Jester> _jesters)
    {
        var tween = DOTween.Sequence();
        Debug.Log($"queue {_jesters.Count} wps: {m_WayPointList.Count} ");
        _jesters.Zip(m_WayPointList, (jester, waypoint) =>
        {
            tween.Join(jester.GoToPosition(waypoint.transform.position));
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
       return _jester.GoToPosition(m_OffscreenWaypoint.transform.position);
    }

    public Tween PlayKingAcceptJester(Jester _jester)
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(_jester.GoToPosition(m_ThroneRoomExitWaypoint.transform.position))
           .Join(_jester.ResetSprite(1.0f));
        seq.Append(_jester.GoToPosition(m_OffscreenWaypoint.transform.position));
        return seq;
    }

    public Tween PlayKingRefuseJester(Jester _jester)
    {
        return null;
        return _jester.GoToPosition(m_ThroneRoomExitWaypoint.transform.position);
    }
}

