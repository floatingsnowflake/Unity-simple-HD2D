using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime.Utils
{
  public class SortingSetter : ScreenTransitionOptionApplier
  {
    public override void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options)
    {
      if (options is ScreenTransitionCanvasBasedOptions canvasOptions)
      {
        if (target.TryGetComponent(out Canvas canvas))
        {
          canvas.sortingLayerID = canvasOptions.sortingLayer;
          canvas.sortingOrder = canvasOptions.sortingOrder;
        }
        else if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
          spriteRenderer.sortingLayerID = canvasOptions.sortingLayer;
          spriteRenderer.sortingOrder = canvasOptions.sortingOrder;
        }
      }
    }
  }
}