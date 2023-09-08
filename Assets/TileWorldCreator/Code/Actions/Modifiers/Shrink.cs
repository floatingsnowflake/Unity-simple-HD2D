using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.Utilities;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Shrink")]
	public class Shrink : TWCBlueprintAction, ITWCAction
	{
		public override bool ShowFoldout
		{
			get{ return false;}
		}
		
		public ITWCAction Clone()
		{
			var _r = new Shrink();
			return _r;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			List<Vector2Int> borderTiles = new List<Vector2Int>();
	        for (int x = 0; x < map.GetLength(0); x ++)
	        {
	            for (int y = 0; y < map.GetLength(1); y ++)
	            {
	                
		            var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, map);
		            if (_neighbours < 9)
	                {
		            	borderTiles.Add(new Vector2Int(x, y));
	                }
	            }
	        }
	        
	        
			for (int c = 0; c < borderTiles.Count; c++)
			{
				map[borderTiles[c].x, borderTiles[c].y] = false;
			}
	
	        return map;
	    }
	
	
	    #if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc) {}
		#endif
		public float GetGUIHeight(){ return 0f; }
		
	    
	}
}