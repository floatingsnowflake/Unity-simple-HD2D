using System;
using TeamMingo.Toolbox.Runtime.Attributes;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  [Serializable]
  public abstract class ScreenTransitionCanvasBasedOptions : IScreenTransitionOptions
  {
    [TagSelector]
    public string cameraTag = "MainCamera";
    [SortingLayer] 
    public int sortingLayer;
    public int sortingOrder;
  }
}