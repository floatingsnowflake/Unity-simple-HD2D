using System;
using System.Linq;
using TeamMingo.Ase.Editor.Processors;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Ase.Editor.Settings
{
  [Serializable]
  [AseProcessor(typeof(LayerImportProcessor))]
  public class LayerImportSettings : AseImportSettings
  {
    [AseSelector(AseSelectableData.Layers)]
    public string layer;
  }

  [CustomPropertyDrawer(typeof(LayerImportSettings))]
  public class LayerImportSettingsEditor : AseImportSettingsEditor
  {
    protected override float GetSubSettingsHeight(SerializedProperty property)
    {
      return EditorGUIUtility.singleLineHeight * 1 + EditorGUIUtility.standardVerticalSpacing * 1;
    }

    protected override void OnSubSettingsInspectorGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.PropertyField(GetLineRect(position, 0), property.FindPropertyRelative("layer"));
    }
  }
}