using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public interface IScreenTransitionOptions
  {
    public interface ISpriteOptions
    {
      public Sprite Sprite { get; }
    }

    public interface IColorOptions
    {
      public Color Color { get; }
    }
    
  }
  
  public enum EDirection
  {
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4
  }
  
}