using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TWC;
using TWC.Utilities;

namespace TWC.editor
{
	
	[CustomEditor(typeof(TileWorldCreator4TilesPreset))]
	public class TileWorldCreatorTilesPresetEditor : Editor
	{
		
		private TileWorldCreator4TilesPreset preset;
		
		private Texture2D iconEdgeTile;
		private Texture2D iconExteriorCornerTile;
		private Texture2D iconInteriorCornerTile;
		private Texture2D iconFillTile;
		
		public void OnEnable()
		{
			if (target == null) return;
			preset = (TileWorldCreator4TilesPreset)target;
		}
		
		public override void OnInspectorGUI()
		{
			if (iconEdgeTile == null)
			{
				LoadResources();
			}
			using (var check = new EditorGUI.ChangeCheckScope())
			{
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconEdgeTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Edge:");
							preset.edgeTile = (GameObject)EditorGUILayout.ObjectField("", preset.edgeTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.edgeTileRotationOffset = EditorGUILayout.Vector3Field("", preset.edgeTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.edgeTileScalingOffset = EditorGUILayout.Vector3Field("", preset.edgeTileScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconExteriorCornerTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Exterior Corner:");
							preset.exteriorCornerTile = (GameObject)EditorGUILayout.ObjectField("", preset.exteriorCornerTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.exteriorCornerTileRotationOffset = EditorGUILayout.Vector3Field("", preset.exteriorCornerTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.exteriorCornerScalingOffset = EditorGUILayout.Vector3Field("", preset.exteriorCornerScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconInteriorCornerTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Interior Corner");
							preset.interiorCornerTile = (GameObject)EditorGUILayout.ObjectField("", preset.interiorCornerTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.interiorCornerTileRotationOffset = EditorGUILayout.Vector3Field("", preset.interiorCornerTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.interiorCornerScalingOffset = EditorGUILayout.Vector3Field("", preset.interiorCornerScalingOffset);
						}
					}
				}
						
				using (new GUILayout.HorizontalScope("Box"))
				{
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(new GUIContent(iconFillTile, ""), GUILayout.Width(40), GUILayout.Height(40));
					}
							
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Fill:");
							preset.fillTile = (GameObject)EditorGUILayout.ObjectField("", preset.fillTile, typeof(GameObject), true);
						}
								
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Rotation offset:");
							preset.fillTileRotationOffset = EditorGUILayout.Vector3Field("", preset.fillTileRotationOffset);
						}
						
						using (new GUILayout.HorizontalScope())
						{
							GUILayout.Label("Scaling offset:");
							preset.fillTileScalingOffset = EditorGUILayout.Vector3Field("", preset.fillTileScalingOffset);
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
			iconEdgeTile = EditorUtilities.LoadIcon("edgeTile.png");
			iconExteriorCornerTile = EditorUtilities.LoadIcon("exteriorCornerTile.png");
			iconInteriorCornerTile = EditorUtilities.LoadIcon("interiorCornerTile.png");
			iconFillTile = EditorUtilities.LoadIcon("fillTile.png");
		}
	}

}


