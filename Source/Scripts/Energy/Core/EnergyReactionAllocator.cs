using System;
using System.Collections.Generic;
using MLibrary;

public class EnergyReactionAllocator : Singleton<EnergyReactionAllocator>
{
  private Dictionary<string, EnergyReaction> _energyProcessorTypeReactionContainer;
  private class Private
  {
    // エナジーリアクション接尾辞
    // 例： PlayerEnergyReaction, SpringEnergyReaction
    public static readonly string ENERGY_REACTION_CLASS_SUFFIX = "EnergyReaction";
  }

  /// <summary>
  /// リアクションアロケーターを初期化
  /// </summary>
  public void Initialize()
  {
    // 複数回の初期化を防ぐ
    if (_energyProcessorTypeReactionContainer != null)
    {
      return;
    }

    _energyProcessorTypeReactionContainer = new Dictionary<string, EnergyReaction>();
  }

  /// <summary>
  /// リアクションインスタンスのメモリを確保する
  /// </summary>
  /// <param name="ProcessorTypeName">エナジープロセッサタイプ文字列</param>
  /// <returns>確保したリアクションインスタンス参照</returns>
  private EnergyReaction Allocate(string ProcessorTypeName)
  {
    // リフレクションで新しいリアクションインスタンスを作成
    string reactionClassName = ProcessorTypeName + Private.ENERGY_REACTION_CLASS_SUFFIX;
    Type reactionClassType = Type.GetType(reactionClassName);
    EnergyReaction reactions = (EnergyReaction)Activator.CreateInstance(reactionClassType);

    _energyProcessorTypeReactionContainer.Add(ProcessorTypeName, reactions);

    return reactions;
  }

  /// <summary>
  /// リアクションインタフェースを取得
  /// </summary>
  /// <param name="processorTypeString">エナジープロセッサタイプ文字列</param>
  /// <returns>リアクションインスタンス参照</returns>
  public EnergyReaction GetReaction(string processorTypeString)
  {
    // 初期化した状態を保証する
    if (_energyProcessorTypeReactionContainer == null)
    {
      Initialize();
    }

    // 既存のインスタンスが見つからなかったら新しいインスタンスを作る
    if (!_energyProcessorTypeReactionContainer.TryGetValue(processorTypeString, out EnergyReaction reactions))
    {
      reactions = Allocate(processorTypeString);
    }

    return reactions;
  }
  
  /// <summary>
  /// メモリを解放する
  /// </summary>
  public void Terminate()
  {
    if (_energyProcessorTypeReactionContainer != null)
    {
      _energyProcessorTypeReactionContainer.Clear();
      _energyProcessorTypeReactionContainer = null;
    }
  }
}
