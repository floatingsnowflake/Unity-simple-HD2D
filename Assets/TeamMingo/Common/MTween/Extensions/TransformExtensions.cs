using TeamMingo.MTween;
using UnityEngine;
using UnityEngine.UI;

namespace TeamMingo.Common.MTween.Extensions
{
  public static class TransformExtensions
  {
    public static TweenInstance TweenLocalScale(this RectTransform trans, Vector3 target)
    {
      return trans.TweenValue(target, (t) => t.localScale, (t, v, _) =>
      {
        t.localScale = v;
      });
    }
  }
}