using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  
  [ScreenTransitionAttribute(prefab = "")]
  public class ScreenTransitionFade : ScreenTransitionGeneric<ScreenTransitionFade.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("Fade")]
    public class Options : ScreenTransitionCanvasBasedOptions, IScreenTransitionOptions.IColorOptions, IScreenTransitionOptions.ISpriteOptions
    {
      public float duration = 1;
      public Color color = Color.black;
      public EasingType ease = EasingType.Linear;
      public Sprite screenSprite;

      public Color Color => color;
      public Sprite Sprite => screenSprite;
    }

    protected override void PrepareEnter(Options options)
    {
      var graphic = GetComponent<Graphic>();
      var color = options.color;
      color.a = 0;
      graphic.color = color; 
    }

    protected override void PrepareExit(Options options)
    {
      var graphic = GetComponent<Graphic>();
      var color = options.color;
      color.a = 1;
      graphic.color = color; 
    }

    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      var graphic = GetComponent<Graphic>();
      graphic.ClearAllTween();
      yield return graphic.TweenFade(1)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }

    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      var graphic = GetComponent<Graphic>();
      graphic.ClearAllTween();
      yield return graphic.TweenFade(0)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }
  }
}