using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEditor;

using TWC.OdinSerializer;
using TWC.Actions;
using TWC.Utilities;

/*

	TileWorldCreator version 3 by doorfortyfour
	version 3.0.0
	
*/


namespace TWC
{
	/// <summary>
	/// Main TileWorldCreator component
	/// </summary>
	[AddComponentMenu("TileWorldCreator")]
	public class TileWorldCreator : MonoBehaviour
	{
		/// <summary>
		/// Asset file with all relevant data
		/// </summary>
		public TileWorldCreatorAsset twcAsset;
		
		private Camera sceneViewCamera;
		private GameObject _worldObject;

		/// <summary>
		/// World game object where clusters should be parented
		/// </summary>
		public GameObject worldObject
		{
			set { _worldObject = value; }
			get
			{
				if (_worldObject == null)
				{
					_worldObject = GameObject.Find(twcAsset.worldName);
					if (_worldObject == null)
					{
						_worldObject = new GameObject (twcAsset.worldName);
						
						// Set game object to the scene of this gameobject
						UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(_worldObject, this.gameObject.scene);
					}
				}
					
				// Set world object rotation based on map orientation
				//if (twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
				//{
				//	_worldObject.transform.localRotation = Quaternion.identity;
				//}
				//else 
				//{
				//	_worldObject.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
				//}
				
				_worldObject.transform.localPosition = this.transform.localPosition;
				
				return _worldObject;
			}
		}
	
		// EVENTS
		////////////////////////////////////////
		public delegate void OnBlueprintLayersCompleteEvent(TileWorldCreator _twc);
		public OnBlueprintLayersCompleteEvent OnBlueprintLayersComplete;
	
		public delegate void OnBuildLayersCompleteEvent(TileWorldCreator _twc);
		public OnBuildLayersCompleteEvent OnBuildLayersComplete;
		////////////////////////////////////////
	
		/// <summary>
		/// Store all generated maps for each blueprint layer
		/// string = guid
		/// </summary>
		public Dictionary<string, WorldMap> generatedBlueprintMaps = new Dictionary<string, WorldMap>();
		
				
		public bool buildResultNoChanges = false;
		private bool priorityBuild = false;
		
		// Keep track of all executed blueprint layers
		private int _executedBlueprintLayersCount;
		private int executedBlueprintLayersCount
		{
			get
			{
				return _executedBlueprintLayersCount;
			}
			set
			{
				_executedBlueprintLayersCount ++;
				
				int _totalBlueprintLayers = 0;
				for (int i = 0; i < twcAsset.mapBlueprintLayers.Count; i ++)
				{
					if (twcAsset.mapBlueprintLayers[i].active)
					{
						_totalBlueprintLayers ++;
					}
				}
				
				if (_executedBlueprintLayersCount == _totalBlueprintLayers)
				{
					if (OnBlueprintLayersComplete != null)
					{
						OnBlueprintLayersComplete(this);
					}
				}
			}
		}
		
		
		// Keep track of all executed build layers
		private int _executedBuildLayersCount;
		public int executedBuildLayersCount
		{
			get
			{
				return _executedBuildLayersCount;
			}
			set
			{
				_executedBuildLayersCount ++;
				
				
				if (_executedBuildLayersCount == twcAsset.mapBuildLayers.Count)
				{
					
					executeAllBuildLayers = false;
					
					if (OnBuildLayersComplete != null)
					{
						OnBuildLayersComplete(this);
					}
					
					if (priorityBuild)
					{
						
						// Assign temp build layer list back to original list
						twcAsset.mapBuildLayers = new List<TWCBuildLayer>();
						foreach(var item in tmpBuildLayers)
						{
							twcAsset.mapBuildLayers.Add(item);
						}
					}
						
					priorityBuild = false;
					
				}
				else
				{
					if (executeAllBuildLayers)
					{
						ExecuteNextBuildLayer(forceRebuild);
					}
				}
			}
		}
		
	
		
		private bool executeAllBuildLayers = false;
		private bool forceRebuild = false;
		public int currentSeed;
		
		int lastSeed;
		
		private	List<TWCBuildLayer> tmpBuildLayers;

