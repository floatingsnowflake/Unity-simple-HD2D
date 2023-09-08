using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TeamMingo.Toolbox.Editor.Core
{
  public abstract class SimpleSelectorPropertyDrawer : PropertyDrawer
  {
    protected abstract bool HasDefault { get; }
    protected abstract void Fill(List<string> list);
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var list = new List<string>();
      
      Fill(list);
      
      var value = property.stringValue;
      var index = -1;
      var start = HasDefault ? 1 : 0;

      if (string.IsNullOrEmpty(value))
      {
        index = HasDefault ? 0 : -1;
      }
      else
      {
        for (int i = start; i < list.Count; i++)
        {
          if (list[i] == value)
          {
            index = i;
            break;
          }
        }
      }

      var labels = Array.ConvertAll(list.ToArray(), i => new GUIContent(i));
      label = EditorGUI.BeginProperty(position, label, property);
      EditorGUI.BeginChangeCheck();
      index = EditorGUI.Popup(position, label, index, labels);
      if (EditorGUI.EndChangeCheck())
      {
        property.stringValue = index >= start ? list[index] : string.Empty;
      }

      EditorGUI.EndProperty();
    }
  }
}