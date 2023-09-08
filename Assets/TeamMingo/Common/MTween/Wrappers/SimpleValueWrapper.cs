using System;
using TeamMingo.MTween;

namespace TeamMingo.Common.MTween.Wrappers
{
  public class SimpleValueWrapper<T, V> : TweenInstance.ITargetWrapper
  {
    public object Target { get; private set; }

    private T _target;

    private Func<T, string, V> _getter;
    private Action<T, string, V, V> _setter;

    public SimpleValueWrapper(T target, Func<T, string, V> getter, Action<T, string, V, V> setter)
    {
      Target = target;
      _target = target;
      _getter = getter;
      _setter = setter;
    }
    
    public object GetValue(string key)
    {
      return _getter(_target, key);
    }

    public void SetValue(string key, object value, object delta)
    {
      _setter(_target, key, (V) value, (V) delta);
    }
  }
  
  

  public static class TweenInstanceSimple
  {
    public static TweenInstance Create<T, V>(T target, Func<T, string, V> getter, Action<T, string, V, V> setter)
    {
      return new TweenInstance(new SimpleValueWrapper<T, V>(target, getter, setter));
    }
  }
}