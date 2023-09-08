using System;
using System.IO;
using MonoGame.Aseprite.ContentPipeline.Models;
using MonoGame.Aseprite.ContentPipeline.Serialization;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace TeamMingo.Ase.Editor
{
  [CustomEditor(typeof(AseImporter))]
  public class AseImporterEditor : ScriptedImporterEditor
  {
    
    public static AsepriteDocument CurrentDocument { get; private set; }

    private AsepriteDocument _document;
    
    public override void OnEnable()
    {
      base.OnEnable();
      
      var asset = Selection.activeObject;
      if (asset)
      {
        var assetPath = AssetDatabase.GetAssetPath(asset);
        if (assetPath != null && (assetPath.EndsWith(".ase") || assetPath.EndsWith(".aseprite")))
        {
          var assetFullPath = $"{Application.dataPath}{assetPath.Substring("Assets".Length)}";
          _document = AsepriteDocument.FromFile(assetFullPath);
          CurrentDocument = _document;
        }
      }
    }

    public override void OnDisable()
    {
      base.OnDisable();
      _document = null;
      CurrentDocument = null;
    }

    public override void OnInspectorGUI()
    {
      CurrentDocument = _document;
      
      serializedObject.Update();

      var flagsProp = serializedObject.FindProperty("importFlags");
      var flags = (AseImporter.EImportFlags) flagsProp.intValue;

      EditorGUILayout.PropertyField(flagsProp);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("pixelsPerUnit"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"));

      if (flags.HasFlag(AseImporter.EImportFlags.LayerToSprite))
      {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("layersImporting"));
      }
      
      if (flags.HasFlag(AseImporter.EImportFlags.TagToAnimation))
      {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animationsImporting"));
      }
      
      if (flags.HasFlag(AseImporter.EImportFlags.Tileset))
      {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tilesetImporting"));
      }
      
      serializedObject.ApplyModifiedProperties();
      ApplyRevertGUI();
    }
  }
}