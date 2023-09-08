using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWC.OdinSerializer;

//[assembly: BindTypeNameToType("TWC.Actions.TWCMapInstantiationLayer", typeof(TWC.Actions.TWCBuildLayer))]


namespace TWC.Actions
{
	/// <summary>
	/// Base class for all instantiation layers
	/// </summary>
	//[System.Serializable]
	public class TWCBuildLayer
	{
		[OdinSerialize]
		public Guid guid;
		[OdinSerialize]
		public Guid assignedGenerationLayerGuid;
		public string layerName;
		public bool foldout;
		public bool active = true;
		
		
		public virtual void DrawGUI(TileWorldCreatorAsset _twcAsset){}
		public virtual void Execute(TileWorldCreator _twc, bool _forceRebuild){}
		
		public virtual TWCBuildLayer Clone()
		{
			return null;
		}
	
	}
}