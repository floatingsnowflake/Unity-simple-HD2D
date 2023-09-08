using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;
using TWC.Utilities;

/*
	Return all overlapping tiles.
*/

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Overlap")]
	public class Overlap : TWCBlueprintAction, ITWCAction
	{
		public Guid layer1;
		public Guid layer2;
		
		private TWCGUILayout guiLayout;
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public int layer;
			public TileWorldCreator twc;
		}
		
		public ITWCAction Clone()
		{
			var _r = new Overlap();
			
			_r.layer1 = this.layer1;
			_r.layer2 = this.layer2;

			return _r;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			
				
			var _fromMap1 = _twc.GetMapOutputFromBlueprintLayer(layer1);
			var _fromMap2 = _twc.GetMapOutputFromBlueprintLayer(layer2);
		
				
				
			if (_fromMap1 == null)
			{
				Debug.LogWarning("TileWorldCreator: Overlap modifier - Layer not assigned");
				return map;
			}
			
			if (_fromMap2 == null)
			{
				Debug.LogWarning("TileWorldCreator: Overlap modifier - Layer not assigned");
				return map;
			}
			
			
			for (int x = 0; x < _fromMap1.GetLength(0); x ++)
			{
				for (int y = 0; y < _fromMap1.GetLength(1); y ++)
				{
					if (_fromMap1[x,y] && _fromMap2[x,y])
					{
						map[x,y] = true;
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
				var _layerName1 = "";
				var _layerData1 = _asset.GetBlueprintLayerData(layer1);
				if (_layerData1 != null)
				{
					_layerName1 = _layerData1.layerName;
				}
				
				var _layerName2 = "";
				var _layerData2 = _asset.GetBlueprintLayerData(layer2);
				if (_layerData2 != null)
				{
					_layerName2 = _layerData2.layerName;
				}
				
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_layerName1), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
					
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
						_data.layer = 0;
						
						if (_twc != null)
						{
							_data.twc = _twc;
						}				
						
						menu.AddItem(new GUIContent(_names[n]), false, AssignLayer, _data);
					}
					
					menu.ShowAsContext();
				}
				
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_layerName2), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
					
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
						_data.layer = 1;
						
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
			if (_d.layer == 0)
			{
				layer1 = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
			}
			
			if (_d.layer == 1)
			{
				layer2 = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
			}
		}
	}
}