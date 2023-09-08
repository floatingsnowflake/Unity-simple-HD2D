using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TWC;

namespace TWC.Utilities
{
	/// <summary>
	/// Creates a custom mesh collider based on the tiles
	/// </summary>
	public static class MeshColliderGenerator
	{
		
		public class BorderData
		{
			public Vector2 vertexPosition;
			public NeighboursLocation location;
			
			public BorderData (Vector2 _pos, NeighboursLocation _location)
			{
				vertexPosition = _pos;
				location = _location;
			}
		}
		
		static List<BorderData> borderData;
		static List<Vector3> newVertices;	
		static List<int> newTriangles;
	
		static  int squareCount;
	
	
		public static Mesh GenerateMeshCollider(TileWorldCreator _twc, string _layerGuid, int _cluster, float _height, float _heightOffset, float _borderOffset)
		{
		
			WorldMap _map = _twc.GetGeneratedBlueprintMap(_layerGuid + "_UNSUBD");
			bool[,] _boolMap = _twc.GetMapOutputFromBlueprintLayer(_layerGuid);
			
			borderData = new List<BorderData>();
			newVertices = new List<Vector3>();	
			newTriangles = new List<int>();
			
			foreach(var _position in _map.clusters[_cluster].Keys)
			{
				var _tileData = _map.clusters[_cluster][_position];
							
				GenSquare((int)_tileData.position.x, (int)_tileData.position.z, _height, _heightOffset, _borderOffset, _twc.twcAsset.cellSize, _tileData.neighboursLocation);
				borderData.Add(new BorderData(new Vector2(_tileData.position.x, _tileData.position.z), _tileData.neighboursLocation));
			}
			
			
			// Generate border 
			GetBorderVertecies();
				
			for (int i = 0; i < borderData.Count; i ++)
			{
				GenBorder(borderData[i].vertexPosition, borderData[i].location, _height, _heightOffset, _borderOffset, _twc.twcAsset.cellSize);	
			}
	
			var _mesh = UpdateMesh();
			return _mesh;
		}
		
	
		
