using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MLibrary;
public class ScenarioTextRepository : Singleton<ScenarioTextRepository>
{
  private class Private
  {
    public static readonly string SCENARIO_TEXT_DATA_FOLDER = "ScenarioTextData";
  }
  private Dictionary<string, ScenarioTextDatas> _scenarioDataDict = new Dictionary<string, ScenarioTextDatas>();
  public void Initialize()
  {
    if(_scenarioDataDict != null)
    {
      ScenarioTextDatas[] datas = Resources.LoadAll(Private.SCENARIO_TEXT_DATA_FOLDER, typeof(ScenarioTextDatas)) as ScenarioTextDatas[];
      foreach(var data in datas)
      {
        _scenarioDataDict.Add(data.name,data);
      }
    }
  }

  public ScenarioTextDatas GetScenarioData(string dataListName)
  {
    if(_scenarioDataDict.ContainsKey(dataListName))
    {
      return _scenarioDataDict[dataListName];
    }
    else
    {
#if UNITY_EDITOR
      Debug.Log($"Try loading data {dataListName}");
#endif
      if (LoadScenarioDataImpl(dataListName))
      {
        return _scenarioDataDict[dataListName];
      }
    }

    return null;
  }

  private bool LoadScenarioDataImpl(string dataListName)
  {
    ScenarioTextDatas datas;
    
    #if UNITY_EDITOR
    {
      string assetPath = $"Assets/Resources/ScenarioTextData/{dataListName}.asset";
      datas = AssetDatabase.LoadAssetAtPath<ScenarioTextDatas>(assetPath);
    }
    #else
    {
      datas = Resources.Load($"ScenarioTextData/{dataListName}") as ScenarioTextDatas;
    }
    #endif

    if(datas == null)
    {
      {
        #if UNITY_EDITOR
            Debug.LogError($"Can't find file {dataListName} in path {Application.dataPath}/Resources/ScenarioTextData");
        #endif
      }

      return false;
    }

    _scenarioDataDict.Add(dataListName,datas);
    
    return true;
  }
}