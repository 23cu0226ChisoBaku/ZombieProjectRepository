/*

PoolItem.cs

Author : MAI ZHICONG(バクチソウ)

Description : オブジェクトプールアイテム

Update History: 2024/05/27 作成
                2025/03/04 整理整頓

Version : alpha_1.0.0

Encoding : UTF-8 

*/

using System;

namespace MLibrary.Pool
{
  public abstract class PoolItem<T> : IDisposable where T : class,new()
  {
    public T Item => _poolObject;
    protected T _poolObject;                                // プールアイテム
    private Action<PoolItem<T>> _recycleCallback;           // リサイクルコールバック

    public PoolItem(T obj)
    {
      _poolObject = obj;
      _recycleCallback = null;
    }
    ~PoolItem()
    {
      Dispose(false);
    }

    /// <summary>
    /// アイテムをアロケートする時呼び出されるコールバック
    /// </summary>
    public void OnAllocate()
    {
      InitItem();
    }

    /// <summary>
    /// アイテムをリサイクル
    /// </summary>
    public void Recycle()
    {
      OnRecycle();
      _recycleCallback?.Invoke(this);
    }

    /// <summary>
    /// リサイクルコールバックを設定する
    /// </summary>
    /// <param name="callback"></param>
    public void SetRecycleCallback(Action<PoolItem<T>> callback)
    {
      _recycleCallback = callback;
    }

    /// <summary>
    /// プールアイテムを破棄する
    /// 外で呼び出さないで
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }

    /// <summary>
    /// ***Virtual***
    /// プールアイテムを初期化する
    /// </summary>
    protected virtual void InitItem(){}
    /// <summary>
    /// ***Virtual***
    /// 回収される時呼び出されるコールバック
    /// </summary>
    protected virtual void OnRecycle() {}

    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        GC.SuppressFinalize(this);
      }

      _recycleCallback = null;
    }
  }
}
