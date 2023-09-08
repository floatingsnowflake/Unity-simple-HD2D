using System;
using System.Globalization;
using UnityEngine;

namespace TeamMingo.Common.Runtime.Utils
{
  public static class TimeUtils {

    public static long NowMillis() {
      return (long) DateTimeOffset.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public static long NowSeconds() {
      return (long) DateTimeOffset.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static string Now()
    {
      return DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture);
    }

    public static long GameMillis()
    {
      return Mathf.FloorToInt(Time.time * 1000);
    }

    public static long GameSeconds()
    {
      return Mathf.FloorToInt(Time.time);
    }

  }
}