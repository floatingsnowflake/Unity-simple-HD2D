using System;
using UnityEngine;

namespace TeamMingo.Input.Runtime
{
  [Flags]
  public enum InputDirection
  {
    None = 0,
    Up = 1, 
    Down = 2, 
    Left = 4, 
    Right = 8,
    LeftUp = 16,
    RightUp = 32,
    LeftDown = 64, 
    RightDown = 128,
    Everything = ~0
  }

  public static class InputDirectionExtensions
  {
    private static Vector2[] _directions = new Vector2[]
    {
      Vector2.up, Vector2.down, Vector2.left, Vector2.right,
      new(-1, 1), new(1, 1),
      new(-1, -1), new(1, -1)
    };
    
    public static Vector2 ToVector2(this InputDirection direction)
    {
      return _directions[(int) direction];
    }
    
    public static InputDirection Parse(Vector2 value)
    {
      var isZeroX = value.x == 0;
      var isZeroY = value.y == 0;

      if (!isZeroX || !isZeroY)
      {
        var index = 0;
        var angle = 180f;
        for (var i = 0; i < _directions.Length; i++)
        {
          var dir = _directions[i];
          var dirAngle = Vector2.Angle(value, dir);
          if (angle >= dirAngle)
          {
            angle = dirAngle;
            index = i;
          }
        }
        return (InputDirection) Enum.GetValues(typeof(InputDirection)).GetValue(index + 1);
      }

      return InputDirection.None;
    }
  }
}