using System;
using System.Collections.Generic;
using System.Reflection;
using TeamMingo.Configs.Runtime;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Configs.Editor
{
  
  [CustomEditor(typeof(Configuration))]
  public class ConfigurationEditor : UnityEditor.Editor
  {

    public class ConfigInfo
    {
      public Type ScriptableType;
      public ConfigurationAttribute Attribute;
    }
    
    private Dictionary<string, ConfigInfo> _configDict;
    
    private void OnEnable()
    {
      _configDict = new Dictionary<string, ConfigInfo>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        var assemblyName = assembly.GetName().Name;
        if (!assemblyName.StartsWith("TeamMingo.")) {
          continue;
        }
        
        foreach (var type in assembly.GetTypes())
        {
          var configAttr = type.GetCustomAttribute<ConfigurationAttribute>();
          if (configAttr == null) continue;
          _configDict[configAttr.field] = new ConfigInfo()
          {
            ScriptableType = type,
            Attribute = configAttr
          };
        }
      }
    }

    private void OnDisable()
    {
      _configDict = null;
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.Space(1);
      EditorGUILayout.BeginVertical();
      EditorGUILayout.Space();
      
      foreach (var configKV in _configDict)
      {
        var info = configKV.Value;
        var property = serializedObject.FindProperty(configKV.Key);

        EditorGUILayout.LabelField(info.Attribute.module);
        var dirty = EditorGUILayout.PropertyField(property, GUIContent.none);
        if (property.objectReferenceValue && dirty)
        {
          var type = property.objectReferenceValue.GetType();
          if (!type.IsAssignableFrom(info.ScriptableType))
          {
            property.objectReferenceValue = null;
          }
        }

        EditorGUILayout.Space();
      }
      
      EditorGUILayout.EndVertical();
      EditorGUILayout.Space(1);
      EditorGUILayout.EndHorizontal();
      
      serializedObject.ApplyModifiedProperties();
    }
  }
}