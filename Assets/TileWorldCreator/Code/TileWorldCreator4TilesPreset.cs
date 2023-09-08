using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWC.OdinSerializer;

namespace TWC
{
	[CreateAssetMenu(menuName = "TileWorldCreator/New 3D 4-Tiles Preset")]
	public class TileWorldCreator4TilesPreset : SerializedScriptableObject
	{
		public bool useRulesForEdgeTile;
		public bool useRulesForExtCornerTile;
		public bool useRulesForIntCornerTile;
		public bool useRulesForFillTile;
		
		public GameObject edgeTile;
		public GameObject exteriorCornerTile;
		public GameObject interiorCornerTile;
		public GameObject fillTile;
		
		public Vector3 edgeTileRotationOffset;
		public Vector3 exteriorCornerTileRotationOffset;
		public Vector3 interiorCornerTileRotationOffset;
		public Vector3 fillTileRotationOffset;	
		
		public Vector3 edgeTileScalingOffset;
		public Vector3 exteriorCornerScalingOffset;
		public Vector3 interiorCornerScalingOffset;
		public Vector3 fillTileScalingOffset;
		
	}
}