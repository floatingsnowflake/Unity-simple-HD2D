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
//	// Define the category for this action (Generator or Modifier)
//	// Generator
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Generators)]
	
//	// Set the name of this action
	[ActionNameAttribute(Name="DotGrid")]
//	// Inherit from TWCBlueprintAction and implement the ITWCAction interface
	public class DotGrid : TWCBlueprintAction, ITWCAction
	{
		public int spacing = 2;


		// Custom gui layout. If we want to implement a custom gui for this action we need this 
		// guilayout, so that the reorderable list can draw the gui correctly.
		private TWCGUILayout guiLayout;

		// Clone function
		// This is called when the user duplicated this action
		// Simply return a new class of this type and assign all parameters
		public ITWCAction Clone()
		{
			var _r = new DotGrid();
			return _r;
		}
		
		
//		// The actual execution method which gets called by TileWorldCreator.
//		// Here you can make your map modifications. Make sure to return the new map.
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			//for loop to go thru all x values
	        for (int x = 0; x < map.GetLength(0); x ++)
	        {
				//for each value go thru all y values, so we have gone thru all values
	            for (int y = 0; y < map.GetLength(1); y ++)
	            {

						//and the y value of a given square based on our modulo number
					if(x%spacing==0)
					{
						if(y%spacing==0)
						{
							
							map[x,y] = true;
						}
					}


	            }
	        } 

	        return map;
		}
	    
	    #if UNITY_EDITOR
		// Custom gui for this action
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			// Use the guiLayout to make sure the gui is drawn correctly in the reorderable list
			using (guiLayout = new TWCGUILayout(_rect))
			{


				guiLayout.Add();
				spacing = EditorGUI.IntField(guiLayout.rect, "Spacing", spacing);


			}
		}
		#endif
//	
//		// Must be implemented when using a custom gui.
//		// Here we're returning the height of the guiLayout.
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
//		
	}
}