using System;
using System.Linq;
using TeamMingo.Ase.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Ase.Editor
{
  public enum AseSelectableData
  {
    Layers,
    Tags,
    Tileset,
  }

  public class AseSelectorAttribute : PropertyAttribute
  {
    public readonly AseSelectableData Data;

    public AseSelectorAttribute(AseSelectableData data)
    {
      Data = data;
    }
  }

  [CustomPropertyDrawer(typeof(AseSelectorAttribute))]
  public class AseSelectorPropertyDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      
      EditorGUI.BeginProperty(position, label, property);
      var doc = AseImporterEditor.CurrentDocument;
      
      if (doc == null)
      {
        EditorGUI.LabelField(position, "Aseprite document is null");
        EditorGUI.EndProperty();
        return;
      }

      var items = new string[0];

      var attrib = (AseSelectorAttribute) attribute;
      switch (attrib.Data)
      {
        case AseSelectableData.Layers: items = doc.Layers.Select(_ => _.Name).ToArray(); break;
        case AseSelectableData.Tags: items = doc.Tags.Select(_ => _.Name).ToArray(); break;
        case AseSelectableData.Tileset: items = doc.Tilesets.Select(_ => _.Name).ToArray(); break;
      }
      
      var index = Array.IndexOf(items, property.stringValue);

      var newIndex = EditorGUI.Popup(position, label.text, index, items);
      if (index != newIndex)
      {
        if (newIndex >= 0 && newIndex < items.Length)
        {
          property.stringValue = items[newIndex];
        }
      }
      EditorGUI.EndProperty();
    }
  }
}