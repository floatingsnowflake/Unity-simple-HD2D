using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace TWC.Utilities
{
	[InitializeOnLoad]
	public static class TileWorldCreatorUndoRedo
	{
		public static TileWorldCreator currentTWC;
		
		
		static void OnInit()
		{
			RegisterUndo();
		}
		
		[UnityEditor.Callbacks.DidReloadScripts]
		public static void Init()
		{
			RegisterUndo();
		}
		
		public static void RegisterUndo()
		{
			Undo.undoRedoPerformed -= UndoRedoPerformed;
			Undo.undoRedoPerformed += UndoRedoPerformed;
		}
		
		static void UndoRedoPerformed()
		{
			if (currentTWC == null)
				return;
				
			currentTWC.ExecuteAllBlueprintLayers();
			currentTWC.ExecuteAllBuildLayers(false);
			
		}
	}
}
#endif