using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using TWC.OdinSerializer;
using TWC.Actions;

//[assembly: BindTypeNameToType("TWC.TileWorldCreatorAsset.MapGenerationLayerData", typeof(TWC.TileWorldCreatorAsset.BlueprintLayerData))]
	
namespace TWC
{
		
	[CreateAssetMenu(menuName = "TileWorldCreator/New TileWorldCreator Asset")]
	public class TileWorldCreatorAsset : SerializedScriptableObject
	{
 
		public class BlueprintLayerData
		{
			/// <summary>
			/// Unique identifier
			/// </summary>
			[OdinSerialize]
			public Guid guid;
			/// <summary>
			/// is layer active or not
			/// </summary>
			[OdinSerialize]
			public bool active = true;
			/// <summary>
			/// Layer name
			/// </summary>
			[OdinSerialize]
			public string layerName;
			/// <summary>
			/// Generated map output from this layer
			/// </summary>
			[OdinSerialize]
			public bool[,] map;
			
			[OdinSerialize]
			public bool randomSeedOverride;
			[OdinSerialize]
			public bool useCustomSeed;
			[OdinSerialize]
			public int customLayerRandomSeed;
			
			// Editor properties
			[System.NonSerialized]
			public bool foldout;
			[OdinSerialize]
			public Color previewColor;
			[NonSerialized]
			public Texture2D previewTextureMap;
			
			#if UNITY_EDITOR
			[System.NonSerialized]
			public UnityEditorInternal.ReorderableList reorderableList;
			#endif
			
			[OdinSerialize]
			public bool mapResultFailed;		
			[System.Serializable]
			public class ActionStack
			{
				public string actionName;
				public ITWCAction action;
				
				public ActionStack(){}
				
				public ActionStack(string _actionName, ITWCAction _action)
				{
					actionName = _actionName;
					action = _action;
				}
				
				public ActionStack(ActionStack _originalStack)
				{
					this.action = _originalStack.action;
					this.actionName = _originalStack.actionName;
					(this.action as TWCBlueprintAction).active = (_originalStack.action as TWCBlueprintAction).active;
					(this.action as TWCBlueprintAction).guid = (_originalStack.action as TWCBlueprintAction).guid;		
				}
			}
			[OdinSerialize]
			public List<ActionStack> stack;
	
	
			public BlueprintLayerData(string _layerName, bool _newGuid)
			{
				layerName = _layerName;
				
				if (_newGuid)
				{
					guid = Guid.NewGuid();
				}
				
				stack = new List<ActionStack>();
			}
			
			
			public BlueprintLayerData Clone(bool _newGuid) 
			{
				var _r = new BlueprintLayerData(this.layerName, _newGuid);
				
				_r.layerName = this.layerName;
				
				if (!_newGuid)
				{
					_r.guid = this.guid;
				}
				
				if (this.map != null)
				{
					_r.map = new bool[this.map.GetLength(0), this.map.GetLength(1)];
					System.Array.Copy(this.map, _r.map, this.map.Length);
				}
				
				_r.previewColor = this.previewColor;
				_r.previewTextureMap = this.previewTextureMap;	
				_r.stack = new List<ActionStack>( );
				
				for (int s = 0; s < this.stack.Count; s ++)
				{
					var _newAction = this.stack[s].action.Clone() as ITWCAction;
					var _newActionStack = new ActionStack(this.stack[s].actionName, _newAction);
					
					(_newActionStack.action as TWCBlueprintAction).active  = (this.stack[s].action as TWCBlueprintAction).active;
			

					_r.stack.Add(_newActionStack);
				}
	
				return _r;
			}
			
			
			public BlueprintLayerData (TileWorldCreatorSaveableData.BlueprintLayerData _data)
			{
				this.guid = _data.guid;
				this.active = _data.active;
				this.layerName = _data.layerName;
				//this.map = _data.map;
				this.previewColor = _data.previewColor;
				this.previewTextureMap = _data.previewTextureMap;
			
				this.stack = new List<ActionStack>();
				
				for (int i = 0; i < _data.stack.Count; i++)
				{
					this.stack.Add(new ActionStack(_data.stack[i].actionName, _data.stack[i].action));
				}
			}
		}
	
		 
		 
		/// STACKS
		// Blueprint layers
		[OdinSerialize]
		[FormerlySerializedAs("mapGenerationLayers")]
		public List<BlueprintLayerData> mapBlueprintLayers = new List<BlueprintLayerData>();
		// Build layers
		[OdinSerialize]
		[FormerlySerializedAs("mapInstantiationLayers")]
		public List<TWCBuildLayer> mapBuildLayers = new List<TWCBuildLayer>();
		
		[OdinSerialize]
		public string worldName = "TileWorldCreator_Map";
		[OdinSerialize]
		public int mapWidth = 20, mapHeight = 20;
		[OdinSerialize]
		public float cellSize = 2;
		[OdinSerialize]
		public bool useRandomSeed;
		[OdinSerialize]
		public int randomSeed;
		[OdinSerialize]
		public bool useNewRandomSeedForEveryLayer;
		[OdinSerialize]
		public bool mergePreviewTextures;
		[OdinSerialize]
		public bool useCustomClusterCelSize;
		[OdinSerialize]
		public int customClusterCellSize;
		
		// Editor property
		[OdinSerialize]
		public bool showSettings;
		
		public enum MapOrientation
		{
			XZ,
			XY
		}
		
		[OdinSerialize]
		public MapOrientation mapOrientation;
		
		public BlueprintLayerData GetBlueprintLayerData(System.Guid _guid)
		{
			for (int i = 0; i < mapBlueprintLayers.Count; i ++)
			{
				if (mapBlueprintLayers[i].guid == _guid)
				{
					return mapBlueprintLayers[i];
				}
			}
			
			return null;
		}
	}

}