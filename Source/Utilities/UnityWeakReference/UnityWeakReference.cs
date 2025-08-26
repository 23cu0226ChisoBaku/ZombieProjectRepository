using System;

namespace Unity_MTool
{
  public sealed class UnityWeakReference<T> : WeakReference where T : UnityEngine.Object
  {
    public UnityWeakReference(T target) : base(target) {}

    public override bool IsAlive
    {
      get
      {
        return Target != null;
      }
    }

    public new T Target 
    { 
      get => base.Target as T;
      set => base.Target = value; 
    }

    public bool TryGetTarget(out T target)
    {
      target = Target;
      return target != null;
    }
  }
}