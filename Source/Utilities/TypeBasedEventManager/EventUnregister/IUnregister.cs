using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IUnregister
{
  void Unregister();
}

public struct EventUnregister<T> : IUnregister
{
  public Action<T> OnEvent;
  public IEventManager EventManager;
  void IUnregister.Unregister()
  {
    if (EventManager != null)
    {
      EventManager.UnregisterEvent(OnEvent);
    }
    EventManager = null;
    OnEvent = null;
  }
}

public static class UnregisterExtension
{
  public static void UnregisterEventOnDestroy(this IUnregister unregister, GameObject gameObject)
  {
    gameObject.GetOrAddComponent<UnregisterEventOnDestroy>().AddUnregister(unregister);
  }
}
