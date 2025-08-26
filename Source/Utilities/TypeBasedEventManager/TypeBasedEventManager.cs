using System.Collections.Generic;
using UnityEngine;
using System;
using MLibrary;

public interface IEventManager
{
    void Send<T>() where T : new();
    void Send<T>(T entity);
    IUnregister RegisterEvent<T>(Action<T> onEvent);
    void UnregisterEvent<T>(Action<T> onEvent);
}
public class TypeBasedEventManager : Singleton<TypeBasedEventManager>, IEventManager
{
    private interface EventMarkerInterface {}
    private class OnEventEntity<T> : EventMarkerInterface
    {
      public Action<T> OnEvent = entity => { };
    }
    private Dictionary<Type, EventMarkerInterface> _eventsContainer = new Dictionary<Type, EventMarkerInterface>();

    public void Send<T>() where T : new()
    {
      T eventEntity = new T();
      Send(eventEntity);
    }

    public void Send<T>(T entity)
    {
      Type eventType = typeof(T);
      if (_eventsContainer.TryGetValue(eventType, out EventMarkerInterface myOnEvent))
      {
        (myOnEvent as OnEventEntity<T>).OnEvent.Invoke(entity);
      }
      else
      {
        Debug.LogWarning("Event of '" + eventType.ToString() + "' hasn't register");
      }
    }

    public IUnregister RegisterEvent<T>(Action<T> onEvent)
    {
      var type = typeof(T);
      if(_eventsContainer.TryGetValue(type, out EventMarkerInterface eventMarker))
      {
        (eventMarker as OnEventEntity<T>).OnEvent += onEvent;
      }
      else
      {
        eventMarker = new OnEventEntity<T>{ OnEvent = onEvent };
        _eventsContainer.Add(type, eventMarker);
      }

      return new EventUnregister<T>()
      {
        OnEvent = onEvent,
        EventManager = this
      };
    }

    public void UnregisterEvent<T>(Action<T> onEvent)
    {
      var type = typeof(T);
      if (_eventsContainer.TryGetValue(type, out EventMarkerInterface eventMarker))
      {
        (eventMarker as OnEventEntity<T>).OnEvent -= onEvent;
      }
    }
}