		/// <summary>
		/// Creates and adds a new generated map from layer name including a tmp map
		/// so we can compare all the changes made from the last one.
		/// </summary>
		/// <param name="_mapName"></param>
		/// <returns></returns>
		WorldMap AddNewGeneratedBlueprintMap(string _guid, int _width, int _height)
		{
			
			
			WorldMap _oldMap = null;
			
			if (generatedBlueprintMaps.ContainsKey(_guid))
			{
				_oldMap = generatedBlueprintMaps[_guid];	
			}
			
			WorldMap map = null;
			if (!twcAsset.useCustomClusterCelSize)
			{
				map = new WorldMap(_oldMap, _width, _height);
			}
			else
			{
				map = new WorldMap(_oldMap, _width, _height, twcAsset.customClusterCellSize);
			}
			
			if (!generatedBlueprintMaps.ContainsKey(_guid))
			{	
				generatedBlueprintMaps.Add(_guid, map);
			}
			
	
			// a map already exists with the same name
			// we will copy the existing map to a temp name
			// this allows us to compare the new and the old map and only rebuild the 
			// changed tiles / clusters
			if (generatedBlueprintMaps.ContainsKey(_guid + "_TMP"))
			{
				// remove old tmp map
				generatedBlueprintMaps.Remove(_guid + "_TMP");
			}
			
			//Debug.Log("generated new map " + _mapName + "_TMP");
			
			var _existingMap = generatedBlueprintMaps[_guid];
			var _tmpMap = new WorldMap(_existingMap, _width, _height);
			
			foreach (var _cluster in _existingMap.clusters.Keys)
			{
				
				_tmpMap.clusters.Add(_cluster, new Dictionary<Vector2Int, TileData>());
				
				foreach (var _position in _existingMap.clusters[_cluster].Keys)
				{
					TileData _tileData = new TileData();
					TileData _existingData = _existingMap.clusters[_cluster][_position];
					
					_tileData.tileType = _existingData.tileType;
					_tileData.position = _existingData.position;
					_tileData.rotation = _existingData.rotation;
					_tileData.tile = _existingData.tile;
					
					_tmpMap.clusters[_cluster].Add(_position, _tileData);
				}
			}
			
			generatedBlueprintMaps.Add( _guid + "_TMP", _tmpMap);
			
			generatedBlueprintMaps[_guid] = map;
					
			
			
			return map;
		}
		
		
		/// <summary>
		/// Return final generated map from a blueprint layer. The returning WorldMap data 
		/// contains all information for the build layers to instantiate the tiles correctly.
		/// </summary>
		/// <param _guid="The layer unique guid or the layer name"></param>
		/// <returns></returns>
		public WorldMap GetGeneratedBlueprintMap(string _guid)
		{
			if (generatedBlueprintMaps.ContainsKey(_guid))
			{
				return generatedBlueprintMaps[_guid];
			}
			else
			{
				// Search for the guid
				for( var m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
				{
					if (twcAsset.mapBlueprintLayers[m].layerName == _guid)
					{
						if (generatedBlueprintMaps.ContainsKey(twcAsset.mapBlueprintLayers[m].guid.ToString()))
						{
							return generatedBlueprintMaps[twcAsset.mapBlueprintLayers[m].guid.ToString()];
						}
					}
				}
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Return generated blueprint map.
		/// </summary>
		/// <param name="_mapName"></param>
		/// <returns></returns>
		public bool[,] GetMapOutputFromBlueprintLayer(string _mapName)
		{
			for (int i = 0; i < twcAsset.mapBlueprintLayers.Count; i ++)
			{
				if (twcAsset.mapBlueprintLayers[i].layerName == _mapName)
				{
					return twcAsset.mapBlueprintLayers[i].map;
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Return the generated map from a layer
		/// </summary>
		/// <param name="_guid"></param>
		/// <returns></returns>
		public bool[,] GetMapOutputFromBlueprintLayer(System.Guid _guid)
		{
			for (int i = 0; i < twcAsset.mapBlueprintLayers.Count; i ++)
			{
				if (twcAsset.mapBlueprintLayers[i].guid == _guid)
				{
					return twcAsset.mapBlueprintLayers[i].map;
				}
			}
			
			return null;
		}
		
		
		/// <summary>
		/// Return the tile data from a position
		/// </summary>
		/// <param name="_layer"></param>
		/// <param name="_position"></param>
		/// <returns></returns>
		public TileData GetTileData(string _layerName, Vector3 _position)
		{
			TileData _tileData = null;
			var _map = GetGeneratedBlueprintMap(_layerName);
			if (_map == null)
			{
				Debug.Log("NO MAP - " + _layerName);
				return null;
			}
			var _hashMap = _map.GetPositionHashMapKey(_position);
			
			
			if (_map.clusters.ContainsKey(_hashMap))
			{
				Vector2Int _mapIndex = Vector2Int.zero;
				
				if (twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
				{
					_mapIndex = new Vector2Int((int)_position.x, (int)_position.z);
				}
				else
				{
					_mapIndex = new Vector2Int((int)_position.x, (int)_position.y);
				}
				
				
				if (_map.clusters[_hashMap].ContainsKey(_mapIndex))
				{
					_tileData = _map.clusters[_hashMap][_mapIndex];
					
				}
			}
		
			
			return _tileData;
		}
		
		
		
		#region clusters
		public GameObject AddCluster(string _layerName, Guid _layerGuid, int _clusterKey)
		{
			var _key = _layerName + "_Cluster_" + _clusterKey;
			GameObject _obj = null;
			
		
			var _cluster = FindCluster(_layerGuid, _clusterKey);
			if (_cluster != null)
			{
				DestroyImmediate(_cluster);
			}
			
			_obj = new GameObject(_key);
			
			var _clusterComponent = _obj.AddComponent<ClusterIdentifier>();
			//_clusterComponent.layerName = _layerName;
			_clusterComponent.clusterID = _clusterKey;
			_clusterComponent.layerGuid = _layerGuid;
				
				
			return _obj;
		}
		
		
		public void DestroyAllClusters(Guid _layerGuid)
		{
			
			// Find cluster in scene
			var _clusters = worldObject.GetComponentsInChildren<ClusterIdentifier>();

			for (int c = 0; c < _clusters.Length; c ++)
			{
				if (_clusters[c].layerGuid == _layerGuid)
				{
					DestroyImmediate(_clusters[c].gameObject);
				}
			}
		}
		
		public GameObject FindCluster(Guid _layerGuid, int _clusterID)
		{
			// Find cluster in scene
			var _clusters = worldObject.GetComponentsInChildren<ClusterIdentifier>();
			for (int c = 0; c < _clusters.Length; c ++)
			{
				if (_clusters[c].layerGuid == _layerGuid && _clusters[c].clusterID == _clusterID)
				{
					return _clusters[c].gameObject;
				}
			}
			
			return null;
		}
		
		
		
		public GameObject AddLayerObject(string _layerName, Guid _layerGuid)
		{
			
			GameObject _layerObject = FindLayer(_layerGuid);
		
			if (_layerObject == null)
			{
			
				_layerObject = new GameObject(_layerName + "_layer");
				
				var _layerObjectComponent = _layerObject.AddComponent<LayerIdentifier>();
				_layerObjectComponent.assignedLayer = _layerGuid;
			
			}


			// Reset position
			_layerObject.transform.localPosition = Vector3.zero;
			_layerObject.transform.localScale = new Vector3(1,1,1);
			//_layerObject.transform.localRotation = Quaternion.identity;
			
			if (twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			{
				_layerObject.transform.localRotation = Quaternion.identity;
			}
			else 
			{
				_layerObject.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
			}
			
			
			//var _cellSize = (twcAsset.cellSize * 0.5f) + 0.5f;
			//var _cellOffset = ((_cellSize * 2f) / 4f) * twcAsset.cellSize;
			
			//if (twcAsset.cellSize <= 1)
			//{
			//	_cellOffset = twcAsset.cellSize;
			//}
			
			return _layerObject;
		}
		
		GameObject FindLayer(Guid _guid)
		{
			var _layers = worldObject.GetComponentsInChildren<LayerIdentifier>();
			for (int i = 0; i < _layers.Length; i ++)
			{
				if (_layers[i].assignedLayer == _guid)
				{
					return _layers[i].gameObject;
				}
			}
			 
			return null;
		}
		
		public void DestroyLayer(Guid _layerGuid)
		{
			var _layers = worldObject.GetComponentsInChildren<LayerIdentifier>();
			for (int c = 0; c < _layers.Length; c ++)
			{
				if (_layers[c].assignedLayer == _layerGuid)
				{
					DestroyImmediate(_layers[c].gameObject);
				}
			}
			
		}
		#endregion
		
		
		
		
		#region savingAndLoading
		// SAVE AND LOAD
		//////////////////
		
		/// <summary>
		/// Save blueprint layer stack
		/// </summary>
		/// <param name="_filePath"></param>
		public void SaveBlueprintStack(string _filePath)
		{		
			
			TileWorldCreatorSaveableData _saveable = new TileWorldCreatorSaveableData(twcAsset);
			var bytes = TWC.OdinSerializer.SerializationUtility.SerializeValue(_saveable, DataFormat.Binary);
		
		
			System.IO.File.WriteAllBytes(_filePath, bytes);
		}
		
		
		/// <summary>
		/// Load the blueprint layer stack and execute all blueprint layers
		/// </summary>
		/// <param name="_filePath"></param>
		public void LoadBlueprintStackAndExecute(string _filePath)
		{
			if (!System.IO.File.Exists(_filePath)) return;
		
			byte[] bytes = System.IO.File.ReadAllBytes(_filePath);
			
			TileWorldCreatorSaveableData _saveable = TWC.OdinSerializer.SerializationUtility.DeserializeValue<TileWorldCreatorSaveableData>(bytes, DataFormat.Binary);
			twcAsset = _saveable.AssignToAsset(twcAsset);
			
			generatedBlueprintMaps = new Dictionary<string, WorldMap>();	
			ExecuteAllBlueprintLayers();
		}
		
		/// <summary>
		/// Load the blueprint layer stack
		/// </summary>
		/// <param name="_filePath"></param>
		public void LoadBlueprintStack(string _filePath)
		{
			if (!System.IO.File.Exists(_filePath)) return;
		
			byte[] bytes = System.IO.File.ReadAllBytes(_filePath);


			TileWorldCreatorSaveableData _saveable = TWC.OdinSerializer.SerializationUtility.DeserializeValue<TileWorldCreatorSaveableData>(bytes, DataFormat.Binary);
			twcAsset = _saveable.AssignToAsset(twcAsset);
			
		}
		//////////////////
		#endregion
		
		
		
		/// <summary>
		/// Execute all layers in the blueprint stack
		/// </summary>
		public void ExecuteAllBlueprintLayers()
		{
			_executedBlueprintLayersCount = 0;
	
	
			int _seed = System.Environment.TickCount;
			if (twcAsset.useRandomSeed)
			{
				_seed = twcAsset.randomSeed;
			}
			
			currentSeed = _seed;
			lastSeed = currentSeed;
			UnityEngine.Random.InitState(_seed);
			
			
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				ExecuteBlueprintLayer(m);
			}
			
			//if (OnBlueprintLayersComplete != null)
			//{
			//	OnBlueprintLayersComplete(this);
			//}
		}
		
		/// <summary>
		/// Execute specific layer in the blueprint stack
		/// </summary>
		/// <param name="_layerName"></param>
		public void ExecuteBlueprintLayer(string _layerName)
		{
			_executedBlueprintLayersCount = 0;
	
	
			int _seed = System.Environment.TickCount;
			if (twcAsset.useRandomSeed)
			{
				_seed = twcAsset.randomSeed;
			}
			
			currentSeed = _seed;
			lastSeed = currentSeed;
			UnityEngine.Random.InitState(_seed);
			
			
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					ExecuteBlueprintLayer(m);
				}
			}
			
			//if (OnBlueprintLayersComplete != null)
			//{
			//	OnBlueprintLayersComplete(this);
			//}
		}
		
		void ExecuteBlueprintLayer(int _layerIndex)
		{
	
			
			if (!twcAsset.mapBlueprintLayers[_layerIndex].active)
				return;
				
			bool[,] _map = new bool[twcAsset.mapWidth, twcAsset.mapHeight];
	
			// Generate new random seed for every single layer
			if (!twcAsset.useRandomSeed && twcAsset.useNewRandomSeedForEveryLayer)
			{
				int _seed = System.Environment.TickCount + UnityEngine.Random.Range(0, 100000);
				currentSeed = _seed;
				lastSeed = currentSeed;
				
				Debug.Log("1 " + currentSeed);
				UnityEngine.Random.InitState(_seed);
			}
			
		
			
			if (!twcAsset.useRandomSeed && !twcAsset.useNewRandomSeedForEveryLayer)
			{
				currentSeed = lastSeed;	
			}
			
			// Random seed override is active for this blueprint layer
			if (twcAsset.mapBlueprintLayers[_layerIndex].randomSeedOverride)
			{
				
				if (twcAsset.mapBlueprintLayers[_layerIndex].useCustomSeed)
				{
					lastSeed = currentSeed;
					currentSeed = twcAsset.mapBlueprintLayers[_layerIndex].customLayerRandomSeed;
				}
				else
				{
					lastSeed = currentSeed;
					currentSeed = System.Environment.TickCount;
				}
				
				
				UnityEngine.Random.InitState(currentSeed);
				
			}
			
			if (!twcAsset.mapBlueprintLayers[_layerIndex].randomSeedOverride)
			{
				currentSeed = lastSeed;
			}
			
			
			bool _mapResultFailed = false;
			
			if (twcAsset.mapHeight <= 0 || twcAsset.mapWidth <= 0)
			{
				Debug.LogWarning("Map size is 0");
				return;
			}
			
			for (int s = 0; s < twcAsset.mapBlueprintLayers[_layerIndex].stack.Count; s ++)
			{	
				var _action = twcAsset.mapBlueprintLayers[_layerIndex].stack[s].action as TWCBlueprintAction;
				if (_action.active)
				{
					
					// EXECUTE STACK
					_map = twcAsset.mapBlueprintLayers[_layerIndex].stack[s].action.Execute(_map, this);
				
				
					// Check after each stack if map has at least on tile which is true.
					// if not map blueprint has failed resulting in a map without any results.
					bool _result = false;
					for (int x = 0; x < _map.GetLength(0); x ++)
					{
						for (int y = 0; y < _map.GetLength(1); y ++)
						{
							if (_map[x,y])
							{
								_result = true;
							}
							
							if (_result)
							{
								break;
							}
						}
						
						if (_result)
						{
							
							
							break;
						}
					}
					
					if (!_result)
					{
						//Debug.LogWarning("Map stack failed");
						_action.resultFailed = true;
						_mapResultFailed = true;
					}
					else
					{
						_action.resultFailed = false;
					}
					
					
				}
				
				if (!twcAsset.mapBlueprintLayers[_layerIndex].randomSeedOverride)
				{
					currentSeed = lastSeed;
				}
			}
			
			
			twcAsset.mapBlueprintLayers[_layerIndex].mapResultFailed = _mapResultFailed;
			twcAsset.mapBlueprintLayers[_layerIndex].map = new bool[twcAsset.mapWidth, twcAsset.mapHeight];
			twcAsset.mapBlueprintLayers[_layerIndex].map = _map;
			    	    
			CreateFinalBlueprintMap(twcAsset.mapBlueprintLayers[_layerIndex]);
			
			    	    
			// generate preview texture
			if (_layerIndex-1 >= 0 && twcAsset.mergePreviewTextures)
			{
				twcAsset.mapBlueprintLayers[_layerIndex].previewTextureMap = UpdatePreviewTexture(twcAsset.mapBlueprintLayers[_layerIndex].map, twcAsset.mapBlueprintLayers[_layerIndex].previewColor, twcAsset.mapBlueprintLayers[_layerIndex - 1].previewTextureMap);
			}
			else
			{
				twcAsset.mapBlueprintLayers[_layerIndex].previewTextureMap = UpdatePreviewTexture(twcAsset.mapBlueprintLayers[_layerIndex].map, twcAsset.mapBlueprintLayers[_layerIndex].previewColor, null);
			}
			
			
			executedBlueprintLayersCount += 1;
		}
		
		
			
		void CreateFinalBlueprintMap(TileWorldCreatorAsset.BlueprintLayerData _layerData)
		{
			
			bool[,] _map = _layerData.map;
			bool[,] _subdMap = new bool[1,1];
			
			// Subdivide the map so that a single tile (1x1) turns in to four tiles (2x2).
			_subdMap = TileWorldCreatorUtilities.SubdivideMap(_map);
		

			// Final subdivided map
			var _finalMapSubD = AddNewGeneratedBlueprintMap(_layerData.guid.ToString(), _subdMap.GetLength(0), _subdMap.GetLength(1));
			
			// Final unsubdivided map
			var _finalMap = AddNewGeneratedBlueprintMap(_layerData.guid.ToString() + "_UNSUBD", _map.GetLength(0), _map.GetLength(1));
			
			
			// Add data to our final map
			for (int x = 0; x < _subdMap.GetLength(0); x ++)
			{
				for (int y = 0; y < _subdMap.GetLength(1); y ++)
				{
					
					if (_subdMap[x,y])
					{
						AddTileData(_finalMapSubD, _subdMap, x, y, true);
					}
					
				}
			}
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
					
					if (_map[x,y])
					{
						AddTileData(_finalMap, _map, x, y, false);
					}
				}
			}
		}
		
		
		void AddTileData(WorldMap _tilemap, bool[,] _map, int _x, int _y, bool _isSubd)
		{
			TileData _tileData = new TileData();
					    
			var _location = TileWorldCreatorUtilities.GetNeighboursLocation(_map, _x, _y);
			var _yRotation = TileWorldCreatorUtilities.ReturnTileRotation(_location);
			var _neigboursCount = TileWorldCreatorUtilities.CountNeighbours(_x, _y, _map);
			Quaternion _rotation = Quaternion.Euler(0, _yRotation, 0);

			
            var _position = new Vector3(_x, 0, _y);

			_tileData.position = _position;
			_tileData.rotation = _rotation;
			_tileData.yRotation = _yRotation;
			_tileData.neighboursLocation = _location;
			
			
			// CORNER
			if (_neigboursCount == 4)
			{
				_tileData.tileType = TileData.TileType.exteriorCorner;
			}
			// CORNER
			if (_neigboursCount == 5)
			{
				_tileData.tileType = TileData.TileType.exteriorCorner;
			}
			// CORNER OR STRAIGHT
			if (_neigboursCount == 6)
			{
				// CORNER
				if (_location.northWest && _location.north && _location.northEast && _location.east && _location.southEast)
				{			
					_tileData.tileType = TileData.TileType.exteriorCorner;
				}
				// CORNER
				if (_location.southWest && _location.south && _location.southEast && _location.east && _location.northEast)
				{
					_tileData.tileType = TileData.TileType.exteriorCorner;
				}
				// CORNER
				if (_location.southWest && _location.west && _location.northWest && _location.north && _location.northEast)
				{
					_tileData.tileType = TileData.TileType.exteriorCorner;
				}
				// CORNER
				if (_location.southWest && _location.south && _location.southEast && _location.west && _location.northWest)
				{
					_tileData.tileType = TileData.TileType.exteriorCorner;
				}
								
				// STRAIGHT
				if (_location.west && _location.east && _location.southWest && _location.south && _location.southEast)
				{
					_tileData.tileType = TileData.TileType.edge;
				}
				// STRAIGHT
				if (_location.west && _location.east && _location.northWest && _location.north && _location.northEast)
				{
					_tileData.tileType = TileData.TileType.edge;
				}
				// STRAIGHT
				if (_location.west && _location.northWest && _location.north && _location.southWest && _location.south)
				{
					_tileData.tileType = TileData.TileType.edge;
				}
				// STRAIGHT
				if (_location.east && _location.north && _location.northEast && _location.south && _location.southEast)
				{
					_tileData.tileType = TileData.TileType.edge;
				}
			}
							
			// INTERIOR CORNER
			if (_neigboursCount == 8)
			{
				_tileData.tileType = TileData.TileType.interiorCorner;
			}
			// FULL
			if (_neigboursCount == 9)
			{
				_tileData.tileType = TileData.TileType.fill;
			}
			
						
			_tilemap.AddTile(_position, _tileData);
		}
		
		
		
		/// <summary>
		/// Execute specific layer in the build layer stack
		/// </summary>
		/// <param name="_layerName"></param>
		public void ExecuteBuildLayer(string _layerName, bool _forceRebuild)
		{
			_executedBuildLayersCount = 0;
			executeAllBuildLayers = false;
			buildResultNoChanges = false;
			
			forceRebuild = _forceRebuild;
			
			for (int i = 0; i < twcAsset.mapBuildLayers.Count; i ++)
			{
				if (twcAsset.mapBuildLayers[i].layerName == _layerName)
				{
					twcAsset.mapBuildLayers[i].Execute(this, _forceRebuild);
				}
			}
		}
		
		/// <summary>
		/// Execute specific layer in the build layer stack
		/// </summary>
		/// <param name="_layerName"></param>
		/// <param name="_forceRebuild"></param>
		public void ExecuteBuildLayer(Guid _guid, bool _forceRebuild)
		{
			_executedBuildLayersCount = 0;
			executeAllBuildLayers = false;
			buildResultNoChanges = false;
			
			forceRebuild = _forceRebuild;
			
			for (int i = 0; i < twcAsset.mapBuildLayers.Count; i ++)
			{
				if (twcAsset.mapBuildLayers[i].guid == _guid)
				{
					twcAsset.mapBuildLayers[i].Execute(this, _forceRebuild);
				}
			}
		}
		
		
		/// <summary>
		/// Execute all layers from the build stack.
		/// </summary>
		/// <param name="_forceCompleteRebuild"></param>
		public void ExecuteAllBuildLayers(bool _forceCompleteRebuild)
		{
			_executedBuildLayersCount = 0;
			executeAllBuildLayers = true;
			buildResultNoChanges = false;
			
			forceRebuild = _forceCompleteRebuild;
			
			if (twcAsset.mapBuildLayers[_executedBuildLayersCount].active)
			{
				twcAsset.mapBuildLayers[_executedBuildLayersCount].Execute(this, _forceCompleteRebuild);
			}
			else
			{
				executedBuildLayersCount += 1;
			}
		}
		
		/// <summary>
		/// Execute all layers from the build stack and set a priority build layer which will be builded first.
		/// </summary>
		/// <param name="_priorityBuildLayerName"></param>
		/// <param name="_forceCompleteRebuild"></param>
		public void ExecuteAllBuildLayers(string _priorityBuildLayerName, bool _forceCompleteRebuild)
		{
			_executedBuildLayersCount = 0;
			executeAllBuildLayers = true;
			buildResultNoChanges = false;
			
			priorityBuild = true;
			
			forceRebuild = _forceCompleteRebuild;
			
			// Copy current list to a temp list
			tmpBuildLayers = new List<TWCBuildLayer>();
			foreach(var item in twcAsset.mapBuildLayers)
			{
				var _originalGuid = item.guid;
				
				tmpBuildLayers.Add(item.Clone());
				tmpBuildLayers[tmpBuildLayers.Count-1].guid = _originalGuid;
			}
			
			// Move priority build layer to the top
			for (int i = 0; i < twcAsset.mapBuildLayers.Count; i ++)
			{
				if (twcAsset.mapBuildLayers[i].layerName == _priorityBuildLayerName)
				{
					var _b = twcAsset.mapBuildLayers[i];
					twcAsset.mapBuildLayers.RemoveAt(i);
					twcAsset.mapBuildLayers.Insert(0, _b);
				}
			}
			
			
			if (twcAsset.mapBuildLayers[_executedBuildLayersCount].active)
			{
				twcAsset.mapBuildLayers[_executedBuildLayersCount].Execute(this, _forceCompleteRebuild);
			}
			else
			{
				executedBuildLayersCount += 1;
			}
		}
		
		
		void ExecuteNextBuildLayer(bool _forceCompleteRebuild)
		{
			if (_executedBuildLayersCount < twcAsset.mapBuildLayers.Count)
			{
				if (twcAsset.mapBuildLayers[_executedBuildLayersCount].active)
				{
					twcAsset.mapBuildLayers[_executedBuildLayersCount].Execute(this, _forceCompleteRebuild);
				}
				else
				{
					executedBuildLayersCount += 1;
				}
			}
		}
		
		
		/// <summary>
		/// Update the preview texture based on the map and color
		/// </summary>
		/// <param name="map"></param>
		/// <param name="color"></param>
		/// <param name="previousTexture"></param>
		/// <returns></returns>
		public Texture2D UpdatePreviewTexture(bool[,] map, Color color, Texture2D previousTexture)
		{
			var _w = map.GetLength(0);
			var _h = map.GetLength(1);
			
			if (_w > 0 && _h > 0)
			{
				var _sqr = (int)Mathf.Max(_w, _h);
				var _previewTexture = new Texture2D(_sqr, _sqr);
				_previewTexture.wrapMode = TextureWrapMode.Clamp;
				_previewTexture.filterMode = FilterMode.Point;
				Color32[] _colors = new Color32[_sqr * _sqr];
				
				if (twcAsset.mergePreviewTextures && previousTexture != null)
				{
					_colors  = previousTexture.GetPixels32();
				}
				
				for (int x = 0; x < _sqr; x ++)
				{
					for (int y = 0; y < _sqr; y ++)
					{
						try
						{
						if (map[x,y])
						{
							_colors[y * _previewTexture.height + x] = new Color(color.r, color.g, color.b, 255f/255f);
						}
						else if (!twcAsset.mergePreviewTextures && previousTexture == null)
						{
							_colors[y * _previewTexture.height + x] = Color.black;
						}
						}
							catch{}
					}
				}
							    		
				_previewTexture.SetPixels32 (_colors);
				_previewTexture.Apply();
							    		
				return _previewTexture;
				
			}
			
			return null;
		}

		
		/// <summary>
		/// Modify map at runtime.
		/// It is important to assign a paint generator to the layer for this to work.
		/// </summary>
		/// <param name="_layerName"></param>
		/// <param name="_x"></param>
		/// <param name="_y"></param>
		/// <param name="_value"></param>
		public void ModifyMap(string _layerName, int _x, int _y, bool _value)
		{
			//Debug.Log("Modify map");
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					for (int s = 0; s < twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
					{
						var _paint = twcAsset.mapBlueprintLayers[m].stack[s].action  as Paint;
						if (_paint != null)
						{
							_paint.ModifyMap(_x, _y, _value, this);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Fill paint map by the value (true or false)
		/// </summary>
		/// <param name="_layerName"></param>
		/// <param name="_value"></param>
		public void FillMap(string _layerName, bool _value)
		{
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					for (int s = 0; s < twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
					{
						var _paint = twcAsset.mapBlueprintLayers[m].stack[s].action  as Paint;
						if (_paint != null)
						{
							_paint.FillMap(_value, this);
						}
					}
				}
			}
		}
		
		
		/// <summary>
		/// Use it with a paint modifier. Copies the map and adds it to the paint modifier to be able to paint on it.
		/// </summary>
		/// <param name="_layerName"></param>
		public void CopyMap(string _layerName)
		{
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					for (int s = 0; s < twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
					{
						var _paint = twcAsset.mapBlueprintLayers[m].stack[s].action  as Paint;
						if (_paint != null)
						{
							_paint.CopyMap(this);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Get action from action stack by its Guid. Get guid from the editor.
		/// </summary>
		/// <param name="_layerName"></param>
		/// <param name="_guid"></param>
		/// <returns></returns>
		public TWCBlueprintAction GetAction(string _layerName, string _guid)
		{
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					for (int s = 0; s < twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
					{
						if ((twcAsset.mapBlueprintLayers[m].stack[s].action  as TWCBlueprintAction).guid.ToString() == _guid)
						{
							return twcAsset.mapBlueprintLayers[m].stack[s].action  as TWCBlueprintAction;
						}
					}
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Get an action from the action stack by its index.
		/// </summary>
		/// <param name="_layerName"></param>
		/// <param name="_stackIndex"></param>
		/// <returns></returns>
		public TWCBlueprintAction GetAction(string _layerName, int _stackIndex)
		{
			for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
			{
				if (twcAsset.mapBlueprintLayers[m].layerName == _layerName)
				{
					if (_stackIndex < twcAsset.mapBlueprintLayers[m].stack.Count)
					{
						return twcAsset.mapBlueprintLayers[m].stack[_stackIndex].action as TWCBlueprintAction;
					}
				}
			}
			
			return null;
		}
	
		/// <summary>
		/// Set a custom random seed for map generation
		/// </summary>
		/// <param name="_seed"></param>
		public void SetCustomRandomSeed(int _seed)
		{
			twcAsset.useRandomSeed = true;
			twcAsset.randomSeed = _seed;
		}
		
		/// <summary>
		/// Disable custom random seed for map generation
		/// </summary>
		public void DisableCustomRandomSeed()
		{
			twcAsset.useRandomSeed = false;
		}
		
		
	    void OnDrawGizmos()
		{
			if (twcAsset == null)
				return;
			
		    for (int m = 0; m < twcAsset.mapBlueprintLayers.Count; m ++)
		    {
			    for (int s = 0; s < twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
			    {
				    var _action = twcAsset.mapBlueprintLayers[m].stack[s].action  as TWCBlueprintAction;
				    if (_action != null)
				    {
					    _action.DrawGizmos();
				    }
			    }
		    }
		    
			DebugDrawClusterCells();
		}
		
		
		void DebugDrawClusterCells()
		{
			if (!twcAsset.useCustomClusterCelSize)
				return;
			
			
			var _clusterSize = twcAsset.customClusterCellSize;
			
			if (_clusterSize <= 0)
			{
				return;
			}
		
			
			var _x = Mathf.CeilToInt((twcAsset.mapWidth / _clusterSize) * 2);
			var _y = Mathf.CeilToInt((twcAsset.mapHeight / _clusterSize) * 2);
			
			for (int x = 0; x < _x + 2; x ++)
			{
				for (int y = 0; y < _y + 2; y ++)
				{
					
					Vector3 _pos = Vector3.zero;
					
					if (twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
					{
						_pos = new Vector3(transform.localPosition.x + (x * _clusterSize + (_clusterSize * 0.5f)), transform.localPosition.y + 0.2f, transform.localPosition.z + (y * _clusterSize + (_clusterSize * 0.5f)));
						
						if (IsVisibleByCamera(_pos))
						{
							Gizmos.DrawWireCube(_pos, new Vector3(_clusterSize, 0.05f, _clusterSize));
						}
					}
					else
					{
						_pos = new Vector3(transform.localPosition.x + (x * _clusterSize + (_clusterSize * 0.5f)), transform.localPosition.y + (y * _clusterSize + (_clusterSize * 0.5f)), transform.localPosition.z - 0.2f);	
						
						if (IsVisibleByCamera(_pos))
						{
							Gizmos.DrawWireCube(_pos, new Vector3(_clusterSize - 0.1f, _clusterSize - 0.1f, 0.05f));
						}
					}
				}
			}
		}
		
				
		bool IsVisibleByCamera(Vector3 _pos)
		{
			Vector3 viewPos = Vector3.zero;
			if (sceneViewCamera == null)
			{				
				if (Camera.current != null)
				{
					sceneViewCamera = Camera.current;
				}
			}
			
			if (sceneViewCamera != null)
			{
				viewPos = Camera.current.WorldToViewportPoint(_pos);		
			}
			
			if (viewPos.x >= 0.0f && viewPos.x <= 1f && viewPos.y >= 0.0f && viewPos.y <= 1f)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	 
	}

}
