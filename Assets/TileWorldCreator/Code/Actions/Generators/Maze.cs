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
	[ActionNameAttribute(Name="Maze")]
	public class Maze : TWCBlueprintAction, ITWCAction
	{
		private int height, width;
		
		public bool onlyOutputPlayerStartPos;
		public bool onlyOutputPlayerEndPos;
		
		
		public Vector2Int startPosition;
		public Vector2Int endPosition;
	
		bool[,] mazeMap;
		float lastDistance = 0;
		float dist = 0;
	
		TWCGUILayout guiLayout;
		
		public ITWCAction Clone()
		{
			var _r = new Maze();
			
			_r.onlyOutputPlayerStartPos = this.onlyOutputPlayerStartPos;
			_r.onlyOutputPlayerEndPos = this.onlyOutputPlayerEndPos;
			
			return _r;
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				onlyOutputPlayerStartPos = EditorGUI.Toggle (guiLayout.rect, "only start position", onlyOutputPlayerStartPos);
				
				guiLayout.Add();
				onlyOutputPlayerEndPos = EditorGUI.Toggle (guiLayout.rect, "only end position", onlyOutputPlayerEndPos);
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
			
		
		public bool[,] Execute(bool[,] _map, TileWorldCreator _twc)
		{
			
			// Make sure to set the seed from TileWorldCreator
			UnityEngine.Random.InitState(_twc.currentSeed);
			
			
			// Assign properties from config
			width = _map.GetLength(0);
			height = _map.GetLength(1);
	
			mazeMap = new bool[width, height];
			mazeMap = GenerateMaze(height, width);
	
	
			// Find end position after generation
			var _pos = FindEndPosition(mazeMap);
			endPosition = new Vector2Int(_pos.x, _pos.y);    
	
			if (onlyOutputPlayerStartPos || onlyOutputPlayerEndPos)
			{
				mazeMap = new bool[width, height];	
			}
			
			if (onlyOutputPlayerStartPos)
			{
				mazeMap[startPosition.x, startPosition.y] = true;
			}
	
			if (onlyOutputPlayerEndPos)
			{
				mazeMap[endPosition.x, endPosition.y] = true;
			}
			
			
			// Invert maze map
			for (int x = 0; x < mazeMap.GetLength(0); x ++)
			{
				for (int y = 0; y < mazeMap.GetLength(1); y ++)
				{
					mazeMap[x,y] = !mazeMap[x,y];
				}
			}
			
	
			return TileWorldCreatorUtilities.MergeMap(_map, mazeMap);
	
		}
	
	
		bool[,] GenerateMaze(int _height, int _width)
		{
			// create temp maze
			bool[,] _tmpMaze = new bool[_width, _height];
	
			// Fill all cells
			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					_tmpMaze[x, y] = false;
				}
			}
	
	      
			int _x = 1;
			int _y = 0;
	 
			
			_x = Random.Range(0, width - 1);
			_y = Random.Range(0, height - 1);
			
	
			startPosition = new Vector2Int(_x, _y);
	
			// Clear start cell
			_tmpMaze[_x, _y] = true;
	
			//carve
			// if start position is at border of the map
			// move two cells to the inside
			//if (_x == 0)
			//{
			//	for (int i = 0; i < 2; i++)
			//	{
			//		_tmpMaze[_x + i, _y] = true;
			//	}
	
			//	_x += 1;
			//}
			//else if (_y == 0)
			//{
			//	for (int i = 0; i < 2; i++)
			//	{
			//		_tmpMaze[_x, _y + i] = true;
			//	}
	
			//	_y += 1;
			//}
			//else if (_x == width - 1)
			//{
			//	for (int i = 0; i < 2; i++)
			//	{
			//		_tmpMaze[_x - i, _y] = true;
			//	}
	
			//	_x -= 1;
			//}
			//else if (_y == height - 1)
			//{
			//	for (int i = 0; i < 2; i++)
			//	{
			//		_tmpMaze[_x, _y - i] = true;
			//	}
	
			//	_y -= 1;
			//}
	
			//create maze using DFS (Depth First Search)
			DepthFirstSearch(_tmpMaze, _x, _y);
	
	
			//return maze
			return _tmpMaze;
		}
	
		void DepthFirstSearch(bool[,] _maze, int r, int c)
		{
	       
			//Directions
			// 1 - West
			// 2 - North
			// 3 - East
			// 4 - South
			int[] _directions = new int[] { 1, 2, 3, 4 };
	
			//if (!linear)
			//{
				Shuffle(_directions);
			//}
	
			// Look in a random direction 3 block ahead
			for (int i = 0; i < _directions.Length; i ++)
			{
				switch(_directions[i])
				{
				case 1: // West
					// Check whether 3 cells to the left is out of maze
					if (r - 3 <= 1)
						continue;
	
					if (_maze[r - 3, c ] != true)
					{  
						_maze[r - 3, c] = true;
						_maze[r - 2, c] = true;
						_maze[r - 1, c] = true;
	
						DepthFirstSearch(_maze, r - 3, c);
					}
					break;
				case 2: // North
					// Check whether 3 cells up is out of maze
					if (c + 3 >= height - 1)
						continue;
	
					if (_maze[r, c + 3] != true)
					{
						_maze[r, c + 3] = true;
						_maze[r, c + 2] = true;
						_maze[r, c + 1] = true;
	
						DepthFirstSearch(_maze, r, c + 3);
					}
	
					break;
				case 3: // East
					// Check whether 3 cells to the right is out of maze
					if (r + 3 >= width - 1)
						continue;
	
					if (_maze[r + 3, c] != true)
					{
						_maze[r + 3, c] = true;
						_maze[r + 2, c] = true;
						_maze[r + 1, c] = true;
	
						DepthFirstSearch(_maze, r + 3, c);
					}
					break;
				case 4: // South
					// Check whether 3 cells down is out of maze
					if (c - 3 <= 1)
						continue;
	
					if (_maze[r, c - 3] != true)
					{
						_maze[r, c - 3] = true;
						_maze[r, c - 2] = true;
						_maze[r, c - 1] = true;
	
	
						DepthFirstSearch(_maze, r, c - 3);
					}
					break;
				}
			}
		}
	
		void Shuffle<T>(T[] _array)
		{
			for (int i = _array.Length; i > 1; i --)
			{
				// Pick random element to swap
				int j = Random.Range(0, i);
				// Swap
				T _tmp = _array[j];
				_array[j] = _array[i - 1];
				_array[i - 1] = _tmp;
			}
		}
	
	
		Vector2Int FindEndPosition(bool[,] _mazeMap)
		{
			var _pos = Vector2Int.zero;
	
			lastDistance = 0;
	
			for (int x = 0; x < _mazeMap.GetLength(0); x ++)
			{
				for (int y = 0; y < _mazeMap.GetLength(1); y ++)
				{
					if (_mazeMap[x, y])
					{
						
						// we have found an end position
						// check the distance from this position to start position
						dist = Vector2Int.Distance(startPosition, new Vector2Int(x, y));
						if (dist > lastDistance)
						{
							_pos = new Vector2Int(x, y);
							lastDistance = dist;
						}
						
					}
				}
			}
	
			return _pos;
		}
	}

}