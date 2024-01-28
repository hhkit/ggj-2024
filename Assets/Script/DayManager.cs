using System.Linq;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;


public class DayManager : Manager
{
    public TextAsset[] days;
    public LevelConfig currentDay { get; private set; }
    public int currentDayIndex { get; private set; }

    public static int CurrentLevel = 0;

    public override void ManagerInit()
    {
        DeserializeDayData(CurrentLevel);
    }

    void DeserializeDayData(int index)
    {
        var day = days.ElementAt(index);
        if (day == null)
            return;

        currentDayIndex = index;

        var deserializer = new DeserializerBuilder().Build();
        var level = deserializer.Deserialize<LevelConfig>(new StringReader(day.text));
        currentDay = level;

        foreach (var line in level.intro)
        {
            Debug.Log(line);
        }
    }
}
