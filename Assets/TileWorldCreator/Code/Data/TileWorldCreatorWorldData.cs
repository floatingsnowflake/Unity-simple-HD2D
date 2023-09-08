using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Unity.Collections;

using TWC.OdinSerializer;

namespace TWC
{



	/// <summary>
	/// Store all relevant tile data
	/// </summary>
	[System.Serializable]
	public class TileData
	{
		public GameObject tile;
		
		public enum TileType
		{
			edge,
			exteriorCorner,
			interiorCorner,
			fill
		}
		
		public TileType tileType;
		public NeighboursLocation neighboursLocation;
		public Vector3 position;
		public Quaternion rotation;
		public float yRotation;
		

		public TileData(){}
		
		public TileData (GameObject _tile)
		{
			tile = _tile;
		}
	}
	
	/// <summary>
	/// Store the location of the neighbours
	/// </summary>
	public class NeighboursLocation
	{
		public bool north;
		public bool west;
		public bool east;
		public bool south;
		public bool northWest;
		public bool northEast;
		public bool southWest;
		public bool southEast;
	}
	
	[System.Serializable]
	public class WorldMap
	{
	
		/// <summary>
		/// The generated final map output partitioned into clusters.
		/// int: HashMap id for each cluster
		/// Vector2Int: Grid position of the tile
		/// TileData: Tile data
		/// </summary>
		public Dictionary<int, Dictionary<Vector2Int, TileData>> clusters = new Dictionary<int, Dictionary<Vector2Int, TileData>>();
	
		
		private int clusterCellSize = 20;
		public int ClusterCellSize
		{
			get
			{
				return clusterCellSize;
			}
			set
			{
				clusterCellSize = value;
			}
		}
		
		public int clusterYMultiplier = 1000;	
		public bool mapSizeChanged;
		
		/// <summary>
		/// Generate a unique hashmap key based on the tile position and cluster size
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public int GetPositionHashMapKey(Vector3 position)
		{
			return (int) (Mathf.Floor(position.x / clusterCellSize) + (clusterYMultiplier * Mathf.Floor(position.z / clusterCellSize)));		
		}
		
		public Vector2Int GetQuadrantPosition(Vector2Int mapPosition)
		{
			return new Vector2Int((int)Mathf.Floor(mapPosition.x / clusterCellSize),  (int)Mathf.Floor(mapPosition.y / clusterCellSize));
		}
			
		public WorldMap(WorldMap _oldMap, int _width, int _height)
		{
			// Calculate the cluster size based on map width and height
			var _cellSize = (int)(Mathf.Sqrt(_width * _height) * 0.2f);
			clusterCellSize = _cellSize;
		
			if (_oldMap != null)
			{
				if (clusterCellSize != _oldMap.clusterCellSize)
				{
					mapSizeChanged = true;
				}
				else
				{
					mapSizeChanged = false;
				}
			}
			else
			{
				mapSizeChanged = true;
			}
			
			clusters = new Dictionary<int, Dictionary<Vector2Int, TileData>>();
		}
			
		public WorldMap(WorldMap _oldMap, int _width, int _height, int _cellSize)
		{
			clusterCellSize = _cellSize;
		
			if (_oldMap != null)
			{
				if (clusterCellSize != _oldMap.clusterCellSize)
				{
					mapSizeChanged = true;
				}
				else
				{
					mapSizeChanged = false;
				}
			}
			else
			{
				mapSizeChanged = true;
			}
			
			clusters = new Dictionary<int, Dictionary<Vector2Int, TileData>>();
		}
	
	
		public void AddTile(Vector3 _position, TileData _tileData)
		{	
			int hashMapKey = GetPositionHashMapKey(new Vector3(_position.x, _position.y, _position.z));
	
			if (clusters.ContainsKey(hashMapKey))
			{
	
				var _posKey = new Vector2Int((int)_position.x, (int)_position.z);
				
				if (clusters[hashMapKey].ContainsKey(_posKey))
				{
					clusters[hashMapKey][_posKey] = _tileData;
				}
				else
				{
					clusters[hashMapKey].Add(_posKey, _tileData);
				}
			}
			else
			{
				var _gridObject = new Dictionary<Vector2Int, TileData>();
				_gridObject.Add(new Vector2Int((int)_position.x, (int)_position.z), _tileData);
			
				clusters.Add(hashMapKey, _gridObject);
			}
	
		}
	
			
		/*
		// All neighbouring quadrants
		hashMapKey;
		hashMapKey + 1
		hashMapKey - 1
		hashMapKey + quadrantYMultiplier
		hashMapKey - quadrantYMultiplier
		hashMapKey + 1 + quadrantYMultiplier
		hashMapKey - 1 + quadrantYMultiplier
		hashMapKey + 1 - quadrantYMultiplier
		hashMapKey - 1 - quadrantYMultiplier
		*/	
	}

}
