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
	[ActionNameAttribute(Name="Random Noise")]
	public class RandomNoise : TWCBlueprintAction, ITWCAction
	{
		
		public float weight;
		private TWCGUILayout guiLayout;
		
		public ITWCAction Clone()
		{
			var _r = new RandomNoise();
			
			_r.weight = this.weight;
			
			return _r;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			// Make sure to set the seed from TileWorldCreator
			UnityEngine.Random.InitState(_twc.currentSeed);
			
			
			bool[,] randomMap = new bool[map.GetLength(0), map.GetLength(1)];
			
			for (int x = 0; x < randomMap.GetLength(0); x ++)
			{
				for (int y = 0; y < randomMap.GetLength(1); y ++)
				{
					float sample = Mathf.PerlinNoise((float)x / (float)randomMap.GetLength(0) + Random.Range(0, 100), (float)y / (float)randomMap.GetLength(1) + Random.Range(0, 100));
					
					if (sample > weight)
					{
						randomMap[x,y] = true;
					}
				}
			}
			
			return TileWorldCreatorUtilities.MergeMap(map, randomMap);
		}
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				weight = EditorGUI.Slider(guiLayout.rect, "Weight:", weight, 0f, 1f);
			}
		}
		#endif
		
		
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
		
	  
	}
}