using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Windows;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;


public class DayManager : MonoBehaviour
{
    public TextAsset[] days;
    // Start is called before the first frame update

    public LevelConfig currentDay { get; private set; }
    void Start()
    {
        var firstDay = days.First();
        if (firstDay == null)
            return;

        var deserializer = new DeserializerBuilder().Build();
        var level = deserializer.Deserialize<LevelConfig>(new StringReader(firstDay.text));
        currentDay = level;

        foreach (var line in level.intro)
        {
            Debug.Log(line);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
