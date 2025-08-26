using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(version:1, ext:"xlsx")]
public class ExcelImportSystem : FileImportSystem<ExcelImportSettingData>
{

  public override void OnImportAsset(AssetImportContext ctx)
  {
    AssetImportImpl(ctx);
  }

  protected override bool TryFindFile(string FilePath, out ExcelImportSettingData TargetAssetObject)
  {
    bool IsFound = false;
    TargetAssetObject = null;
    string filename = Path.GetFileNameWithoutExtension(FilePath);
    ExcelImportSettings settings = Resources.Load(Zombie.Global.GlobalParam.ENERGY_PROCESSOR_SETTING_DATA_PATH) as ExcelImportSettings;

    if(settings != null)
    {
      foreach(ExcelImportSettingData setting in settings.Data)
      {
        if ((setting == null) || (setting.MasterObject == null))
        {
          continue;
        }

        if (!setting.MasterObject.name.Equals(filename))
        {
          continue;
        }

        TargetAssetObject = setting;
        IsFound = true;     
        break;
      }
      Resources.UnloadAsset(settings);
    }
    return IsFound;
  }

  protected override void UpdateAssetObj(ExcelImportSettingData SettingData, StringBuilder JsonData)
  {
    if (SettingData == null || SettingData.MasterObject == null)
    {
      return;
    }

    // JsonデータをScriptableObjectに書き込む
    JsonUtility.FromJsonOverwrite(JsonData.ToString(), SettingData.MasterObject);
    EditorUtility.SetDirty(SettingData.MasterObject);
  }
}
