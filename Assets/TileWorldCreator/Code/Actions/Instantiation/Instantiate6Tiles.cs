using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using TWC.Utilities;
using TWC.editor;

namespace TWC.Actions
{
	
	[System.Serializable]
	[ActionNameAttribute(Name="3D 6-Tiles")]
	public class Instantiate6Tiles : TWCBuildLayer
	{	
		public int selectedLayerIndex;
		
		
		public Guid initialOrientationLayerGuid;
		public bool useOnlyOrientationLayer;
		
	
		public class TilePresets
		{
			public float rndSelectionWeight = 1f;
			public bool foldout;
			
			#if UNITY_EDITOR
			public Editor editor;
			#endif
			
			public TileWorldCreator6TilesPreset preset;
		}
		
		public List<TilePresets> tiles;
		
		public bool mergeTiles = true;
		public bool keepPrefabConnection = false;
		public bool scaleTileByCellSize = true;
		
		public enum ColliderTypeVariantA
		{
			none,
			MeshCollider,
			TileCollider
		}
		
		public enum ColliderTypeVariantB
		{
			none,
			TileCollider
		}
		
		public float tileColliderHeightOffset; // The height position from where the colliders height should be added.
		public float tileColliderHeight = 1; // The actual height of the collider
		public float tileColliderBorderOffset;
		
		public ColliderTypeVariantA colliderTypeVariantA;
		public ColliderTypeVariantB colliderTypeVariantB;
	
		public LayerMask assignLayer;
		public bool setShadowCastingMode;
		public ShadowCastingMode shadowCasting = ShadowCastingMode.On;
		
		public Vector3 globalPositionOffset;
		public Vector3 globalRotationOffset;
		public Vector3 globalScalingOffset;
		
		public List<string> ignoreLayers = new List<string>();
		
		WorldMap tileMap;
		WorldMap tileMapTMP;
		
		private static Texture2D iconEdgeTile;
		private static Texture2D iconExteriorCornerTile;
		private static Texture2D iconInteriorCornerTile;
		private static Texture2D iconFillTile;
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public TileWorldCreatorAsset twcAsset;
		}
		
		public class GenericMenuIgnoreLayerData
		{
			public int selectedIndex;
			public int ignoreLayerListIndex;
			public TileWorldCreatorAsset twcAsset;
		}
		
