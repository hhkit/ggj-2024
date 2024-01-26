using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

public class Jester : MonoBehaviour
{
    public Joke m_Joke;
    public JesterRace m_JokerRace;
    public static float JESTERSPEED = 2;
    void Start()
    {

    }

    void Update()
    {

    }

    public void GoToPosition(Vector3 _targetPos)
    {
        float distance = Vector3.Distance(_targetPos, transform.position);
        transform.DOMove(_targetPos, distance / JESTERSPEED);

    }

    public void GoToKing(Path _path)
    {
        transform.DOPath(_path, 1);
    }

}
