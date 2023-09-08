using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWC.OdinSerializer;

namespace TWC
{
	[CreateAssetMenu(menuName = "TileWorldCreator/New 3D 6-Tiles Preset")]
	public class TileWorldCreator6TilesPreset : SerializedScriptableObject
	{
		public bool useRulesForEdgeTile;
		public bool useRulesForExtCornerTile;
		public bool useRulesForIntCornerTile;
		public bool useRulesForFillTile;
		
		public GameObject singleTile;
		public GameObject deadEndTile;
		public GameObject straightTile;
		public GameObject cornerTile;
		public GameObject threeWayTile;
		public GameObject fourWayTile;
		
		public Vector3 deadEndTileRotationOffset;
		public Vector3 straightTileRotationOffset;
		public Vector3 cornerTileRotationOffset;
		public Vector3 threeWayTileRotationOffset;	
		public Vector3 fourWayTileRotationOffset;
		
		public Vector3 singleTileScalingOffset;
		public Vector3 deadEndTileScalingOffset;
		public Vector3 straightTileScalingOffset;
		public Vector3 cornerTileScalingOffset;
		public Vector3 threeWayTileScalingOffset;
		public Vector3 fourWayTileScalingOffset;
	}
}