		public override TWCBuildLayer Clone()
		{
			var _r = new Instantiate6Tiles();
			
			_r.guid = Guid.NewGuid();
			_r.layerName = this.layerName;
			_r.ignoreLayers = new List<string>();
			
			for (int i = 0; i < this.ignoreLayers.Count; i ++)
			{
				_r.ignoreLayers.Add(this.ignoreLayers[i]);
			}
			
			_r.selectedLayerIndex = this.selectedLayerIndex;
			_r.assignedGenerationLayerGuid = this.assignedGenerationLayerGuid;
			
			_r.initialOrientationLayerGuid = this.initialOrientationLayerGuid;
			_r.useOnlyOrientationLayer = this.useOnlyOrientationLayer;
			
			_r.tiles = new List<TilePresets>();
			
			if (this.tiles != null)
			{
				for (int i = 0; i < this.tiles.Count; i ++)
				{
					TilePresets _p = new TilePresets();
					_p.preset = this.tiles[i].preset;
					
					_r.tiles.Add(_p);
				}
			}
			
			_r.mergeTiles = this.mergeTiles;
			_r.keepPrefabConnection = this.keepPrefabConnection;
			_r.scaleTileByCellSize = this.scaleTileByCellSize;
			_r.assignLayer = this.assignLayer;
			_r.setShadowCastingMode = this.setShadowCastingMode;
			_r.shadowCasting = this.shadowCasting;
			
			
			_r.colliderTypeVariantA = this.colliderTypeVariantA;
			_r.colliderTypeVariantB = this.colliderTypeVariantB;
			
			_r.globalPositionOffset = this.globalPositionOffset;
			_r.globalRotationOffset = this.globalRotationOffset;
			_r.globalScalingOffset = this.globalScalingOffset;
			
			return _r as TWCBuildLayer;
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(TileWorldCreatorAsset _asset)
		{
			if (iconEdgeTile == null)
			{
				LoadResources();
			}
			
		
			layerName = EditorGUILayout.TextField("Layer name", layerName); 
		    
			var _names =  EditorUtilities.GetAllGenerationLayerNames(_asset);
			var _blueprintLayerName = "";
			var _blueprintLayerData = _asset.GetBlueprintLayerData(assignedGenerationLayerGuid);
			
			var _initOrientationLayerName = "";
			var _initOrientationLayerData = _asset.GetBlueprintLayerData(initialOrientationLayerGuid);
			
			if (_blueprintLayerData != null)
			{
				_blueprintLayerName = _blueprintLayerData.layerName;
			}
			
			if (_initOrientationLayerData != null)
			{
				_initOrientationLayerName = _initOrientationLayerData.layerName;
			}
			
			using (new GUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("Blueprint layer", GUILayout.MaxWidth(150));
				
				if (EditorGUILayout.DropdownButton(new GUIContent(_blueprintLayerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
						
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
						_data.twcAsset = _asset;
						menu.AddItem(new GUIContent(_names[n]), false, AssignBlueprintLayer, _data);
					}
						
					menu.ShowAsContext();
				}
			}
			
			using (new GUILayout.HorizontalScope("Box"))
			{
				EditorGUILayout.LabelField ("Initial orientation layer", GUILayout.MaxWidth(150));
			
				if (EditorGUILayout.DropdownButton(new GUIContent(_initOrientationLayerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
					
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
						_data.twcAsset = _asset;
						menu.AddItem(new GUIContent(_names[n]), false, AssignInitalOrientationLayer, _data);
					}
						
					menu.ShowAsContext();
				}
				
				if (GUILayout.Button("clear"))
				{
					initialOrientationLayerGuid = Guid.Empty;
				}
				
				useOnlyOrientationLayer = EditorGUILayout.Toggle("Use only orientation layer", useOnlyOrientationLayer);
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				scaleTileByCellSize = EditorGUILayout.Toggle("Scale tile by cell size:", scaleTileByCellSize);
			}
		    
			using (new GUILayout.VerticalScope("Box"))
			{
				globalPositionOffset = EditorGUILayout.Vector3Field("Global position offset", globalPositionOffset);
				globalRotationOffset = EditorGUILayout.Vector3Field("Global rotation offset", globalRotationOffset);
				globalScalingOffset = EditorGUILayout.Vector3Field("Global scaling offset", globalScalingOffset);
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				mergeTiles = EditorGUILayout.Toggle(new GUIContent("Merge tiles", "Merge tiles into clusters, this is highly recommended for larger maps."), mergeTiles);
		
				if (!mergeTiles)
				{
					keepPrefabConnection = EditorGUILayout.Toggle(new GUIContent("Keep prefab connection", "Keep the prefab connection for each tile. (Only in editor)"), keepPrefabConnection);
				}
			}
			
			if (mergeTiles)
			{
				using (new GUILayout.VerticalScope("Box"))
				{
					colliderTypeVariantA = (ColliderTypeVariantA)EditorGUILayout.EnumPopup("Collision type", colliderTypeVariantA);
					
					if (colliderTypeVariantA == ColliderTypeVariantA.TileCollider)
					{			
						tileColliderHeight = EditorGUILayout.FloatField("Collider height", tileColliderHeight);
						tileColliderHeightOffset = EditorGUILayout.FloatField("Collider height offset", tileColliderHeightOffset);
						tileColliderBorderOffset = EditorGUILayout.Slider("Collider border offset", tileColliderBorderOffset, -1f, 1f);
					}
				}
			}
			else
			{
				using (new GUILayout.VerticalScope("Box"))
				{
					colliderTypeVariantB = (ColliderTypeVariantB)EditorGUILayout.EnumPopup("Collision type", colliderTypeVariantB);
					
					if (colliderTypeVariantB == ColliderTypeVariantB.TileCollider)
					{
						tileColliderHeight = EditorGUILayout.FloatField("Collider height", tileColliderHeight);
						tileColliderHeightOffset = EditorGUILayout.FloatField("Collider height offset", tileColliderHeightOffset);
						tileColliderBorderOffset = EditorGUILayout.Slider("Collider border offset", tileColliderBorderOffset, -1f, 1f);
					}
				}
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				using (new GUILayout.HorizontalScope())
				{
					assignLayer = EditorGUILayout.LayerField("Assign layer", assignLayer);
				}
				
				setShadowCastingMode = EditorGUILayout.Toggle("Set shadow casting mode", setShadowCastingMode);
				
				using (new GUILayout.HorizontalScope())
				{
					if (setShadowCastingMode)
					{
					GUILayout.Space(15);
					GUILayout.Label("Shadow casting");
					shadowCasting = (ShadowCastingMode)EditorGUILayout.EnumPopup(shadowCasting);
					}
				}
			}
			
			GUILayout.Space(5);
			
			using (new GUILayout.VerticalScope("Box"))
			{
				
				GUILayout.Label("Tiles Presets", "boldLabel");
				
				if (tiles == null)
				{
					tiles = new List<TilePresets>();
				}
				
				if (GUILayout.Button("Add Tiles Preset"))
				{
					tiles.Add(new TilePresets());
				}
				
				for (int t = 0; t < tiles.Count; t ++)
				{
					using (new GUILayout.HorizontalScope())
					{
					
						if (tiles[t].preset != null)
						{
							tiles[t].foldout = EditorGUILayout.Foldout(tiles[t].foldout, "");
						}
						
						GUILayout.Label("Weight");
						tiles[t].rndSelectionWeight = EditorGUILayout.Slider(tiles[t].rndSelectionWeight, 0f, 1f);
						
						EditorGUI.BeginChangeCheck();
						tiles[t].preset = (TileWorldCreator6TilesPreset)EditorGUILayout.ObjectField(tiles[t].preset, typeof(TileWorldCreator6TilesPreset), false);
						if (EditorGUI.EndChangeCheck())
						{
							if (tiles[t].preset != null)
							{
								tiles[t].editor = Editor.CreateEditor(tiles[t].preset);
							}
						}
						
						
						if (GUILayout.Button("x", GUILayout.Width(20)))
						{
							tiles.RemoveAt(t);
						}
					}
					
					if (tiles[t].foldout && tiles[t].preset != null)
					{
						if (tiles[t].editor == null)
						{
							tiles[t].editor = Editor.CreateEditor(tiles[t].preset);
						}
						
						tiles[t].editor.OnInspectorGUI();
					}
				
				}
			}
			
		  
	    	
			using (new GUILayout.VerticalScope("Box"))
			{
				GUILayout.Label("Ignore layers", "boldLabel");
				
				if (GUILayout.Button("Add ignore layer"))
				{
					ignoreLayers.Add("");	
				}
				
				var _availableLayerNames =  EditorUtilities.GetAllGenerationLayerNames(_asset);
				
				for (int i = 0; i < ignoreLayers.Count; i ++)
				{
					using (new GUILayout.HorizontalScope())
					{
					
						var _selectedIgnoreLayerName = "";
						Guid _guid = Guid.Empty;
						Guid.TryParse(ignoreLayers[i], out _guid);
						if (_guid != Guid.Empty)
						{
							var _selectedIgnoreLayerData = _asset.GetBlueprintLayerData(_guid);
							if (_selectedIgnoreLayerData != null)
							{
								_selectedIgnoreLayerName = _selectedIgnoreLayerData.layerName;
							}
						}
						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField("Ignore Layer");
							if (EditorGUILayout.DropdownButton(new GUIContent(_selectedIgnoreLayerName), FocusType.Keyboard))
							{
								GenericMenu menu = new GenericMenu();
						
								for (int n = 0; n < _availableLayerNames.Length; n ++)
								{
									var _data = new GenericMenuIgnoreLayerData();
									_data.selectedIndex = n;
									_data.twcAsset = _asset;
									_data.ignoreLayerListIndex = i;
									menu.AddItem(new GUIContent(_availableLayerNames[n]), false, AssignIgnoreLayer, _data);
								}
						
								menu.ShowAsContext();
							}
						}
						
						
						if (GUILayout.Button("x", GUILayout.Width(20)))
						{
							ignoreLayers.RemoveAt(i);	
						}
					}
				}
			}
			

		}
		#endif
	
		public override void Execute(TileWorldCreator _twc, bool _forceRebuild)
		{
			tileMap = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_UNSUBD");
		
			if (tileMap != null)
			{
				tileMapTMP = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_UNSUBD" + "_TMP");
				
				// Tilemap has no clusters therefore the map is empty and we can destroy the layer object
				if (tileMap.clusters.Keys.Count <= 0)
				{
					var _layerObject = _twc.AddLayerObject(layerName, guid);
					MonoBehaviour.DestroyImmediate(_layerObject);
					
					// to make sure tileworldcreator executes next layer increase execution build layers by one
					_twc.executedBuildLayersCount += 1;
				}
				else
				{
					if (!Application.isPlaying)
					{
						#if UNITY_EDITOR
						TWC.Utilities.EditorCoroutines.Execute(InstantiateIE(_twc, _forceRebuild));
						#endif
					}
					else
					{
						_twc.StartCoroutine(InstantiateIE(_twc, _forceRebuild));
					}
				}
			}
			else
			{
				Debug.LogWarning("No blueprint map found, please make sure you have assigned a blueprint layer to your build layer");
			}

		}
		
		
		void LoadResources()
		{
			iconEdgeTile = EditorUtilities.LoadIcon("edgeTile.png");
			iconExteriorCornerTile = EditorUtilities.LoadIcon("exteriorCornerTile.png");
			iconInteriorCornerTile = EditorUtilities.LoadIcon("interiorCornerTile.png");
			iconFillTile = EditorUtilities.LoadIcon("fillTile.png");
		}
		
		
			
		void AssignBlueprintLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			assignedGenerationLayerGuid = _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}
		
		void AssignInitalOrientationLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			initialOrientationLayerGuid = _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}
		
		
		void AssignIgnoreLayer(object _data)
		{
			var _d = _data as GenericMenuIgnoreLayerData;
			//Debug.Log("Assign ignore layer " + _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].layerName);
			ignoreLayers[_d.ignoreLayerListIndex] = _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid.ToString();
		}
		
		
		IEnumerator InstantiateIE(TileWorldCreator _twc, bool _forceRebuild)
		{
			List<int> modifiedClusters = new List<int>(); 
		
			var _layerObject = _twc.AddLayerObject(layerName, guid);
			_layerObject.transform.SetParent(_twc.worldObject.transform, false);
			
			
			_layerObject.transform.localPosition = new Vector3(
				_layerObject.transform.localPosition.x + globalPositionOffset.x,
				_layerObject.transform.localPosition.y + globalPositionOffset.y,
				_layerObject.transform.localPosition.z + globalPositionOffset.z
			);
								
					
			_layerObject.transform.localScale = new Vector3(
				_layerObject.transform.localScale.x + globalScalingOffset.x,
				_layerObject.transform.localScale.y + globalScalingOffset.y,
				_layerObject.transform.localScale.z + globalScalingOffset.z
			);
					
					
			_layerObject.transform.localRotation = Quaternion.Euler(new Vector3(
				_layerObject.transform.localRotation.eulerAngles.x + globalRotationOffset.x,
				_layerObject.transform.localRotation.eulerAngles.y + globalRotationOffset.y,
				_layerObject.transform.localRotation.eulerAngles.z + globalRotationOffset.z
			));
			
			
			
			// Compare New map and last temp map
			if (tileMapTMP != null)
			{
				foreach (var _cluster in tileMap.clusters.Keys)
				{
					var _addCluster = false;
					var _buildNeigbouringClusters = true;
					var _intCluster = -1;
					Vector2Int _quadrantPosition = Vector2Int.zero;
					
					foreach (var _position in tileMap.clusters[_cluster].Keys)
					{
						if (tileMapTMP.clusters.ContainsKey(_cluster))
						{
							if (tileMapTMP.clusters[_cluster].ContainsKey(_position))
							{
							}
							else
							{	
								var _clr = tileMap.GetPositionHashMapKey(new Vector3(_position.x, 0, _position.y));
				
								_addCluster = true;
								_intCluster = _clr;
								
								// Check if we should update all adjacent clusters as well.
								// To do this we look if the tile position is located on the border of a cluster
								_quadrantPosition = tileMap.GetQuadrantPosition(_position);
								
								var _minQuadrantPosition = new Vector2Int(_quadrantPosition.x * tileMap.ClusterCellSize, _quadrantPosition.y * tileMap.ClusterCellSize);
								var _maxQuadrantPosition = new Vector2Int(_minQuadrantPosition.x + tileMap.ClusterCellSize, _minQuadrantPosition.y + tileMap.ClusterCellSize);
						
								if (_position.x > _minQuadrantPosition.x + 2 && _position.x < _maxQuadrantPosition.x - 2 && _position.y > _minQuadrantPosition.y + 2 && _position.y < _maxQuadrantPosition.y - 2)
								{
									_buildNeigbouringClusters = false;
								}
							}
						}
						else
						{
							_addCluster = true;
							_intCluster = _cluster;
						}
					}
					
					if (_addCluster)
					{
						if (!_buildNeigbouringClusters)
						{
							modifiedClusters.Add(_intCluster);
						}
						else
						{
							modifiedClusters.Add(_intCluster);
							modifiedClusters.Add(_intCluster + 1);
							modifiedClusters.Add(_intCluster - 1);
							modifiedClusters.Add(_intCluster + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 - tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 - tileMap.clusterYMultiplier);
						}
					}
				}
				
				foreach (var _oldCluster in tileMapTMP.clusters.Keys)
				{
					var _addCluster = false;
					var _intCluster = -1;
					var _buildNeigbouringClusters = true;
					Vector2Int _quadrantPosition = Vector2Int.zero;
					
					
					foreach (var _oldPosition in tileMapTMP.clusters[_oldCluster].Keys)
					{
						if (tileMap.clusters.ContainsKey(_oldCluster))
						{
							if (tileMap.clusters[_oldCluster].ContainsKey(_oldPosition))
							{
							}
							else
							{	
								var _clr = tileMapTMP.GetPositionHashMapKey(new Vector3(_oldPosition.x, 0, _oldPosition.y));
				
								_addCluster = true;
								_intCluster = _clr;
								
								// Check if we should update all adjacent clusters as well.
								// To do this we look if the tile position is located on the border of a cluster
								_quadrantPosition = tileMapTMP.GetQuadrantPosition(_oldPosition);
								
								var _minQuadrantPosition = new Vector2Int(_quadrantPosition.x * tileMapTMP.ClusterCellSize, _quadrantPosition.y * tileMapTMP.ClusterCellSize);
								var _maxQuadrantPosition = new Vector2Int(_minQuadrantPosition.x + tileMapTMP.ClusterCellSize, _minQuadrantPosition.y + tileMapTMP.ClusterCellSize);
						
								if (_oldPosition.x > _minQuadrantPosition.x + 2 && _oldPosition.x < _maxQuadrantPosition.x - 2 && _oldPosition.y > _minQuadrantPosition.y + 2 && _oldPosition.y < _maxQuadrantPosition.y - 2)
								{
									_buildNeigbouringClusters = false;
								}
							}
						}
						else
						{
							_addCluster = true;
							_intCluster = _oldCluster;
						}
					}
					
					if (_addCluster)
					{
						if (!_buildNeigbouringClusters)
						{
							modifiedClusters.Add(_intCluster);
						}
						else
						{
							modifiedClusters.Add(_intCluster);
							modifiedClusters.Add(_intCluster + 1);
							modifiedClusters.Add(_intCluster - 1);
							modifiedClusters.Add(_intCluster + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 - tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 - tileMapTMP.clusterYMultiplier);
						}
					}
				}
				
				/// Check if ignored layers have changed compared to the tilemap
				foreach (var _changedIgnoreLayersCluster in tileMap.clusters.Keys)
				{
					var _intCluster = -1;
					var _addCluster = false;
					var _buildNeigbouringClusters = true;
					Vector2Int _quadrantPosition = Vector2Int.zero;
					
					foreach (var _position in tileMap.clusters[_changedIgnoreLayersCluster].Keys)
					{
						if (IgnoreTileCheck(_position, _twc))
						{
							var _clr = tileMap.GetPositionHashMapKey(new Vector3(_position.x, 0, _position.y));
							
							_addCluster = true;
							_intCluster = _clr;
								
							// Check if we should update all adjacent clusters as well.
							// To do this we look if the tile position is located on the border of a cluster
							_quadrantPosition = tileMap.GetQuadrantPosition(_position);
								
							var _minQuadrantPosition = new Vector2Int(_quadrantPosition.x * tileMap.ClusterCellSize, _quadrantPosition.y * tileMap.ClusterCellSize);
							var _maxQuadrantPosition = new Vector2Int(_minQuadrantPosition.x + tileMap.ClusterCellSize, _minQuadrantPosition.y + tileMap.ClusterCellSize);
						
							if (_position.x > _minQuadrantPosition.x + 2 && _position.x < _maxQuadrantPosition.x - 2 && _position.y > _minQuadrantPosition.y + 2 && _position.y < _maxQuadrantPosition.y - 2)
							{
								_buildNeigbouringClusters = false;
							}
						}
						
					}
					
					if (_addCluster)
					{
						if (!_buildNeigbouringClusters)
						{
							modifiedClusters.Add(_intCluster);
						}
						else
						{
							modifiedClusters.Add(_intCluster);
							modifiedClusters.Add(_intCluster + 1);
							modifiedClusters.Add(_intCluster - 1);
							modifiedClusters.Add(_intCluster + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 + tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 - tileMap.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 - tileMap.clusterYMultiplier);
						}
					}
				}
				
				/// Check if ignored layers have changed compared to the tilemap tmp
				foreach (var _changedIgnoreLayersCluster in tileMapTMP.clusters.Keys)
				{
					var _intCluster = -1;
					var _addCluster = false;
					var _buildNeigbouringClusters = true;
					Vector2Int _quadrantPosition = Vector2Int.zero;
					
					foreach (var _position in tileMapTMP.clusters[_changedIgnoreLayersCluster].Keys)
					{
							
						if (IgnoreTileCheck(_position, _twc))
						{
							var _clr = tileMapTMP.GetPositionHashMapKey(new Vector3(_position.x, 0, _position.y));
							
							_addCluster = true;
							_intCluster = _clr;
								
							// Check if we should update all adjacent clusters as well.
							// To do this we look if the tile position is located on the border of a cluster
							_quadrantPosition = tileMapTMP.GetQuadrantPosition(_position);
								
							var _minQuadrantPosition = new Vector2Int(_quadrantPosition.x * tileMapTMP.ClusterCellSize, _quadrantPosition.y * tileMapTMP.ClusterCellSize);
							var _maxQuadrantPosition = new Vector2Int(_minQuadrantPosition.x + tileMapTMP.ClusterCellSize, _minQuadrantPosition.y + tileMapTMP.ClusterCellSize);
						
							if (_position.x > _minQuadrantPosition.x + 2 && _position.x < _maxQuadrantPosition.x - 2 && _position.y > _minQuadrantPosition.y + 2 && _position.y < _maxQuadrantPosition.y - 2)
							{
								_buildNeigbouringClusters = false;
							}
						}
						
					}
					
					if (_addCluster)
					{
						if (!_buildNeigbouringClusters)
						{
							modifiedClusters.Add(_intCluster);
						}
						else
						{
							modifiedClusters.Add(_intCluster);
							modifiedClusters.Add(_intCluster + 1);
							modifiedClusters.Add(_intCluster - 1);
							modifiedClusters.Add(_intCluster + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 + tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster + 1 - tileMapTMP.clusterYMultiplier);
							modifiedClusters.Add(_intCluster - 1 - tileMapTMP.clusterYMultiplier);
						}
					}
				}
			}
			
			
			// clean up modified cluster list and remove duplicates
			modifiedClusters = modifiedClusters.Distinct().ToList();
			
			var _changedAnything = false;
			
			if (modifiedClusters.Count > 0 && !tileMap.mapSizeChanged && !_forceRebuild)
			{
				var _clusterCount = 1;
				
				for (int c = 0; c < modifiedClusters.Count; c ++)
				{
						
					if (tileMap.clusters.ContainsKey(modifiedClusters[c]))
					{				
						
						#if UNITY_EDITOR
						if (!Application.isPlaying)
						{
							EditorUtility.DisplayProgressBar("TileWorldCreator", "Instantiating tiles", (float)_clusterCount / (float)modifiedClusters.Count);
						}
						#endif
						
						_changedAnything = true;
				
						var _clusterObject = _twc.AddCluster(layerName, guid, modifiedClusters[c]);
					
						foreach (var _position in tileMap.clusters[modifiedClusters[c]].Keys)
						{
							if (!IgnoreTileCheck(_position, _twc))
							{
								TileData _tileData = tileMap.clusters[modifiedClusters[c]][_position];
								TileWorldCreator6TilesPreset _tilePreset = GetRandomTilesPreset();
								GameObject _tileObject = null;
								
								

								if (_tilePreset == null)
									continue;
									

								_tileObject = InstantiateTile(_twc, _tileData, _clusterObject, _tilePreset);
								
								
								if (!mergeTiles && _tileObject != null)
								{
									
									_tileObject.layer = assignLayer.value;
									
									if (setShadowCastingMode)
									{
										_tileObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
									}
								}
							}
						}
					
						TWC.Utilities.MeshCombiner _comb = null;
						if (mergeTiles)
						{
							_comb = _clusterObject.AddComponent<TWC.Utilities.MeshCombiner>();
							_comb.CreateMultiMaterialMesh = true;
							_comb.DestroyCombinedChildren = true;
						
							_comb.CombineMeshes(false);
							
							
							switch (colliderTypeVariantA)
							{
							case ColliderTypeVariantA.MeshCollider:
								_clusterObject.AddComponent<MeshCollider>();
								break;
							case ColliderTypeVariantA.TileCollider:
								var _meshCollider = _clusterObject.AddComponent<MeshCollider>();
								var _collisionMesh = MeshColliderGenerator.GenerateMeshCollider(_twc, assignedGenerationLayerGuid.ToString(), modifiedClusters[c], tileColliderHeight, tileColliderHeightOffset, tileColliderBorderOffset);
						
								_meshCollider.sharedMesh = _collisionMesh;
								break;
							}
						
							_clusterObject.layer = assignLayer.value;
							
							if (setShadowCastingMode)
							{	
								_clusterObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
							} 
						}
						else
						{
							if (colliderTypeVariantB == ColliderTypeVariantB.TileCollider)
							{
								var _meshCollider = _clusterObject.AddComponent<MeshCollider>();
								var _collisionMesh = MeshColliderGenerator.GenerateMeshCollider(_twc, assignedGenerationLayerGuid.ToString(), modifiedClusters[c], tileColliderHeight, tileColliderHeightOffset, tileColliderBorderOffset);
						
								_meshCollider.sharedMesh = _collisionMesh;
							}
						}
						
						
						
						_clusterCount++;
						
						if (_clusterObject != null)
						{
							_clusterObject.transform.SetParent(_layerObject.transform, false);
						}
						
						
						
							//if (mergeTiles)
							//{
							
							//else
							//{
							//	var _childRenderers = _clusterObject.GetComponentsInChildren<MeshRenderer>();
							//	for (int r = 0; r < _childRenderers.Length; r ++)
							//	{
							//		_childRenderers[r].shadowCastingMode = shadowCasting;
							//	}
							//}
						//}
						
						yield return null;
						
						if (mergeTiles)
						{
							MonoBehaviour.DestroyImmediate(_comb);
						}
						
					}
					else
					{
						var _cl = _twc.FindCluster(guid, modifiedClusters[c]);
						if (_cl != null)
						{
							MonoBehaviour.DestroyImmediate(_cl);
						}
						
						yield return null;
					}
				}
				
				#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					EditorUtility.ClearProgressBar();
				}
				#endif
			}
			else if (tileMap.mapSizeChanged ||  _forceRebuild)
			{
				_changedAnything = true;
			
				// Rebuild complete map						
				_twc.DestroyAllClusters(guid);
				
				tileMap.mapSizeChanged = false;
				
				var _clusterCount = 1;
				
				foreach (var _cluster in tileMap.clusters.Keys)
				{
					
					#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						EditorUtility.DisplayProgressBar("TileWorldCreator", "Instantiating tiles", (float)_clusterCount / (float)tileMap.clusters.Keys.Count);
					}
					#endif
					
					var _clusterObject = _twc.AddCluster(layerName, guid, _cluster);
					
					foreach (var _position in tileMap.clusters[_cluster].Keys)
					{
						if (!IgnoreTileCheck(_position, _twc))
						{
							TileData _tileData = tileMap.clusters[_cluster][_position];
							TileWorldCreator6TilesPreset _tilePreset = GetRandomTilesPreset();
							GameObject _tileObject = null;
							
							if (_tilePreset == null)
								continue;
	
							_tileObject = InstantiateTile(_twc, _tileData, _clusterObject, _tilePreset);
							
							
							if (!mergeTiles && _tileObject != null)
							{
								_tileObject.layer = assignLayer.value;
								
								if (setShadowCastingMode)
								{
									_tileObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
								}
							}
						}
					}
					
					TWC.Utilities.MeshCombiner _comb = null;
					if (mergeTiles)
					{
						_comb = _clusterObject.AddComponent<TWC.Utilities.MeshCombiner>();
						_comb.CreateMultiMaterialMesh = true;
						_comb.DestroyCombinedChildren = true;
						_comb.CombineMeshes(false);
							
							
						switch (colliderTypeVariantA)
						{
						case ColliderTypeVariantA.MeshCollider:
							_clusterObject.AddComponent<MeshCollider>();
							break;
						case ColliderTypeVariantA.TileCollider:
							var _meshCollider = _clusterObject.AddComponent<MeshCollider>();
							var _collisionMesh = MeshColliderGenerator.GenerateMeshCollider(_twc, assignedGenerationLayerGuid.ToString(), _cluster, tileColliderHeight, tileColliderHeightOffset, tileColliderBorderOffset);
						
							_meshCollider.sharedMesh = _collisionMesh;
							break;
						}
						
						_clusterObject.layer = assignLayer.value;
						
						if (setShadowCastingMode)
						{
							_clusterObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
						}

					}
					else
					{
						if (colliderTypeVariantB == ColliderTypeVariantB.TileCollider)
						{
							var _meshCollider = _clusterObject.AddComponent<MeshCollider>();
							var _collisionMesh = MeshColliderGenerator.GenerateMeshCollider(_twc, assignedGenerationLayerGuid.ToString(), _cluster, tileColliderHeight, tileColliderHeightOffset, tileColliderBorderOffset);
						
							_meshCollider.sharedMesh = _collisionMesh;
						}
					}

						
					_clusterCount++;
					
					
					if (_clusterObject != null)
					{
						_clusterObject.transform.SetParent(_layerObject.transform, false);
					}
					
					
					yield return null;
						
					if (mergeTiles)
					{
						MonoBehaviour.DestroyImmediate(_comb);
					}
				}
			}
			
			
			
			
		
			
			yield return null;
			
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EditorUtility.ClearProgressBar();
			}
			#endif
			
			_twc.executedBuildLayersCount += 1;
		
			// nothing has changed, display info message to user for forcing a rebuild
			if (!_changedAnything)
			{
				_twc.buildResultNoChanges = true;
			}
		}
		
		
		GameObject InstantiateTile(TileWorldCreator _twc, TileData _tileData, GameObject _clusterObject, TileWorldCreator6TilesPreset _tilePreset)
		{
			GameObject _tileObject = null;
			
			bool[,] orientationMap = null;
			
			if (initialOrientationLayerGuid != Guid.Empty)
			{
				orientationMap = _twc.GetMapOutputFromBlueprintLayer(initialOrientationLayerGuid);
			}
		
			
			var _north = _tileData.neighboursLocation.north;
			var _south = _tileData.neighboursLocation.south;
			var _west = _tileData.neighboursLocation.west;
			var _east = _tileData.neighboursLocation.east;
		
		
			if (orientationMap != null)
			{
			
				if (useOnlyOrientationLayer)
				{
					_north = false;
					_south = false;
					_west = false;
					_east = false;
				}
			
				for (int x = -1; x < 2; x ++)
				{
					for (int y = -1; y < 2; y ++)
					{
						try
						{
							var _pos = new Vector2Int((int)_tileData.position.x + x, (int)_tileData.position.z + y);
							
							if (orientationMap[_pos.x, _pos.y])
							{
								if (x == -1 && y == 0) // west
								{
									_west = true;
								}
								else if (x == 1 && y == 0) // east
								{
									_east = true;
								}
								else if (x == 0 && y == 1) // north
								{
									_north = true;
								}
								else if (x == 0 && y == -1) // south
								{
									_south = true;
								}
							}
						}
						catch{}
					}
				}
			}

			/*
								
			NONE
			!north && !_south && !_west && !_east
										
			Dead end
			_north && !_south && !_west && !_east
			!_north && _south && !_west && !_east
			!_north && !_south && _west && !_east
			!_north && !_south && !_west && _east
								
			Straight
			_north && _south
			_east && _west
								
			Corner
			_north && _east
			_north && _west
			_south && _east
			_south && _west
								
			Crossway 3
			_north && _south && _east
			_north && _south && _west
			_south && _east && _west
			_north && _west && _east
								
			Crossway 4
			_north && _south && _west && _east
								
			*/
								
							
			// None
			if (!_north && !_south && !_east && !_west)
			{
				if (_tilePreset.singleTile != null)
				{
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.singleTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.singleTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.singleTile, _tileData.position, _tileData.rotation);		
					#endif
					
					// Automatic scaling based on cellsize
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.singleTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.singleTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.singleTileScalingOffset.z);
				}
			}		
			
			// Dead end
			if ((_north && !_south && !_west && !_east) ||
			(!_north && _south && !_west && !_east) ||
			(!_north && !_south && _west && !_east) || 
			(!_north && !_south && !_west && _east))
			{
				if (_tilePreset.deadEndTile != null)			
				{
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.deadEndTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.deadEndTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.deadEndTile, _tileData.position, _tileData.rotation);
					#endif
					
					// Set rotation based on neighbouring situation
					if (_north && !_south && !_west && !_east)
					{
					}
					else if (!_north && _south && !_west && !_east)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
					}	
					else if (!_north && !_south && _west && !_east)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
					}
					else if (!_north && !_south && !_west && _east)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
					}
					
					
					//Add rotation offset
					_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(
					_tileObject.transform.localRotation.eulerAngles.x + _tilePreset.deadEndTileRotationOffset.x,
					_tileObject.transform.localRotation.eulerAngles.y + _tilePreset.deadEndTileRotationOffset.y,
					_tileObject.transform.localRotation.eulerAngles.z + _tilePreset.deadEndTileRotationOffset.z));
							
					// Automatic scaling based on cellsize
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.deadEndTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.deadEndTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.deadEndTileScalingOffset.z);
				}
			}
								
					
								
								
			// Straight
			if ((_north && _south && !_west && !_east) || 
			(!_north && !_south && _west && _east))
			{
				if (_tilePreset.straightTile != null)
				{
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.straightTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.straightTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.straightTile, _tileData.position, _tileData.rotation);
					#endif
					
					// Set rotation based on neighbouring situation
					if (!_north && !_south && _west && _east)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
					}
							
							
					//Add rotation offset
					_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(
					_tileObject.transform.localRotation.eulerAngles.x + _tilePreset.straightTileRotationOffset.x,
					_tileObject.transform.localRotation.eulerAngles.y + _tilePreset.straightTileRotationOffset.y,
					_tileObject.transform.localRotation.eulerAngles.z + _tilePreset.straightTileRotationOffset.z));
								
					// Automatic scaling based on cellsize
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.straightTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.straightTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.straightTileScalingOffset.z);
				}
			}
							
								
								
			// Corner
			if ((_north && !_south && _east && !_west) ||
			(_north && !_south && !_east && _west) || 
			(!_north && _south && _east && !_west) || 
			(!_north && _south && !_east && _west))
			{
				if (_tilePreset.cornerTile != null)
				{
					
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.cornerTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.cornerTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.cornerTile, _tileData.position, _tileData.rotation);
					#endif
					
					// Set Rotation based on neighbouring situation
					
					if (_north && !_south && !_east && _west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
					}
					else if (_north && !_south && _east && !_west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
					}		
					else if (!_north && _south && _east && !_west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
					}
					else if (!_north && _south && !_east && _west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
					}
					
					//Add rotation offset
					_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(
					_tileObject.transform.localRotation.eulerAngles.x + _tilePreset.cornerTileRotationOffset.x,
					_tileObject.transform.localRotation.eulerAngles.y + _tilePreset.cornerTileRotationOffset.y,
					_tileObject.transform.localRotation.eulerAngles.z + _tilePreset.cornerTileRotationOffset.z));
							
					// Automatic scaling based on cellsize
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.cornerTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.cornerTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.cornerTileScalingOffset.z);
				}
			}
								
							
								
			// Three way
			if ((_north && _south && _east && !_west) ||
			(_north && _south && !_east && _west) ||
			(_north & !_south && _east && _west ) ||
			(!_north && _south && _east && _west))
			{
				if (_tilePreset.threeWayTile != null)
				{
					
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.threeWayTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.threeWayTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.threeWayTile, _tileData.position, _tileData.rotation);
					#endif
					
					// Set rotation based on neighbouring situation
					if (_north && _south && !_east && _west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(0, 0, 0));
					}
					else if (_north && _south && _east && !_west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(0, 180, 0));
					}	
					else if (_north && !_south && _east && _west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(0, 90, 0));
					}
					else if (!_north && _south && _east && _west)
					{
						_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(0, -90, 0));
					}
									
					//Add rotation offset
					_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(
					_tileObject.transform.localRotation.eulerAngles.x + _tilePreset.threeWayTileRotationOffset.x,
					_tileObject.transform.localRotation.eulerAngles.y + _tilePreset.threeWayTileRotationOffset.y,
					_tileObject.transform.localRotation.eulerAngles.z + _tilePreset.threeWayTileRotationOffset.z));
							
					// Automatic scaling based on cellsize
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.threeWayTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.threeWayTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.threeWayTileScalingOffset.z);
				}
			}
								
							
								
			// Four way
			if (_north && _south && _west && _east)
			{
				if (_tilePreset.fourWayTile != null)
				{
					#if UNITY_EDITOR
					if (!mergeTiles && keepPrefabConnection)
					{
						_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(_tilePreset.fourWayTile as GameObject);
						_tileObject.transform.position = _tileData.position;
						_tileObject.transform.rotation = _tileData.rotation;
					}
					else
					{
						_tileObject = MonoBehaviour.Instantiate(_tilePreset.fourWayTile, _tileData.position, _tileData.rotation);
					}
					#else
					_tileObject = MonoBehaviour.Instantiate(_tilePreset.fourWayTile, _tileData.position, _tileData.rotation);
					#endif
					
					//Add rotation offset
					_tileObject.transform.localRotation = Quaternion.Euler( new Vector3(
					_tileObject.transform.localRotation.eulerAngles.x + _tilePreset.fourWayTileRotationOffset.x,
					_tileObject.transform.localRotation.eulerAngles.y + _tilePreset.fourWayTileRotationOffset.y,
					_tileObject.transform.localRotation.eulerAngles.z + _tilePreset.fourWayTileRotationOffset.z));
						
					// Automatic scaling based on cellsize
					
					if (scaleTileByCellSize)
					{
						if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y,
								_twc.twcAsset.cellSize);
						}
						else
						{
							_tileObject.transform.localScale = new Vector3(
								_twc.twcAsset.cellSize,
								_twc.twcAsset.cellSize,
								_tileObject.transform.localScale.y);
						}
					}
							
					
					// Add scaling offset
					_tileObject.transform.localScale = new Vector3(
					_tileObject.transform.localScale.x + _tilePreset.fourWayTileScalingOffset.x,
					_tileObject.transform.localScale.y + _tilePreset.fourWayTileScalingOffset.y,
					_tileObject.transform.localScale.z + _tilePreset.fourWayTileScalingOffset.z);
				}
			}
			
			
			if (_tileObject != null)
			{
								
				var _wrldPos =  ReturnTileWorldPosition(new Vector2Int((int)_tileData.position.x, (int)_tileData.position.z), _twc);
				_tileObject.transform.localPosition = new Vector3(_wrldPos.x * 2, _wrldPos.y * 2, _wrldPos.z * 2);
									
				_tileData.tile = _tileObject;
								
				_tileObject.transform.SetParent(_clusterObject.transform, false);				
			}
			
			
			return _tileObject;
		}
	
		
		
		Vector3 ReturnTileWorldPosition(Vector2Int _pos, TileWorldCreator _twc)
		{
			var _cellSize = _twc.twcAsset.cellSize;
			var _cellOffset = _cellSize / 4;
			//_cellSize = (_cellSize * 0.5f) + 0.5f;
			//var _cellOffset = (_cellSize * 2f) / 4f;
			
			//if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			//{
				return new Vector3(((_pos.x * _cellSize) / 2) + _cellOffset, 0, ((_pos.y * _cellSize) / 2) + _cellOffset);
			//}
			//else
			//{
			//	return new Vector3(((_pos.x * _cellSize) / 2) + _cellOffset, 0, ((_pos.y * _cellSize) / 2) + _cellOffset);
			//}
		}
		
		
		public TileWorldCreator6TilesPreset GetRandomTilesPreset()
		{
			// Count all weights
			if (tiles.Count == 0)
				return null;
			
			
		
			
			float t = 0;
			
			for (int i = 0; i < tiles.Count; i ++)
			{
				t += tiles[i].rndSelectionWeight;
			}
			
		 
			float rnd = UnityEngine.Random.value;
			float s = 0f;
		 
			
			
			for (int i = 0; i < tiles.Count; i ++)
			{
				var w = tiles[i].rndSelectionWeight;
				s += w / t;
				if (s >= rnd)
				{
					return tiles[i].preset;
				}
			}
		 
			return null;
		}
	    
	    
		bool IgnoreTileCheck(Vector2Int _position, TileWorldCreator _twc)
		{
			for (int i = 0; i < ignoreLayers.Count; i ++)
			{
				var _map = _twc.GetMapOutputFromBlueprintLayer(Guid.Parse(ignoreLayers[i]));

				try
				{
					if (_map[_position.x, _position.y])
					{
						return true;
					}
				}
				catch{}

			}
			
			return false;
		}
	}
}
