using TeamMingo.ScreenTransition.Runtime;
using UnityEditor;
using UnityEngine;

namespace Mingo.I18n.Editor
{
  
  [CustomEditor(typeof(ScreenTransition))]
  public class ScreenTransitionEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();

      EditorGUI.BeginDisabledGroup(!Application.isPlaying);
      if (GUILayout.Button("Enter"))
      {
        if (target is ScreenTransition transition)
        {
          transition.Enter();
        }
      }

      if (GUILayout.Button("Exit"))
      {
        if (target is ScreenTransition transition)
        {
          transition.Exit();
        }
      }
      EditorGUI.EndDisabledGroup();
    }
  }
}