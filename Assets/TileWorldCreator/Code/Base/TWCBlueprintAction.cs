using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWC.OdinSerializer;

//[assembly: BindTypeNameToType("TWC.Actions.TWCMapGenerationAction", typeof(TWC.Actions.TWCBlueprintAction))]


namespace TWC.Actions
{
	/// <summary>
	/// Base class for all map blueprint layer actions
	/// </summary>
	[System.Serializable]
	public class TWCBlueprintAction
	{
		public Guid guid;
		public bool active = true;
		public bool foldout;
		
		// If there's no gui to show, this can be overriden to false
		public virtual bool ShowFoldout
		{
			get{ return true; }
		}
		
		public bool resultFailed;
		
		public virtual void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _twcAsset, TileWorldCreator _twc){}
		public virtual void DrawGizmos(){}
		public virtual void DrawSceneGUI(Rect _sceneView){}
		
		// Runtime map modifications (only implemented by Paint generator action)
		public virtual void ModifyMap(int _x, int _y, bool _value, TileWorldCreator _twc){}
		public virtual void FillMap(bool _value, TileWorldCreator _twc){}
		public virtual void CopyMap(TileWorldCreator _twc){}
		
		public TWCBlueprintAction()
		{
			guid = Guid.NewGuid();
			active = true;
			foldout = false;
		}
	}
	
	
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class ActionCategoryAttribute : Attribute
	{
		public enum CategoryTypes
		{
			Generators,
			Modifiers
		}
		
		public CategoryTypes Category{get;set;}
	}
	
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class ActionNameAttribute : Attribute
	{
		public string Name{get;set;}
	}
}