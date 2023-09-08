using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public interface IValueHandler {

    Type HandleType { get; }
    
    object DoPlus(object a, object b);
    object DoSubtract(object a, object b);
    object DoEasing(object start, object end, float easingValue);

  }

  public abstract class ValueHandler<T> : IValueHandler {

    public abstract Type HandleType { get; }

    public virtual object DoPlus(object a, object b) {
      T newA = (T) Convert.ChangeType(a, typeof(T));
      T newB = (T) Convert.ChangeType(b, typeof(T));
      return Plus(newA, newB);
    }

    public virtual object DoSubtract(object a, object b) {
      T newA = (T) Convert.ChangeType(a, typeof(T));
      T newB = (T) Convert.ChangeType(b, typeof(T));
      return Subtract(newA, newB);
    }

    public object DoEasing(object start, object end, float easingValue) {
      T newStart = (T) Convert.ChangeType(start, typeof(T));
      T newEnd = (T) Convert.ChangeType(end, typeof(T));
      return Easing(newStart, newEnd, easingValue);
    }

    public abstract T Plus(T a, T b);
    public abstract T Subtract(T a, T b);
    public abstract T Easing(T start, T end, float easingValue);

  }

  public static class ValueHandlers
  {
    
    internal static readonly ValueHandlerTable valueHandlerTable = new ValueHandlerTable();

    static ValueHandlers()
    {
      valueHandlerTable.AddValueHandler(new FloatValueHandler());
      valueHandlerTable.AddValueHandler(new Vector3ValueHandler());
      valueHandlerTable.AddValueHandler(new Vector2ValueHandler());
      valueHandlerTable.AddValueHandler(new ColorValueHandler());
    }

    internal class ValueHandlerTable : Dictionary<Type, IValueHandler> {
      
      internal void AddValueHandler(IValueHandler valueHandler) {
        Type type = valueHandler.HandleType;
        if (ContainsKey(type)) {
          throw TweenException.ExistsValueHandlerForTheType(type);
        }
        Add(type, valueHandler);
      }

      internal IValueHandler GetValueHandler(Type type) {
        if (!ContainsKey(type)) {
          throw TweenException.UnhandledValueType(type);
        }
        return this[type];
      }

    }
  }

}