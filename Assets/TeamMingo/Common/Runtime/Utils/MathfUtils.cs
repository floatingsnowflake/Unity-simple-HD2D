using System;
using UnityEngine;

namespace TeamMingo.Common.Runtime.Utils
{
  public static class MathfUtils
  {
    public static float precision = 0.001f;

    public static bool IsZero(float test)
    {
      return Mathf.Abs(test) < precision;
    }
    
    public static bool IsZero(double test)
    {
      return Math.Abs(test) < precision;
    }
    
    public static bool Greater(float test, float target)
    {
      return test - target > precision;
    }

    public static bool Less(float test, float target)
    {
      return target - test > precision;
    }
    
  }
}