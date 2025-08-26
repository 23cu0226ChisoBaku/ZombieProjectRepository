using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

// Zombie text file = ztf
[ScriptedImporter(version:1, ext:"ztf")]
public class ZtfImportSystem : FileImportSystem<ScenarioImportMasterData>
{
  private class Private
  {
    public static readonly string SCENARIO_IMPORT_DATA_OBJECT_PATH = "ScenarioImportDatas/ScenarioImportDatas";
  }
  public override void OnImportAsset(AssetImportContext ctx)
  {
    AssetImportImpl(ctx);
  }

  protected override bool TryFindFile(string FilePath, out ScenarioImportMasterData TargetAssetObject)
  {
    bool IsFound = false;
    TargetAssetObject = default;
    string filename = Path.GetFileNameWithoutExtension(FilePath);

    ScenarioImportDatas ImportDatas = Resources.Load(Private.SCENARIO_IMPORT_DATA_OBJECT_PATH) as ScenarioImportDatas;
    IReadOnlyList<ScenarioImportMasterData> MasterDatas = ImportDatas.GetReadOnlyList();
    if(ImportDatas != null)
    {
      foreach(var element in MasterDatas)
      {
        if ((element == null) || (element.MasterData == null))
        {
          continue;
        }

        if (!element.MasterData.name.Equals(filename))
        {
          continue;
        }

        TargetAssetObject = element;
        IsFound = true;
        break;
      }

      Resources.UnloadAsset(ImportDatas);
    }
    else
    { 
      Debug.LogWarning("Import is null");
    }

    return IsFound;
  }

  protected override void UpdateAssetObj(ScenarioImportMasterData Data, StringBuilder JsonData)
  {
    if ((Data == null) || (Data.MasterData == null))
    {
      return;
    }

    JsonUtility.FromJsonOverwrite(JsonData.ToString(), Data.MasterData);
    EditorUtility.SetDirty(Data.MasterData);
  }
}
