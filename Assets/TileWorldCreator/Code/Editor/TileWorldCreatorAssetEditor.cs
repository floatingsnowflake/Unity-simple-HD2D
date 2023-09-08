using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Rendering;
#endif

using System.Linq;
using System.Reflection;

using TWC;
using TWC.Actions;
using TWC.Utilities;

namespace TWC.editor
{

	[CustomEditor(typeof(TileWorldCreatorAsset))]
	public class TileWorldCreatorAssetEditor : Editor
	{
		public TileWorldCreatorAsset tileWorldCreatorAsset;
		
		public class AvailableActions
		{
			public string name;
			public string typeName;
			public string assemblyName;
			
			public AvailableActions (string _name, string _typeName, string _assemblyName)
			{
				name = _name;
				typeName = _typeName;
				assemblyName = _assemblyName;
			}
		}
		
		public List<AvailableActions> stackGenerators = new List<AvailableActions>();
		public List<AvailableActions> stackModifiers = new List<AvailableActions>();
		public List<AvailableActions> buildLayerActions = new List<AvailableActions>();
		
		private int layerHeight = 30;
		
		private static GUIStyle elementButton;
		private static GUIStyle whiteBoxStyle;
		
		private Texture2D previewTexture;
		
		private static Texture2D iconObjectLayer;
		private static Texture2D iconTilesLayer;
		private static Texture2D icon4TilesLayer;
		private static Texture2D icon6TilesLayer;
		private static Texture2D icon2DTilesLayer;
		private static Texture2D iconBlueprintLayers;
		private static Texture2D iconBuildLayers;
		private static Texture2D iconPlus;
		private static Texture2D iconUp;
		private static Texture2D iconDown;
		private static Texture2D iconExecute;
		private static Texture2D iconRemove;
		private static Texture2D iconDuplicate;
		private static Texture2D iconDestroy;
		private static Texture2D iconBuild;
		
		private bool forceRebuild;
		private bool leftControl;
		
		private string twcGuid;
		private long twcFile;
		
		public class GenericMenuData
		{
			public string name;
			public string typeName;
			public string assemblyName;
			public int mapStackIndex;
		}
		
		
		public void OnEnable()
		{
			try
			{
				tileWorldCreatorAsset = (TileWorldCreatorAsset) target;
			}catch{}
		}
		
	
		void LoadResources()
		{
			iconObjectLayer = TWC.editor.EditorUtilities.LoadIcon("objectLayer.png");
			iconTilesLayer = TWC.editor.EditorUtilities.LoadIcon("tilesLayer.png");
			icon4TilesLayer = TWC.editor.EditorUtilities.LoadIcon("4TilesBuildLayer.png");
			icon6TilesLayer = TWC.editor.EditorUtilities.LoadIcon("6TilesBuildLayer.png");
			icon2DTilesLayer = TWC.editor.EditorUtilities.LoadIcon("2DTilesLayer.png");
			iconBlueprintLayers = TWC.editor.EditorUtilities.LoadIcon("blueprintLayers.png");
			iconBuildLayers = TWC.editor.EditorUtilities.LoadIcon("buildLayers.png");
			iconPlus = TWC.editor.EditorUtilities.LoadIcon("plus.png");
			iconUp = TWC.editor.EditorUtilities.LoadIcon("up.png");
			iconDown = TWC.editor.EditorUtilities.LoadIcon("down.png");
			iconExecute = TWC.editor.EditorUtilities.LoadIcon("execute.png");
			iconRemove = TWC.editor.EditorUtilities.LoadIcon("remove.png");
			iconDuplicate = TWC.editor.EditorUtilities.LoadIcon("duplicate.png");
			iconDestroy = TWC.editor.EditorUtilities.LoadIcon("destroy.png");
			iconBuild = TWC.editor.EditorUtilities.LoadIcon("build.png");
		}
		
		public override void OnInspectorGUI()
		{
			DrawGUI(null);
		}
		
