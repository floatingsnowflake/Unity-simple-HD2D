using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWC.OdinSerializer;

namespace TWC
{
	/// <summary>
	/// Cluster object identifier
	/// </summary>
	public class ClusterIdentifier : SerializedMonoBehaviour 
	{
		public Guid layerGuid;
		public int clusterID;
	}
}