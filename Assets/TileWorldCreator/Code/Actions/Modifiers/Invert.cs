using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Invert")]
	public class Invert : TWCBlueprintAction, ITWCAction
	{
		
		public override bool ShowFoldout
		{
			get{ return false;}
		}
		
		public ITWCAction Clone()
		{
			var _r = new Invert();
			return _r;
		}
		
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
	        for (int x = 0; x < map.GetLength(0); x ++)
	        {
	            for (int y = 0; y < map.GetLength(1); y ++)
	            {
	                map[x,y] = !map[x,y];
	            }
	        }
	
	        return map;
		}
	    
		public float GetGUIHeight(){ return 0; }
	}
}