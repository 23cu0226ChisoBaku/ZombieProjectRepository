using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zombie.Input
{
  public enum EInputAxis : byte
  {
    X,
    Y
  }
}

public interface IZombiePlayerInput : IDisposable
{
  bool IsInputPressed(string inputName);
  void UpdateKeyState();
  float GetInputValueByAxis(Zombie.Input.EInputAxis Axis);
}

public class PlayerInput : IZombiePlayerInput
{
  private class EInputState
  {
    public static readonly int PRESSED = 1;
    public static readonly int NONE = 0;
    public static readonly int RELEASED = -1;
  }

  private Dictionary<string, int> _inputStatements;
  private Dictionary<string, Func<bool>> _inputStateUpdateMethods;

  public PlayerInput()
  {
    _inputStatements = new Dictionary<string, int>()
    {
      ["Jump"] = EInputState.NONE,
      ["SelectLeft"] = EInputState.NONE,
      ["SelectRight"] = EInputState.NONE,
      ["ReleaseEnergy"] = EInputState.NONE,
      ["SwitchMode"] = EInputState.NONE
    };
    _inputStateUpdateMethods = new Dictionary<string, Func<bool>>()
    {
      ["Jump"] = IsJumpPressed,
      ["SelectLeft"] = IsSelectLeftPressed,
      ["SelectRight"] = IsSelectRightPressed,
      ["ReleaseEnergy"] = IsReleaseEnergyPressed,
      ["SwitchMode"] = IsSwitchModePressed
    };
  }

  ~PlayerInput()
  {
    Dispose(false);
  }

  void IZombiePlayerInput.UpdateKeyState()
  {
    foreach (var (inputTypeName, inputCheckFunc) in _inputStateUpdateMethods)
    {
      bool bIsPressed = inputCheckFunc.Invoke();
      if (bIsPressed)
      {
        _inputStatements[inputTypeName] = EInputState.PRESSED;
      }
      else
      {
        // 前のフレームに離されたら押されていない状態に戻す
        if (_inputStatements[inputTypeName] == EInputState.RELEASED)
        {
          _inputStatements[inputTypeName] = EInputState.NONE;
        }
        // 前のフレームに押されたインプットを離された状態に切り替える
        else if (_inputStatements[inputTypeName] == EInputState.PRESSED)
        {
          _inputStatements[inputTypeName] = EInputState.RELEASED;
        }
      }
    }
  }

  bool IZombiePlayerInput.IsInputPressed(string inputName)
  {
    if (_inputStatements.TryGetValue(inputName, out int state))
    {
      return state == EInputState.PRESSED;
    }

    return false;
  }

  float IZombiePlayerInput.GetInputValueByAxis(Zombie.Input.EInputAxis Axis)
  {
    return Axis switch
    {
      Zombie.Input.EInputAxis.X => Input.GetAxis("GamePadHorizontal") + Input.GetAxisRaw("Keyboard Horizontal"),
      Zombie.Input.EInputAxis.Y => throw new NotImplementedException(),
      _                         => throw new ArgumentException()
    };
  }

  void IDisposable.Dispose()
  {
    Dispose(true);
  }

  private void Dispose(bool disposing)
  {
    if (disposing)
    {
      GC.SuppressFinalize(this);
    }

    _inputStatements.Clear();
    _inputStateUpdateMethods.Clear();
  }

  private bool IsJumpPressed()
  {
    // jump                 (keyboard)                          (Logicool F310)
    return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton1);
  }
  private bool IsSelectLeftPressed()
  {
    // select left          (keyboard)                          (Logicool F310)
    return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton4);
  }
  private bool IsSelectRightPressed()
  {
    // select right         (keyboard)                          (Logicool F310)
    return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.JoystickButton5);
  }
  private bool IsReleaseEnergyPressed()
  {
    // release energy       (keyboard)                          (Logicool F310)
    return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0);
  }
  private bool IsSwitchModePressed()
  {
    // enter select mode    (keyboard)                          (Logicool F310)
    return Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton3);
  }
}
