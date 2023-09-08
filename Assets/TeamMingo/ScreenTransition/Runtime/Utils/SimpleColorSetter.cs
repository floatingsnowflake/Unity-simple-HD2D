using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Utils
{
  public class SimpleColorSetter : ScreenTransitionOptionApplier
  {
    public override void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options)
    {
      if (options is not IScreenTransitionOptions.IColorOptions colorOptions) return;
      var color = colorOptions.Color;
      if (target.TryGetComponent(out Graphic graphic))
      {
        graphic.color = new Color(color.r, color.g, color.b, graphic.color.a);
      }
      else if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
      {
        spriteRenderer.color = new Color(color.r, color.g, color.b, spriteRenderer.color.a);
      }
    }
  }
}