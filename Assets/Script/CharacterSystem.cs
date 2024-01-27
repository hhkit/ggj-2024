using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSystem : MonoBehaviour
{
    public static CharacterSystem instance;

    [SerializeField] private GameObject m_mainWaypoint;
    [SerializeField] private GameObject m_WaypointHolder;
    [SerializeField] private List<GameObject> m_WayPointList;
    [SerializeField] private List<GameObject> m_KingPath;
    private List<Vector3> m_KingPathVector;

    private static int WAYPOINTCOUNT = 20;

    private void Awake()
    {
        instance = this;
        Vector3 currentPos = m_mainWaypoint.transform.position;
        float dist = 1.4f;

        for (int i = 0; i < WAYPOINTCOUNT; ++i)
        {
            GameObject tmp = new GameObject();
            tmp.transform.SetParent(m_WaypointHolder.transform);
            currentPos -= new Vector3(dist, 0, 0);
            tmp.transform.position += currentPos;
            m_WayPointList.Add(tmp);
        }
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

    public void MoveJesters(Jester[] _jesters)
    {
        GameSystem.instance.m_CurrentJester.GoToPosition(m_WayPointList[0].transform.position);
        int count = 1;
        foreach (Jester item in _jesters)
        {
            if (count > WAYPOINTCOUNT)
                break;

            item.GoToPosition(m_WayPointList[count].transform.position);

            count++;
        }
    }

    public void SendJesterToKing(Jester _jester)
    {
       // Path path = new Path(DG.Tweening.PathType.CatmullRom, m_KingPathVector.ToArray(), 1);
        _jester.GoToKing(m_KingPathVector.ToArray());
    }

    public void RefuseJester(Jester _jester)
    {
        Path path = new Path(DG.Tweening.PathType.CatmullRom, m_KingPathVector.ToArray(), 1);
        //_jester.GoToKing(path);
    }

}

