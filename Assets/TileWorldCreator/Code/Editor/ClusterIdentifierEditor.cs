using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TWC;


namespace TWC.editor
{
	[CustomEditor(typeof(ClusterIdentifier))]
	public class ClusterIdentifierEditor : Editor
	{
		public ClusterIdentifier ci;
		
		public void OnEnable()
		{
			ci = (ClusterIdentifier)target;
		}
		
		public override void OnInspectorGUI()
		{
			GUILayout.Label("Layer");
			GUILayout.Label(ci.layerGuid.ToString());
			GUILayout.Label("Cluster");
			GUILayout.Label(ci.clusterID.ToString());
		}
	}
}