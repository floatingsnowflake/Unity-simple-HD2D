using TeamMingo.Configs.Runtime;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Configs.Editor
{
  public class ConfigurationWindow : EditorWindow
  {
    [MenuItem("Team Mingo/Configuration", priority = 0)]
    public static void ShowWindow()
    {
      GetWindow<ConfigurationWindow>("TeamMingo Configuration").Show();
    }
    
    private UnityEditor.Editor _editor;
    private Configuration _configuration;
 
    private void OnEnable()
    {
      _configuration = Resources.Load<Configuration>("TeamMingo Configuration");
      if (!_configuration)
      {
        _configuration = CreateInstance<Configuration>();
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
          AssetDatabase.CreateFolder("Assets", "Resources");
        }

        AssetDatabase.CreateAsset(_configuration, "Assets/Resources/" + Configuration.Path + ".asset");
        AssetDatabase.SaveAssets();
      }
      _editor = UnityEditor.Editor.CreateEditor(_configuration);
    }
 
    private void OnGUI()
    {
      _editor.OnInspectorGUI();
    }
  }
}