using System.Linq;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;


public class DayManager : Manager
{
    public TextAsset[] days;
    public LevelConfig currentDay { get; private set; }

    public override void ManagerInit()
    {
        DeserializeDayData(0);
    }

    void DeserializeDayData(int index)
    {
        var day = days.ElementAt(index);
        if (day == null)
            return;

        var deserializer = new DeserializerBuilder().Build();
        var level = deserializer.Deserialize<LevelConfig>(new StringReader(day.text));
        currentDay = level;

        foreach (var line in level.intro)
        {
            Debug.Log(line);
        }
    }
}
