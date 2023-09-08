using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;
using TWC.Utilities;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Player Position")]
	public class PlayerPosition : TWCBlueprintAction, ITWCAction
	{
		private TWCGUILayout guiLayout;
		
		public enum MapPosition
		{
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight
		}
		
		public MapPosition mapPosition;
		
		public ITWCAction Clone()
		{
			var _r = new PlayerPosition();
			
			_r.mapPosition = this.mapPosition;
			
			return _r;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			var _divisionX = map.GetLength(0) / 3;
			var _divisionY = map.GetLength(1) / 3;
			
			int _selectedX = 0;
			int _selectedY = 0;
			
			switch (mapPosition)
			{
				case MapPosition.TopLeft:
					_selectedX = 1;
					_selectedY = 3;
					break;
				case MapPosition.TopCenter:
					_selectedX = 2;
					_selectedY = 3;
					break;
				case MapPosition.TopRight:
					_selectedX = 3;
					_selectedY = 3;
					break;
				case MapPosition.MiddleLeft:
					_selectedX = 1;
					_selectedY = 2;
					break;
				case MapPosition.MiddleCenter:
					_selectedX = 2;
					_selectedY = 2;
					break;
				case MapPosition.MiddleRight:
					_selectedX = 3;
					_selectedY = 2;
					break;
				case MapPosition.BottomLeft:
					_selectedX = 1;
					_selectedY = 1;
					break;
				case MapPosition.BottomCenter:
					_selectedX = 2;
					_selectedY = 1;
					break;
				case MapPosition.BottomRight:
					_selectedX = 3;
					_selectedY = 1;
					break;
			}
			
			var _maxX = _divisionX * _selectedX;
			var _maxY = _divisionY * _selectedY;
			var _minX = _maxX - _divisionX;
			var _minY = _maxY - _divisionY;
			
			bool[,] _newMap = new bool[map.GetLength(0), map.GetLength(1)];
			
			var _centerX = (_divisionX  / 2) + _minX;
			var _centerY = (_divisionY / 2) + _minY;
	
			bool _found = false;
		
			// Try to find position in the exact center tile of the selected position partition
			if (_centerX < map.GetLength(0) && _centerY < map.GetLength(1) && _centerX >= 0 && _centerY >= 0)
			{
				if (map[_centerX,_centerY])
				{
					var _c = TileWorldCreatorUtilities.CountNeighbours(_centerX, _centerY, map);
					if (_c >= 8)
					{
						_newMap[_centerX,_centerY] = true;
						_found = true;
					}
				}
			}
			
			
			if (!_found)
			{
				// No center tile found.
				// Try to find position in selected position partition
				for (int x = _minX; x < _maxX; x ++)
				{
					for (int y = _minY; y < _maxY; y ++)
					{
						if (x < map.GetLength(0) && y < map.GetLength(1) && x >= 0 && y >= 0)
						{
							if (map[x, y])
							{
								var _c = TileWorldCreatorUtilities.CountNeighbours(x, y, map);
								if (_c >= 8)
								{
									_newMap[x,y] = true;
									_found = true;
								}
							}
						}
						
						if (_found)
						{
							break;
						}
					}
					
					if (_found)
					{
						break;
					}
				}
			}
			
			
			return _newMap;
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				mapPosition = (MapPosition) EditorGUI.EnumPopup(guiLayout.rect, mapPosition);
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
		
	}
}