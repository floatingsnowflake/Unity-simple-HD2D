using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  public class ScreenTransitionSlide : ScreenTransitionGeneric<ScreenTransitionSlide.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("Slide")]
    public class Options : ScreenTransitionCanvasBasedOptions, IScreenTransitionOptions.ISpriteOptions, IScreenTransitionOptions.IColorOptions
    {
      public float duration = 1;
      public Color color = Color.white;
      public EasingType ease = EasingType.Linear;
      public EDirection direction = EDirection.Left;
      public Sprite screenSprite;

      public Color Color => color;
      public Sprite Sprite => screenSprite;
    }
    
    public Image image;
    
    protected override void PrepareEnter(Options options)
    {
      var trans = image.rectTransform;
      var size = trans.rect.size;
      trans.anchoredPosition = GetAnchorPos(size, options);
    }
  
    protected override void PrepareExit(Options options)
    {
      var trans = image.GetComponent<RectTransform>();
      trans.anchoredPosition = new Vector2(0, 0);
    }
  
    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      image.rectTransform.ClearAllTween();
      yield return image.rectTransform.TweenAnchorPos(Vector2.zero)
        .Easing(options.ease)
        .Duration(options.duration)
        .WaitForComplete();
      yield return null;
    }
  
    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      image.rectTransform.ClearAllTween();
      var pos = GetAnchorPos(image.rectTransform.rect.size, options);
      yield return image.rectTransform.TweenAnchorPos(pos)
        .Easing(options.ease)
        .Duration(options.duration)
        .WaitForComplete();
      yield return null;
    }

    private Vector2 GetAnchorPos(Vector2 size, Options options)
    {
      switch (options.direction)
      {
        case EDirection.Right:
          return new Vector2(size.x, 0);
        case EDirection.Up:
          return new Vector2(0, size.y);
        case EDirection.Down:
          return new Vector2(0, -size.y);
        default:
          return new Vector2(-size.x, 0);
      }
    }
  }
}