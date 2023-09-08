using System;
using TeamMingo.Ase.Editor.Processors;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Ase.Editor.Settings
{
  [Serializable]
  [AseProcessor(typeof(AnimationImportProcessor))]
  public class AnimationImportSettings : AseImportSettings
  {
    [AseSelector(AseSelectableData.Tags)]
    public string tag;
    public bool loop;
  }
  
  [CustomPropertyDrawer(typeof(AnimationImportSettings))]
  public class AnimationImportSettingsEditor : AseImportSettingsEditor
  {
    protected override float GetSubSettingsHeight(SerializedProperty property)
    {
      return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
    }

    protected override void OnSubSettingsInspectorGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.PropertyField(GetLineRect(position, 0), property.FindPropertyRelative("tag"));
      EditorGUI.PropertyField(GetLineRect(position, 1), property.FindPropertyRelative("loop"));
    }
  }
}