		public void DrawGUI(TileWorldCreator tileWorldCreator)
		{
			
			if (iconObjectLayer == null)
			{
				LoadResources();
			}
			
			
			if (stackGenerators.Count == 0)
			{
				GetTypes<TWCBlueprintAction>(out stackGenerators, out stackModifiers);
			}
		    
			if (buildLayerActions.Count == 0)
			{
				GetTypesBuildLayer(out buildLayerActions);
			}
		    
			var elementBackground = new GUIStyle("RL Element"){};
				
			elementButton = new GUIStyle("Button")
			{
				
				hover = elementBackground.onHover,
				active = elementBackground.onActive,
									
				normal = new GUIStyleState 
				{ 
				background = elementBackground.onNormal.background,
						#if UNITY_EDITOR
				textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,	
						#endif
					},
					
				alignment = TextAnchor.MiddleLeft,
									
				overflow = new RectOffset(25, 131, 1, 3),
				padding = new RectOffset(10, 0, 0, 0)
			};
		   
			var _texture = EditorGUIUtility.whiteTexture;
		    
			whiteBoxStyle = new GUIStyle();    
			whiteBoxStyle.normal.background = _texture;
		   
		   
			// Check for left control key down or up
			int controlID2 = GUIUtility.GetControlID(FocusType.Passive);
			var e = Event.current;
			if (e.GetTypeForControl(controlID2) == EventType.KeyDown)
			{
				if (e.keyCode == KeyCode.LeftControl)
				{
					leftControl = true;
				}
			}
				
			if (e.GetTypeForControl(controlID2) == EventType.KeyUp)
			{
				if (e.keyCode == KeyCode.LeftControl)
				{
					leftControl = false;
				}	
			}

			
		   
			using (new GUILayout.VerticalScope("Box"))
			{
				tileWorldCreatorAsset.showSettings = GUILayout.Toggle( tileWorldCreatorAsset.showSettings, "Settings", "Foldout");
			
		   
				if (tileWorldCreatorAsset.showSettings)
				{
					using (new GUILayout.VerticalScope("Box"))
					{
						tileWorldCreatorAsset.worldName = EditorGUILayout.TextField("World name:", tileWorldCreatorAsset.worldName);
						tileWorldCreatorAsset.mapWidth = EditorGUILayout.IntField("Map width:", tileWorldCreatorAsset.mapWidth);
						tileWorldCreatorAsset.mapHeight = EditorGUILayout.IntField("Map height:", tileWorldCreatorAsset.mapHeight);	
						tileWorldCreatorAsset.cellSize = EditorGUILayout.Slider("Cell size:", tileWorldCreatorAsset.cellSize, 1f, 100f);
						
						tileWorldCreatorAsset.mapOrientation = (TileWorldCreatorAsset.MapOrientation)EditorGUILayout.EnumPopup("Map orientation:", tileWorldCreatorAsset.mapOrientation);
						
						
						using (new GUILayout.VerticalScope("Box"))
						{
							GUILayout.Label("Random Seed:");
							
							tileWorldCreatorAsset.useRandomSeed = EditorGUILayout.Toggle("Custom random seed:",tileWorldCreatorAsset.useRandomSeed);
							if (tileWorldCreatorAsset.useRandomSeed)
							{
								tileWorldCreatorAsset.randomSeed = EditorGUILayout.IntField("Random seed:", tileWorldCreatorAsset.randomSeed);
							}
							else
							{
								tileWorldCreatorAsset.useNewRandomSeedForEveryLayer = EditorGUILayout.Toggle("New random seed for every layer", tileWorldCreatorAsset.useNewRandomSeedForEveryLayer);
							}
						}
						
						tileWorldCreatorAsset.mergePreviewTextures = EditorGUILayout.Toggle("Merge preview textures", tileWorldCreatorAsset.mergePreviewTextures);
			    	
					}
				
				
					using (new GUILayout.VerticalScope("Box"))
					{
						
						tileWorldCreatorAsset.useCustomClusterCelSize = EditorGUILayout.Toggle("Use custom cluster cell size:", tileWorldCreatorAsset.useCustomClusterCelSize);
			    	
						if (tileWorldCreatorAsset.useCustomClusterCelSize)
						{
							tileWorldCreatorAsset.customClusterCellSize = EditorGUILayout.IntField("Custom cluster cell size:", tileWorldCreatorAsset.customClusterCellSize);
							
							EditorGUILayout.HelpBox("Warning changing the cluster cell size can result in slow instantiation.", MessageType.Warning);
						}
					}
				}
			}
			
			//try
			//{
				BlueprintLayers(tileWorldCreator);
				BuildLayers(tileWorldCreator);
			//}
			//catch{}
			
			
			if (tileWorldCreator != null)
			{
				GUILayout.Space(20);
				
				if (tileWorldCreator.buildResultNoChanges)
				{
					EditorGUILayout.HelpBox("No changes on map, force a rebuild by holding the left-CTRL key an click on the Execute all layers button", MessageType.Info);
				}
				
				using (new GUILayout.VerticalScope("Box"))
				{
					if (GUILayout.Button(new GUIContent("Execute all layers", "Hold Left-CTRL to force a complete rebuild"), GUILayout.Height(50)))
					{
						tileWorldCreator.OnBlueprintLayersComplete += MapGenerationComplete;
						tileWorldCreator.ExecuteAllBlueprintLayers();
					}
				}
				
			}
			
			if (leftControl)
			{
				try
				{
					using (new GUILayout.HorizontalScope("Box"))
					{
						GUILayout.Label("Force Rebuild");	
						GUILayout.FlexibleSpace();
					}
				}
				catch
				{
					
				}
			}
			
			
			EditorUtility.SetDirty(tileWorldCreatorAsset);
		}
		
