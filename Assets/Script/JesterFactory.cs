using UnityEngine;

public class JesterFactory : Manager
{
    public GameObject jesterPrefab;
    public override void ManagerInit()
    {
        // do nothing
    }

    public Jester CreateJester(Joke joke)
    {
        var jester = Instantiate(jesterPrefab).GetComponent<Jester>();
        jester.m_Joke = joke;
        return jester;
    }
}

