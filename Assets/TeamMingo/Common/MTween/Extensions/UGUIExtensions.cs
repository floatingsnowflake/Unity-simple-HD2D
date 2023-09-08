using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.Common.MTween.Extensions
{
  public static class UGUIExtensions
  {
    public static TweenInstance TweenFade(this Graphic graphic, float target)
    {
      return graphic.TweenValue(target, (t) => t.color.a, (t, v, _) =>
      {
        var color = t.color;
        color.a = (float) v;
        t.color = color;
      });
    }
    
    public static TweenInstance TweenFade(this CanvasGroup graphic, float target)
    {
      return graphic.TweenValue(target, (t) => t.alpha, (t, v, _) =>
      {
        t.alpha = v;
      });
    }

    public static TweenInstance TweenAnchorPos(this RectTransform trans, Vector2 target)
    {
      return trans.TweenValue(target, (t) => t.anchoredPosition, (t, v, _) =>
      {
        t.anchoredPosition = v;
      });
    }
  }
}