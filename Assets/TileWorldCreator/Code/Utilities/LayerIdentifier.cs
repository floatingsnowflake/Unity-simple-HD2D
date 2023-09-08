using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TWC.OdinSerializer;

namespace TWC
{
	/// <summary>
	/// Identifier for instantiation layer game objects
	/// </summary>
	public class LayerIdentifier : SerializedMonoBehaviour
	{
		public Guid assignedLayer;
	}
}