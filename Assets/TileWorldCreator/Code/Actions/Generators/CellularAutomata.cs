using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;
using TWC.Utilities;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Generators)]
	[ActionNameAttribute(Name="Cellular Automata")]
	public class CellularAutomata : TWCBlueprintAction, ITWCAction
	{
		public int numberOfSteps = 2;
		public int deathLimit = 4;
		public int birthLimit = 4;
	    // public int chanceToStartAlive;
	    private bool[,] newMap;
	    private int height;
	    private int width;
		private TWCGUILayout guiLayout;
		
		public ITWCAction Clone()
		{
			var _r = new CellularAutomata();
			
			_r.numberOfSteps = this.numberOfSteps;
			_r.deathLimit = this.deathLimit;
			_r.birthLimit = this.birthLimit;
			
			return _r;
		}
	
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				numberOfSteps = EditorGUI.IntField(guiLayout.rect, "number of steps", numberOfSteps);
				guiLayout.Add();
				deathLimit = EditorGUI.IntField(guiLayout.rect, "death limit", deathLimit);
				guiLayout.Add();
				birthLimit = EditorGUI.IntField(guiLayout.rect, "birth limit", birthLimit);
			}
			
			
		}
		#endif
		
		
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
		
		
	  
		public bool[,] Execute(bool[,] map, TileWorldCreator _twc)
		{
	        height = map.GetLength(1);
			width = map.GetLength(0);
	        
			// Make sure to set the seed from TileWorldCreator
			Debug.Log(_twc.currentSeed);
			
			UnityEngine.Random.InitState(_twc.currentSeed);
			
	        var _boolMap = Generate();
	
			return TileWorldCreatorUtilities.MergeMap(map, _boolMap);
	    }
	
	
		public bool[,] Generate()
		{
	
			newMap = new bool[width, height];
			InitialiseMap(newMap);
			
			
			//run simulation step for number of steps
			//more steps results in smoother worlds
			for (int i = 0; i < numberOfSteps; i++)
			{
			     newMap = DoSimulationStep(newMap);
			}
			
			return newMap;
		}
		
		bool[,] InitialiseMap(bool[,] map)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
	            	map[x,y] = UnityEngine.Random.Range(0f, 1f) < 0.5f ? false : true;
				}
			}
	
			return map;
		}
		
		
		//Do Simulation steps
		//-------------------
		bool[,] DoSimulationStep(bool[,] oldMap)
		{
			bool[,] tmpMap = new bool[width, height];
	
			//Loop over each row and column of the map
			for (int x = 0; x < oldMap.GetLength(0); x++)
			{
				for (int y = 0; y < oldMap.GetLength(1); y++)
				{
					var _count = CountNeighbours(x, y, oldMap);
					
					//The new value is based on our simulation rules
					//First, if a cell is alive but has too few neighbours, kill it.
					if (oldMap[x, y])
					{
						if (_count < deathLimit)
						{
							tmpMap[x, y] = false;
						}
						else
						{
							tmpMap[x, y] = true;
						}
	
					}
					//Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
					else
					{
						if (_count > birthLimit)
						{
							tmpMap[x, y] = true;
						}
						else
						{
							tmpMap[x, y] = false;
						}
						
					}
				}
			}
	
			return tmpMap;
	
		}
	
	
	     int CountNeighbours(int x, int y, bool[,] _map)
	    {
	        int _neighbours = 0;
	        for (int i = -1; i < 2; i ++)
	        {
	            for (int j = -1; j < 2; j ++)
	            {
	             
	              try{
	                    if (_map[x+i,y+j])
	                    {
	                        _neighbours++;
	                    }
	              }
	              catch{}
	            }
	        }
	
	        return _neighbours;
	    }
	}
}
