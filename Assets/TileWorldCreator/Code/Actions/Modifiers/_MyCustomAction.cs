//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//using TWC.editor;

//namespace TWC.Actions
//{
//	// Define the category for this action (Generator or Modifier)
//	// Modifier
//	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
//	// Generator
//	//[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Generators)]
	
//	// Set the name of this action
//	[ActionNameAttribute(Name="MyCustomAction")]
//	// Inherit from TWCBlueprintAction and implement the ITWCAction interface
//	public class MyCustomAction : TWCBlueprintAction, ITWCAction
//	{
		
//		// Clone function
//		// This is called when the user duplicated this action
//		// Simply return a new class of this type and assign all parameters
//		public ITWCAction Clone()
//		{
//			var _r = new MyCustomAction();
//			return _r;
//		}
		
//		// Custom gui layout. If we want to implement a custom gui for this action we need this 
//		// guilayout, so that the reorderable list can draw the gui correctly.
//		TWCGUILayout guiLayout;
		
//		// The actual execution method which gets called by TileWorldCreator.
//		// Here you can make your map modifications. Make sure to return the new map.
//		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
//		{
//			// Simple example: 
//			// Here we are inverting all tiles of the map
//	        for (int x = 0; x < map.GetLength(0); x ++)
//	        {
//	            for (int y = 0; y < map.GetLength(1); y ++)
//	            {
//	                map[x,y] = !map[x,y];
//	            }
//	        }
	
//			// return the newly modified map
//	        return map;
//		}
	    
//	    #if UNITY_EDITOR
//		// Custom gui for this action
//		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
//		{
//			// Use the guiLayout to make sure the gui is drawn correctly in the reorderable list
//			using (guiLayout = new TWCGUILayout(_rect))
//			{
//				// Example: Show a label
//				// Before adding a gui element first call guiLayout.Add();
//				guiLayout.Add();
//				// Use the guilayout.rect for the correct rect
//				GUI.Label(guiLayout.rect, "Hello World");
//			}
//		}
//		#endif
	
//		// Must be implemented when using a custom gui.
//		// Here we're returning the height of the guiLayout.
//		public float GetGUIHeight()
//		{ 
//			if (guiLayout != null)
//			{
//				return guiLayout.height;
//			}
//			else
//			{
//				return 18;
//			}
//		}
		
//	}
//}