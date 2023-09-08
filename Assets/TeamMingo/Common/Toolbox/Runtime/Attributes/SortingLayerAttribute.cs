using System;
using UnityEngine;

namespace TeamMingo.Toolbox.Runtime.Attributes
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public sealed class SortingLayerAttribute : PropertyAttribute {}

}