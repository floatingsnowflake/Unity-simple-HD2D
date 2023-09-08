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
	[ActionNameAttribute(Name="Smooth")]
	public class Smooth : TWCBlueprintAction, ITWCAction
	{
		public int smoothCount;
	    
		private int neighbourCount = 4;
	    private List<Vector2Int> cells;
		private TWCGUILayout guiLayout;
		
		public ITWCAction Clone()
		{
			var _r = new Smooth();
			
			_r.smoothCount = this.smoothCount;
			
			return _r;
		}
	  
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
	       
	        cells = new List<Vector2Int>();
			neighbourCount = 4;
			
	        for (int i = 0; i < smoothCount; i ++)
	        {
	            for (int x = 0; x < map.GetLength(0); x ++)
	            {
	                for (int y = 0; y < map.GetLength(1); y ++)
	                {
	                    if (map[x,y])
	                    {
		                    var _neighbours = TWC.Utilities.TileWorldCreatorUtilities.CountNeighbours(x, y, map);
	                      
	                        if (_neighbours <= neighbourCount)
	                        {
	                            cells.Add(new Vector2Int(x, y));
	                        }
	                    }
	                }
	            }
	
	            for (int c = 0; c < cells.Count; c++)
	            {
	                map[cells[c].x, cells[c].y] = false;
	            }
	        }
	        
	
	        return map;
	    }
	
	    int CountNeighbours(int x, int y, bool[,] _map)
	    {
	        int _neighbours = 0;
	        for (int i = -1; i < 2; i ++)
	        {
	            for (int j = -1; j < 2; j ++)
	            {

	              try{
	                    if (_map[x+i,y+j])
	                    {
	                        _neighbours++;
	                    }
	              }
	              catch{}
	             
	            }
	        }
	
	        return _neighbours;
	    }
	
	    #if UNITY_EDITOR
	  
	    
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				smoothCount = EditorGUI.IntSlider(guiLayout.rect, "Smooth", smoothCount, 1, 10);
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
