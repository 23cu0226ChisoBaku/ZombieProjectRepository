
/// <summary>
/// エナジープロセッサDTO
/// </summary>
[System.Serializable]
public class EnergyProcessorSettingDataDTO
{ 
  /// <summary>
  /// エナジープロセッサタイプ文字列
  /// </summary>
  public string EnergyProcessorType;
  public int MaxStockableEnergyNum;
  public string DefaultEnergy;
  public bool IsConsumable;
}
