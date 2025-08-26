using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ExcelImportSetting/SettingsList" , fileName = "ExcelImportSettings")]
public class ExcelImportSettings : ScriptableObject
{
  public List<ExcelImportSettingData> Data = new List<ExcelImportSettingData>();
}
