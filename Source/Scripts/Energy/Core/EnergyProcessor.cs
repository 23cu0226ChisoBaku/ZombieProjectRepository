using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// エナジー処理を行うクラス
/// </summary>
public sealed class EnergyProcessor
{
  /// <summary>
  /// 持っているエナジーのリスト
  /// </summary>
  private List<Energy> _energies = new List<Energy>();
  
  /// <summary>
  /// エナジーが消耗できるか
  /// </summary>
  private bool _isEnergyConsumable = false;

  /// <summary>
  /// エナジーを保存できる最大数
  /// </summary>
  private int _maxStockableEnergyNum = 0;

  /// <summary>
  /// エナジープロセッサのタイプ
  /// </summary>
  // TODO String以外の何かでタイプを表せ
  private string _type = null;

  public EnergyProcessor(string type, bool isEnergyConsumable = false)
  {
    Assert.IsFalse(string.IsNullOrEmpty(type));

    // リポジトリから設定データを取得
    EnergyProcessorSettingDataDTO setting = EnergyProcessorSettingRepository.Instance.GetEnergyProcessorSettingData(type);
    Assert.IsNotNull(setting);

    // 初期化
    {
      EEnergyType defaultEnergyType = (EEnergyType)Enum.Parse(typeof(EEnergyType), setting.DefaultEnergy);
      if (defaultEnergyType != EEnergyType.None)
      {
        // リフレクションでインスタントを生成
        string energyClassTypeName = setting.DefaultEnergy + "Energy";
        Type energyClassType = Type.GetType(energyClassTypeName);
        Energy baseEnergy = (Energy)Activator.CreateInstance(energyClassType);

        _energies.Add(baseEnergy);
      }
    }

    _isEnergyConsumable = isEnergyConsumable;
    _maxStockableEnergyNum = setting.MaxStockableEnergyNum;
    _type = type;

  }

  /// <summary>
  /// 保存されているエナジーのタイプを取得
  /// </summary>
  /// <returns></returns>
  public List<EEnergyType> GetEnergyTypes()
  {
    List<EEnergyType> types = new List<EEnergyType>();

    // 保存できる最大数まで取得
    // TODO 余計なメモリが確保される
    for (int i = 0; i < _maxStockableEnergyNum; ++i)
    {
      if (i < _energies.Count)
      {
        types.Add(_energies[i].EnergyType);
      }
      // 今持っているエナジー以外のインデックスにNoneをいれる
      else
      {
        types.Add(EEnergyType.None);
      }
    }

    return types;
  }

  /// <summary>
  /// エナジーが持っているか
  /// </summary>
  /// <returns>持っていればtrueを返す、それ以外はfalse</returns>
  public bool IsEnergyStockEmpty() => _energies.Count == 0;

  /// <summary>
  /// エナジーをリリース
  /// </summary>
  /// <param name="index">インデックス</param>
  /// <param name="targetPosition">リリースワールド座標</param>
  /// <returns>リリースされたエナジーインスタント</returns>
  public Energy ReleaseEnergyByIndex(int index, EnergyReactionContext SourceContext, Vector3 targetPosition)
  {
    Energy releasedEnergy = GetEnergyByIndex(index);
    if (releasedEnergy != null)
    {
      releasedEnergy.OnRelease(targetPosition);

      // エナジー吸収する時のコールバックインタフェースを取得
      EnergyReaction typeReaction = EnergyReactionAllocator.Instance.GetReaction(_type);
      typeReaction.OnReleaseReact(releasedEnergy.EnergyType, SourceContext);

      if (_isEnergyConsumable)
      {
        _energies.RemoveAt(index);
      }
    }

    return releasedEnergy;
  }

  /// <summary>
  /// エナジーを吸収する
  /// </summary>
  /// <param name="energy">エナジーインスタント</param>
  /// <param name="ctx">エナジーリアクションコンテキスト</param>
  public void AbsorbEnergy(Energy energy, EnergyReactionContext ctx, bool bIsSourceSelf = true)
  {
    if (energy == null)
    {
      return;
    }

    // エナジー吸収する時のコールバックインタフェースを取得
    EnergyReaction typeReaction = EnergyReactionAllocator.Instance.GetReaction(_type);
    typeReaction.OnAbsorbReact(energy.EnergyType, ctx);

    // エナジー吸収してストックにいれる
    if (_energies.Count < _maxStockableEnergyNum)
    {
      energy.OnAbsorb();

      if (bIsSourceSelf)
      {
        _energies.Add(energy);
      }
    }
  }

  /// <summary>
  /// エナジーインスタントを取得
  /// </summary>
  /// <param name="index">インデックス</param>
  /// <returns>有効なインデックスだったら有効なエナジーインスタントを返す、それ以外はnull</returns>
  private Energy GetEnergyByIndex(int index)
  {
    if (index >= _energies.Count || index >= _maxStockableEnergyNum)
    {
      return null;
    }

    return _energies[index];
  }
}
