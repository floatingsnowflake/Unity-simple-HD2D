using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TWC.Actions
{
	public interface ITWCAction
	{
		bool[,] Execute(bool[,] _map, TileWorldCreator _twc);
		ITWCAction Clone();
		float GetGUIHeight();
	}
}