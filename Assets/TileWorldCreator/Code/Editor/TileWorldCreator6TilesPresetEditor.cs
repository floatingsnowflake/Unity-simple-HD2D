using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TWC;
using TWC.Utilities;

namespace TWC.editor
{
	
	[CustomEditor(typeof(TileWorldCreator6TilesPreset))]
	public class TileWorldCreator6TilesPresetEditor : Editor
	{
		
		private TileWorldCreator6TilesPreset preset;
		
		private Texture2D iconSingleTile;
		private Texture2D iconStraightTile;
		private Texture2D iconDeadEndTile;
		private Texture2D iconCornerTile;
		private Texture2D iconThreeWayTile;
		private Texture2D iconFourWayTile;
		
		public void OnEnable()
		{
			if (target == null) return;
			preset = (TileWorldCreator6TilesPreset)target;
		}
		
		public override void OnInspectorGUI()
		{
			if (iconSingleTile == null)
			{
				LoadResources();
			}
			using (var check = new EditorGUI.ChangeCheckScope())
			{
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconSingleTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Single:");
							preset.singleTile = (GameObject)EditorGUILayout.ObjectField("", preset.singleTile, typeof(GameObject), true);
						}
								
						//using (new GUILayout.HorizontalScope())
						//{
						//	GUILayout.Label("Rotation offset:");
						//	preset.edgeTileRotationOffset = EditorGUILayout.Vector3Field("", preset.edgeTileRotationOffset);
						//}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.singleTileScalingOffset = EditorGUILayout.Vector3Field("", preset.singleTileScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconDeadEndTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Dead End:");
							preset.deadEndTile = (GameObject)EditorGUILayout.ObjectField("", preset.deadEndTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.deadEndTileRotationOffset = EditorGUILayout.Vector3Field("", preset.deadEndTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.deadEndTileScalingOffset = EditorGUILayout.Vector3Field("", preset.deadEndTileScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconStraightTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Straight:");
							preset.straightTile = (GameObject)EditorGUILayout.ObjectField("", preset.straightTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.straightTileRotationOffset = EditorGUILayout.Vector3Field("", preset.straightTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.straightTileScalingOffset = EditorGUILayout.Vector3Field("", preset.straightTileScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconCornerTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Corner:");
							preset.cornerTile = (GameObject)EditorGUILayout.ObjectField("", preset.cornerTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.cornerTileRotationOffset = EditorGUILayout.Vector3Field("", preset.cornerTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.cornerTileScalingOffset = EditorGUILayout.Vector3Field("", preset.cornerTileScalingOffset);
						}
					}
				}
			
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconThreeWayTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Three way:");
							preset.threeWayTile = (GameObject)EditorGUILayout.ObjectField("", preset.threeWayTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.threeWayTileRotationOffset = EditorGUILayout.Vector3Field("", preset.threeWayTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.threeWayTileScalingOffset = EditorGUILayout.Vector3Field("", preset.threeWayTileScalingOffset);
						}
					}
				}
			
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconFourWayTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Four way:");
							preset.fourWayTile = (GameObject)EditorGUILayout.ObjectField("", preset.fourWayTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.fourWayTileRotationOffset = EditorGUILayout.Vector3Field("", preset.fourWayTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.fourWayTileScalingOffset = EditorGUILayout.Vector3Field("", preset.fourWayTileScalingOffset);
						}
					}
				}
			
				if (check.changed)
				{
					EditorUtility.SetDirty(preset);
				}
			
			}
			
			
		}
		
		void LoadResources()
		{
			iconSingleTile = EditorUtilities.LoadIcon("singleTile.png");
			iconDeadEndTile = EditorUtilities.LoadIcon("deadEndTile.png");
			iconStraightTile = EditorUtilities.LoadIcon("straightTile.png");
			iconCornerTile = EditorUtilities.LoadIcon("cornerTile.png");
			iconThreeWayTile = EditorUtilities.LoadIcon("threeWayTile.png");
			iconFourWayTile = EditorUtilities.LoadIcon("fourWayTile.png");
		}
	}

}


