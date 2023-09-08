using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TWC;

namespace TWC.editor
{
	[CustomEditor(typeof(LayerIdentifier))]
	public class LayerIdentifierEditor : Editor
	{
		
		public LayerIdentifier li;
		
		public void OnEnable()
		{
			li = (LayerIdentifier)target;
		}
		
		public override void OnInspectorGUI()
		{
			GUILayout.Label(li.assignedLayer.ToString());
		}
	}
}
