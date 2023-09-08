using System;
using TeamMingo.Ase.Editor.Processors;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamMingo.Ase.Editor.Settings
{
  [Serializable]
  [AseProcessor(typeof(TilesetImportProcessor))]
  public class TilesetImportSettings : AseImportSettings
  {
    [AseSelector(AseSelectableData.Tileset)]
    public string tileset;
    public bool generateTiles;
    public Color tileColor = Color.white;
    public Tile.ColliderType tileColliderType = Tile.ColliderType.Sprite;
  }

  [CustomPropertyDrawer(typeof(TilesetImportSettings))]
  public class TilesetImportSettingsEditor : AseImportSettingsEditor
  {
    protected override float GetSubSettingsHeight(SerializedProperty property)
    {
      var generateTilesProp = property.FindPropertyRelative("generateTiles");
      var lineCount = 2;
      if (generateTilesProp.boolValue)
      {
        lineCount = 4;
      }
      return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * lineCount;
    }

    protected override void OnSubSettingsInspectorGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EditorGUI.PropertyField(GetLineRect(position, 0), property.FindPropertyRelative("tileset"));

      var generateTilesProp = property.FindPropertyRelative("generateTiles");
      EditorGUI.PropertyField(GetLineRect(position, 1), generateTilesProp);

      if (generateTilesProp.boolValue)
      {
        EditorGUI.PropertyField(GetLineRect(position, 2), property.FindPropertyRelative("tileColor"));
        EditorGUI.PropertyField(GetLineRect(position, 3), property.FindPropertyRelative("tileColliderType"));
      }
    }
  }
}