using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IFactory<T>
{
    T Create();
}
public class Factory<T>:IDisposable,IFactory<T> where T : class,new()
{
    private Func<T> _factoryFunc;

    public Factory(Func<T> factory)
    {
        _factoryFunc = factory;
    }

    ~Factory()
    {
        Dispose(false);
    }

    public void SetFactory(Func<T> factory)
    {
        _factoryFunc = factory;
    }

    public T Create()
    {
        if(_factoryFunc == null)
        {
            if(Application.isEditor)
            {
                Debug.Log("factoryFunc is not initialized"); 
            }
            return new T();
        }
        else
        {
            return _factoryFunc();
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
            
        }

        _factoryFunc = null;
    }

}
