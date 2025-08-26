using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScenarioText/TextList",fileName = "NewScenarioTextDataList")]
public class ScenarioTextDatas : ScriptableObject
{
  public List<ScenarioTextData> TextData = new List<ScenarioTextData>();

  public Dictionary<int, string> GetTextData()
  {
    Dictionary<int, string> settings = new Dictionary<int, string>();

    foreach(var data in TextData)
    {
      if(settings.ContainsKey(data.TextID))
      {
        continue;
      }
      settings.Add(data.TextID, data.Text);
    }

    return settings;
  }

  public ScenarioTextData[] GetSectionDatas()
  {
    return TextData.ToArray();
  }
}

public static class ScenarioTextDatasExtensions
{
  public static int GetScenarioTextDataCount(this ScenarioTextDatas textDatas)
  {
    return textDatas.TextData.Count;
  }
}
