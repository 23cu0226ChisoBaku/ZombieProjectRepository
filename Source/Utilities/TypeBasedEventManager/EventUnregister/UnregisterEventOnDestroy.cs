using System.Collections.Generic;
using UnityEngine;

public class UnregisterEventOnDestroy : MonoBehaviour
{
  private HashSet<IUnregister> _unregisters = new HashSet<IUnregister>();

  public void AddUnregister(IUnregister unregister)
  {
    _unregisters.Add(unregister);
  }

  private void OnDestroy()
  {
    foreach (var unregister in _unregisters)
    {
      if (unregister != null)
      {
        unregister.Unregister();
      }
    }

    _unregisters.Clear();
  }
}

