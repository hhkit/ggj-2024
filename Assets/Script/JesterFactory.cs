using UnityEngine;

public class JesterFactory : Manager
{
    public GameObject jesterPrefab;
    public Transform jesterStartWaypoint;
    public override void ManagerInit()
    {
        // do nothing
    }

    public Jester CreateJester(Joke joke)
    {
        var jester = Instantiate(jesterPrefab, jesterStartWaypoint.position, jesterStartWaypoint.rotation).GetComponent<Jester>();
        jester.m_Joke = joke;
        return jester;
    }

    public Jester CreateAssassin()
    {
        return null;
    }
}

