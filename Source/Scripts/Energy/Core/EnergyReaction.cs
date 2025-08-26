using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エナジーリアクションコンテキスト
/// </summary>
public class EnergyReactionContext
{
  public GameObject UserObject;
  public Rigidbody2D UserRigidbody;
  public Animator UserAnimator;
  public GameObject EnergySourceObject;
}

public abstract class EnergyReaction
{
  private Dictionary<int, Action<EnergyReactionContext>> _absorbReactions;
  private Dictionary<int, Action<EnergyReactionContext>> _releaseReactions;

  public EnergyReaction()
  {
    InitReactions();
  }

  /// <summary>
  /// エナジー吸収に反応する
  /// </summary>
  /// <param name="type">エナジータイプ</param>
  /// <param name="ctx">エナジーリアクションコンテキスト</param>
  public void OnAbsorbReact(EEnergyType type, EnergyReactionContext ctx)
  {
    if (_absorbReactions.TryGetValue(type.ConvertToInt(), out Action<EnergyReactionContext> reactionInstance))
    {
      reactionInstance?.Invoke(ctx);
    }
  }

  /// <summary>
  /// エナジーリリースに反応する
  /// </summary>
  /// <param name="type">エナジータイプ</param>
  /// <param name="ctx">エナジーリアクションコンテキスト</param>
  public void OnReleaseReact(EEnergyType type, EnergyReactionContext ctx)
  {
    if (_releaseReactions.TryGetValue(type.ConvertToInt(), out Action<EnergyReactionContext> reactionInstance))
    {
      reactionInstance?.Invoke(ctx);
    }
  }

  private void InitReactions()
  {
    _absorbReactions = new Dictionary<int, Action<EnergyReactionContext>>()
    {
      [EEnergyType.Fire.ConvertToInt()] = OnAbsorbFire,
      [EEnergyType.Up.ConvertToInt()] = OnAbsorbUp,
      [EEnergyType.Electric.ConvertToInt()] = OnAbsorbElectric,
    };

    _releaseReactions = new Dictionary<int, Action<EnergyReactionContext>>()
    {
      [EEnergyType.Fire.ConvertToInt()] = OnReleaseFire,
      [EEnergyType.Up.ConvertToInt()] = OnReleaseUp,
      [EEnergyType.Electric.ConvertToInt()] = OnReleaseElectric,
    };
  }

  protected virtual void OnAbsorbFire(EnergyReactionContext ctx) { }
  protected virtual void OnAbsorbUp(EnergyReactionContext ctx) { }
  protected virtual void OnAbsorbElectric(EnergyReactionContext ctx) { }

  protected virtual void OnReleaseUp(EnergyReactionContext ctx) { }
  protected virtual void OnReleaseFire(EnergyReactionContext ctx) { }
  protected virtual void OnReleaseElectric(EnergyReactionContext ctx) { }
}
