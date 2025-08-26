using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Pool
{
    // プールクラス
    public class ZPool<T> : IZPool<T> where T : class,new()
    {
        
        protected struct Element
        {
            internal PoolItem<T> Value;
        } 
        public int Size => _size;
        public int Capacity => _capacity;

        // プールオブジェクト最大値（上限）
        private static readonly int MAX_POOL_CAPACITY = 1000;
        // プールオブジェクトデフォルト値（不正値が入った場合入れ替わる）
        private static readonly int DEFAULT_POOL_CAPACITY = 20;
        // スレッドセーフ用
        private readonly object padlock = new object();
        protected Element[] _pool;// オブジェクト容器

        private int _poolAllocateIndex = 0;
        private int _poolRecycleIndex = 0;
        private int _size = 0;
        private int _capacity = 0;

        private bool _isDisposed;
    
        public ZPool(int capacity) 
        {
            // 負の値はデフォルトサイズ
            if (capacity < 0)
            {
                _capacity = DEFAULT_POOL_CAPACITY;
            }
            // Maxを超えたらマックスサイズ
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

        ~ZPool()
        {
            Dispose(false);
        }
        public virtual void InitPoolObject(Func<PoolItem<T>> factory)
        {
            
            // factoryはnullじゃないとエラーメッセージを出力
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
            
            // PoolItemを生成
            for (int i = 0; i < _capacity; i++)
            {
                _pool[i].Value = factory.Invoke();
                _pool[i].Value.SetRecycleCallback(Recycle);
            }

            _size = _capacity;
            _poolAllocateIndex = 0;
            _poolRecycleIndex = 0;
        }

        public PoolItem<T> Allocate()
        {
            lock(padlock)
            {
                // Poolが空っぽ
                if(_size == 0)
                {
                    if(Application.isEditor)
                    {
                        Debug.LogWarning("Can't Allocate PoolItem");
                    }
                    return null;
                }
                else
                {
                    int allocIndex = _poolAllocateIndex;
                    _poolAllocateIndex = (_poolAllocateIndex + 1) % _capacity;
                    _pool[allocIndex].Value.OnAllocate();
                    --_size;

                    return _pool[allocIndex].Value;
                }
            }
        }

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
