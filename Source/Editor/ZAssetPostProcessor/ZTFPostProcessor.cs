using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Zombie.Global;

public class ZTFPostProcessor : AssetPostprocessor
{
  static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
  {
    foreach(var filepath in importedAssets)
    {
      string fileExt = Path.GetExtension(filepath);
      if(fileExt.Equals(".ztf"))
      {
        string filename = Path.GetFileNameWithoutExtension(filepath);

        ScenarioImportMasterData file = null;
        ScenarioImportDatas datas = Resources.Load(GlobalParam.SCENARIO_IMPORT_DATA_PATH) as ScenarioImportDatas;
        IReadOnlyList<ScenarioImportMasterData> d = datas.GetReadOnlyList();
        foreach(var data in d)
        {
          if(data.MasterData== null)
            continue;
          if(!data.MasterData.name.Equals(filename))
            continue;

          file = data;
        }

        if(file == null)
        {   
          ScenarioTextDatas scenarioTextDatas = ScriptableObject.CreateInstance<ScenarioTextDatas>();     
          WriteZTFDataToScriptableObj(filepath,ref scenarioTextDatas);
          AssetDatabase.CreateAsset(scenarioTextDatas,$"Assets/Resources/ScenarioTextData/{filename}.asset");       
          datas.AddMasterData(scenarioTextDatas);
        }                           

        EditorUtility.SetDirty(datas);
        AssetDatabase.SaveAssets();
      }
    }

    foreach(var deleted in deletedAssets)
    {
      string fileExt = Path.GetExtension(deleted);
      if(fileExt.Equals(".ztf"))
      {
        string filename = Path.GetFileNameWithoutExtension(deleted);
        Debug.Log($"{filename} deleted");

        ScenarioImportMasterData file = null;
        ScenarioImportDatas datas = Resources.Load(GlobalParam.SCENARIO_IMPORT_DATA_PATH) as ScenarioImportDatas;
        IReadOnlyList<ScenarioImportMasterData> d = datas.GetReadOnlyList();
        foreach(var data in d)
        {
          if(data.MasterData== null)
            continue;
          if(!data.MasterData.name.Equals(filename))
            continue;

          file = data;
        }

        if(file != null)
        {   
          datas.RemoveMasterData(file);
          string scriptableObjFilepath = $"{Application.dataPath}/Resources/ScenarioTextData/{filename}.asset";
          if(File.Exists(scriptableObjFilepath))
          {
            File.Delete(scriptableObjFilepath);
            File.Delete($"{scriptableObjFilepath}.meta");
          }
        }                           

        EditorUtility.SetDirty(datas);
        AssetDatabase.SaveAssets();
      }
    }
  }

  private static void WriteZTFDataToScriptableObj(string filepath, ref ScenarioTextDatas datasToScriptableObj)
  {
    if(!File.Exists(filepath))
    {
      return;
    } 

    ConvertToJsonHelper.ConvertToJson(filepath,out StringBuilder jsonFormat);
    JsonUtility.FromJsonOverwrite(jsonFormat.ToString(), datasToScriptableObj);
    EditorUtility.SetDirty(datasToScriptableObj);
  }
}