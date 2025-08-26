/*

Singleton.cs

Author : MAI ZHICONG(バクチソウ)

Description : シングルトンパターン（MonoBehaviourと一般クラス対応済み）

Update History: 2024/02/28 作成
                2025/03/04 整理整頓

Version : alpha_1.0.0

Encoding : UTF-8 

*/

using UnityEngine;
using UnityEngine.Assertions;

namespace MLibrary
{
  /**
  *  @brief MonoBehaviourシングルトン
  *  @param T シングルトンにしたいMonoBehaviourクラス
  */
  public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static volatile T _instance;                    // インスタンス
    private static readonly object padlock = new object();  // 排他ロックオブジェクト
    private static bool _isApplicationQuitting = false;     // アプリを閉じた（またはエディタプレイを止めた）かを表すフラグ
    public static T Instance
    {
      get
      {
        if (_isApplicationQuitting)
        {
          return null;
        }

        if (_instance == null)
        {
          _instance = FindObjectOfType<T>();

          // シーンに見つからなかったら新しく作る
          if (_instance == null)
          {
            lock(padlock)
            {
              GameObject singleton = new GameObject(typeof(T).Name);
              _instance = singleton.AddComponent<T>();

              // シングルトンのため、シーン切り替えで破壊されないようにする
              DontDestroyOnLoad(singleton);
            }
          }
        }

        return _instance;
      }
    }

    protected virtual void Awake()
    {
      if (_instance == null)
      {
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        // シングルトンが既に作られたため、以降作られたオブジェクトを削除
        Destroy(gameObject);
      }
    }

    private void OnApplicationQuit() 
    {
      _isApplicationQuitting = true;
    }

    protected SingletonMono() {}
  }

  /**
  *  @brief シングルトン
  *  @param T シングルトンにしたいクラス
  */
  public abstract class Singleton<T> where T : class, new()
  {
    private static volatile T _instance;                    // インスタンス
    private static readonly object padlock = new object();  // 排他ロックオブジェクト
    public static T Instance
    {
      get
      {
        if (_instance == null)
        {
          // 排他ロック
          lock (padlock)
          {
            if (_instance == null)
            {
              _instance = new T();
            }
          }
        }

        return _instance;
      }
    }

    protected Singleton() 
    {
      // シングルトンが存在したため、二回以降作られたらアサーションを吐き出す
      Assert.IsNull(_instance);
    }
  }
}
