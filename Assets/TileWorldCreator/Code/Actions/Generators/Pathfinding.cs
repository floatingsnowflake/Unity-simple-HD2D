using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;
using TWC.Utilities;
using TWC.Actions;
using TWC;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Generators)]
	[ActionNameAttribute(Name="Pathfinding")]
	public class Pathfinding : TWCBlueprintAction, ITWCAction
	{
		public class Node : IHeapItem<Node>
		{
			public bool walkable;
			public Vector2Int position;
			public int gridX;
			public int gridY;
			
			public int gCost;
			public int hCost;
			
			public int fCost
			{
				get { return gCost + hCost;}
			}
			
			public Node parent;
			
			int heapIndex;
			
			public Node(bool _walkable, Vector2Int _position, int _gridX, int _gridY)
			{
				walkable = _walkable;
				position = _position;
				
				gridX = _gridX;
				gridY = _gridY;
			}
			
			
			public int HeapIndex
			{
				get 
				{
					return heapIndex;
				}
				set
				{
					heapIndex = value;
				}
			}
			
			public int CompareTo(Node nodeToCompare)
			{
				int compare = fCost.CompareTo(nodeToCompare.fCost);
				if (compare == 0)
				{
					compare = hCost.CompareTo(nodeToCompare.hCost);
				}
				return -compare;
			}
		}
		
		
		public int MaxSize
		{
			get
			{
				return sizeWidth * sizeHeight;
			}
		}
		
		public Guid navigationLayer;
		public Guid startLayer;
		public Guid targetLayer;
		
		public bool Multipath;
		public bool outputOnlyStartPosition;
		public bool outputOnlyEndPosition;
		
		TWCGUILayout guiLayout;
		
		Node[,] grid;
		
		int sizeWidth;
		int sizeHeight;
		
		public enum PathfindingOption
		{
			RandomStartRandomTarget,
			RandomStartMultipleTargets,
			MultipleStartsRandomTarget
		}
		
		public PathfindingOption pathfindingOption;
		
		public class GenericMenuData
		{
			public int selectedIndex;
			public TileWorldCreator twc;
		}
		
		public ITWCAction Clone()
		{
			var _r = new Pathfinding();
				
			_r.navigationLayer = this.navigationLayer;
			_r.startLayer = this.startLayer;
			_r.targetLayer = this.targetLayer;
				
			return _r;
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				var _names =  EditorUtilities.GetAllGenerationLayerNames(_asset);
				var _navigationLayerName = "";
				var _navigationLayerData = _asset.GetBlueprintLayerData(navigationLayer);
				if (_navigationLayerData != null)
				{
					_navigationLayerName = _navigationLayerData.layerName;
				}
				
				var _startLayerName = "";
				var _startLayerData = _asset.GetBlueprintLayerData(startLayer);
				if (_startLayerData != null)
				{
					_startLayerName = _startLayerData.layerName;
				}
				
				var _endLayerName = "";
				var _endLayerData = _asset.GetBlueprintLayerData(targetLayer);
				if (_endLayerData != null)
				{
					_endLayerName = _endLayerData.layerName;
				}
				
				
				guiLayout.Add();
				pathfindingOption = (PathfindingOption)EditorGUI.EnumPopup(guiLayout.rect, "Pathfinding option", pathfindingOption);
				
				if (pathfindingOption == PathfindingOption.RandomStartRandomTarget)
				{
					guiLayout.Add();
					outputOnlyStartPosition = EditorGUI.Toggle(guiLayout.rect, "output only start position", outputOnlyStartPosition);
					guiLayout.Add();
					outputOnlyEndPosition = EditorGUI.Toggle(guiLayout.rect, "output only end position", outputOnlyEndPosition);
				}
				else if (pathfindingOption == PathfindingOption.RandomStartMultipleTargets)
				{
					guiLayout.Add();
					outputOnlyStartPosition = EditorGUI.Toggle(guiLayout.rect, "output only start position", outputOnlyStartPosition);
				}
				else if (pathfindingOption == PathfindingOption.MultipleStartsRandomTarget)
				{
					guiLayout.Add();
					outputOnlyEndPosition = EditorGUI.Toggle(guiLayout.rect, "output only end position", outputOnlyEndPosition);
				}
				
				guiLayout.Add();
				EditorGUI.LabelField(guiLayout.rect, "Navigation layer");
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_navigationLayerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
						
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
							
						if (_twc != null)
						{
							_data.twc = _twc;
						}
							
						menu.AddItem(new GUIContent(_names[n]), false, AssignNavigationLayer, _data);
					}
						
					menu.ShowAsContext();
				}
				
				guiLayout.Add();
				EditorGUI.LabelField(guiLayout.rect,"Start layer");
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_startLayerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
						
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
							
						if (_twc != null)
						{
							_data.twc = _twc;
						}
							
						menu.AddItem(new GUIContent(_names[n]), false, AssignStartLayer, _data);
					}
						
					menu.ShowAsContext();
				}
				
				guiLayout.Add();
				EditorGUI.LabelField(guiLayout.rect, "Target layer");
				guiLayout.Add();
				if (EditorGUI.DropdownButton(guiLayout.rect, new GUIContent(_endLayerName), FocusType.Keyboard))
				{
					GenericMenu menu = new GenericMenu();
						
					for (int n = 0; n < _names.Length; n ++)
					{
						var _data = new GenericMenuData();
						_data.selectedIndex = n;
							
						if (_twc != null)
						{
							_data.twc = _twc;
						}
							
						menu.AddItem(new GUIContent(_names[n]), false, AssignTargetLayer, _data);
					}
						
					menu.ShowAsContext();
				}
			}
		}
		#endif
		
		void AssignNavigationLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			navigationLayer = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}
		
		void AssignStartLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			startLayer = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
		}
		
		void AssignTargetLayer(object _data)
		{
			var _d = _data as GenericMenuData;
			targetLayer = _d.twc.twcAsset.mapBlueprintLayers[_d.selectedIndex].guid;
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
				
		
		public bool[,] Execute(bool[,] _map, TileWorldCreator _twc)
		{
			// Make sure to set the seed from TileWorldCreator
			UnityEngine.Random.InitState(_twc.currentSeed);
			
			
			CreatePathfindingGrid(_twc);
			
			var _startLayer = _twc.GetMapOutputFromBlueprintLayer(startLayer);
			var _targetLayer = _twc.GetMapOutputFromBlueprintLayer(targetLayer);
			
			var _startPositions = new List<Vector2Int>();	
			var _endPositions = new List<Vector2Int>();
			
			for (int x = 0; x < _startLayer.GetLength(0); x ++)
			{
				for (int y = 0; y < _startLayer.GetLength(1); y ++)
				{
					if (_startLayer[x,y])
					{
						_startPositions.Add(new Vector2Int(x,y));
					}
				}
			}
			
			for (int x = 0; x < _targetLayer.GetLength(0); x ++)
			{
				for (int y = 0; y < _targetLayer.GetLength(1); y ++)
				{
					if (_targetLayer[x,y])
					{
						_endPositions.Add(new Vector2Int(x,y));
					}
				}
			}
	
			if (_endPositions.Count > 0 && _startPositions.Count > 0)
			{
				
				bool[,] _pathMap = new bool[sizeWidth, sizeHeight];
				
				switch(pathfindingOption)
				{
					case PathfindingOption.RandomStartRandomTarget:
						
						var _startPos1 = _startPositions[UnityEngine.Random.Range(0, _startPositions.Count)];
						var _endPos1 = _endPositions[UnityEngine.Random.Range(0, _endPositions.Count)];
						
						_pathMap = FindPath(_startPos1, _endPos1);
						
						if (_pathMap != null)
						{
							_pathMap[_startPos1.x, _startPos1.y] = true;	
							_pathMap[_endPos1.x, _endPos1.y] = true;	
							
							if (outputOnlyStartPosition || outputOnlyEndPosition)
							{
						
								bool[,] _returnMap = new bool[sizeWidth, sizeHeight];
								if (outputOnlyStartPosition)
								{
									_returnMap[_startPos1.x, _startPos1.y] = true;
								}
						
								if (outputOnlyEndPosition)
								{
									_returnMap[_endPos1.x, _endPos1.y] = true;
								}
						
								return _returnMap;
							}
						}
						
						break;
						
					case PathfindingOption.RandomStartMultipleTargets:
						
						var _startPos2 = _startPositions[UnityEngine.Random.Range(0, _startPositions.Count)];
						var _pathCount1 = _endPositions.Count;
						
						for (int i = 0; i < _pathCount1; i ++)
						{
							var _endPos2 = _endPositions[i];
							
							var _tmpPath = FindPath(_startPos2, _endPos2);
							if (_tmpPath != null)
							{
								for (int x = 0; x < sizeWidth; x ++)
								{
									for (int y = 0; y < sizeHeight; y ++)
									{
										if (_tmpPath[x,y])
										{
											_pathMap[x,y] = true;
										}
									}
								}
							}
						
						}
						
						_pathMap[_startPos2.x, _startPos2.y] = true;	
						
						
						if (outputOnlyStartPosition)
						{
					
							bool[,] _returnMap = new bool[sizeWidth, sizeHeight];
							if (outputOnlyStartPosition)
							{
								_returnMap[_startPos2.x, _startPos2.y] = true;
							}
					
							return _returnMap;
						}
					
						break;
						
					case PathfindingOption.MultipleStartsRandomTarget:
						
						var _endPos3 = _endPositions[UnityEngine.Random.Range(0, _endPositions.Count)];
						var _pathCount2 = _startPositions.Count;
						
						for (int i = 0; i < _pathCount2; i ++)
						{
							var _startPos3 = _startPositions[i];
							
							var _tmpPath = FindPath(_startPos3, _endPos3);
							if (_tmpPath != null)
							{
								for (int x = 0; x < sizeWidth; x ++)
								{
									for (int y = 0; y < sizeHeight; y ++)
									{
										if (_tmpPath[x,y])
										{
											_pathMap[x,y] = true;
										}
									}
								}
							}
						
						}
						
						_pathMap[_endPos3.x, _endPos3.y] = true;	
						
						
						if (outputOnlyEndPosition)
						{
					
							bool[,] _returnMap = new bool[sizeWidth, sizeHeight];
							
					
							if (outputOnlyEndPosition)
							{
								_returnMap[_endPos3.x, _endPos3.y] = true;
							}
					
							return _returnMap;
						}
						
						break;
				}
				
				
			
				
			
				return _pathMap;
			}
			else
			{
				return _map;
			}
		}
		
		void CreatePathfindingGrid(TileWorldCreator _twc)
		{
			var _navigationMap = _twc.GetMapOutputFromBlueprintLayer(navigationLayer);
			grid = new Node[_navigationMap.GetLength(0), _navigationMap.GetLength(1)];
			
			sizeWidth = _navigationMap.GetLength(0);
			sizeHeight = _navigationMap.GetLength(1);
			
			for (int x = 0; x < _navigationMap.GetLength(0); x ++)
			{
				for (int y = 0; y < _navigationMap.GetLength(1); y ++)
				{
					grid[x,y] = new Node(_navigationMap[x,y], new Vector2Int(x,y), x, y);
				}
			}
		}
		
		bool[,] FindPath(Vector2Int _startPosition, Vector2Int _targetPosition)
		{
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			
			Node startNode = grid[_startPosition.x, _startPosition.y];
			Node targetNode = grid[_targetPosition.x, _targetPosition.y];
			
			Heap<Node> openSet = new Heap<Node>(MaxSize);
			HashSet<Node> closeSet = new HashSet<Node>();
			
			
			openSet.Add(startNode);
	
			while(openSet.Count > 0)
			{
				Node currentNode = openSet.RemoveFirst();
	
				closeSet.Add(currentNode);
				
				// We have found the target
				if (currentNode == targetNode)
				{
				
					//sw.Stop();
					
					//UnityEngine.Debug.Log("path found " + sw.ElapsedMilliseconds + " ms");
					return RetracePath(startNode, targetNode);
				}
				
				foreach (Node neighbour in GetNeighbours(currentNode))
				{
					if (!neighbour.walkable || closeSet.Contains(neighbour))
					{
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
						{
							openSet.Add(neighbour);
						}
						else
						{
							openSet.UpdateItem(neighbour);
						}
					}
				}
			}
			
			return null;
		}
		
		bool[,] RetracePath(Node _startNode, Node _endNode)
		{
			bool[,] path = new bool[sizeWidth, sizeHeight];
			
			Node currentNode = _endNode;
			
			while (currentNode != _startNode)
			{
				path[currentNode.position.x, currentNode.position.y] = true;
				currentNode = currentNode.parent;
			}
			
			path[_endNode.position.x, _endNode.position.y] = true;
	
			
			return path;
		}
		
		public List<Node> GetNeighbours(Node node)
		{
			List<Node> neighbours = new List<Node>();
			
			for (int x = -1; x < 2; x ++)
			{
				for (int y = -1; y < 2; y ++)
				{
					if (x == 0 && y == 0)
						continue;
					if (x == -1 && y == 1)
						continue;
					if (x == 1 && y == 1)
						continue;
					if (x == -1 && y == -1)
						continue;
					if (x == 1 && y == -1)
						continue;
						
					int checkX = node.gridX + x;
					int checkY = node.gridY + y;
					
					if (checkX >= 0 && checkX < sizeWidth && checkY >= 0 && checkY < sizeHeight)
					{
						neighbours.Add(grid[checkX, checkY]);
					}
				}
			}
			
			return neighbours;
		}
		
		int GetDistance(Node nodeA, Node nodeB)
		{
			int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
			int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
			
			if (dstX > dstY)
			{
				return 14 * dstY + 10 * (dstX - dstY);
			}
			
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
	
	
	
	#region heap
	public class Heap<T> where T : IHeapItem<T>
	{
		T[] items;
		int currentItemCount;
		
		public Heap(int maxHeapSize)
		{
			items = new T[maxHeapSize];
		}
		
		public void Add(T item)
		{
			item.HeapIndex = currentItemCount;
			items[currentItemCount] = item;
			SortUp(item);
			currentItemCount ++;
		}
		
		public T RemoveFirst()
		{
			T firstItem = items[0];
			currentItemCount--;
			
			items[0] = items[currentItemCount];
			items[0].HeapIndex = 0;
			
			SortDown(items[0]);
			return firstItem;
		}
		
		public void UpdateItem(T item)
		{
			SortUp(item);
		}
		
		public int Count
		{
			get
			{
				return currentItemCount;
			}
		}
		
		public bool Contains(T item)
		{
			return Equals(items[item.HeapIndex], item);
		}
		
		void SortDown(T item)
		{
			while(true)
			{
				int childIndexLeft = item.HeapIndex * 2 + 1;
				int childIndexRight = item.HeapIndex * 2 + 2;
				int swapIndex = 0;
				
				if (childIndexLeft < currentItemCount)
				{
					swapIndex = childIndexLeft;
					
					if (childIndexRight < currentItemCount)
					{
						if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
						{
							swapIndex = childIndexRight;
						}
					}
					
					if (item.CompareTo(items[swapIndex]) < 0)
					{
						Swap(item, items[swapIndex]);
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}
			}
		}
		
		void SortUp(T item)
		{
			int parentIndex = (item.HeapIndex - 1) / 2;
			
			while(true)
			{
				T parentItem = items[parentIndex];
				if (item.CompareTo(parentItem) > 0)
				{
					Swap (item, parentItem);
				}
				else
				{
					break;
				}
				
				parentIndex = (item.HeapIndex - 1) / 2;
			}
		}
		
		void Swap(T itemA, T itemB)
		{
			items[itemA.HeapIndex] = itemB;
			items[itemB.HeapIndex] = itemA;
			
			int itemAIndex = itemA.HeapIndex;
			
			itemA.HeapIndex = itemB.HeapIndex;
			itemB.HeapIndex = itemAIndex;
		}
	}
	
	public interface IHeapItem<T> : IComparable<T>
	{
		int HeapIndex
		{
			get;
			set;
		}
	}
	
	#endregion heap
}
