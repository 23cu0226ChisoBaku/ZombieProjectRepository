/*

Pool.cs

Author : MAI ZHICONG(バクチソウ)

Description : オブジェクトプール

Update History: 2024/05/27 作成
                2025/03/04 整理整頓

Version : alpha_1.0.0

Encoding : UTF-8 

*/

using System;
using UnityEngine;

namespace MLibrary.Pool
{
  /// <summary>
  /// オブジェクトプール
  /// </summary>
  /// <typeparam name="T">プールアイテム型</typeparam>
  public class Pool<T> : IDisposable where T : class, new()
  {
    // GC回避構造体
    protected struct Element
    {
      internal PoolItem<T> Value;
    } 
    public int Size 
    {
      get => _size;
    }
    public int Capacity
    {
      get => _capacity;
    } 

    private static readonly int MAX_POOL_CAPACITY;              // プールアイテム上限
    private static readonly int DEFAULT_POOL_CAPACITY;          // プールアイテムデフォルト値
    private readonly object padlock = new object();             // 排他ロックオブジェクト
    private int _size = 0;
    private int _capacity = 0;
    private int _poolAllocateIndex = 0;
    private int _poolRecycleIndex = 0;
    private bool _isDisposed;
    protected Element[] _pool;                                  // プールアイテムバッファ

    static Pool()
    {
      MAX_POOL_CAPACITY = 1000;
      DEFAULT_POOL_CAPACITY = 20;
    }
    public Pool(int capacity) 
    {
      // 無効の初期値だとデフォルト値で初期化
      if (capacity < 0)
      {
        _capacity = DEFAULT_POOL_CAPACITY;
      }
      // 最大値を越えないようにする
      else if (capacity > MAX_POOL_CAPACITY) 
      {
        _capacity = MAX_POOL_CAPACITY;
      }
      else
      {
        _capacity = capacity;
      }

      _isDisposed = false;
    }

    ~Pool()
    {
      Dispose(false);
    }

    /// <summary>
    /// ***Virtual***
    /// プールを初期化
    /// </summary>
    public virtual void InitPoolObject(Func<PoolItem<T>> factory)
    {
      if(factory == null)
      {
        if(Application.isEditor)
        {
          Debug.LogError("Can't Init Pool");
          Debug.LogError("Factory is null");
        }
        
        return;
      }

      _pool = new Element[_capacity];
      
      // PoolItem初期化
      for (int i = 0; i < _capacity; i++)
      {
        _pool[i].Value = factory.Invoke();
        _pool[i].Value.SetRecycleCallback(Recycle);
      }

      _size = _capacity;
      _poolAllocateIndex = 0;
      _poolRecycleIndex = 0;
    }

    /// <summary>
    /// プールアイテムを取得
    /// </summary>
    /// <returns>プールアイテム（初期化されていない場合nullを返す）</returns>
    public PoolItem<T> Allocate()
    {
      lock(padlock)
      {
        if(_size == 0)
        {
          if(Application.isEditor)
          {
            Debug.LogWarning("Can't Allocate PoolItem");
          }
          return null;
        }
      
        int allocIndex = _poolAllocateIndex;
        _poolAllocateIndex = (_poolAllocateIndex + 1) % _capacity;
        _pool[allocIndex].Value.OnAllocate();
        --_size;

        return _pool[allocIndex].Value;
      }
    }

    /// <summary>
    /// プールアイテムをリサイクル
    /// </summary>
    /// <param name="item">プールアイテム</param>
    private void Recycle(PoolItem<T> item) 
    {
      lock(padlock)
      {
        if(_isDisposed)
        {
          item.Dispose();
          return;
        }

        if (_size == _capacity)
        {
          item.Dispose();
          return;
        }

        _pool[_poolRecycleIndex].Value = item;
        _poolRecycleIndex = (_poolRecycleIndex + 1) % _capacity;
        ++_size;
      }
    }
    public void Dispose()
    {
      Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if(disposing)
      {
        GC.SuppressFinalize(this);
      }

      foreach(var item in _pool)
      {
        item.Value.Dispose();
      }

      _isDisposed = true;
    }
  }
}
