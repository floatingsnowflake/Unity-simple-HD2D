using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.Toolbox.Runtime.Attributes;
using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  public class ScreenTransitionMask : ScreenTransitionGeneric<ScreenTransitionMask.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("Mask")]
    public class Options : ScreenTransitionCanvasBasedOptions, IScreenTransitionOptions.ISpriteOptions, IScreenTransitionOptions.IColorOptions
    {
      public float duration = 1;
      public Color color = Color.black;
      public EasingType ease = EasingType.Linear;
      public Sprite sprite;
      public float scale = 20;
      public Sprite screenSprite;

      public bool enableTrack;
      [TagSelector]
      public string trackTag;
      
      public Color Color => color;
      public Sprite Sprite => sprite;
    }
    
    public Image hole;
    public Image screen;

    protected override void PrepareEnter(Options options)
    {
      hole.rectTransform.localScale = Vector3.one * options.scale;
      if (options.screenSprite)
      {
        screen.sprite = options.screenSprite;
      }
      if (options.enableTrack)
      {
        Track(hole.rectTransform, options.trackTag);
      }
    }
  
    protected override void PrepareExit(Options options)
    {
      hole.rectTransform.localScale = Vector3.zero;
      if (options.screenSprite)
      {
        screen.sprite = options.screenSprite;
      }
      if (options.enableTrack)
      {
        Track(hole.rectTransform, options.trackTag);
      }
    }
  
    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      hole.rectTransform.ClearAllTween();
      yield return hole.rectTransform.TweenLocalScale(Vector3.zero)
        .Duration(options.duration)
        .Easing(options.ease)
        .OnUpdate(() =>
        {
          if (options.enableTrack)
          {
            Track(hole.rectTransform, options.trackTag);
          }
        })
        .WaitForComplete();
      yield return null;
    }

    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      hole.rectTransform.ClearAllTween();
      yield return hole.rectTransform.TweenLocalScale(Vector3.one * options.scale)
        .Duration(options.duration)
        .Easing(options.ease)
        .OnUpdate(() =>
        {
          if (options.enableTrack)
          {
            Track(hole.rectTransform, options.trackTag);
          }
        })
        .WaitForComplete();
      yield return null;
    }
  }
}