using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Subtract")]
	public class Subtract : TWCBlueprintAction, ITWCAction
	{
		public int selectedLayerIndex;
		public Guid guidCopyLayer;
		
		TWCGUILayout guiLayout;
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public TileWorldCreator twc;
		}
		
		public ITWCAction Clone()
		{
			var _r = new Subtract();
			
			_r.selectedLayerIndex = this.selectedLayerIndex;
			_r.guidCopyLayer = this.guidCopyLayer;
			
			return _r;
		}
	  
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			var _fromMap = _twc.GetMapOutputFromBlueprintLayer(guidCopyLayer);
			
			if (_fromMap == null)
			{
				//Debug.LogWarning("No blueprint map assigned");
				return map;
			}
			
			for (int x = 0; x < _fromMap.GetLength(0); x ++)
			{
				for (int y = 0; y < _fromMap.GetLength(1); y ++)
				{
					
					if (_fromMap[x,y])
					{
						map[x,y] = false;
					}
					
				}
			}
	
			return map;
		}
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				var _names =  EditorUtilities.GetAllGenerationLayerNames(_asset);
				var _layerName = "";
				var _layerData = _asset.GetBlueprintLayerData(guidCopyLayer);
				if (_layerData != null)
				{
					_layerName = _layerData.layerName;
				}
				
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_layerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
					
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
						
						if (_twc != null)
						{
							_data.twc = _twc;
						}
						
						menu.AddItem(new GUIContent(_names[n]), false, AssignLayer, _data);
					}
					
					menu.ShowAsContext();
				}
			}
		}
		#endif
		
		public float GetGUIHeight()
		{
			if (guiLayout != null)
			{
				return guiLayout.height;
			}
			else
			{
				return 18;
			}
		}
		
		
		void AssignLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			guidCopyLayer = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}
		
	
	}
}