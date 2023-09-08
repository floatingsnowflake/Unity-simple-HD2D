using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWC;
using TWC.OdinSerializer;
using TWC.Actions;

namespace TWC
{
	//Serializeable data which we can save in to a file
	[System.Serializable]
	public class TileWorldCreatorSaveableData
	{
		
		// DEFAULT SETTINGS
		////////////////////
		[OdinSerialize]
		public string worldName;
		[OdinSerialize]
		public int mapWidth, mapHeight;
		[OdinSerialize]
		public float cellSize;
		[OdinSerialize]
		public bool useRandomSeed;
		[OdinSerialize]
		public int randomSeed;
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
		////////////////////
		
		
		
		
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
			//[OdinSerialize]
			//public bool[,] map;
				
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
				
				// Only when loading or saving
				if (!_newGuid)
				{
					_r.guid = this.guid;
				}
				
				//if (this.map != null)
				//{
				//	_r.map = new bool[this.map.GetLength(0), this.map.GetLength(1)];
				//	System.Array.Copy(this.map, _r.map, this.map.Length);
				//}
					
				_r.previewColor = this.previewColor;
				_r.previewTextureMap = this.previewTextureMap;	
				_r.stack = new List<ActionStack>( );
					
				for (int s = 0; s < this.stack.Count; s ++)
				{
					var _newAction = this.stack[s].action.Clone() as ITWCAction;
					var _newActionStack = new ActionStack(this.stack[s].actionName, _newAction);
					
					(_newActionStack.action as TWCBlueprintAction).active = (this.stack[s].action as TWCBlueprintAction).active;
					(_newActionStack.action as TWCBlueprintAction).guid = (this.stack[s].action as TWCBlueprintAction).guid;
				
					_r.stack.Add(_newActionStack);
				}
		
				return _r;
			}
			
			
			public BlueprintLayerData (TileWorldCreatorAsset.BlueprintLayerData _data)
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
					var _actionStack = new ActionStack(_data.stack[i].actionName, _data.stack[i].action);
					
					(_actionStack.action as TWCBlueprintAction).active = (_data.stack[i].action as TWCBlueprintAction).active;
					(_actionStack.action as TWCBlueprintAction).guid = (_data.stack[i].action as TWCBlueprintAction).guid;
					
					this.stack.Add(_actionStack); //new ActionStack(_data.stack[i].actionName, _data.stack[i].action));
				}
			}
		}
			 
			 
		// Blueprint layers
		[OdinSerialize]
		public List<BlueprintLayerData> mapBlueprintLayers = new List<BlueprintLayerData>();
		
		
		public TileWorldCreatorSaveableData (TileWorldCreatorAsset _asset)
		{
			worldName = _asset.worldName;
			mapWidth = _asset.mapWidth;
			mapHeight = _asset.mapHeight;
			cellSize = _asset.cellSize;
			//mapOrientation = _asset.mapOrientation;
			useRandomSeed = _asset.useRandomSeed;
			randomSeed = _asset.randomSeed;
			mergePreviewTextures = _asset.mergePreviewTextures;
			customClusterCellSize = _asset.customClusterCellSize;
			
			mapBlueprintLayers = new List<BlueprintLayerData>();
			
			for (int i = 0; i < _asset.mapBlueprintLayers.Count; i ++)
			{	
				var _a = _asset.mapBlueprintLayers[i].Clone(false);
				mapBlueprintLayers.Add(new BlueprintLayerData(_a));
			}
		}
		
		
		
		public TileWorldCreatorAsset AssignToAsset(TileWorldCreatorAsset _asset)
		{
			_asset.worldName = worldName;
			_asset.mapWidth = mapWidth;
			_asset.mapHeight = mapHeight;
			_asset.cellSize = cellSize;
			//_asset.mapOrientation = mapOrientation;
			_asset.useRandomSeed = useRandomSeed;
			_asset.randomSeed = randomSeed;
			_asset.mergePreviewTextures = mergePreviewTextures;
			_asset.customClusterCellSize = customClusterCellSize;
			
			_asset.mapBlueprintLayers = new List<TileWorldCreatorAsset.BlueprintLayerData>();
			
			for (int i = 0; i < mapBlueprintLayers.Count; i ++)
			{	
				var _a = mapBlueprintLayers[i].Clone(false);
				_asset.mapBlueprintLayers.Add(new TileWorldCreatorAsset.BlueprintLayerData(_a));
				_asset.mapBlueprintLayers[_asset.mapBlueprintLayers.Count - 1].stack = new List<TileWorldCreatorAsset.BlueprintLayerData.ActionStack>();
				
				for (int a = 0; a < mapBlueprintLayers[i].stack.Count; a ++)
				{	
					var _newStack = new TileWorldCreatorAsset.BlueprintLayerData.ActionStack(mapBlueprintLayers[i].stack[a].actionName, mapBlueprintLayers[i].stack[a].action);
					
					//Debug.Log((mapBlueprintLayers[i].stack[a].action as TWCBlueprintAction).active);
					(_newStack.action as TWCBlueprintAction).active = (mapBlueprintLayers[i].stack[a].action as TWCBlueprintAction).active;
					(_newStack.action as TWCBlueprintAction).guid = (mapBlueprintLayers[i].stack[a].action as TWCBlueprintAction).guid;
					
					_asset.mapBlueprintLayers[_asset.mapBlueprintLayers.Count - 1].stack.Add(_newStack); //mapBlueprintLayers[i].stack[a]);
				}
			}
			
			
			
			return _asset;
		}
		
		
	}
}
