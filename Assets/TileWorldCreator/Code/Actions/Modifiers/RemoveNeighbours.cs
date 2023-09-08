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
	/// <summary>
	/// Removes surrounding neighbouring tiles by defined radius
	/// </summary>
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Remove Neighbours")]
	public class RemoveNeighbours : TWCBlueprintAction, ITWCAction
	{
		public int radius = 1;
		
		TWCGUILayout guiLayout;
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public TileWorldCreator twc;
		}
		
		public ITWCAction Clone()
		{
			var _r = new RemoveNeighbours();
			
			_r.radius = this.radius;
				
			return _r;
		}
	  
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
            List<Vector2Int> removeNeighbours = new List<Vector2Int>();
			List<Vector2Int> saveTiles = new List<Vector2Int>();

            for (int x = 0; x < map.GetLength(0); x ++)
			{
				for (int y = 0; y < map.GetLength(1); y ++)
				{
					
					for (int i = -radius; i < radius; i ++)
					{
						for(int j = -radius; j < radius; j ++)
						{
							if (x + i > 0 && y + j > 0 && x + i < map.GetLength(0) && y + j < map.GetLength(1))
							{
								if (map[x,y] && map[x + i, y + j] && x + i != x && y + j != y)
								{
									//map[x + i, y + j] = false;
									removeNeighbours.Add(new Vector2Int(x + i, y + j));
									saveTiles.Add(new Vector2Int(x, y));

                                }
							}
						}
					}
				}
			}

			for (int r = 0; r < removeNeighbours.Count; r++)
			{
				var _isSave = false;
				for (int s = 0; s < saveTiles.Count; s++)
				{
					if (removeNeighbours[r] == saveTiles[s])
					{
						//Debug.Log("yes " + saveTiles[s]);
						_isSave = true;
					}
				}

				if (!_isSave)
				{
					//Debug.Log("no " + removeNeighbours[r]);
					map[removeNeighbours[r].x, removeNeighbours[r].y] = false;
				}
			}


			return map;
		}
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();

				radius = EditorGUI.IntField(guiLayout.rect, new GUIContent("radius", "Remove all neighbours inside of this radius"), radius);
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