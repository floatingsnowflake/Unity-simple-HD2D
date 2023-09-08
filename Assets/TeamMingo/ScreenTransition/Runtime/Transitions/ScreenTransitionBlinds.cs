using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  public class ScreenTransitionBlinds : ScreenTransitionGeneric<ScreenTransitionBlinds.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("Blinds")]
    public class Options : ScreenTransitionCanvasBasedOptions, IScreenTransitionOptions.IColorOptions, IScreenTransitionOptions.ISpriteOptions
    {
      public float duration = 1;
      public EasingType ease = EasingType.Linear;
      public int blinds = 16;
      public EDirection direction = EDirection.Left;
      public Color color = Color.black;
      public Sprite screenSprite;
      
      public Color Color => color;
      public Sprite Sprite => screenSprite;
    }
      
    private static readonly int blindsProp = Shader.PropertyToID("_Blinds");
    private static readonly int stepProp = Shader.PropertyToID("_Step");
    private static readonly int directionProp = Shader.PropertyToID("_Direction");

    private Material _spriteMaterial;
    
    public Material SpriteMaterial
    {
      get
      {
        if (_spriteMaterial)
        {
          return _spriteMaterial;
        }
        if (TryGetComponent(out Image image))
        {
          _spriteMaterial = image.material;
        }
    
        else if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
          _spriteMaterial = spriteRenderer.material;
        }
    
        return _spriteMaterial;
      }
    }
    

    protected override void PrepareEnter(Options options)
    {
      SpriteMaterial.SetFloat(blindsProp, options.blinds);
      SpriteMaterial.SetFloat(stepProp, 1);
      SpriteMaterial.SetInt(directionProp, (int) options.direction);
    }

    protected override void PrepareExit(Options options)
    {
      SpriteMaterial.SetFloat(blindsProp, options.blinds);
      SpriteMaterial.SetFloat(stepProp, 0);
      SpriteMaterial.SetInt(directionProp, (int) options.direction);
    }

    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      SpriteMaterial.ClearAllTween();
      yield return SpriteMaterial.TweenFloat("_Step", 0)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }

    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      SpriteMaterial.ClearAllTween();
      yield return SpriteMaterial.TweenFloat("_Step", 1)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }
  }
}