using System;
using UnityEngine;
using MLibrary.Pool;

public sealed class SEPool : Pool<GameObject>
{
  private GameObject _seGroup;
  public SEPool(int capacity) : base(capacity)
  {
    _seGroup = new GameObject("SE_Group");
    _seGroup.SetActive(true);
    GameObject.DontDestroyOnLoad(_seGroup);
  }
  
  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);

    if(disposing)
    {
      GameObject.Destroy(_seGroup);
    }
  }
  public override void InitPoolObject(Func<PoolItem<GameObject>> factory)
  {
    // 初期化する
    base.InitPoolObject(factory);

    foreach(var element in _pool)
    {
      element.Value.Item.transform.SetParent(_seGroup.transform);
      element.Value.Item.SetActive(false);
    }
  }
}