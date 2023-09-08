using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

using TWC.editor;
using System.Xml.Linq;
using TWC.Utilities;

namespace TWC.Actions
{
	[ActionNameAttribute(Name="Objects")]
	public class InstantiateObjects : TWCBuildLayer
	{
		public int selectedCopyLayerIndex;
        //public Guid guidCopyLayer;

        public Guid useTileRotationFrom;
        public bool mergeObjects;
		public bool keepPrefabConnection = false;
		public bool addMeshCollider;
		
		public GameObject prefab;
	
		
		public bool instantiateChilds;
		public bool selectFromPrefabList;
		public GameObject childPrefab;
		public List<GameObject> randomChildPrefabs;
		public int count;
		public float radius;
		
		public bool useSubdividedMap;
	
		public LayerMask assignLayer;
		public bool setShadowCastingMode;
		public ShadowCastingMode shadowCasting = ShadowCastingMode.On;
		
		public bool disableTileRotation;
		
		
		public Vector3 globalPositionOffset, globalRotationOffset, globalScaleOffset;
		public Vector3 localPositionOffset, localRotationOffset, localScaleOffset;
	
		public bool rndPosition;
		public Vector3 minRndPosition, maxRndPosition;
		public bool rndScaling;
		public bool uniformScaling;
		public float minRndUniformScale;
		public float maxRndUniformScale;
		public Vector3 minRndScale, maxRndScale;
		public bool rndRotation;
		public Vector3 minRndRotation, maxRndRotation;
		
		
		WorldMap tileMap;
		WorldMap tileMapTMP;
		
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public TileWorldCreatorAsset twcAsset;
		}
		
