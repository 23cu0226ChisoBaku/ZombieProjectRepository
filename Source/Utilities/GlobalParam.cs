using UnityEngine;
using System;


namespace Zombie.Global
{
  public static class GlobalParam
  {
    public readonly static float TARGET_SELECT_CIRCLE_RADIUS = 4f;
    public readonly static Vector3 UP_ENERGY_POWER = new Vector3(0f, 10f, 0f);
    public readonly static float FOG_DENSE = 0.6f;

    public readonly static Color ENERGY_SELECTED_MIX_COLOR = Color.red;
    public readonly static float RAYCAST_OFFSET = 0.05f;

    #region Player

    public readonly static Vector3 RESET_POSITION = Vector3.zero;

    #endregion
    #region PlayerInput
    //public readonly static KeyCode MOVE_UP = KeyCode.UpArrow;
    //public readonly static KeyCode MOVE_DOWN = KeyCode.DownArrow;
    public readonly static KeyCode JUMP = KeyCode.JoystickButton1;
    public readonly static KeyCode SELECT_LEFT = KeyCode.JoystickButton4;
    public readonly static KeyCode SELECT_RIGHT = KeyCode.JoystickButton5;
    public readonly static KeyCode RELEASE_ENERGY = KeyCode.JoystickButton0;
    public readonly static KeyCode ACTION = KeyCode.JoystickButton2;
    public readonly static KeyCode SWITCH_MODE = KeyCode.JoystickButton3;
    #endregion
    #region CameraParam
    public readonly static float CAMERA_MOVE_SPEED = 0.01f;
    #endregion

    #region ResourcesPath(Resources.Load)
    public readonly static string ENERGY_UI_PATH = "Prefabs/TestData/PlayerEnergyDataUI";
    public readonly static string SWITCH_SCENE_OBJ_PATH = "Prefabs/SwitchScene";
    #endregion

    #region Energy Model
    public readonly static float UP_POWER = 10.5f;
    #endregion

    #region Cheat Code Param
    public readonly static Vector3 GOAL_POS = new Vector3(140f, -5f, 0f);
    #endregion

    public static readonly string ENERGY_PROCESSOR_SETTING_DATA_PATH = "ExcelImportSettings/EnergyProcessorSettings";
    public static readonly string SCENARIO_IMPORT_DATA_PATH = "ScenarioImportDatas/ScenarioImportDatas";
  }
  // static class GlobalParams

}// namespace Zombie.Global
