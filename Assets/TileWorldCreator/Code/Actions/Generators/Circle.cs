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
	[ActionNameAttribute(Name="Circle")]
	public class Circle : TWCBlueprintAction, ITWCAction
	{
		public bool randomPosition;
		public int positionX, positionY;
		public int radius;
		private TWCGUILayout guiLayout;
		
		
		public ITWCAction Clone()
		{
			var _r = new Circle();
			
			_r.radius = this.radius;
			
			return _r;
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			
			// Make sure to set the seed from TileWorldCreator
			UnityEngine.Random.InitState(_twc.currentSeed);
			
			var _position = new Vector2Int(positionX, positionY);
			
			if (randomPosition)
			{
				_position = new Vector2Int(Random.Range(0, map.GetLength(0)), Random.Range(0, map.GetLength(1)));
			}
			
			try
			{
				map[_position.x, _position.y] = true;
				for (int x = 0; x < map.GetLength(0); x ++)
				{
					for (int y = 0; y < map.GetLength(1); y ++)
					{
						// Get distance to center
						var _dist = Vector2Int.Distance(new Vector2Int(x, y), _position);
						if (_dist <= radius)
						{ 
							map[x,y] = true;
						}
					}
				}
			}
			catch{}
			
			
			return map;
		}
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				radius = EditorGUI.IntField(guiLayout.rect, "Radius", radius);
				guiLayout.Add();
				randomPosition = EditorGUI.Toggle(guiLayout.rect, "Random position", randomPosition);
				if (!randomPosition)
				{
					guiLayout.Add();
					positionX = EditorGUI.IntField(guiLayout.rect, "Position X:", positionX);
					guiLayout.Add();
					positionY = EditorGUI.IntField(guiLayout.rect, "Position Y:", positionY);
				}
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