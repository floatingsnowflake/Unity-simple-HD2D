using System.Reflection;
using TeamMingo.Toolbox.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace TeamMingo.Toolbox.Editor.PropertyDrawers
{
  [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
  public sealed class SortingLayerPropertyDrawer : PropertyDrawer
  {
    private static readonly MethodInfo sortingLayerFieldMethodInfo =
      typeof(EditorGUI).GetMethod(
        "SortingLayerField",
        BindingFlags.Static | BindingFlags.NonPublic,
        null,
        new[]
        {
          typeof(Rect),
          typeof(GUIContent),
          typeof(SerializedProperty),
          typeof(GUIStyle),
          typeof(GUIStyle)
        },
        null);


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      if (property.propertyType != SerializedPropertyType.Integer)
      {
        Debug.LogError("SortingLayer property should be an integer (the layer id)");
      }
      else
      {
        SortingLayerField(position, label, property, EditorStyles.popup, EditorStyles.label);
      }
    }


    public static void SortingLayerField(Rect position,
      GUIContent label,
      SerializedProperty layerID,
      GUIStyle style,
      GUIStyle labelStyle)
    {
      if (sortingLayerFieldMethodInfo != null)
      {
        object[] parameters = new object[]
        {
          position,
          label,
          layerID,
          style,
          labelStyle
        };
        sortingLayerFieldMethodInfo.Invoke(null, parameters);
      }
    }
  }
}