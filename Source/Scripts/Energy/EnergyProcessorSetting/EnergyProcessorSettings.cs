using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnergyProcessorBuilder/EnergyProcessorSettings", fileName = "EnergyProcessorSettings")]
public class EnergyProcessorSettings : ScriptableObject
{
  public List<EnergyProcessorSettingDataDTO> Data = new List<EnergyProcessorSettingDataDTO>();

  public Dictionary<string, EnergyProcessorSettingDataDTO> GetEnergyProcessorSettings()
  {
    Dictionary<string, EnergyProcessorSettingDataDTO> settings = new Dictionary<string, EnergyProcessorSettingDataDTO>();
    foreach(var data in Data)
    {
      settings.Add(data.EnergyProcessorType, data);
    }

    return settings;
  }
}
