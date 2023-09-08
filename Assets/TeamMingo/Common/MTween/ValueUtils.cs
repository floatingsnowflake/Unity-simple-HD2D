using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TeamMingo.MTween {

  public static class ValueUtils {

    public static Type GetValueType(object value) {
      var type = value.GetType();
      if (type == typeof(float) || type == typeof(double) || type == typeof(int)) {
        return typeof(float);
      } else {
        return type;
      }
    }

    public static float Plus(float a, float b) {
      return a + b;
    }

    public static float Subtract(float a, float b) {
      return a - b;
    }

    public static float Easing(float valueStart, float valueEnd, float easingValue) {
      return valueStart + (valueEnd - valueStart) * easingValue;
    }
  }

}