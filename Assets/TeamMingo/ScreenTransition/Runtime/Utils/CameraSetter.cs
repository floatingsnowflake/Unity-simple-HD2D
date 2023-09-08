using TeamMingo.Common.Runtime.Extensions;
using UnityEngine;

namespace TeamMingo.ScreenTransition.Runtime.Utils
{
  public class CameraSetter : ScreenTransitionOptionApplier
  {
    public override void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options)
    {
      if (options is not ScreenTransitionCanvasBasedOptions canvasOptions) return;
      if (TryGetComponent(out Canvas canvas))
      {
        if (canvasOptions.cameraTag.IsNullOrWhitespace())
        {
          canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
          canvas.renderMode = RenderMode.ScreenSpaceCamera;
          canvas.worldCamera = GameObject.FindWithTag(canvasOptions.cameraTag).GetComponent<Camera>();
        }
      }
    }
  }
}