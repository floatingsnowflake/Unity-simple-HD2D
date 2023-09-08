using System;
using TeamMingo.Common.MTween.Wrappers;
using TeamMingo.MTween;
using TeamMingo.Tween;
using UnityEngine;

namespace TeamMingo.Common.MTween.Extensions
{
  public static class ComponentExtensions
  {
    public static void ClearAllTween(this Component component)
    {
      var behaviours = component.GetComponents<TweenBehaviour>();
      if (behaviours != null && behaviours.Length > 0)
      {
        foreach (var tweenBehaviour in behaviours)
        {
          UnityEngine.Object.Destroy(tweenBehaviour);
        }
      }
      GlobalTweenBehaviour.Instance.ClearAllTween(component);
    }
    
    public static TweenInstance TweenKeyedValue<T, V>(this T component, string key, V target, Func<T, string, V> getter, Action<T, string, V, V> setter) where T : Component
    {
      var tweenInstance = TweenInstanceSimple.Create<T, V>(component,  getter, setter);
      var tweenBehaviour = component.gameObject.AddComponent<TweenBehaviour>();
      tweenBehaviour.SetTween(tweenInstance);
      return tweenInstance.To(key, target);
    }
    
    public static TweenInstance TweenValue<T, V>(this T component, V target, Func<T, V> getter, Action<T, V, V> setter) where T : Component
    {
      var tweenInstance = TweenInstanceSimple.Create<T, V>(component, ((c, s) => getter(c)), (c, s, arg3, arg4) => setter(c, arg3, arg4));
      var tweenBehaviour = component.gameObject.AddComponent<TweenBehaviour>();
      tweenBehaviour.SetTween(tweenInstance);
      return tweenInstance.To(TweenInstance.CreateKey(), target);
    }
  }
}