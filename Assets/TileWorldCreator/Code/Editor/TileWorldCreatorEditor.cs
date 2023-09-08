using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System.Linq;
using System.Reflection;

using TWC;
using TWC.Utilities;
using TWC.Actions;


namespace TWC.editor
{
	[CustomEditor(typeof(TileWorldCreator))]
	public class TileWorldCreatorEditor : Editor
	{
	    public TileWorldCreator tileWorldCreator;
	
	
		private Editor twcAssetEditor;
		
		private Texture2D logo;
		
		public class GenericMenuData
		{
			public string type;
			public int mapStackIndex;
		}
		
		
		public class GenericOptionMenu
		{
			public string selected;
		}
	
	    public void OnEnable()
		{
			LoadResources();
			if (target == null) return;
			tileWorldCreator = (TileWorldCreator) target;
		}
		
		void LoadResources()
		{
			logo = EditorUtilities.LoadIcon("twcLogo.png");
		}
	    
	    public override void OnInspectorGUI()
		{
	    	
			
			try
			{

				using (new GUILayout.HorizontalScope("Toolbar"))
				{
					GUILayout.FlexibleSpace();
					
					if (GUILayout.Button("...", "toolbarButton"))
					{
						GenericMenu _menu = new GenericMenu();
						
						_menu.AddItem(new GUIContent("Save", ""), false, OptionMenuSelected, "Save");
						_menu.AddItem(new GUIContent("Load", ""), false, OptionMenuSelected, "Load");
						_menu.AddSeparator("");
						_menu.AddItem(new GUIContent("Readme", ""), false, OptionMenuSelected, "Readme");
						_menu.AddItem(new GUIContent("Documentation", ""), false, OptionMenuSelected, "Documentation");
						
						_menu.ShowAsContext();
							
					}
				}
				
				
				GUI.color = new Color (38f/255f,38f/255f,38f/255f,255f/255f);
				using (new GUILayout.HorizontalScope("Box"))
				{
					GUI.color = Color.white;
					using (new GUILayout.VerticalScope())
					{
						GUILayout.Label(logo);
					}
	
				}
				
			}
			catch(System.Exception e)
			{
				if(ExitGUIUtility.ShouldRethrowException(e))
				{
					throw;
				}
				//Debug.LogException(e);
			}
			
			EditorGUI.BeginChangeCheck();
			using (new GUILayout.HorizontalScope("Box"))
			{
				tileWorldCreator.twcAsset = (TileWorldCreatorAsset)EditorGUILayout.ObjectField("TileWorldCreator Asset", tileWorldCreator.twcAsset, typeof(TileWorldCreatorAsset), false);
			}
			
			
			if (tileWorldCreator.twcAsset == null)
			{
				EditorGUILayout.HelpBox("Please create and assign a new TileWorldCreator asset", MessageType.Info);
				return;
			}
			
			
			if (twcAssetEditor == null)
			{
				twcAssetEditor = Editor.CreateEditor(tileWorldCreator.twcAsset);
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				if (twcAssetEditor == null)
				{
					twcAssetEditor = Editor.CreateEditor(tileWorldCreator.twcAsset);
				}
			}
			
			var _ed = twcAssetEditor as TileWorldCreatorAssetEditor;
	
				_ed.DrawGUI(tileWorldCreator);
				
				
			
		}
	    
		void OptionMenuSelected(object _data)
		{
			var _d = _data as string;
			
			switch(_d)
			{
			case "Save":
				string savepath = EditorUtility.SaveFilePanel("Save blueprint stack", Application.dataPath, "blueprint", "json");
				if (!string.IsNullOrEmpty(savepath))
				{
					tileWorldCreator.SaveBlueprintStack(savepath);
				}
				break;
			case "Load":
				string loadpath = EditorUtility.OpenFilePanel("Load blueprint stack", Application.dataPath, "json");
				if (!string.IsNullOrEmpty(loadpath))
				{
					tileWorldCreator.LoadBlueprintStack(loadpath);
				}
				break;
			case "Documentation":
				Application.OpenURL("https://doorfortyfour.github.io/TileWorldCreator");
				break;
			case "Readme":
				var ids = AssetDatabase.FindAssets("Readme t:TWCReadme");
				if (ids.Length == 1)
				{
					var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));
			
					Selection.objects = new UnityEngine.Object[]{readmeObject};
				}
				else
				{
					Debug.Log("Couldn't find a readme");
				}
				break;
			}
		}


		public void OnSceneGUI()
		{
			if (tileWorldCreator.twcAsset == null)
				return;
				

			for (int m = 0; m < tileWorldCreator.twcAsset.mapBlueprintLayers.Count; m ++)
			{
				for (int s = 0; s < tileWorldCreator.twcAsset.mapBlueprintLayers[m].stack.Count; s ++)
				{
					var _action = tileWorldCreator.twcAsset.mapBlueprintLayers[m].stack[s].action  as TWCBlueprintAction;
					if (_action != null)
					{
						_action.DrawSceneGUI(SceneView.lastActiveSceneView.position);
					}
				}
			}
			
			
		}
	
	}
}