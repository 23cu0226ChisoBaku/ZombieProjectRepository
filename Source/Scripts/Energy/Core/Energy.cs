using System;

/// <summary>
/// エナジータイプ
/// </summary>
public enum EEnergyType : byte
{
  None = 0,
  Up,
  Fire,
  Electric,
}

/// <summary>
/// エナジークラス
/// </summary>
public abstract class Energy
{
  public EEnergyType EnergyType { get; private set; }

  public Energy(EEnergyType type)
  {
    EnergyType = type;
  }

  /// <summary>
  /// エナジーを吸収する
  /// </summary>
  internal void OnAbsorb()
  {
    OnAbsorbImpl();
  }

  /// <summary>
  /// エナジーをリリースする
  /// </summary>
  /// <param name="Position">リリースするワールド座標</param>
  internal void OnRelease(UnityEngine.Vector3 Position)
  {
    OnReleaseImpl(Position);
  }

  /// <summary>
  /// エナジーが吸収された時呼び出されるコールバック
  /// </summary>
  private protected virtual void OnAbsorbImpl() { }

  /// <summary>
  /// エナジーがリリースされた時呼び出されるコールバック
  /// </summary>
  private protected virtual void OnReleaseImpl(UnityEngine.Vector3 Position) { }
}



