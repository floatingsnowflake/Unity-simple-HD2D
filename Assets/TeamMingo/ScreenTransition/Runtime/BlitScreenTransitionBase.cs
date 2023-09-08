using System.Collections;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime
{
  public abstract class BlitScreenTransitionBase<T> : ScreenTransitionGeneric<T> where T : IScreenTransitionOptions
  {
    public Material blitMaterial;
    
    protected override IEnumerator ProcessEnter(IScreenTransitionOptions options)
    {
      ScreenTransitionSettings.Get().SetMaterial(blitMaterial);
      return base.ProcessEnter(options);
    }

    protected override IEnumerator ProcessExit(IScreenTransitionOptions options)
    {
      ScreenTransitionSettings.Get().SetMaterial(blitMaterial);
      yield return base.ProcessExit(options);
      ScreenTransitionSettings.Get().SetMaterial(null);
    }
  }
}