using TeamMingo.Common.MTween.Wrappers;
using TeamMingo.MTween;
using UnityEngine;

namespace TeamMingo.Common.MTween.Extensions
{
  public static class MaterialExtensions
  {
    public static void ClearAllTween(this Material material) {
      GlobalTweenBehaviour.Instance.ClearAllTween(material);
    }

    public static TweenInstance TweenFloat(this Material material, string key, float target)
    {
      var tweenInstance = TweenInstanceSimple.Create<Material, float>(material, ((t, s) => t.GetFloat(s)),
        (t, s, v, d) => t.SetFloat(s, v));
      GlobalTweenBehaviour.Instance.AddTween(tweenInstance);
      return tweenInstance.To(key, target);
    }
  }
}