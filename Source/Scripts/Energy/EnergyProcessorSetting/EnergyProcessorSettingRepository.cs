using System.Collections.Generic;
using UnityEngine;
using MLibrary;

/// <summary>
/// エナジープロセッサ設定リポジトリ
/// </summary>
public class EnergyProcessorSettingRepository : Singleton<EnergyProcessorSettingRepository>
{
  /// <summary>
  /// プロセッサ設定パラメーターデータコンテイナー
  /// </summary>
  private Dictionary<string, EnergyProcessorSettingDataDTO> _energyProcessorSettings;

  /// <summary>
  /// ビルダーを初期化する
  /// </summary>
  /// <returns>初期化成功したらtrueを返し、それ以外はfalse</returns>
  public bool Initialize()
  {
    // 複数回の初期化を防ぐ
    if (_energyProcessorSettings != null)
    {
      return true;
    }

    // データオブジェクトを読み込む
    EnergyProcessorSettings settings = Resources.Load(Zombie.Global.GlobalParam.ENERGY_PROCESSOR_SETTING_DATA_PATH) as EnergyProcessorSettings;

    if (settings == null)
    {

#if UNITY_EDITOR
      Debug.LogError("Can't find EnergyProcessorSettings");
#endif

      return false;
    }

    _energyProcessorSettings = settings.GetEnergyProcessorSettings();
    return true;
  }

  /// <summary>
  /// データオブジェクトを取得
  /// </summary>
  /// <param name="type">プロセッサタイプ</param>
  /// <returns>見つけたらデータオブジェクトを返す、それ以外はnullを返す</returns>
  public EnergyProcessorSettingDataDTO GetEnergyProcessorSettingData(string type)
  {
    if (string.IsNullOrEmpty(type))
    {
#if UNITY_EDITOR
      Debug.LogError("type name is invalid");
#endif
      return null;
    }

    // 初期化した状態の利用を確保する
    if (_energyProcessorSettings == null)
    {
      Initialize();
    }

    if (!_energyProcessorSettings.TryGetValue(type, out EnergyProcessorSettingDataDTO setting))
    {
#if UNITY_EDITOR
      Debug.LogError("Doesn't contain setting of " + type.ToString());
#endif
      return null;
    }

    return setting;
  }

  /// <summary>
  /// ビルダーを解放する
  /// </summary>
  public void Deinitialize()
  {
    if (_energyProcessorSettings != null)
    {
      _energyProcessorSettings.Clear();
      _energyProcessorSettings = null;
    }

    Resources.UnloadUnusedAssets();
  }
}
