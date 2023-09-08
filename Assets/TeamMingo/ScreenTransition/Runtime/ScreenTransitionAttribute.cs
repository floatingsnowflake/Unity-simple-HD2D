using System;

namespace TeamMingo.ScreenTransition.Runtime
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ScreenTransitionAttribute : Attribute
  {
    public string prefab;
  }
}