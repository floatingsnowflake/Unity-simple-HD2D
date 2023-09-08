using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  public class ScreenTransitionTextureStep : ScreenTransitionGeneric<ScreenTransitionTextureStep.Options>
  {
    
    [Serializable]
    [SerializeReferenceDropdownName("TextureStep")]
    public class Options : ScreenTransitionCanvasBasedOptions, IScreenTransitionOptions.IColorOptions, IScreenTransitionOptions.ISpriteOptions
    {
      public float duration = 1;
      public EasingType ease = EasingType.Linear;
      public Color color = Color.black;
      [Min(0)]
      public float smoothing = 0.2f;
      public Sprite transitionSprite;
      public Texture2D screenTexture;
      
      public Color Color => color;
      public Sprite Sprite => transitionSprite;
    }
      
    private static readonly int stepProp = Shader.PropertyToID("_Step");
    private static readonly int smoothingProp = Shader.PropertyToID("_Smoothing");
    private static readonly int screenTexProp = Shader.PropertyToID("_ScreenTex");

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
      SpriteMaterial.SetFloat(smoothingProp, options.smoothing);
      SpriteMaterial.SetFloat(stepProp, 0);
      SpriteMaterial.SetTexture(screenTexProp, options.screenTexture);
    }

    protected override void PrepareExit(Options options)
    {
      SpriteMaterial.SetFloat(smoothingProp, options.smoothing);
      SpriteMaterial.SetFloat(stepProp, 1);
      SpriteMaterial.SetTexture(screenTexProp, options.screenTexture);
    }

    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      SpriteMaterial.ClearAllTween();
      yield return SpriteMaterial.TweenFloat("_Step", 1)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }

    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      SpriteMaterial.ClearAllTween();
      yield return SpriteMaterial.TweenFloat("_Step", 0)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }
  }
}