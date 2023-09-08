using System;
using System.Collections;
using TeamMingo.Common.MTween.Extensions;
using TeamMingo.MTween;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime.Transitions
{
  public class ScreenTransitionBlitTextureStep : BlitScreenTransitionBase<ScreenTransitionBlitTextureStep.Options>
  {
    [Serializable]
    [SerializeReferenceDropdownName("BlitTextureStep")]
    public class Options : IScreenTransitionOptions
    {
      public float duration = 1;
      public EasingType ease = EasingType.Linear;
      [Min(0)]
      public float smoothing = 0.2f;

      public Color color = Color.white;
      public Texture2D transitionTexture;
      public Texture2D screenTexture;
      
    }
      
    private static readonly int stepProp = Shader.PropertyToID("_Step");
    private static readonly int smoothingProp = Shader.PropertyToID("_Smoothing");
    private static readonly int transitionTexProp = Shader.PropertyToID("_TransitionTex");
    private static readonly int screenTexProp = Shader.PropertyToID("_ScreenTex");
    private static readonly int colorProp = Shader.PropertyToID("_Color");

    protected override void PrepareEnter(Options options)
    {
      blitMaterial.SetFloat(smoothingProp, options.smoothing);
      blitMaterial.SetFloat(stepProp, 0);
      blitMaterial.SetTexture(transitionTexProp, options.transitionTexture);
      blitMaterial.SetTexture(screenTexProp, options.screenTexture);
      blitMaterial.SetColor(colorProp, options.color);
    }

    protected override void PrepareExit(Options options)
    {
      blitMaterial.SetFloat(smoothingProp, options.smoothing);
      blitMaterial.SetFloat(stepProp, 1);
      blitMaterial.SetTexture(transitionTexProp, options.transitionTexture);
      blitMaterial.SetTexture(screenTexProp, options.screenTexture);
      blitMaterial.SetColor(colorProp, options.color);
    }

    protected override IEnumerator ProcessEnter(Options options)
    {
      PrepareEnter(options);
      blitMaterial.ClearAllTween();
      yield return blitMaterial.TweenFloat("_Step", 1)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }

    protected override IEnumerator ProcessExit(Options options)
    {
      PrepareExit(options);
      blitMaterial.ClearAllTween();
      yield return blitMaterial.TweenFloat("_Step", 0)
        .Duration(options.duration)
        .Easing(options.ease)
        .WaitForComplete();
      yield return null;
    }
  }
}