		static void GenSquare( int x, int y, float _height, float _heightOffset, float _borderOffset, float _cellSize, NeighboursLocation _location)
		{
			
			float _xOffset = -.25f;
			float _zOffset = .75f;
			
			var _cSize = _cellSize;
			//_cSize = (_cSize * 0.5f) + 0.5f;
			var _cellOffset = _cSize / 4; //((_cSize * 2f) / 4f);
			
			newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, ((y + _zOffset) * _cSize) + _cellOffset));
			newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, ((y + _zOffset)* _cSize) + _cellOffset));
			newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, ((y - 1 + _zOffset)* _cSize) + _cellOffset));
			newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, ((y - 1 + _zOffset) * _cSize) + _cellOffset));
			Triangle();
			squareCount++;
			
			if (!_location.north)
			{
				var _p = newVertices[newVertices.Count-3];
				_p = new Vector3(_p.x , _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-3] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z + _borderOffset);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			if (!_location.south)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x , _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-2];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z - _borderOffset);
				newVertices[newVertices.Count-2	] = _p2;
			}
			
			
			if (!_location.west)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z );
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x - _borderOffset, _p2.y, _p2.z );
				newVertices[newVertices.Count-4	] = _p2;
			}
			
				
			if (!_location.east)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z );
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x + _borderOffset, _p2.y, _p2.z );
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			
			
			if (_location.north && _location.northEast && _location.east && !_location.southEast && _location.south && _location.southWest && _location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-2] = _p;
			}
			
			if (_location.north && _location.northEast && _location.east && _location.southEast && _location.south && !_location.southWest && _location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-1] = _p;
			}
			
			if (_location.north && !_location.northEast && _location.east && _location.southEast && _location.south && _location.southWest && _location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-3];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-3] = _p;
			}
			
			if (_location.north && _location.northEast && _location.east && _location.southEast && _location.south && _location.southWest && _location.west && !_location.northWest)
			{
				var _p = newVertices[newVertices.Count-4];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-4] = _p;
			}
		}
		
		
		static void GenBorder(Vector2 _pos, NeighboursLocation _location, float _height, float _heightOffset, float _borderOffset, float _cellSize)
		{
			int x = (int)_pos.x;
			int y = (int)_pos.y;
		
			float _xOffset = -.25f;
			float _zOffset = .75f;
			
			var _cSize = _cellSize;
			//_cSize = (_cSize * 0.5f) + 0.5f;
			var _cellOffset = _cSize / 4; // ((_cSize * 2f) / 4f);
			
			// Top side
			if (!_location.north)
			{
				newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset, (_heightOffset + _height) - _height, (((y + _zOffset) * _cSize) + _cellOffset)  + _borderOffset));
				newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, (_heightOffset + _height) - _height, (((y + _zOffset) * _cSize) + _cellOffset) + _borderOffset));
				newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, (((y + _zOffset) * _cSize) + _cellOffset) + _borderOffset));
				newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, (((y + _zOffset) * _cSize) + _cellOffset) + _borderOffset));
				Triangle();
				squareCount++;
			}	
			
			// Add offset to corner (corner: top - left)
			if (!_location.north && !_location.west)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x - _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			// Add offset to corner  (corner: top - right)
			if (!_location.north && !_location.east)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x + _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			// Add offset to interior corner  (corner: bottom - left)
			if (!_location.northEast && !_location.north && _location.northWest && _location.south && _location.southWest && _location.southEast)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x + _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			// Add offset to interior corner  (corner: bottom - right)
			if (_location.northEast && !_location.north && !_location.northWest && _location.south && _location.southWest && _location.southEast)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x - _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			// Bottom side
			if (!_location.south)
			{
				newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset , (_heightOffset + _height) - (_height), (((y - 1 + _zOffset) * _cSize) + _cellOffset) - _borderOffset));
				newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, (_heightOffset + _height) - (_height), (((y - 1 + _zOffset) * _cSize) + _cellOffset) - _borderOffset));
				newVertices.Add( new Vector3 (((x + 1 + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, (((y - 1 + _zOffset) * _cSize) + _cellOffset) - _borderOffset));
				newVertices.Add( new Vector3 (((x + _xOffset) * _cSize) + _cellOffset, _heightOffset + _height, (((y - 1 + _zOffset) * _cSize) + _cellOffset) - _borderOffset));
				Triangle();
				squareCount++;
			}
			
			// Add offset to corner (corner: bottom - left)
			if (!_location.south && !_location.west)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x - _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			
			// Add offset to corner (corner: bottom - right)
			if (!_location.south && !_location.east)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x + _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			// Add offset to interior corner  (corner: top - left)
			if (_location.northEast && _location.north && _location.northWest && !_location.south && _location.southWest && !_location.southEast)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x + _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x + _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			// Add offset to interior corner  (corner: top - right)
			if (_location.northEast && _location.north && _location.northWest && !_location.south && !_location.southWest && _location.southEast)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x - _borderOffset, _p.y, _p.z);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x - _borderOffset, _p2.y, _p2.z);
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			
			
			// Left side
			if (!_location.west)
			{
				newVertices.Add( new Vector3 ((((x + _xOffset) * _cSize) + _cellOffset) - _borderOffset, (_heightOffset + _height) - (_height), ((y -1 + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + _xOffset) * _cSize) + _cellOffset) - _borderOffset, (_heightOffset + _height) - (_height), ((y + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + _xOffset) * _cSize) + _cellOffset) - _borderOffset, _heightOffset + _height, ((y + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + _xOffset) * _cSize) + _cellOffset) - _borderOffset, _heightOffset + _height, ((y -1 + _zOffset) * _cSize) + _cellOffset));
				Triangle();
				squareCount++;
			}
			
			// Add offset to corner (corner: top - left)
			if (!_location.north && !_location.west)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x, _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z + _borderOffset);
				newVertices[newVertices.Count-3] = _p2;
			}
			
			// Add offset to corner (corner: bottom - left)
			if (!_location.south && !_location.west)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x, _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z - _borderOffset);
				newVertices[newVertices.Count-4] = _p2;
			}
			
			// Add offset to interior corner  (corner: top - left)
			if ( _location.north && _location.northEast && _location.east && _location.southEast && _location.south && !_location.southWest && !_location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x , _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x, _p2.y, _p2.z - _borderOffset);
				newVertices[newVertices.Count-3	] = _p2;
			}
			
			// Add offset to interior corner  (corner: top - left)
			if ( _location.north && _location.northEast && _location.east && _location.southEast && _location.south && _location.southWest && !_location.west && !_location.northWest)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x , _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x, _p2.y, _p2.z + _borderOffset);
				newVertices[newVertices.Count-4] = _p2;
			}
			
			// Right side
			if (!_location.east)
			{
				newVertices.Add( new Vector3 ((((x + 1 + _xOffset) * _cSize) + _cellOffset) + _borderOffset, (_heightOffset + _height) - (_height), ((y + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + 1 + _xOffset) * _cSize) + _cellOffset) + _borderOffset, (_heightOffset + _height) - (_height), ((y - 1 + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + 1 + _xOffset) * _cSize) + _cellOffset) + _borderOffset, _heightOffset + _height, ((y - 1 + _zOffset) * _cSize) + _cellOffset));
				newVertices.Add( new Vector3 ((((x + 1 + _xOffset) * _cSize) + _cellOffset) + _borderOffset, _heightOffset + _height, ((y + _zOffset) * _cSize) + _cellOffset));
				Triangle();
				squareCount++;
			}
			
			// Add offset to corner (corner: top - right)
			if (!_location.north && !_location.east)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x, _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z + _borderOffset);
				newVertices[newVertices.Count-4] = _p2;
			}
			
			// Add offset to corner (corner: bottom - right)
			if (!_location.south && !_location.east)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x, _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x , _p2.y, _p2.z - _borderOffset);
				newVertices[newVertices.Count-3] = _p2;
			}
			
			
			// Add offset to interior corner  (corner: top - left)
			if ( _location.north && _location.northEast && !_location.east && !_location.southEast && _location.south && _location.southWest && _location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-1];
				_p = new Vector3(_p.x , _p.y, _p.z - _borderOffset);
				newVertices[newVertices.Count-1] = _p;
				
				var _p2 = newVertices[newVertices.Count-4];
				_p2 = new Vector3(_p2.x, _p2.y, _p2.z - _borderOffset);
				newVertices[newVertices.Count-4	] = _p2;
			}
			
			// Add offset to interior corner  (corner: top - left)
			if ( _location.north && !_location.northEast && !_location.east && _location.southEast && _location.south && _location.southWest && _location.west && _location.northWest)
			{
				var _p = newVertices[newVertices.Count-2];
				_p = new Vector3(_p.x , _p.y, _p.z + _borderOffset);
				newVertices[newVertices.Count-2] = _p;
				
				var _p2 = newVertices[newVertices.Count-3];
				_p2 = new Vector3(_p2.x, _p2.y, _p2.z + _borderOffset);
				newVertices[newVertices.Count-3] = _p2;
			}
		
		}
		
		static void Triangle()
		{
			newTriangles.Add(squareCount*4);
			newTriangles.Add((squareCount*4)+1);
			newTriangles.Add((squareCount*4)+3);
			newTriangles.Add((squareCount*4)+1);
			newTriangles.Add((squareCount*4)+2);
			newTriangles.Add((squareCount*4)+3);
		}
		
		
		static void GetBorderVertecies()
		{
	
			borderData = borderData.Distinct().ToList();
	
			List<Vector2> removeVertices = new List<Vector2>();
			
			for (int i = 0; i < borderData.Count; i ++)
			{
	
				int _hasNeighbours = 0;
				try
				{
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x - 1, borderData[i].vertexPosition.y - 1))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x, borderData[i].vertexPosition.y - 1))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x + 1, borderData[i].vertexPosition.y - 1))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x - 1, borderData[i].vertexPosition.y))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x + 1, borderData[i].vertexPosition.y))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x - 1, borderData[i].vertexPosition.y + 1))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x, borderData[i].vertexPosition.y + 1))
					{
						_hasNeighbours ++;
					}
					if (borderData[i].vertexPosition == new Vector2(borderData[i].vertexPosition.x + 1, borderData[i].vertexPosition.y + 1))
					{
						_hasNeighbours ++;
					}
				}catch{}
				
				if (_hasNeighbours == 8)
				{
					removeVertices.Add(borderData[i].vertexPosition);
				}
			}
			
			for (int j = 0; j < removeVertices.Count; j ++)
			{
				for (int i = 0; i < borderData.Count; i ++)
				{
					if (borderData[i].vertexPosition == (removeVertices[j]))
					{
						borderData.RemoveAt(i);
					}
				}
			}
		}
		
	
		
		static Mesh UpdateMesh()
		{
			var _mesh = new Mesh();
			
			_mesh.Clear ();
			_mesh.vertices = newVertices.ToArray();
			_mesh.triangles = newTriangles.ToArray();
	
			_mesh.Optimize ();
			_mesh.RecalculateNormals ();
			_mesh.RecalculateBounds();
			
			squareCount=0;
			newVertices.Clear();
			newTriangles.Clear();
			
			return _mesh;
		}
	    
	}
}
