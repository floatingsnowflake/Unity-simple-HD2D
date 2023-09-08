using System;
using System.Collections.Generic;
using System.Linq;
using TeamMingo.Configs.Runtime;
using TeamMingo.ScreenTransition.Runtime;
using TeamMingo.Toolbox.Editor.Core;
using UnityEditor;

namespace Mingo.I18n.Editor
{
  [CustomPropertyDrawer(typeof(ScreenTransitionSelectorAttribute))]
  public class ScreenTransitionSelectorPropertyDrawer : SimpleSelectorPropertyDrawer
  {
    protected override bool HasDefault => true;
    protected override void Fill(List<string> list)
    {
      var settings = (ScreenTransitionSettings) Configuration.Get().screenTransitionSettings;
      if (settings)
      {
        list.AddRange(settings.transitions.Select(_ => _.name));
      }
    }
  }
}