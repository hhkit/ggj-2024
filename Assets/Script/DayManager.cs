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

#if UNITY_EDITOR
    static bool firstLoad = true;
    public int DayOverride = 0;
#endif

    public override void ManagerInit()
    {
#if UNITY_EDITOR
        if (firstLoad)
        {
            CurrentLevel = DayOverride;
            firstLoad = false;
        }
#endif

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
    }
}
