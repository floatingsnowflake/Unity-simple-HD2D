using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.ScreenTransition.Runtime.Utils
{
  public class RandomSpriteSetter : ScreenTransitionOptionApplier
  {
    public Sprite[] sprites;
    
    public override void ApplyOptions(EScreenTransitionAction action, IScreenTransitionOptions options)
    {
      if (action == EScreenTransitionAction.Enter)
      {
        var sprite = sprites[Random.Range(0, sprites.Length)];
        if (target.TryGetComponent(out Image image))
        {
          image.sprite = sprite;
        }

        if (target.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
          spriteRenderer.sprite = sprite;
        }
      }
      
    }
  }
}