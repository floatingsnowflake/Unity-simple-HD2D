using System;
using System.Linq;
using UnityEngine;

namespace TeamMingo.Common.Runtime.Extensions
{
  public static class GameObjectExtensions
  {
    public static T[] QueryAll<T>(this GameObject obj) where T : class
    {
      var components = obj.GetComponents<MonoBehaviour>();
      return components.OfType<T>().ToArray();
    }
    
    public static MonoBehaviour[] QueryAll(this GameObject obj, Type type)
    {
      var components = obj.GetComponents<MonoBehaviour>();
      return components.Where(_ => _.GetType().IsAssignableFrom(type)).ToArray();
    }

    public static T EnsureComponent<T>(this GameObject obj) where T : Component
    {
      if (!obj.TryGetComponent(out T component))
      {
        component = obj.AddComponent<T>();
      }

      return component;
    }
  }
}