		void MapGenerationComplete(TileWorldCreator _twc)
		{
			_twc.OnBlueprintLayersComplete -= MapGenerationComplete;
			_twc.ExecuteAllBuildLayers(leftControl);
			
			
			leftControl = false;
		}
		
		void BlueprintLayers(TileWorldCreator tileWorldCreator)
		{
			
			// Map generation stack
			///
			using (new GUILayout.VerticalScope("TextArea"))
			{
				using (new GUILayout.HorizontalScope())
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(iconBlueprintLayers);
					}
					
					var richTextStyle = new GUIStyle(GUI.skin.label);
					richTextStyle.richText = true;
					richTextStyle.alignment = TextAnchor.MiddleCenter;
					GUILayout.Label("<size=16>Blueprint Layers</size>", richTextStyle, GUILayout.Height(layerHeight));
					
					GUILayout.FlexibleSpace();
					using (new GUILayout.VerticalScope())
					{
						//GUILayout.Label("Map Generation layers", "boldLabel");
						//GUILayout.Space(2);
						if (GUILayout.Button(new GUIContent(iconPlus, ""), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
						{
							tileWorldCreatorAsset.mapBlueprintLayers.Add(new TileWorldCreatorAsset.BlueprintLayerData("", true));
						}
					}
			    
				}
				
			
				EditorUtilities.DrawUILine(Color.black, 1, 0);
				for(int m = 0; m < tileWorldCreatorAsset.mapBlueprintLayers.Count; m ++)
				{

					var rowBackground = "AnimationRowEven";
					//if (m % 2 == 0)
					//{
					//	rowBackground = "AnimationRowOdd";
					//}
					//else
					//{
					//	rowBackground = "AnimationRowEven";
					//}
					
								
					using (new GUILayout.VerticalScope(rowBackground))
					{
						using (new GUILayout.HorizontalScope("Button", GUILayout.Height(layerHeight)))
						{
							if (!tileWorldCreatorAsset.mapBlueprintLayers[m].mapResultFailed)
							{
								GUI.color = Color.green;
							}
							else
							{
								GUI.color = Color.yellow;
							}
							
							GUILayout.Box("", whiteBoxStyle, GUILayout.Width(3), GUILayout.Height(layerHeight));
							GUI.color = Color.white;
							
							
							using (new GUILayout.HorizontalScope())
							{
						    	
						    	
								tileWorldCreatorAsset.mapBlueprintLayers[m].active = GUILayout.Toggle(tileWorldCreatorAsset.mapBlueprintLayers[m].active, "", GUILayout.Width(20), GUILayout.Height(layerHeight));
								
						
								// PREVIEW TEXTURE
								if (tileWorldCreatorAsset.mapBlueprintLayers[m].previewTextureMap != null)
								{
									GUILayout.Label("",GUILayout.Width(layerHeight));
									var _lastRect = GUILayoutUtility.GetLastRect();
									var _previewRect = new Rect(_lastRect.x, _lastRect.y, layerHeight, layerHeight);
									EditorGUI.DrawPreviewTexture(_previewRect, tileWorldCreatorAsset.mapBlueprintLayers[m].previewTextureMap);
								}
								else
								{
									GUILayout.Box("", "TextArea", GUILayout.Width(layerHeight), GUILayout.Height(layerHeight));
								}
								
								GUILayout.Label(iconBlueprintLayers, GUILayout.Width(30));
								
					    			
								//tileWorldCreatorAsset.mapBlueprintLayers[m].foldout = GUILayout.Toggle(tileWorldCreatorAsset.mapBlueprintLayers[m].foldout, tileWorldCreatorAsset.mapBlueprintLayers[m].layerName, "Foldout", GUILayout.Height(layerHeight));
								if (GUILayout.Button( tileWorldCreatorAsset.mapBlueprintLayers[m].layerName, "Foldout", GUILayout.Height(layerHeight)))
								{
									if (tileWorldCreatorAsset.mapBlueprintLayers[m].reorderableList == null)
									{
										try
										{
											CreateReorderableList(m, tileWorldCreator);
										}
										catch{}
										
									}
									
									tileWorldCreatorAsset.mapBlueprintLayers[m].foldout = !tileWorldCreatorAsset.mapBlueprintLayers[m].foldout;
								}
								
								
								// Up / Down buttons
								if (m > 0)
								{
									if (GUILayout.Button(new GUIContent(iconUp, "Move Up"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
									{
										var _a = tileWorldCreatorAsset.mapBlueprintLayers[m];
										tileWorldCreatorAsset.mapBlueprintLayers.RemoveAt(m);
										tileWorldCreatorAsset.mapBlueprintLayers.Insert(m - 1, _a);
										
										CreateReorderableList(m - 1, tileWorldCreator);
										CreateReorderableList(m, tileWorldCreator);
									}
								}
								if (m + 1 < tileWorldCreatorAsset.mapBlueprintLayers.Count)
								{
									if (GUILayout.Button(new GUIContent(iconDown, "Move Down"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
									{
										var _a = tileWorldCreatorAsset.mapBlueprintLayers[m];
										tileWorldCreatorAsset.mapBlueprintLayers.RemoveAt(m);
										tileWorldCreatorAsset.mapBlueprintLayers.Insert(m + 1, _a);
										
										CreateReorderableList(m + 1, tileWorldCreator);
										CreateReorderableList(m, tileWorldCreator);
									}
								}
									
								// Duplicate button
								if (GUILayout.Button(new GUIContent(iconDuplicate, "Duplicate this layer"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
								{
									var _original = (TileWorldCreatorAsset.BlueprintLayerData)tileWorldCreatorAsset.mapBlueprintLayers[m];
									TileWorldCreatorAsset.BlueprintLayerData _clone = _original.Clone(true);
									tileWorldCreatorAsset.mapBlueprintLayers.Add(_clone);
								}
								
								if (tileWorldCreator == null)
								{
									GUI.enabled = false;
								}
								else
								{
									GUI.enabled = true;
								}
									
								// Layer execute button
								if (GUILayout.Button(new GUIContent(iconExecute, "Execute this layer"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
								{
									tileWorldCreator.ExecuteBlueprintLayer(tileWorldCreatorAsset.mapBlueprintLayers[m].layerName);
								}
									
								GUI.enabled = true;
								
								// Remove layer
								if (GUILayout.Button(new GUIContent(iconRemove, "Remove this layer"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
								{
								
									if (EditorUtility.DisplayDialog("Remove layer?",
										"Are you sure you want to remove layer: " + tileWorldCreatorAsset.mapBlueprintLayers[m].layerName, "Yes", "No"))
									{
										tileWorldCreatorAsset.mapBlueprintLayers.RemoveAt(m);
									}
									GUIUtility.ExitGUI();
								}
							}
							
						}
				    	
				    	
						//try
						//{
						if (tileWorldCreatorAsset.mapBlueprintLayers[m].foldout)
						{
							
							EditorGUI.indentLevel ++;
							
							// PREVIEW THUMBNAIL LARGE
							using (new GUILayout.HorizontalScope())
							{
								using (new GUILayout.VerticalScope())
								{
									GUILayout.Label("", GUILayout.Width(100), GUILayout.Height(100));
									var _previewRect = GUILayoutUtility.GetLastRect();
									GUI.Box(new Rect(_previewRect.x + 20, _previewRect.y + 2, _previewRect.width, _previewRect.height), "");
									
									if (tileWorldCreatorAsset.mapBlueprintLayers[m].previewTextureMap != null)
									{
										GUI.DrawTexture(new Rect(_previewRect.x + 20, _previewRect.y + 2, _previewRect.width, _previewRect.height), tileWorldCreatorAsset.mapBlueprintLayers[m].previewTextureMap, ScaleMode.ScaleToFit);
									}
								}
								
								GUILayout.Space(20);
								
								
							using (new GUILayout.VerticalScope())
							{
								
								
								using (new GUILayout.HorizontalScope())
								{
									GUILayout.Label("Layer name:");
									tileWorldCreatorAsset.mapBlueprintLayers[m].layerName = EditorGUILayout.TextField("", tileWorldCreatorAsset.mapBlueprintLayers[m].layerName);
								}
								
								using (new GUILayout.HorizontalScope())
								{
									GUILayout.Label("Color:");
									tileWorldCreatorAsset.mapBlueprintLayers[m].previewColor = EditorGUILayout.ColorField(new GUIContent(""), tileWorldCreatorAsset.mapBlueprintLayers[m].previewColor, true, false, false);
								}
								
								
								using (new GUILayout.VerticalScope("Box"))
								{
									using (new GUILayout.HorizontalScope())
									{
										GUILayout.Label("Override random seed:");
										tileWorldCreatorAsset.mapBlueprintLayers[m].randomSeedOverride = EditorGUILayout.Toggle(tileWorldCreatorAsset.mapBlueprintLayers[m].randomSeedOverride);
									}	
								
									if (tileWorldCreatorAsset.mapBlueprintLayers[m].randomSeedOverride)
									{
										tileWorldCreatorAsset.mapBlueprintLayers[m].useCustomSeed = EditorGUILayout.Toggle("use custom seed", tileWorldCreatorAsset.mapBlueprintLayers[m].useCustomSeed );
										
										if (tileWorldCreatorAsset.mapBlueprintLayers[m].useCustomSeed)
										{
											tileWorldCreatorAsset.mapBlueprintLayers[m].customLayerRandomSeed = EditorGUILayout.IntField("custom seed", tileWorldCreatorAsset.mapBlueprintLayers[m].customLayerRandomSeed);
										}
									}
								}
											
							} 
							
								
							}
			    	
							EditorUtilities.DrawUILine(Color.black, 1, 0);
							
							
							// DRAW LAYER STACK
							using (new GUILayout.HorizontalScope())
							{
					    		
								GUILayout.Space(20);
						    	
								using (new GUILayout.VerticalScope())
								{
						    		
						    			
									if (tileWorldCreatorAsset.mapBlueprintLayers[m].reorderableList == null)
									{
										CreateReorderableList(m, tileWorldCreator);		
									}
						    		
									//try
									//{
										tileWorldCreatorAsset.mapBlueprintLayers[m].reorderableList.DoLayoutList();
									//}
									//catch(System.Exception e)
									//{
									//	if(ExitGUIUtility.ShouldRethrowException(e))
									//	{
									//		throw;
									//	}
									//}
								}
							}
							EditorGUI.indentLevel --;
				    	
				    	
						} // Horizontal Scope
						//}
						//catch(System.Exception e)
						//{
						//	if(ExitGUIUtility.ShouldRethrowException(e))
						//	{
						//		throw;
						//	}
						//	Debug.LogException(e);
						//}
					} // Vertical Scope
			    	
			    	
					EditorUtilities.DrawUILine(Color.black, 1, 0);
				}
			    
			
				GUILayout.Space(1);

			
				if (tileWorldCreator == null)
				{
					GUI.enabled = false;
				}
				else
				{
					GUI.enabled = true;
				}
							
				if (GUILayout.Button("Execute all blueprint layers", GUILayout.Height(30)))
				{
					tileWorldCreator.ExecuteAllBlueprintLayers();
				}
				
				GUI.enabled = true;
			}
		    
			
		}
		
		void CreateReorderableList(int _layerIndex, TileWorldCreator _twc)
		{
			
		
			if (tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack == null)
			{
				tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack = new List<TileWorldCreatorAsset.BlueprintLayerData.ActionStack>();
			}

			if (_layerIndex > tileWorldCreatorAsset.mapBlueprintLayers.Count)
				return;
		
			tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].reorderableList = new ReorderableList(tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack, typeof(TileWorldCreatorAsset.BlueprintLayerData.ActionStack), true, true, true, true)
			{
				displayAdd = true,
				displayRemove = false,
				draggable = true,
				drawHeaderCallback = DrawHeader,
							    			
				onSelectCallback =  
				{
								    				
				},
								    		
				drawElementCallback = (rect, index, isActive, isFocused) =>
				{
								    		
					var _stack = tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack[index].action  as TWCBlueprintAction;
									    		
					if (!_stack.resultFailed)
					{
						GUI.color = Color.green;
					}
					else
					{
						GUI.color = Color.yellow;
					}
					GUI.Box(new Rect(rect.x, rect.y, 3, 18),"", whiteBoxStyle);
					GUI.color = Color.white;
									    		
					_stack.active = EditorGUI.Toggle(new Rect(rect.x + 7, rect.y, 30, 20), "", _stack.active);
					
					if (_stack.ShowFoldout)
					{
						_stack.foldout = EditorGUI.Foldout(new Rect(rect.x + 35, rect.y, 20, 20), _stack.foldout, "");
					}
					
					EditorGUI.LabelField(new Rect(rect.x + 37, rect.y, rect.width, 20), tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack[index].actionName.ToString(), "");
									    		
					// Copy guid
					//if(GUI.Button(new Rect(rect.width + 10, rect.y, 38, 18), new GUIContent("Guid", "Copy guid to clipboard"), "miniButton")){ GUIUtility.systemCopyBuffer = (tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack[index].action  as TWCBlueprintAction).guid.ToString(); }
					// Remove
					if(GUI.Button(new Rect(rect.width + 48, rect.y, 18, 18), "-", "miniButton")){ tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack.RemoveAt(index); }
									    		
					if (_stack.foldout)
					{
						if (_twc == null)
						{
							_twc = FindTWCComponent();
						}
						_stack.DrawGUI(rect, _layerIndex, tileWorldCreatorAsset, _twc);
					}
	
				},
							
			
				elementHeightCallback = (index) =>
				{
					if (_layerIndex > tileWorldCreatorAsset.mapBlueprintLayers.Count)
						return 20;

					var _stackT1 = tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack[index].action  as ITWCAction;
					var _stackT2 = tileWorldCreatorAsset.mapBlueprintLayers[_layerIndex].stack[index].action  as TWCBlueprintAction;
					if (_stackT2.foldout)
					{
						return _stackT1.GetGUIHeight();
					}
					else
					{
						return 20f;
					}
				},
								    		
				onAddCallback = (list) =>
				{
								    			
									    		
					GenericMenu menu = new GenericMenu();
								    			
								    			
					for (int a = 0; a < stackGenerators.Count; a ++)
					{
						GenericMenuData _genericMenuData = new GenericMenuData();
						_genericMenuData.name = stackGenerators[a].name;
						_genericMenuData.typeName = stackGenerators[a].typeName;
						_genericMenuData.assemblyName = stackGenerators[a].assemblyName;
						_genericMenuData.mapStackIndex = _layerIndex;
							    		
						menu.AddItem(new GUIContent("Generators/"+stackGenerators[a].name), false, AddNewAction, _genericMenuData);
					}
									    		
					for (int a = 0; a < stackModifiers.Count; a ++)
					{
						GenericMenuData _genericMenuData = new GenericMenuData();
						_genericMenuData.name = stackModifiers[a].name;
						_genericMenuData.typeName = stackModifiers[a].typeName;
						_genericMenuData.assemblyName = stackModifiers[a].assemblyName;
						_genericMenuData.mapStackIndex = _layerIndex;
							    		
						menu.AddItem(new GUIContent("Modifiers/"+stackModifiers[a].name), false, AddNewAction, _genericMenuData);
					}
					    	
					// display the menu
					menu.ShowAsContext();
				}
							    		
			};
		}
		
		
		TileWorldCreator FindTWCComponent()
		{
			string _path = AssetDatabase.GUIDToAssetPath(twcGuid);
			var _tileWorldCreator = AssetDatabase.LoadAssetAtPath<TileWorldCreator>(_path)  as TileWorldCreator;
				
			if (_tileWorldCreator == null)
			{
			
				var _twc = GameObject.FindObjectOfType<TileWorldCreator>();
				if (_twc != null)
				{
					_tileWorldCreator = _twc;
					
					if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_tileWorldCreator, out twcGuid, out twcFile))
					{
					}
				}
			}
			
			return _tileWorldCreator;
		}
		
		
		void BuildLayers(TileWorldCreator tileWorldCreator)
		{
			// Tile placement stack
			using (new GUILayout.VerticalScope("TextArea"))
			{
				using (new GUILayout.HorizontalScope())
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(iconBuildLayers);
					}
			
				
					
					var richTextStyle = new GUIStyle(GUI.skin.label);
					richTextStyle.richText = true;
					richTextStyle.alignment = TextAnchor.MiddleCenter;
					GUILayout.Label("<size=16>Build Layers</size>", richTextStyle, GUILayout.Height(layerHeight));
					
					GUILayout.FlexibleSpace();
					using (new GUILayout.VerticalScope())
					{
						if (GUILayout.Button(new GUIContent(iconPlus, ""), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
					{
						GenericMenu menu = new GenericMenu();
						for (int a = 0; a < buildLayerActions.Count; a ++)
						{
							GenericMenuData _genericMenuData = new GenericMenuData();
							_genericMenuData.name = buildLayerActions[a].name;
							_genericMenuData.typeName = buildLayerActions[a].typeName;
							_genericMenuData.assemblyName = buildLayerActions[a].assemblyName;
							_genericMenuData.mapStackIndex = a;
									    		
							menu.AddItem(new GUIContent(buildLayerActions[a].name), false, AddNewBuildLayer, _genericMenuData);
						}
							    	
						// display the menu
						menu.ShowAsContext();
					}
					}
				}
			    
			    
				EditorUtilities.DrawUILine(Color.black, 1, 0);
			    
				for (int t = 0; t < tileWorldCreatorAsset.mapBuildLayers.Count; t ++)
				{
					using (new GUILayout.VerticalScope("AnimationRowEven"))
					{
			    		
			    		
						using (new GUILayout.HorizontalScope("Button"))
						{
							tileWorldCreatorAsset.mapBuildLayers[t].active = GUILayout.Toggle(tileWorldCreatorAsset.mapBuildLayers[t].active, "", GUILayout.Width(20), GUILayout.Height(layerHeight));
							
							if (tileWorldCreatorAsset.mapBuildLayers[t].GetType() == typeof(InstantiateTiles))
							{
								GUILayout.Label(icon4TilesLayer, GUILayout.Width(30));
							}
							else if (tileWorldCreatorAsset.mapBuildLayers[t].GetType() == typeof(Instantiate6Tiles))
							{
								GUILayout.Label(icon6TilesLayer, GUILayout.Width(30));
							}
							else
							{
								GUILayout.Label(iconObjectLayer, GUILayout.Width(30));
							}
							
							tileWorldCreatorAsset.mapBuildLayers[t].foldout = GUILayout.Toggle(tileWorldCreatorAsset.mapBuildLayers[t].foldout, tileWorldCreatorAsset.mapBuildLayers[t].layerName, "Foldout", GUILayout.Height(layerHeight));
							
							// Up / Down buttons
							if (t > 0)
							{
								if (GUILayout.Button(new GUIContent(iconUp, "Move up"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
								{
									var _a = tileWorldCreatorAsset.mapBuildLayers[t];
									tileWorldCreatorAsset.mapBuildLayers.RemoveAt(t);
									tileWorldCreatorAsset.mapBuildLayers.Insert(t - 1, _a);
								}
							}
							if (t + 1 < tileWorldCreatorAsset.mapBlueprintLayers.Count)
							{
								if (GUILayout.Button(new GUIContent(iconDown, "Move down"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
								{
									var _a = tileWorldCreatorAsset.mapBuildLayers[t];
									tileWorldCreatorAsset.mapBuildLayers.RemoveAt(t);
									tileWorldCreatorAsset.mapBuildLayers.Insert(t + 1, _a);
								}
							}
				
							// DUPLICATE
							if (GUILayout.Button(new GUIContent(iconDuplicate, "Duplicate layer"), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
							{
								var _clone = (TWCBuildLayer)tileWorldCreatorAsset.mapBuildLayers[t].Clone();
								
								_clone.layerName = tileWorldCreatorAsset.mapBuildLayers[t].layerName + "_" + (tileWorldCreatorAsset.mapBuildLayers.Count - 1);
								
								tileWorldCreatorAsset.mapBuildLayers.Add(_clone as TWCBuildLayer);
							}

							
							// EXECUTE
							if (GUILayout.Button(new GUIContent(iconExecute, "Execute this layer, hold Left-CTRL key to force complete rebuild of the map."), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
							{
								//tileWorldCreator.ExecuteBuildLayer(tileWorldCreatorAsset.mapBuildLayers[t].layerName, leftControl);
								tileWorldCreator.ExecuteBuildLayer(tileWorldCreatorAsset.mapBuildLayers[t].guid, leftControl);
							}
			
							if (GUILayout.Button(new GUIContent(iconDestroy, "Destroy all instantiated objects, generated by this layer."), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
							{
								// Remove all instantiated objects
								tileWorldCreator.DestroyLayer(tileWorldCreatorAsset.mapBuildLayers[t].guid);
							}
			
							// REMOVE
							if (GUILayout.Button(new GUIContent(iconRemove, "Remove this layer, hold Left-CTRL key to skip dialog."), GUILayout.Width(layerHeight), GUILayout.Height(layerHeight)))
							{
								if (leftControl)
								{
									// Remove all instantiated objects
									tileWorldCreator.DestroyLayer(tileWorldCreatorAsset.mapBuildLayers[t].guid);
									// Remove layer
									tileWorldCreatorAsset.mapBuildLayers.RemoveAt(t);
								}
								else
								{
									if (EditorUtility.DisplayDialog("Remove layer?",
										"Are you sure you want to remove layer: " + tileWorldCreatorAsset.mapBuildLayers[t].layerName, "Yes", "No"))
									{
										if (tileWorldCreator != null)
										{
											// Remove all instantiated objects
											tileWorldCreator.DestroyLayer(tileWorldCreatorAsset.mapBuildLayers[t].guid);
										}
										
										// Remove layer
										tileWorldCreatorAsset.mapBuildLayers.RemoveAt(t);
										
									}
								}
								GUIUtility.ExitGUI();
							}

						}
			    	
						//try
						//{
							if (tileWorldCreatorAsset.mapBuildLayers[t].foldout)
							{
								EditorGUI.indentLevel ++;
								
								tileWorldCreatorAsset.mapBuildLayers[t].DrawGUI(tileWorldCreatorAsset);
							
								EditorGUI.indentLevel --;
							}
						//}
						//catch{}
			    	
					}
			    	
					EditorUtilities.DrawUILine(Color.black, 1, 0);
				}
		    
				if (tileWorldCreator == null)
				{
					GUI.enabled = false;
				}
				else
				{
					GUI.enabled = true;
				}


				using (new GUILayout.VerticalScope())
				{
					GUI.color = Color.white;
					
					if (GUILayout.Button(new GUIContent("Execute all build layers", "Hold Left-CTRL to force a complete rebuild"), GUILayout.Height(30)))
					{
						tileWorldCreator.ExecuteAllBuildLayers(leftControl);
					}
					
				}

		    
				GUI.enabled = true;
			}
		}
		
		
		void DrawHeader(Rect rect)
		{
			GUI.Label(rect, "Stack", "boldLabel");
		}
		
		void AddNewAction(object _data)
		{
			GenericMenuData data = (GenericMenuData) _data;
			
			var _type = System.Type.GetType(data.typeName +", " +  data.assemblyName, true);
			var _typeInstance = System.Activator.CreateInstance(_type);
			//var _typeInstance = System.Activator.CreateInstance(System.Type.GetType(data.typeName));
			
			//Debug.Log("ADD ACTION TO " + data.mapStackIndex);
			
			tileWorldCreatorAsset.mapBlueprintLayers[data.mapStackIndex].stack.Add(new TileWorldCreatorAsset.BlueprintLayerData.ActionStack(data.name, _typeInstance as ITWCAction));
			
		}
		
		void AddNewBuildLayer(object _data)
		{
			GenericMenuData data = (GenericMenuData) _data;
			
			//Debug.Log(data.typeName + " " + data.assemblyName);
			var _type = System.Type.GetType(data.typeName +", " +  data.assemblyName, true);
			var _typeInstance = System.Activator.CreateInstance(_type);
			
			
			tileWorldCreatorAsset.mapBuildLayers.Add(_typeInstance as TWCBuildLayer);
			
			tileWorldCreatorAsset.mapBuildLayers[tileWorldCreatorAsset.mapBuildLayers.Count-1].guid = System.Guid.NewGuid();
		}
		
		public static void GetTypes<T>(out List<AvailableActions> _stackGenerators, out List<AvailableActions> _stackModifiers) //, out List<string> _typeNames)
		{
	
			_stackGenerators = new List<AvailableActions>();
			_stackModifiers = new List<AvailableActions>();
		    
			var types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(T));
			foreach(var type in types)
			{
				var _categoryAttribute = (ActionCategoryAttribute)type.GetCustomAttribute(typeof(ActionCategoryAttribute), false);
				var _nameAttribute = (ActionNameAttribute)type.GetCustomAttribute(typeof(ActionNameAttribute), false);
				
				Assembly _assembly = Assembly.GetAssembly(type);
				var _assemblyName = _assembly.GetName().Name;
				
				//if (_attribute.Length > 0)
				//{
					//var _categoryAttribute = (ColorAttribute)Attribute.GetCustomAttribute(myType, typeof(ColorAttribute));
					//var _targetAttribute = _attribute.First() as TWCActionAttributes;  
					
					
				var _actionName = type.FullName.ToString();
				if (_nameAttribute != null)
				{
					_actionName = _nameAttribute.Name;
				}
					
					
					
				if (_categoryAttribute.Category == ActionCategoryAttribute.CategoryTypes.Generators)
				{
						
					_stackGenerators.Add(new AvailableActions(_actionName, type.FullName.ToString(), _assemblyName)); //type.FullName.ToString());
				}
				
				if (_categoryAttribute.Category == ActionCategoryAttribute.CategoryTypes.Modifiers)
				{
					_stackModifiers.Add(new AvailableActions(_actionName, type.FullName.ToString(), _assemblyName));
				}
				//}
				
				
				
			}
		}
	    
		public static void GetTypesBuildLayer(out List<AvailableActions> _stackGenerators) //, out List<string> _typeNames)
		{
	
			_stackGenerators = new List<AvailableActions>();
		 
			var types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(TWCBuildLayer));
			foreach(var type in types)
			{
				var _nameAttribute = (ActionNameAttribute)type.GetCustomAttribute(typeof(ActionNameAttribute), false);
		
				Assembly _assembly = Assembly.GetAssembly(type);
				var _assemblyName = _assembly.GetName().Name;
				
				var _actionName = type.FullName.ToString();
				
				if (_nameAttribute != null)
				{
					_actionName = _nameAttribute.Name;
				}
				
				_stackGenerators.Add(new AvailableActions(_actionName, type.FullName.ToString(), _assemblyName));


			}
		}
	}
}