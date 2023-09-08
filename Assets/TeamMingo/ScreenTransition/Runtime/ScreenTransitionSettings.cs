using System;
using System.Linq;
using TeamMingo.Configs.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TeamMingo.ScreenTransition.Runtime
{
  [CreateAssetMenu(menuName = "Team Mingo/Screen Transition/Settings", fileName = "Screen Transition Settings")]
  [Configuration("ScreenTransition", field: "screenTransitionSettings")]
  public class ScreenTransitionSettings : ScriptableObject
  {
    public static ScreenTransitionSettings Get()
    {
      return (ScreenTransitionSettings) Configuration.Get().screenTransitionSettings;
    }
    
    [Serializable]
    public class TransitionItem
    {
      public string name;
      public GameObject overridePrefab;
      [SerializeReferenceDropdown]
      [SerializeReference]
      public IScreenTransitionOptions options;
    }

    public ScriptableRendererData rendererData;

    public TransitionItem[] transitions;

    public TransitionItem GetTransition(string transitionName)
    {
      return transitions.FirstOrDefault(_ => _.name == transitionName);
    }

    public void SetMaterial(Material material)
    {
      if (rendererData)
      {
        var rendererFeature = rendererData.rendererFeatures.FirstOrDefault(_ => _ is ScreenTransitionRendererFeature);
        if (rendererFeature is ScreenTransitionRendererFeature screenTransitionRendererFeature)
        {
          screenTransitionRendererFeature.settings.blitMaterial = material;
        }
      }
    }
  }
}