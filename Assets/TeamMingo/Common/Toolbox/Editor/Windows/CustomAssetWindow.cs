using UnityEditor;
using UnityEngine;

namespace Editor.Windows
{
  public class CustomAssetWindow : EditorWindow
  {
    private Object asset;
    private UnityEditor.Editor assetEditor;
   
    public static CustomAssetWindow Create(Object asset)
    {
      var window = CreateWindow<CustomAssetWindow>($"{asset.name} | {asset.GetType().Name}");
      window.asset = asset;
      window.assetEditor = UnityEditor.Editor.CreateEditor(asset);
      return window;
    }
 
    private void OnGUI()
    {
      GUI.enabled = false;
      asset = EditorGUILayout.ObjectField("Asset", asset, asset.GetType(), false);
      GUI.enabled = true;
 
      EditorGUILayout.Separator();
      EditorGUILayout.Space();
      assetEditor.OnInspectorGUI();
    }
  }
}