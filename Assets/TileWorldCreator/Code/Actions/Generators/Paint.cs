using System;
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
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Generators)]
	[ActionNameAttribute(Name="Paint")]
	public class Paint : TWCBlueprintAction, ITWCAction
	{
		[SerializeField]
		private bool executeBuildLayers;
		[SerializeField]
		private bool paintActive;
		private int clusterSize;
		
		[SerializeField]
		private bool[,] paintMap;
		[SerializeField]
		private int layerIndex = -1;
		[SerializeField]
		private TileWorldCreator tileWorldCreator;
		
		[SerializeField]
		private Texture2D paintPreviewTexture;
		private TWCGUILayout guiLayout;
		
		#pragma warning disable 0414
		[SerializeField]
		private bool registeredToSceneView;
		#pragma warning restore 0414
		
		private string twcGuid;
		private long twcFile;
		private bool controlClick;
		
		private Texture2D iconCopy;
		private Texture2D iconFill;
		private Texture2D iconClear;
		private Texture2D iconActivatePaint;
		private Texture2D iconDeactivatePaint;
		
		private Camera sceneViewCamera;
	
		
		public ITWCAction Clone()
		{
			var _r = new Paint();
			
			_r.paintMap = new bool[this.paintMap.GetLength(0),this.paintMap.GetLength(1)];
			System.Array.Copy(this.paintMap, _r.paintMap, this.paintMap.Length);
			
			_r.layerIndex = this.layerIndex;
			_r.executeBuildLayers = this.executeBuildLayers;
			
			return _r;
		}
		
		
		public override void ModifyMap(int _x, int _y, bool _value, TileWorldCreator _twc)
		{
			if (paintMap == null)
			{
				paintMap = new bool[_twc.twcAsset.mapWidth, _twc.twcAsset.mapHeight];
			}
			
			if (_x >= 0 && _x < paintMap.GetLength(0) && _y >= 0 && _y < paintMap.GetLength(1))
			{
				paintMap[_x, _y] = _value;
			}
		}
		
		public override void FillMap(bool _value, TileWorldCreator _twc)
		{
			if (paintMap == null)
			{
				paintMap = new bool[_twc.twcAsset.mapWidth, _twc.twcAsset.mapHeight];
			}
			
			for (int x = 0; x < paintMap.GetLength(0); x ++)
			{
				for (int y = 0; y < paintMap.GetLength(1); y ++)
				{
					paintMap[x, y] = _value;			
				}	
			}
		}
		
		public override void CopyMap(TileWorldCreator _twc)
		{
			paintMap = _twc.twcAsset.mapBlueprintLayers[layerIndex].map;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			if (paintMap != null)
			{
				return TileWorldCreatorUtilities.MergeMap(map, paintMap);
			}
			else
			{
				return map;
			}
		}
		
		#if UNITY_EDITOR
		TileWorldCreator FindTWCComponent()
		{
			string _path = AssetDatabase.GUIDToAssetPath(twcGuid);
			tileWorldCreator = AssetDatabase.LoadAssetAtPath<TileWorldCreator>(_path)  as TileWorldCreator;
				
			if (tileWorldCreator == null)
			{
			
				var _twc = GameObject.FindObjectOfType<TileWorldCreator>();
				if (_twc != null)
				{
					tileWorldCreator = _twc;
					
					if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tileWorldCreator, out twcGuid, out twcFile))
					{
						//var stringBuilder = new System.Text.StringBuilder();
	
						//stringBuilder.AppendFormat("Asset: " + tileWorldCreator.name +
						//	"\n  Instance ID: " + tileWorldCreator.GetInstanceID() +
						//	"\n  GUID: " + twcGuid +
						//	"\n  File ID: " + twcFile);
						
						//Debug.Log(twcGuid.ToString());
					}
				}
			}
			
			return tileWorldCreator;
		}
		
		
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			layerIndex = _layerIndex;
			tileWorldCreator = _twc;
			
			if (iconCopy == null)
			{
				LoadResources();
			}
			
			// Check if user has selected asset in project view or scene
			var _isInProject = false;
			if (Selection.transforms.Length == 0)
			{
				_isInProject = true;
			}
			
			if (tileWorldCreator == null && !_isInProject)
			{
				string _path = AssetDatabase.GUIDToAssetPath(twcGuid);
				tileWorldCreator = AssetDatabase.LoadAssetAtPath<TileWorldCreator>(_path)  as TileWorldCreator;
				
				if (tileWorldCreator == null)
				{
					tileWorldCreator = FindTWCComponent();
				}

				//EditorGUILayout.HelpBox("Could not find TileWorldCreator component. Please assign the TileWorldCreator component to this field", MessageType.Warning);
				//tileWorldCreator = (TileWorldCreator)EditorGUILayout.ObjectField("TileWorldCreator Component", tileWorldCreator, typeof(TileWorldCreator), true);
				
				return;
			}
			//else
			//{
			//	if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tileWorldCreator, out twcGuid, out twcFile))
			//	{
			//		//var stringBuilder = new System.Text.StringBuilder();
	
			//		//stringBuilder.AppendFormat("Asset: " + tileWorldCreator.name +
			//		//	"\n  Instance ID: " + tileWorldCreator.GetInstanceID() +
			//		//	"\n  GUID: " + twcGuid +
			//		//	"\n  File ID: " + twcFile);
						
			//		//Debug.Log(stringBuilder);
			//	}
			//}
			
			if (_isInProject)
			{
				using (guiLayout = new TWCGUILayout(_rect))
				{
					guiLayout.Add();
					guiLayout.Add();
					EditorGUI.HelpBox(new Rect(guiLayout.rect.x, guiLayout.rect.y - 20, guiLayout.rect.width, guiLayout.height), "Paint generator works only when asset is assigned to a TileWorldCreator component in a scene.", MessageType.Warning);
				}
				
				return;
			}
			else
			{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				if (_asset.mapWidth == 0 || _asset.mapHeight == 0)
				{
					guiLayout.Add();
					guiLayout.Add();
					EditorGUI.HelpBox(guiLayout.rect, "Set map size greator than: 0, 0", MessageType.Info);
				}
				else
				{
				
					if (paintMap == null)
					{

						paintMap = new bool[_asset.mapWidth, _asset.mapHeight];

					}
					
					if (_asset.mapWidth != paintMap.GetLength(0) ||
						_asset.mapHeight != paintMap.GetLength(1))
					{
						guiLayout.Add();
						EditorGUI.HelpBox(guiLayout.rect, "Map size has changed. Map must be rebuilded.", MessageType.Warning);
						guiLayout.Add();
						if (GUI.Button(guiLayout.rect, "Rebuild"))
						{
						
							paintMap = ResizeArray<bool>(paintMap, _asset.mapWidth, _asset.mapHeight);
							
							UpdatePreviewTexture();
						}
					}
					else
					{
						
						guiLayout.Add(30);
						
						if (paintActive)
						{
							GUI.color = Color.green;
						}
						else
						{
							GUI.color = Color.white;
						}
						GUI.Box(new Rect(guiLayout.rect.x - 2, guiLayout.rect.y - 2, guiLayout.rect.width + 4, guiLayout.rect.height + 4), "");
						GUI.color = Color.white;
						
						if (paintPreviewTexture != null)
						{
							var _previewRect = new Rect(guiLayout.rect.x, guiLayout.rect.y, 30, 30);
							EditorGUI.DrawPreviewTexture(_previewRect, paintPreviewTexture);
						}
			
						var _pos = new Rect(paintPreviewTexture == null ? guiLayout.rect.x : guiLayout.rect.x + 35, guiLayout.rect.y, 30, 30);
						
						
						if (GUI.Button(new Rect(_pos.x , _pos.y, 30, 30), new GUIContent(paintActive ? iconDeactivatePaint : iconActivatePaint, "activate/deactivate paint layer"))) //, GUILayout.Width(30), GUILayout.Height(30)))
						{
							SetPaintActive(!paintActive);
						}
						
						
						if (GUI.Button(new Rect(_pos.x + 30 , _pos.y, 30, 30), new GUIContent(iconCopy, "copy map")))
						{
							paintMap = _asset.mapBlueprintLayers[layerIndex].map;
							UpdatePreviewTexture();
						}
						
						if (GUI.Button(new Rect(_pos.x + 60, _pos.y, 30, 30), new GUIContent(iconFill, "Fill map"))) //, GUILayout.Width(30), GUILayout.Height(30)))
						{
							Undo.RegisterCompleteObjectUndo(tileWorldCreator.twcAsset, "TWC: Fill Map");
							
							for (int x = 0; x < paintMap.GetLength(0); x ++)
							{
								for (int y = 0; y < paintMap.GetLength(1); y ++)
								{
									paintMap[x,y] = true;
								}
							}
							
							UpdatePreviewTexture();
							
							if (executeBuildLayers)
							{	
								tileWorldCreator.ExecuteAllBlueprintLayers();
								tileWorldCreator.ExecuteAllBuildLayers(false);
							}
						}
						
						
						if (GUI.Button(new Rect(_pos.x + 90, _pos.y, 30, 30), new GUIContent( iconClear, "Clear map"))) //, GUILayout.Width(30), GUILayout.Height(30)))
						{
							Undo.RegisterCompleteObjectUndo(tileWorldCreator.twcAsset, "TWC: Clear Map");
							for (int x = 0; x < paintMap.GetLength(0); x ++)
							{
								for (int y = 0; y < paintMap.GetLength(1); y ++)
								{
									paintMap[x,y] = false;
								}
							}
							
							
							UpdatePreviewTexture();
							
							if (executeBuildLayers)
							{	
								tileWorldCreator.ExecuteAllBlueprintLayers();
								tileWorldCreator.ExecuteAllBuildLayers(false);
							}
						}
						
					
						
						guiLayout.Add(30);
						executeBuildLayers = EditorGUI.Toggle(guiLayout.rect, "", executeBuildLayers);
						
						GUI.Label(new Rect(guiLayout.rect.x + 30, guiLayout.rect.y+3, guiLayout.rect.width, 20), "Execute build layers after paint");
						guiLayout.Add(10);
					}
				}
			}
			}
		}
		#endif
		
		
		T[,] ResizeArray<T>(T[,] original, int rows, int cols)
		{
			var newArray = new T[rows,cols];
			int minRows = Math.Min(rows, original.GetLength(0));
			int minCols = Math.Min(cols, original.GetLength(1));
			for(int i = 0; i < minRows; i++)
				for(int j = 0; j < minCols; j++)
					newArray[i, j] = original[i, j];
			return newArray;
		}
		
		 
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
		
		public void SetPaintActive(bool _active)
		{
			paintActive = _active;
							
			if (paintActive)
			{
				#if UNITY_EDITOR
				TWC.Utilities.TileWorldCreatorUndoRedo.currentTWC = tileWorldCreator;
				#endif
				registeredToSceneView = true;
			}
		
			if (paintMap == null)
			{
				
				paintMap = new bool[tileWorldCreator.twcAsset.mapWidth, tileWorldCreator.twcAsset.mapHeight];
				
			}
			 
			try
			{
				clusterSize = tileWorldCreator.GetGeneratedBlueprintMap(tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].guid.ToString()).ClusterCellSize;
			
			}catch{}
			
			
			// deactivate all other paint generators
			if (_active)
			{
				for (int p = 0; p < tileWorldCreator.twcAsset.mapBlueprintLayers.Count; p ++ )
				{
					for (int s = 0; s < tileWorldCreator.twcAsset.mapBlueprintLayers[p].stack.Count; s ++)
					{
						var _paint = tileWorldCreator.twcAsset.mapBlueprintLayers[p].stack[s].action as Paint;
						if (_paint != null && _paint != this)
						{
							_paint.paintActive = false;
						}
					}
				}
			}
		}
		
		
		
		public override void DrawGizmos()
		{
			if (!paintActive)
				return;
				
			if (paintMap == null)
				return;
			
			if (tileWorldCreator == null)
			{
				paintActive = false;
				return;
			}
			//DebugDrawClusterCells();
				
			
			Vector3 viewPos = Vector3.zero;
		
			//var _cubeSize = 1 + tileWorldCreator.twcAsset.cellSize - 0.1f;
			//var _gridSize = 1 + tileWorldCreator.twcAsset.cellSize;
			
			var _cubeSize = tileWorldCreator.twcAsset.cellSize - 0.1f;
			var _gridSize = tileWorldCreator.twcAsset.cellSize;
			
			for (int x = 0; x < paintMap.GetLength(0); x ++)
			{
				for (int y = 0; y < paintMap.GetLength(1); y ++)
				{
				
					if (paintMap[x,y])
					{
						if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							//var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.y, tileWorldCreator.transform.localPosition.z + (y * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f);
							var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * tileWorldCreator.twcAsset.cellSize)  + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.y, tileWorldCreator.transform.localPosition.z + (y * tileWorldCreator.twcAsset.cellSize)  + (tileWorldCreator.twcAsset.cellSize * 0.5f));
							
							
							if (IsVisibleByCamera(_pos))
							{
								Gizmos.color = new Color(tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.r, tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.g, tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.b, 255f/255f); 
								Gizmos.DrawCube(_pos, new Vector3(_cubeSize, 0.01f, _cubeSize));
								Gizmos.color = Color.white;
							}
						}
						else
						{
							//var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.y + (y * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.z);			
							var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * tileWorldCreator.twcAsset.cellSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.y + (y * tileWorldCreator.twcAsset.cellSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.z);			
							
							if (IsVisibleByCamera(_pos))
							{
								Gizmos.color = new Color(tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.r, tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.g, tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor.b, 255f/255f); 			
								Gizmos.DrawCube(_pos, new Vector3(_cubeSize, _cubeSize, 0.01f));	
								Gizmos.color = Color.white;
							}
						}
					
					}
					else
					{
						
						if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
						{
							//var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.y, tileWorldCreator.transform.localPosition.z + (y * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f);	
							var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * tileWorldCreator.twcAsset.cellSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.y, tileWorldCreator.transform.localPosition.z + (y * tileWorldCreator.twcAsset.cellSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f));	
							
							
							if (IsVisibleByCamera(_pos))
							{
								Gizmos.color = Color.grey;
								Gizmos.DrawWireCube(_pos, new Vector3(_gridSize, 0.01f, _gridSize));
								Gizmos.color = Color.white;
							}
						}
						else
						{
							
							//var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.y + (y * _gridSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f) + 0.5f, tileWorldCreator.transform.localPosition.z);			
							var _pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * tileWorldCreator.twcAsset.cellSize)  + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.y + (y * tileWorldCreator.twcAsset.cellSize) + (tileWorldCreator.twcAsset.cellSize * 0.5f), tileWorldCreator.transform.localPosition.z);			
							
							if (IsVisibleByCamera(_pos))
							{
								Gizmos.color = Color.grey;
								Gizmos.DrawWireCube(_pos, new Vector3(_gridSize, _gridSize, 0.01f));
								Gizmos.color = Color.white;
							}
						}
					}
					
					
				}
			}
		}
	
		
		private void DebugDrawClusterCells()
		{
			
			try
			{
				clusterSize = tileWorldCreator.GetGeneratedBlueprintMap(tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].guid.ToString()).ClusterCellSize;
			}
			catch{}
			
			if (clusterSize <= 0)
			{
				return;
			}
		
			
			var _gridSize = 1 + tileWorldCreator.twcAsset.cellSize;
			var _x = (paintMap.GetLength(0) / clusterSize) * 2;
			var _y = (paintMap.GetLength(1) / clusterSize) * 2;
			
			for (int x = 0; x < _x + 1; x ++)
			{
				for (int y = 0; y < _y + 1; y ++)
				{
					
					Vector3 _pos = Vector3.zero;
					
					if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
					{
						_pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * clusterSize + (clusterSize * 0.5f)), tileWorldCreator.transform.localPosition.y + 0.2f, tileWorldCreator.transform.localPosition.z + (y * clusterSize+ (clusterSize * 0.5f)));
						
						if (IsVisibleByCamera(_pos))
						{
							Gizmos.DrawWireCube(_pos, new Vector3(clusterSize, 0.05f, clusterSize));
						}
					}
					else
					{
						_pos = new Vector3(tileWorldCreator.transform.localPosition.x + (x * clusterSize + (clusterSize * 0.5f)), tileWorldCreator.transform.localPosition.y + (y * clusterSize+ (clusterSize * 0.5f)), tileWorldCreator.transform.localPosition.z - 0.2f);	
						
						if (IsVisibleByCamera(_pos))
						{
							Gizmos.DrawWireCube(_pos, new Vector3(clusterSize - 0.1f, clusterSize - 0.1f, 0.05f));
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
		
		#if UNITY_EDITOR
		public override void DrawSceneGUI(Rect _sceneView)
		{
			if (tileWorldCreator == null)
			{
				tileWorldCreator = FindTWCComponent();
				return;
			}
			
			if (registeredToSceneView)
			{
			
				Handles.BeginGUI();
			
				using (new GUILayout.VerticalScope("Box", GUILayout.Width(300)))
				{
					GUILayout.Label("Paint: LEFT CTRL + Mouse button");
				}
				
				if (paintActive)
				{
					GUI.color = Color.green;
				}
				else
				{
					GUI.color = Color.white;
				}
				
				if (layerIndex < tileWorldCreator.twcAsset.mapBlueprintLayers.Count)
				{
				
					using (new GUILayout.HorizontalScope("Box", GUILayout.Width(300)))
					{
						GUI.color = Color.white;
						
						GUILayout.Label("Paint layer: " + tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].layerName);
						if (GUILayout.Button("Act./Dea."))
						{
							SetPaintActive(!paintActive);
	
							//var _window =(TileWorldCreatorPaintEditor)SceneView.GetWindow<TileWorldCreatorPaintEditor>(typeof(TileWorldCreatorPaintEditor));
							//_window.Open();
						}
						
						if (GUILayout.Button("x", GUILayout.Width(20)))
						{
							registeredToSceneView = false;
						}
					}
				
				}
	
				Handles.EndGUI();
			}
			
			
			if (!paintActive)
				return;
				
			Event _event = Event.current;
			
			EditorGUIUtility.AddCursorRect(_sceneView, MouseCursor.Link);
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			
			#if UNITY_EDITOR_OSX
			
			if (_event.control)
			{
				controlClick = true;
			}
			
			#else
		
			if (_event.GetTypeForControl(controlID) == EventType.KeyDown)
			{
				if (_event.keyCode == KeyCode.LeftControl)
				{
					controlClick = true;
				}
			}
			#endif
		
			
		
			if (_event.type == EventType.MouseDown && controlClick)
			{
				
				if (IsMouseOverGrid(_event.mousePosition))
				{
					
					Undo.RegisterCompleteObjectUndo(tileWorldCreator.twcAsset, "TWC: Paint");

					var _worldPos = GetWorldPosition(_event.mousePosition);
					var _gridPos = GetGridPosition(_worldPos);
					
					
					SetCell(_gridPos, _event.button == 0 ? true : false);
					
					
					_event.Use();
				}
			}
			else if (_event.type == EventType.MouseDrag && controlClick) // && _event.type != EventType.KeyDown)
			{
				if (IsMouseOverGrid(_event.mousePosition))
				{
					var _worldPos = GetWorldPosition(_event.mousePosition);
					var _gridPos = GetGridPosition(_worldPos);
					
					//Debug.Log(_gridPos);
					SetCell(_gridPos, _event.button == 0 ? true : false);
					
					_event.Use();
				}
			}
			
			
			
			
			if (_event.type == EventType.MouseUp && controlClick)
			{
				if (IsMouseOverGrid(_event.mousePosition))
				{
		
					UpdatePreviewTexture();
					
					if (executeBuildLayers)
					{	
						tileWorldCreator.ExecuteAllBlueprintLayers();
						tileWorldCreator.ExecuteAllBuildLayers(false);
					}
				}
				
				controlClick = false;
			}

			
			if (_event.GetTypeForControl(controlID) == EventType.KeyUp)
			{
				#if UNITY_EDITOR_OSX
				if (!_event.control)
				{
					controlClick = false;
				}
				#else
				if (_event.keyCode == KeyCode.LeftControl)
				{
					controlClick = false;
				}
				#endif
			}
			
			
			if (_event.type == EventType.Layout)
			{
				//this allows _event.Use() to actually function and block mouse input
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
			}
		}
		#endif
		
		void SetCell(Vector3 _gridPos, bool _state)
		{
		
			if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			{
				if ((int)_gridPos.x >= 0 && (int)_gridPos.x < paintMap.GetLength(0) && (int)_gridPos.z >= 0 && (int)_gridPos.z < paintMap.GetLength(1))
				{
					//Debug.Log("Paint");
					paintMap[(int)_gridPos.x, (int)_gridPos.z] = _state; 
				}
			}
			
			if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XY)
			{
				if ((int)_gridPos.x >= 0 && (int)_gridPos.x < paintMap.GetLength(0) && (int)_gridPos.y >= 0 && (int)_gridPos.y < paintMap.GetLength(1))
				{
					//Debug.Log("Paint");
					paintMap[(int)_gridPos.x, (int)_gridPos.y] = _state; 
				}
			}
		}
		
		void UpdatePreviewTexture()
		{
			if (tileWorldCreator == null)
				return;
				
			paintPreviewTexture = tileWorldCreator.UpdatePreviewTexture(paintMap, tileWorldCreator.twcAsset.mapBlueprintLayers[layerIndex].previewColor, null);
		}
		
		
		#if UNITY_EDITOR
		bool IsMouseOverGrid(Vector2 _mousePos)
		{
			Ray _ray = HandleUtility.GUIPointToWorldRay(_mousePos); //Camera.current.ScreenToWorldPoint(_pos);
			float _dist = 0.0f;
			bool _return = false;
			Plane groundPlane;

			if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			{
				groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			}
			else
			{
				groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));	
			}

	
			if (groundPlane.Raycast(_ray, out _dist))
			{
				Vector3 _worldPos = _ray.origin + _ray.direction.normalized * _dist;
	
			
				var _cellSize = (tileWorldCreator.twcAsset.cellSize + 1);
			
				if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
				{	
					if (_worldPos.x -  tileWorldCreator.transform.localPosition.x < (paintMap.GetLength(0) * _cellSize) &&
						_worldPos.z - tileWorldCreator.transform.localPosition.z < (paintMap.GetLength(1) * _cellSize))	
					{
						_return = true;
					}
				}
				else
				{
					if (_worldPos.x -  tileWorldCreator.transform.localPosition.x < (paintMap.GetLength(0) * _cellSize) &&
						_worldPos.y - tileWorldCreator.transform.localPosition.y < (paintMap.GetLength(1) * _cellSize))
					{
						_return = true;
					}
				}
			}
	
			return _return;
		}
		
	
		Vector3 GetWorldPosition(Vector2 _mousePos)
		{
			Ray _ray = HandleUtility.GUIPointToWorldRay(_mousePos);
			float _dist = 0.0f;
			Vector3 _return = new Vector3(0, 0);
			Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			
			if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			{
				groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			}
			else
			{
				groundPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, 0f));
			}
	
			if (groundPlane.Raycast(_ray, out _dist))
			{
				_return = _ray.origin + _ray.direction.normalized * _dist;
			}
			

			return _return;
		}
		#endif
		
		Vector3 GetGridPosition(Vector3 _worldPos)
		{
			Vector3 _gridPos = Vector3.zero;
	
			var _cellSize  = tileWorldCreator.twcAsset.cellSize;
			
			if (tileWorldCreator.twcAsset.mapOrientation == TileWorldCreatorAsset.MapOrientation.XZ)
			{
				_gridPos = new Vector3((Mathf.Floor(_worldPos.x - tileWorldCreator.transform.localPosition.x / 1) / _cellSize), 0.05f, (Mathf.Floor(_worldPos.z - tileWorldCreator.transform.localPosition.z / 1) / _cellSize));
	        
			}
			else
			{
				_gridPos = new Vector3((Mathf.Floor(_worldPos.x - tileWorldCreator.transform.localPosition.x / 1) / _cellSize), (Mathf.Floor(_worldPos.y - tileWorldCreator.transform.localPosition.y / 1) / _cellSize) , 0.05f);
	       
			}
			
			return _gridPos;
		}
		
		
		
		private void LoadResources()
		{
			iconCopy = EditorUtilities.LoadIcon("copy.png");
			iconFill = EditorUtilities.LoadIcon("fill.png");
			iconClear = EditorUtilities.LoadIcon("clear.png");
			iconActivatePaint = EditorUtilities.LoadIcon("paint.png");
			iconDeactivatePaint = EditorUtilities.LoadIcon("paintDeactivate.png");
		}
	
	}
}