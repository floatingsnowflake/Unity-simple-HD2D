using System;
using UnityEngine;
using UnityEngine.UI;
using static TeamMingo.ScreenTransition.Runtime.IScreenTransitionOptions;

namespace TeamMingo.ScreenTransition.Runtime.Utils
{
  public class SimpleSpriteSetter : ScreenTransitionOptionApplier
  {

    public override void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options)
    {
      if (options is not ISpriteOptions spriteOptions) return;
      if (!spriteOptions.Sprite) return;
      if (target.TryGetComponent(out Image image))
      {
        image.sprite = spriteOptions.Sprite;
      }

      if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
      {
        spriteRenderer.sprite = spriteOptions.Sprite;
      }
    }
  }
}