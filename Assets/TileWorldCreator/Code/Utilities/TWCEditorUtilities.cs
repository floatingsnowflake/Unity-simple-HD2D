using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace TWC.editor
{
	public static class EditorUtilities
	{
		
		#if UNITY_EDITOR
		public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
		{
			try
			{
				Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
				r.height = thickness;
				r.y+=padding/2;
				r.x-=2;
				r.width +=6;
				EditorGUI.DrawRect(r, color);
			}
			catch{}
		}
		#endif
		
		public static string GetRelativeResPath()
		{
			//var _theme = "Light";
	
			//	#if UNITY_EDITOR
			//if (EditorGUIUtility.isProSkin)
			//{
			//	_theme = "Dark";
			//}
			//	#endif
				
		
			var _res = System.IO.Directory.EnumerateFiles("Assets/", "TWCResPath.cs", System.IO.SearchOption.AllDirectories);
				
			var _path = "";
				
			var _found = _res.FirstOrDefault();
			if (!string.IsNullOrEmpty(_found))
			{
				_path = _found.Replace("TWCResPath.cs", "").Replace("\\", "/");
				//_path = Path.Combine(_path, _theme);
			}
			
			return _path;
				
				
		}
		
		
		public static Texture2D LoadIcon(string _name)
		{
			#if UNITY_EDITOR
			return (Texture2D)(AssetDatabase.LoadAssetAtPath(GetRelativeResPath() + "/" + _name, typeof(Texture2D)));
			#else
			return null;
			#endif
		}
		
		public static string[] GetAllGenerationLayerNames(TileWorldCreatorAsset _asset)
		{
			string[] _returnList = new string[_asset.mapBlueprintLayers.Count];
			
			for (int i = 0; i < _asset.mapBlueprintLayers.Count; i ++)
			{
				_returnList[i] = _asset.mapBlueprintLayers[i].layerName;
			}
			
			return _returnList;
		}
		
	}
	
	
	public class TWCGUILayout : System.IDisposable
	{
		public Rect rect;
		public float height;	
		public int index;
			
		public TWCGUILayout(Rect _rect)
		{
			rect = _rect;
			#if UNITY_EDITOR
			height = EditorGUIUtility.singleLineHeight;
			#endif
		}
		
		#if UNITY_EDITOR
		public void Add()
		{
			index++;
			rect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
			
			//height = EditorGUIUtility.singleLineHeight * (index + 1);
			height += EditorGUIUtility.singleLineHeight;
		}
		
		public void Add(int _customHeight)
		{
			index++;
			rect = new Rect(rect.x, rect.y + _customHeight, rect.width, _customHeight);
			
			//height = _customHeight * (index + 1);
			height += _customHeight;
		}
		#endif
		
		public void Dispose() {}
	}
	
}