		public override TWCBuildLayer Clone()
		{
			
			var _r = new InstantiateObjects();
			
			_r.guid = Guid.NewGuid();
			_r.selectedCopyLayerIndex = this.selectedCopyLayerIndex;
			_r.useTileRotationFrom = this.useTileRotationFrom;
            _r.mergeObjects = this.mergeObjects;
			_r.keepPrefabConnection = this.keepPrefabConnection;
			_r.assignLayer = this.assignLayer;
			_r.setShadowCastingMode = this.setShadowCastingMode;
			_r.shadowCasting = this.shadowCasting;
			_r.addMeshCollider = this.addMeshCollider;
			_r.prefab = this.prefab;
			_r.instantiateChilds = this.instantiateChilds;
			_r.childPrefab = this.childPrefab;
			_r.selectFromPrefabList = this.selectFromPrefabList;
			_r.randomChildPrefabs = this.randomChildPrefabs;
			_r.count = this.count;
			_r.radius = this.radius;
			_r.useSubdividedMap = this.useSubdividedMap;
			_r.globalPositionOffset = this.globalPositionOffset;
			_r.globalRotationOffset = this.globalRotationOffset;
			_r.globalScaleOffset = this.globalScaleOffset;
			_r.localPositionOffset = this.localPositionOffset;
			_r.localRotationOffset = this.localRotationOffset;
			_r.localScaleOffset = this.localScaleOffset;
			_r.rndPosition = this.rndPosition;
			_r.minRndPosition = this.minRndPosition;
			_r.maxRndPosition = this.maxRndPosition;
			_r.rndScaling = this.rndScaling;
			_r.uniformScaling = this.uniformScaling;
			_r.minRndUniformScale = this.minRndUniformScale;
			_r.maxRndUniformScale = this.maxRndUniformScale;
			_r.minRndScale = this.minRndScale;
			_r.maxRndScale = this.maxRndScale;
			_r.rndRotation = this.rndRotation;
			_r.minRndRotation = this.minRndRotation;
			_r.maxRndRotation = this.maxRndRotation;
			
			return _r as TWCBuildLayer;
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(TileWorldCreatorAsset _twcAsset)
		{
			using (new GUILayout.VerticalScope("Box"))
			{
				layerName = EditorGUILayout.TextField("Name", layerName);
				
				var _names =  EditorUtilities.GetAllGenerationLayerNames(_twcAsset);
				var _fromLayerName = "";
				var _layerData = _twcAsset.GetBlueprintLayerData(assignedGenerationLayerGuid);
				if (_layerData != null)
				{
					_fromLayerName = _layerData.layerName;
				}
				
				using (new GUILayout.HorizontalScope())
				{
					EditorGUILayout.LabelField("Assigned layer");
					if (EditorGUILayout.DropdownButton(new GUIContent(_fromLayerName), FocusType.Keyboard))
					{
						GenericMenu menu = new GenericMenu();
						
						for (int n = 0; n < _names.Length; n ++)
						{
							var _data = new GenericMenuData();
							_data.selectedIndex = n;
							_data.twcAsset = _twcAsset;
							menu.AddItem(new GUIContent(_names[n]), false, AssignLayer, _data);
						}
						
						menu.ShowAsContext();
					}
				}
				
				useSubdividedMap = EditorGUILayout.Toggle("Use subdivided map:", useSubdividedMap);
				
				
				
				prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), false);
				
				using (new GUILayout.VerticalScope("Box"))
				{
					globalPositionOffset = EditorGUILayout.Vector3Field("Global position offset:", globalPositionOffset);
					globalRotationOffset = EditorGUILayout.Vector3Field("Global rotation offset:", globalRotationOffset);
					globalScaleOffset = EditorGUILayout.Vector3Field("Global scale offset:", globalScaleOffset);
				}
				
				using (new GUILayout.VerticalScope("Box"))
				{
					localPositionOffset = EditorGUILayout.Vector3Field("Local position offset:", localPositionOffset);
					localRotationOffset = EditorGUILayout.Vector3Field("Local rotation offset:", localRotationOffset);
					localScaleOffset = EditorGUILayout.Vector3Field("Local scale offset:", localScaleOffset);
				}
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				disableTileRotation = EditorGUILayout.Toggle(new GUIContent("Disable tile rotation", "If enabled this object will not be rotated like a tile object"), disableTileRotation);

				if (!disableTileRotation)
				{
					var _useTileRotationFromLayerName = "";
					var _useTileRotationFromLayerData = _twcAsset.GetBlueprintLayerData(useTileRotationFrom);

                    if (_useTileRotationFromLayerData != null)
                    {
                        _useTileRotationFromLayerName = _useTileRotationFromLayerData.layerName;
                    }

                    var _names = EditorUtilities.GetAllGenerationLayerNames(_twcAsset);

                    using (new GUILayout.HorizontalScope("Box"))
                    {
                        EditorGUILayout.LabelField("Use orientation from layer", GUILayout.MaxWidth(150));

                        if (EditorGUILayout.DropdownButton(new GUIContent(_useTileRotationFromLayerName), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();

                            for (int n = 0; n < _names.Length; n++)
                            {
                                var _data = new GenericMenuData();
                                _data.selectedIndex = n;
                                _data.twcAsset = _twcAsset;
                                menu.AddItem(new GUIContent(_names[n]), false, AssignTileRotationLayer, _data);
                            }

                            menu.ShowAsContext();
                        }

                        if (GUILayout.Button("clear"))
                        {
                            useTileRotationFrom = Guid.Empty;
                        }
                    }
                }
            }

            using (new GUILayout.VerticalScope("Box"))
			{
				instantiateChilds = EditorGUILayout.Toggle("Childs", instantiateChilds);
			    
			
				if (instantiateChilds)
				{
					selectFromPrefabList = EditorGUILayout.Toggle("Select random prefab from list", selectFromPrefabList);
					if (selectFromPrefabList)
					{
						if (randomChildPrefabs == null)
						{
							randomChildPrefabs = new List<GameObject>();
						}
						
						if (GUILayout.Button("+"))
						{	
							randomChildPrefabs.Add(null);
						}
						
						for (int i = 0; i < randomChildPrefabs.Count; i ++)
						{
							using (new GUILayout.HorizontalScope())
							{
								randomChildPrefabs[i] = EditorGUILayout.ObjectField(randomChildPrefabs[i], typeof(GameObject), false) as GameObject;
								
								if (GUILayout.Button("-", GUILayout.Width(20)))
								{
									randomChildPrefabs.RemoveAt(i);
								}
							}
						}
					}
					else
					{
						childPrefab = (GameObject)EditorGUILayout.ObjectField("Child prefab:", childPrefab, typeof(GameObject), false);	
					}
					
					count = EditorGUILayout.IntField("Count:", count);
					radius = EditorGUILayout.FloatField("Radius:", radius);
				}
			
			}
			
			
			using (new GUILayout.VerticalScope("Box"))
			{
				rndPosition = EditorGUILayout.Toggle("Random position", rndPosition);
				
				if (rndPosition)
				{
					using (new GUILayout.VerticalScope())
					{
						minRndPosition = EditorGUILayout.Vector3Field("min pos:", minRndPosition);
						maxRndPosition = EditorGUILayout.Vector3Field("max pos:", maxRndPosition);
					}
				}
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				rndRotation = EditorGUILayout.Toggle("Random rotation", rndRotation);
				
				if (rndRotation)
				{
					using (new GUILayout.VerticalScope())
					{
						minRndRotation = EditorGUILayout.Vector3Field("min rotation", minRndRotation);
						maxRndRotation = EditorGUILayout.Vector3Field("max rotation:", maxRndRotation);
					}
				}
			}
			
			using (new GUILayout.VerticalScope("Box"))
			{
				rndScaling = EditorGUILayout.Toggle("Random scaling", rndScaling);
				
				if (rndScaling)
				{
					uniformScaling = EditorGUILayout.Toggle("Uniform scaling", uniformScaling);
					
					if (!uniformScaling)
					{
						using (new GUILayout.VerticalScope())
						{
							
							minRndScale = EditorGUILayout.Vector3Field("min scale:", minRndScale);
							maxRndScale = EditorGUILayout.Vector3Field("max scale:", maxRndScale);
						}
					}
					else
					{
						using (new GUILayout.VerticalScope())
						{
							minRndUniformScale = EditorGUILayout.FloatField("min scale:", minRndUniformScale);
							maxRndUniformScale = EditorGUILayout.FloatField("max scale:", maxRndUniformScale);
						}
					}
				}
			}
			
			
			using (new GUILayout.VerticalScope("Box"))
			{
				mergeObjects = EditorGUILayout.Toggle("Merge:", mergeObjects);
				
				if (!mergeObjects)
				{
					keepPrefabConnection = EditorGUILayout.Toggle(new GUIContent("Keep prefab connection", "Keep the prefab connection for each tile. (Only in editor)"), keepPrefabConnection);
				}
				
				if (mergeObjects)
				{
					addMeshCollider = EditorGUILayout.Toggle("Add mesh collider", addMeshCollider);
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
		}
	    #endif
	    
		void AssignLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			assignedGenerationLayerGuid = _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}

        void AssignTileRotationLayer(object _data)
        {
            var _d = _data as GenericMenuData;
            useTileRotationFrom = _d.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
        }

        public override void Execute(TileWorldCreator _twc, bool _forceRebuild)
		{

            tileMap = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_UNSUBD");
   //         if (!useSubdividedMap)
			//{	
			//	tileMap = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_UNSUBD");
			//}
			//else	
			//{
			//	tileMap = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString());
			//}
			
			if (prefab == null)
				return;
			
			if (tileMap != null)
			{
				if (!useSubdividedMap)
				{	
					tileMapTMP = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_UNSUBD" + "_TMP");
				}
				else
				{
					tileMapTMP = _twc.GetGeneratedBlueprintMap(assignedGenerationLayerGuid.ToString() + "_TMP");
				}
				
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
		
		
		IEnumerator InstantiateIE(TileWorldCreator _twc, bool _forceRebuild)
		{
			List<int> modifiedClusters = new List<int>();
			
			var _layerObject = _twc.AddLayerObject(layerName, guid);
			_layerObject.transform.SetParent(_twc.worldObject.transform, false);
		
		
		
			// Add global scale offset
			_layerObject.transform.localScale = new Vector3(
				_layerObject.transform.localScale.x + globalScaleOffset.x,
				_layerObject.transform.localScale.y + globalScaleOffset.y,
				_layerObject.transform.localScale.z + globalScaleOffset.z
			);
			
			// Add global position offset
			_layerObject.transform.localPosition = new Vector3(
				_layerObject.transform.localPosition.x + globalPositionOffset.x,
				_layerObject.transform.localPosition.y + globalPositionOffset.y,
				_layerObject.transform.localPosition.z + globalPositionOffset.z);
						
			// Add global rotation offset
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
					var _intCluster = -1;
					var _buildNeigbouringClusters = true;
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
			}
			
			
			// clean up modified cluster list and remove duplicates
			modifiedClusters = modifiedClusters.Distinct().ToList();
			
			// Only rebuild changed parts
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
							EditorUtility.DisplayProgressBar("TileWorldCreator", "Instantiating objects", (float)_clusterCount / (float)modifiedClusters.Count);
						}
						#endif
						
						var _clusterObject = _twc.AddCluster(layerName, guid, modifiedClusters[c]);
					
						foreach (var _position in tileMap.clusters[modifiedClusters[c]].Keys)
						{
							TileData _tileData = tileMap.clusters[modifiedClusters[c]][_position];
						
							//// use object rotation from a different blueprint layer
							//if (useTileRotationFrom != Guid.Empty)
							//{
							//	var _rotationMapData = _twc.GetMapOutputFromBlueprintLayer(useTileRotationFrom);
							//	var _neighbours = TileWorldCreatorUtilities.GetNeighboursLocation(_rotationMapData, _position.x, _position.y);
							//	var _rotationTileData = TileWorldCreatorUtilities.ReturnTileRotation(_neighbours);
								
							//	_tileData.rotation = Quaternion.Euler(0f, _rotationTileData, 0f);
       //                     }


                            // use object rotation from a different blueprint layer
                            if (useTileRotationFrom != Guid.Empty)
                            {

                                WorldMap _rotationMapData = null;
                                if (useSubdividedMap)
                                {
                                    _rotationMapData = _twc.GetGeneratedBlueprintMap(useTileRotationFrom.ToString());
                                }
                                else
                                {
                                    _rotationMapData = _twc.GetGeneratedBlueprintMap(useTileRotationFrom.ToString() + "_UNSUBD");
                                }

                                var _rotationData = _rotationMapData.clusters[modifiedClusters[c]][_position];

                                // override rotation with rotation layer
                                _tileData.rotation = _rotationData.rotation;
                            }


                            GameObject _tileObject = null;
							
							#if UNITY_EDITOR
							if (!mergeObjects && keepPrefabConnection)
							{
								_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab as GameObject);
								_tileObject.transform.position = _tileData.position;
								_tileObject.transform.rotation = disableTileRotation ? Quaternion.identity : _tileData.rotation;
							}
							else
							{
                                _tileObject = MonoBehaviour.Instantiate(prefab,  _tileData.position, disableTileRotation ? Quaternion.identity : _tileData.rotation);
							}
#else
							_tileObject = MonoBehaviour.Instantiate(prefab,  _tileData.position, disableTileRotation ? Quaternion.identity : _tileData.rotation);
#endif

                            var _wrldPos =  ReturnTileWorldPosition(new Vector2Int((int)_tileData.position.x, (int)_tileData.position.z), _twc);
						
							if (!useSubdividedMap)
							{
								_tileObject.transform.localPosition = new Vector3(_wrldPos.x * 2, _wrldPos.y * 2, _wrldPos.z * 2);
							}
							else
							{
								_tileObject.transform.localPosition = _wrldPos;
							}
								
							// Add global position offset
							_tileObject.transform.localPosition = new Vector3(
								_tileObject.transform.localPosition.x + localPositionOffset.x,
								_tileObject.transform.localPosition.y + localPositionOffset.y,
								_tileObject.transform.localPosition.z + localPositionOffset.z);
						
							// Add global rotation offset
							_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(
								_tileObject.transform.localRotation.eulerAngles.x + localRotationOffset.x,
								_tileObject.transform.localRotation.eulerAngles.y + localRotationOffset.y,
								_tileObject.transform.localRotation.eulerAngles.z + localRotationOffset.z
							));
						
							//// Add global scale offset
							_tileObject.transform.localScale = new Vector3(
								_tileObject.transform.localScale.x + localScaleOffset.x,
								_tileObject.transform.localScale.y + localScaleOffset.y,
								_tileObject.transform.localScale.z + localScaleOffset.z
							);
							
							
							if (rndPosition)
							{
								_tileObject.transform.localPosition = RndPosition(_tileObject.transform.localPosition);
							}
							
							if (rndRotation)
							{
								var _rndQ = RndRotation(_tileObject.transform.localRotation);
								_tileObject.transform.localRotation *= _rndQ;
							}
							
							if (rndScaling)
							{
								_tileObject.transform.localScale += RndScale();
							}
							
							
							if (instantiateChilds)
							{
							
								for (int i = 0; i < count; i ++)
								{
									Vector3 _rndPos = (UnityEngine.Random.insideUnitSphere * radius) + _tileObject.transform.localPosition;
									if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
									{
										_rndPos = new Vector3(_rndPos.x, _tileObject.transform.localPosition.y, _rndPos.z);
									}
									else
									{
										_rndPos = new Vector3(_rndPos.x, _rndPos.y, _tileObject.transform.localPosition.z);
									}
									
									GameObject _childObj = null;
									var _childPrefab = selectFromPrefabList ? randomChildPrefabs[UnityEngine.Random.Range(0, randomChildPrefabs.Count)] : childPrefab;
									
									#if UNITY_EDITOR
									if (!mergeObjects && keepPrefabConnection)
									{
										_childObj = (GameObject)PrefabUtility.InstantiatePrefab(_childPrefab as GameObject);
										_childObj.transform.position = _rndPos;
										_childObj.transform.rotation = _tileObject.transform.localRotation;
									}
									else
									{
										_childObj = MonoBehaviour.Instantiate(_childPrefab, _rndPos, _tileObject.transform.localRotation);
									}
									#else
									_childObj = MonoBehaviour.Instantiate(_childPrefab, _rndPos, _tileObject.transform.localRotation);
									#endif
									
									if (rndPosition)
									{
										_childObj.transform.localPosition = RndPosition(_childObj.transform.localPosition);
									}
						
									if (rndRotation)
									{
										var _rndQ = RndRotation(_childObj.transform.localRotation);
										_childObj.transform.localRotation *= _rndQ;
									}
											
									if (rndScaling)
									{
										_childObj.transform.localScale = RndScale();
									}
									
									
									_childObj.transform.SetParent(_clusterObject.transform, false);
								}
							
							}
	
							_tileObject.transform.SetParent(_clusterObject.transform, false);
							
							if (!mergeObjects && _tileObject != null)
							{
								_tileObject.layer = assignLayer.value;
								
								if (setShadowCastingMode)
								{
									_tileObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
								}
							}
						}
						
