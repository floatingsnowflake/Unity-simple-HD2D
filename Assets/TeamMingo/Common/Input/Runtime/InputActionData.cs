using System;
using UnityEngine;

namespace TeamMingo.Input.Runtime
{
  [Serializable]
  public class InputActionData
  {
    public string action;
    public EInputPhase phase = EInputPhase.Down;
    public Vector2 input;
    public float timestamp;
    public bool consumed;

    public bool IsActionDown(string action)
    {
      return this.action == action && phase == EInputPhase.Down;
    }
    
    public bool IsActionUp(string action)
    {
      return this.action == action && phase == EInputPhase.Up;
    }

    public bool IsPressing(bool defaults)
    {
      return phase switch
      {
        EInputPhase.Down => true,
        EInputPhase.Up => false,
        _ => defaults
      };
    }
  }
}