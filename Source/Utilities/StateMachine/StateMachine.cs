using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;

namespace MStateMachine
{
  /// <summary>
  /// Monobehaviour依存ステートマシン
  /// </summary>
  /// <typeparam name="EState">ユーザー定義ステート列挙型</typeparam>
  public class StateMachine<EState> : MonoBehaviour where EState : System.Enum
  {
    // ステートを保存するDictionary
    // Key   : int   (ステート列挙型をintに変換)
    // Value : State 参照
    private Dictionary<int, State<EState>> _states = new Dictionary<int, State<EState>>();

    // 次のステートに切り替えるリクエストを保存するキュー
    // 同じフレームで複数回リクエストがあった場合、先にリクエストしたステートが優先され、同じキューにある他のリクエストを破棄する
    private Queue<EState> _switchNextRequestedQueue = new Queue<EState>();

    // 今のステート
    private State<EState> _currentState;

    // 今のステートの文字列を取得する
    // 今のステートが存在しないとNoneが返す
    protected internal string CurrentStateString => (_currentState != null) ? _currentState.ToString() : "None";

    /// <summary>
    /// ステートを追加する
    /// </summary>
    /// <param name="type">ステート列挙</param>
    /// <param name="state">ステートインスタンス参照</param>
    protected void AddState(EState type, State<EState> state)
    {
      int typeKey = type.ConvertToInt();
      // 存在したステート列挙を入れない
      if (!_states.ContainsKey(typeKey))
      {
        _states.Add(typeKey, state);
      }
    }

    // Unity専用コールバック関数群
    #region Unity lifetime
    private void Start()
    {
      _currentState?.EnterState();
    }

    private void Update()
    {
      _currentState?.UpdateState();

      // 切り替えリクエストがあればステートを切り替える
      if (_switchNextRequestedQueue.Count != 0)
      {
        TransitionToState(_switchNextRequestedQueue.Dequeue());
      }
    }

    private void FixedUpdate()
    {
      _currentState?.FixedUpdateState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      _currentState.OnCollisionEnter2D(collision);
    }

    #region Collision/Trigger 2D Callback
    private void OnCollisionStay2D(Collision2D collision)
    {
      _currentState.OnCollisionStay2D(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
      _currentState.OnCollisionExit2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      _currentState.OnTriggerEnter2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
      _currentState.OnTriggerStay2D(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
      _currentState.OnTriggerExit2D(collision);
    }
    #endregion Collision/Trigger 2D Callback

    #endregion Unity lifetime

    /// <summary>
    /// 次のステートに切り替える
    /// </summary>
    /// <param name="nextState">切り替えるステート列挙</param>
    private void TransitionToState(EState nextState)
    {
      // 同じステートを切り替えない
      if ((_currentState != null) && _currentState.StateKey.Equals(nextState))
      {
        return;
      }

      SwitchNextStateImpl(nextState);

      // すべてのリクエストを削除
      _switchNextRequestedQueue.Clear();

    }

    /// <summary>
    /// 次のステートに切り替えることをリクエストする
    /// </summary>
    /// <param name="nextState">次のステート列挙</param>
    /// <param name="bImmediately">すぐに切り替えるか</param>
    public void RequestSwitchNextState(EState nextState, bool bImmediately = false)
    {
      if (bImmediately)
      {
        SwitchNextStateImpl(nextState);
      }
      // 次のUpdateで切り替える
      else
      {
        _switchNextRequestedQueue.Enqueue(nextState);
      }
    }

    private void SwitchNextStateImpl(EState nextState)
    {
      // 前のステートが存在すればExitを呼び出す
      _currentState?.ExitState();

      int nextStateKey = nextState.ConvertToInt();
      if (_states.ContainsKey(nextStateKey))
      {
        _currentState = _states[nextStateKey];
      }
      else
      {
        Debug.LogError($"Can't find state {nextState}");
      }

      // 切り替えた後のステートのEnterを呼び出す
      _currentState?.EnterState();
    }

    /// <summary>
    /// 今は特定のステートかを確認する
    /// </summary>
    /// <param name="StateKey">確認したいステートタイプ</param>
    /// <returns>今のステートが存在するかつステートタイプと確認したいステートタイプ一致すればtrue,それ以外はfalse</returns>
    public bool IsInState(EState StateKey)
    {
      if (_currentState == null)
      {
        return false;
      }

      return EqualityComparer<EState>.Default.Equals(_currentState.StateKey, StateKey);
    }
  }
}// namespace MStateMachine

public static class EnumConvertToIntExtension
{
  /// <summary>
  /// 列挙を整数型に変換する
  /// </summary>
  /// <typeparam name="TEnum">列挙ジェネリック</typeparam>
  /// <param name="enumVal">列挙の値</param>
  /// <returns>変換した後の整数</returns>
  public static int ConvertToInt<TEnum>(this TEnum enumVal) where TEnum : System.Enum
  {
    return StaticEnumConvertCache<TEnum>.ConvertFunc(enumVal);
  }
  
  private static class StaticEnumConvertCache<TEnum> where TEnum : System.Enum
  {
    public static Func<TEnum, int> ConvertFunc = GenerateConvertFunc<TEnum>();
  }

  private static Func<TEnum, int> GenerateConvertFunc<TEnum>() where TEnum : System.Enum
  {
    var inputParameter = Expression.Parameter(typeof(TEnum));
    var body = Expression.Convert(inputParameter, typeof(int)); // means: (int)input;
    var lambda = Expression.Lambda<Func<TEnum, int>>(body, inputParameter);
    var func = lambda.Compile();

    return func;
  }
}


