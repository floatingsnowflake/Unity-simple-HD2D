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
	[ActionNameAttribute(Name="Texture")]
	public class Texture : TWCBlueprintAction, ITWCAction
	{
		[SerializeField]
		public Texture2D originalTexture;
		[SerializeField]
		public Texture2D modifiedTexture;
		
		#if UNITY_EDITOR
		[SerializeField]
		private float rangeMin = 0f;
		[SerializeField]
		private float rangeMax = 1f;
		#endif
		[SerializeField]
		private float grayscaleRangeMin = 0.1f;
		[SerializeField]
		private float grayscaleRangeMax = 1f;
		
		private TWCGUILayout guiLayout;
	
		public ITWCAction Clone()
		{
			var _r = new Texture();
			
			_r.originalTexture = this.originalTexture;
			
			return _r; 
		}
		
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
			if (originalTexture != null)
			{
				
				modifiedTexture = new Texture2D(map.GetLength(0), map.GetLength(1));
				
				// Resize texture to map size
				if (originalTexture.width != map.GetLength(0) || originalTexture.height != map.GetLength(1))
				{
					modifiedTexture = Resize(originalTexture, map.GetLength(0), map.GetLength(1));
				}
				else
				{
					modifiedTexture = originalTexture;
				}
				
				Color[] _pixels = modifiedTexture.GetPixels();
				
				for (int x = 0; x < map.GetLength(0); x ++)
				{
					for (int y = 0; y < map.GetLength(1); y ++)
					{
						try
						{
							var _pixel = _pixels[(y * map.GetLength(0) + x)].grayscale;
						
							if (_pixel >= grayscaleRangeMin && _pixel <= grayscaleRangeMax)
							{
								map[x,y] = true;
							}

						}
						catch{}
					}
				}
			}
			
			return map;
		}
		
		Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
		{
			RenderTexture rt=new RenderTexture(targetX, targetY,24);
			RenderTexture.active = rt;
			Graphics.Blit(texture2D,rt);
			Texture2D result=new Texture2D(targetX,targetY);
			result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
			result.Apply();
			return result;
		}
		
		// Public method to set a new texture via script
		public void SetTexture(Texture2D _texture)
		{
			originalTexture = _texture;
		}
		
		public void SetGrayscaleRange(float min, float max)
		{
			grayscaleRangeMin = min;
			grayscaleRangeMax = max;
		}
		
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add(100);
				
				var _xOffset = 20;
				
				if (originalTexture != null)
				{
					EditorGUI.DrawPreviewTexture(new Rect(guiLayout.rect.x, guiLayout.rect.y - 80, 100, guiLayout.rect.height), originalTexture);
					_xOffset = 110;
				}
				
				originalTexture = (Texture2D)EditorGUI.ObjectField(new Rect(guiLayout.rect.x + _xOffset, guiLayout.rect.y - 80, guiLayout.rect.width - 110, EditorGUIUtility.singleLineHeight), "Texture", originalTexture, typeof(Texture2D), false);
				
				if (originalTexture != null)
				{
					if (originalTexture.width > _asset.mapWidth || originalTexture.height > _asset.mapHeight)
					{
						guiLayout.Add();
						EditorGUI.HelpBox(guiLayout.rect, "Warning, texture size does not match map size. Texture will be resized.", MessageType.Warning);
					}			
				}

				EditorGUI.LabelField(new Rect(guiLayout.rect.x + _xOffset, guiLayout.rect.y - 60, guiLayout.rect.width- 110, EditorGUIUtility.singleLineHeight), "Grayscale range - Min: " + grayscaleRangeMin.ToString() + " Max: " + grayscaleRangeMax.ToString());
				EditorGUI.MinMaxSlider(new Rect(guiLayout.rect.x + _xOffset, guiLayout.rect.y - 40, guiLayout.rect.width - 110 , EditorGUIUtility.singleLineHeight), ref grayscaleRangeMin, ref grayscaleRangeMax, rangeMin, rangeMax);
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