						TWC.Utilities.MeshCombiner _comb = null;
						if (mergeObjects)
						{
							//TWC.Utilities.MeshCombiner.CombineMesh(_clusterObject, addMeshCollider);
							_comb = _clusterObject.AddComponent<TWC.Utilities.MeshCombiner>();
							_comb.CreateMultiMaterialMesh = true;
							_comb.DestroyCombinedChildren = true;
							_comb.CombineMeshes(false);
							
							if (addMeshCollider)
							{
								_clusterObject.AddComponent<MeshCollider>();
							}
							
							_clusterObject.layer = assignLayer.value;
							
							if (setShadowCastingMode)
							{
								_clusterObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
							}
						}
						
						_clusterCount++;
						
						if (_clusterObject != null)
						{
							_clusterObject.transform.SetParent(_layerObject.transform, false);
						}
				
						
						yield return null;
						
						if (mergeObjects)
						{
							MonoBehaviour.DestroyImmediate(_comb);
						}
					}
					else
					{
						var _cl = _twc.FindCluster( guid, modifiedClusters[c]);
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
			// Rebuild complete map
			else if (tileMap.mapSizeChanged ||  _forceRebuild)
			{
				
				_twc.DestroyAllClusters(guid);

				
				tileMap.mapSizeChanged = false;
				
				var _clusterCount = 1;
				
				foreach (var _cluster in tileMap.clusters.Keys)
				{
					
					#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						EditorUtility.DisplayProgressBar("TileWorldCreator", "Instantiating objects", (float)_clusterCount / (float)modifiedClusters.Count);
					}
					#endif
						
						
					var _clusterObject = _twc.AddCluster(layerName, guid, _cluster);
					
					foreach (var _position in tileMap.clusters[_cluster].Keys)
					{
						TileData _tileData = tileMap.clusters[_cluster][_position];


                        // use object rotation from a different blueprint layer
                        if (useTileRotationFrom != Guid.Empty)
                        {
							
							WorldMap _rotationMapData = null;
							if (useSubdividedMap)
							{
                                _rotationMapData = _twc.GetGeneratedBlueprintMap(useTileRotationFrom.ToString());
                            }
							else
							{
                                _rotationMapData = _twc.GetGeneratedBlueprintMap(useTileRotationFrom.ToString() + "_UNSUBD");
                            }

							var _rotationData = _rotationMapData.clusters[_cluster][_position];

							//var _rotationMapData = _twc.GetMapOutputFromBlueprintLayer(useTileRotationFrom);
							//var _neighbours = TileWorldCreatorUtilities.GetNeighboursLocation(_rotationMapData, _position.x, _position.y);
							//var _rotationTileData = TileWorldCreatorUtilities.ReturnTileRotation(_neighbours);

							for (int b = 0; b < _twc.twcAsset.mapBlueprintLayers.Count; b ++)
							{
								if (_twc.twcAsset.mapBlueprintLayers[b].guid == useTileRotationFrom)
								{
									//Debug.Log(_twc.twcAsset.mapBlueprintLayers[b].layerName);
									//Debug.Log("north: " + _rotationData.neighboursLocation.north + "\n" +
									//	"south: " +_rotationData.neighboursLocation.south + "\n" +
									//	"east: " + _rotationData.neighboursLocation.east + "\n" +
									//	"west: " + _rotationData.neighboursLocation.west + "\n" +
									//	"northEast: " + _rotationData.neighboursLocation.northEast + "\n" + 
									//	"northWest: " + _rotationData.neighboursLocation.northWest + "\n" +
									//	"southEast: " + _rotationData.neighboursLocation.southEast + "\n" + 
									//	"southWest: " + _rotationData.neighboursLocation.southWest);

									var _rot = TileWorldCreatorUtilities.ReturnTileRotation(_rotationData.neighboursLocation);
									Debug.Log(_rot);
								}
							}

							Debug.Log(_rotationData.yRotation);
                           // override rotation with rotation layer
                            _tileData.rotation = _rotationData.rotation;
                        }

                        GameObject _tileObject = null;	
					
						#if UNITY_EDITOR
						if (!mergeObjects && keepPrefabConnection)
						{
							_tileObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab as GameObject);
							_tileObject.transform.position = _tileData.position;
							_tileObject.transform.rotation = disableTileRotation ? Quaternion.identity : _tileData.rotation;
						}
						else
						{
                            _tileObject = MonoBehaviour.Instantiate(prefab,  _tileData.position, disableTileRotation ? Quaternion.identity : _tileData.rotation);
                        }
#else
						_tileObject = MonoBehaviour.Instantiate(prefab,  _tileData.position, disableTileRotation ? Quaternion.identity : _tileData.rotation);
#endif

                        var _wrldPos =  ReturnTileWorldPosition(new Vector2Int((int)_tileData.position.x, (int)_tileData.position.z), _twc);
						
						if (!useSubdividedMap)
						{
							_tileObject.transform.localPosition = new Vector3(_wrldPos.x * 2, _wrldPos.y * 2, _wrldPos.z * 2);
						}
						else
						{
							_tileObject.transform.localPosition = _wrldPos;
						}
								
						//Add global position offset
						_tileObject.transform.localPosition = new Vector3(
							_tileObject.transform.localPosition.x + localPositionOffset.x,
							_tileObject.transform.localPosition.y + localPositionOffset.y,
							_tileObject.transform.localPosition.z + localPositionOffset.z);
						
						// Add global rotation offset
						_tileObject.transform.localRotation = Quaternion.Euler(new Vector3(
							_tileObject.transform.localRotation.eulerAngles.x + localRotationOffset.x,
							_tileObject.transform.localRotation.eulerAngles.y + localRotationOffset.y,
							_tileObject.transform.localRotation.eulerAngles.z + localRotationOffset.z
						));
						
						//// Add global scale offset
						_tileObject.transform.localScale = new Vector3(
							_tileObject.transform.localScale.x + localScaleOffset.x,
							_tileObject.transform.localScale.y + localScaleOffset.y,
							_tileObject.transform.localScale.z + localScaleOffset.z
						);
						
						
						if (rndPosition)
						{
							_tileObject.transform.localPosition = RndPosition(_tileObject.transform.localPosition);
						}
						
						if (rndRotation)
						{
							var _rndQ = RndRotation(_tileObject.transform.localRotation);
							_tileObject.transform.localRotation *= _rndQ;
						}
											
						if (rndScaling)
						{
							_tileObject.transform.localScale += RndScale();
						}
						
						if (instantiateChilds)
						{
							
							for (int i = 0; i < count; i ++)
							{
								Vector3 _rndPos = (UnityEngine.Random.insideUnitSphere * radius) + _tileObject.transform.localPosition;
								_rndPos = new Vector3(_rndPos.x, _tileObject.transform.localPosition.y, _rndPos.z);
								
								GameObject _childObj = null;							
								var _childPrefab = selectFromPrefabList ? randomChildPrefabs[UnityEngine.Random.Range(0, randomChildPrefabs.Count)] : childPrefab;
								
#if UNITY_EDITOR
								if (!mergeObjects && keepPrefabConnection)
								{
									_childObj = (GameObject)PrefabUtility.InstantiatePrefab(_childPrefab as GameObject);
									_childObj.transform.position = _rndPos;
									_childObj.transform.rotation = _tileObject.transform.localRotation;
								}
								else
								{
									_childObj = MonoBehaviour.Instantiate(_childPrefab, _rndPos, Quaternion.identity);
								}
#else
								_childObj = MonoBehaviour.Instantiate(_childPrefab, _rndPos, Quaternion.identity);
#endif
								
								if (rndPosition)
								{
									_childObj.transform.localPosition = RndPosition(_childObj.transform.localPosition);
								}
						
								if (rndRotation)
								{
									var _rndQ = RndRotation(_childObj.transform.localRotation);
									_childObj.transform.localRotation *= _rndQ;
								}
											
								if (rndScaling)
								{
									_childObj.transform.localScale = RndScale();
								}
								
								
								_childObj.transform.SetParent(_clusterObject.transform, false);
							}
							
						}
						
						
						_tileObject.transform.SetParent(_clusterObject.transform, false);

						if (!mergeObjects && _tileObject != null)
						{
							_tileObject.layer = assignLayer.value;
							
							if (setShadowCastingMode)
							{
								_tileObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
							}
						}
					}
					
					TWC.Utilities.MeshCombiner _comb = null;
					
					if (mergeObjects)
					{
						
						_comb = _clusterObject.AddComponent<TWC.Utilities.MeshCombiner>();
						_comb.CreateMultiMaterialMesh = true;
						_comb.DestroyCombinedChildren = true;
						_comb.CombineMeshes(false);
							
						if (addMeshCollider)
						{
							_clusterObject.AddComponent<MeshCollider>();
						}
					
						_clusterObject.layer = assignLayer.value;
					
						if (setShadowCastingMode)
						{
							_clusterObject.GetComponent<MeshRenderer>().shadowCastingMode = shadowCasting;
						}
					}
					
					
					if (_clusterObject != null)
					{
						_clusterObject.transform.SetParent(_layerObject.transform, false);
					}
					
					_clusterCount++;
					
					yield return null;
					
					if (mergeObjects)
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
		}
		
		
		Vector3 ReturnTileWorldPosition(Vector2Int _pos, TileWorldCreator _twc)
		{
			var _cellSize = _twc.twcAsset.cellSize;
			var _cellOffset = _cellSize / 4;
			//_cellSize = (_cellSize * 0.5f) + 0.5f;
			//var _cellOffset = (_cellSize * 2f) / 4f;
			
			return new Vector3(((_pos.x * _cellSize) / 2) + _cellOffset, 0, ((_pos.y * _cellSize) / 2) + _cellOffset);
			//if (_twc.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			//{
			//	return new Vector3(((_pos.x * _cellSize) + _cellOffset), 0, ((_pos.y * _cellSize) + _cellOffset));
			//}
			//else
			//{
			//	return new Vector3(((_pos.x * _cellSize) + _cellOffset), 0, ((_pos.y * _cellSize) + _cellOffset));
			//}
		}
		
		Vector3 RndPosition(Vector3 _position)
		{
			// Add random position
			return new Vector3(
				_position.x + UnityEngine.Random.Range(minRndPosition.x, maxRndPosition.x),
				_position.y + UnityEngine.Random.Range(minRndPosition.y, maxRndPosition.y),
				_position.z + UnityEngine.Random.Range(minRndPosition.z, maxRndPosition.z)
								
			);
		}
		
		Quaternion RndRotation(Quaternion _rotation)
		{
			// Add random rotation
			return Quaternion.Euler(new Vector3(
				UnityEngine.Random.Range(minRndRotation.x, maxRndRotation.x),
				UnityEngine.Random.Range(minRndRotation.y, maxRndRotation.y),
				UnityEngine.Random.Range(minRndRotation.z, maxRndRotation.z)
			));
		}
		
		Vector3 RndScale()
		{
			// Add random scale
			if (!uniformScaling)
			{
				return new Vector3(
					UnityEngine.Random.Range(minRndScale.x, maxRndScale.x),
					UnityEngine.Random.Range(minRndScale.y, maxRndScale.y),
					UnityEngine.Random.Range(minRndScale.z, maxRndScale.z)
				);
			}
			else
			{
				var _rnd = UnityEngine.Random.Range(minRndUniformScale, maxRndUniformScale);
				return new Vector3(_rnd, _rnd, _rnd);
			}
		}
	}
}
