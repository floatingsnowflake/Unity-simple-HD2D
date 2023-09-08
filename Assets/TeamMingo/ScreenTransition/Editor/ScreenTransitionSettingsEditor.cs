using TeamMingo.ScreenTransition.Runtime;
using UnityEditor;
using UnityEngine;

namespace Mingo.I18n.Editor
{
  [CustomEditor(typeof(ScreenTransitionSettings))]
  public class ScreenTransitionSettingsEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      base.OnInspectorGUI();
      
      serializedObject.ApplyModifiedProperties();
    }
  }
}