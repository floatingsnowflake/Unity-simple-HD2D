using System.Collections.Generic;
using TeamMingo.Toolbox.Editor.Core;
using TeamMingo.Toolbox.Runtime.Attributes;
using UnityEditor;
using UnityEditorInternal;

namespace TeamMingo.Toolbox.Editor.PropertyDrawers
{
  [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
  public class TagSelectorPropertyDrawer : SimpleSelectorPropertyDrawer
  {
    protected override bool HasDefault => false;
    protected override void Fill(List<string> list)
    {
      list.AddRange(InternalEditorUtility.tags);
    }
  }
}