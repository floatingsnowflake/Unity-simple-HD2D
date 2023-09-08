using System.IO;
using System.Linq;
using Editor.Windows;
using TeamMingo.Configs.Runtime;
using UnityEditor;
using UnityEngine;

namespace Mingo.I18n.Editor
{
  public static class ScreenTransitionMenus
  {
    [MenuItem("Team Mingo/Screen Transition Settings", priority = 200)]
    private static void ShowScreenTransitionSettings()
    {
      var settings = Configuration.Get().screenTransitionSettings;
      if (!settings)
      {
        EditorUtility.DisplayDialog("Notice", "You have not created and set.", "OK");
        return;
      }

      CustomAssetWindow.Create(settings);
    }
    
    // [MenuItem("Team Mingo/Rename Textures")]
    private static void RenameTextures()
    {
      var folder = EditorUtility.OpenFolderPanel("Select Texture Folder", Application.dataPath, "");
      foreach (var file in Directory.EnumerateFiles(folder))
      {
        var originFileName = Path.GetFileName(file);
        if (originFileName.StartsWith("ScreenTransition")) continue;
        if (!originFileName.EndsWith(".png")) continue;
        var fileName = originFileName.Substring(0, originFileName.Length - 4);

        var fileNameSplit = fileName.Split("-").Select(_ => _.Substring(0, 1).ToUpper() + _.Substring(1));
        fileName = "ScreenTransitionTex-" + string.Join("", fileNameSplit) + ".png";
        File.Copy(Path.Combine(folder, originFileName), Path.Combine(folder, fileName));
        File.Delete(Path.Combine(folder, originFileName));
      }
    }
  }
}