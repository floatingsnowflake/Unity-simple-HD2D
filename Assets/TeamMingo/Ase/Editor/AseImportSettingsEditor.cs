using UnityEditor;
using UnityEngine;

namespace TeamMingo.Ase.Editor
{
  public abstract class AseImportSettingsEditor : PropertyDrawer
  {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      var overridesProp = property.FindPropertyRelative("overrides");
      var overrides = (EAseImportOverrides) overridesProp.intValue;
      var lineCount = 1;
      if (overrides.HasFlag(EAseImportOverrides.Pivot))
      {
        lineCount++;
      }
      if (overrides.HasFlag(EAseImportOverrides.PixelsPerUnit))
      {
        lineCount++;
      }
      return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1) + GetSubSettingsHeight(property);
    }

    protected abstract float GetSubSettingsHeight(SerializedProperty property);

    protected Rect GetLineRect(Rect position, int line)
    {
      return new Rect(position.x,
        position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * line,
        position.width, EditorGUIUtility.singleLineHeight);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.BeginProperty(position, label, property);
      
      OnSubSettingsInspectorGUI(position, property, label);

      var rect = new Rect(position.x, position.y + GetSubSettingsHeight(property), position.width, position.height);
        
      var overridesProp = property.FindPropertyRelative("overrides");
      var overrides = (EAseImportOverrides) overridesProp.intValue;
      EditorGUI.PropertyField(GetLineRect(rect, 0), overridesProp);
      if (overrides.HasFlag(EAseImportOverrides.PixelsPerUnit))
      {
        EditorGUI.PropertyField(GetLineRect(rect, 1), property.FindPropertyRelative("pixelsPerUnit"));
      }
      if (overrides.HasFlag(EAseImportOverrides.Pivot))
      {
        EditorGUI.PropertyField(GetLineRect(rect, 2), property.FindPropertyRelative("pivot"));
      }
      
      EditorGUI.EndProperty();
    }

    protected abstract void OnSubSettingsInspectorGUI(Rect position, SerializedProperty property, GUIContent label);
  }
}