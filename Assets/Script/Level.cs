using System.Collections.Generic;


public struct JesterConfig
{
    public int funny;
    public int lame;
    public bool assassin;
}

public struct LevelConfig
{
    public List<string> intro;
    public string orders;
    public string kingPreference;
    public JesterConfig jesters;
}