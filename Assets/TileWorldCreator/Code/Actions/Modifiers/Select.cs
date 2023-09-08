using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TWC.editor;
using TWC.Utilities;

namespace TWC.Actions
{
	[ActionCategoryAttribute(Category=ActionCategoryAttribute.CategoryTypes.Modifiers)]
	[ActionNameAttribute(Name="Select")]
	public class Select : TWCBlueprintAction, ITWCAction
	{
		public enum SelectType
		{
			border,
			edges,
			corners,
			interiorCorners,
			fill,
			random,
			rule
		}
		
		public SelectType selectType;
		public float randomSelectionWeight;
		//public bool useSelectionRule;
		
		
		
		[System.Serializable]
		public class SelectionRule
		{
			public bool[,] neighbours = new bool[3,3];
			public bool[,] selectionResult = new bool[1,1];
		}
		
		public List<SelectionRule> neighbourSelectionRules = new List<SelectionRule>();
		//public  bool[,] neighbours = new bool[3,3];
		
		private TWCGUILayout guiLayout;
		       
		private bool[,] originalMap;
		
		public ITWCAction Clone()
		{
			var _r = new Select();
			
			_r.selectType = this.selectType;
			_r.randomSelectionWeight = this.randomSelectionWeight;
			
			_r.neighbourSelectionRules = new List<SelectionRule>();
			for (int i = 0; i < this.neighbourSelectionRules.Count; i ++)
			{
				_r.neighbourSelectionRules.Add(this.neighbourSelectionRules[i]);
			}
			
			return _r;
		}
		
	        
		public bool[,] Execute(bool [,] _map, TileWorldCreator _twc)
		{
			
			Random.InitState(_twc.twcAsset.randomSeed);
			
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			originalMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			System.Array.Copy(_map, originalMap, _map.Length);
		
			switch (selectType)
			{
				case SelectType.border:
					_outputMap = SelectBorder(_map);
					break;
				case SelectType.corners:
					_outputMap = SelectCorners(_map);
					break;
				case SelectType.interiorCorners:
					_outputMap = SelectInteriorCorners(_map);
					break;
				case SelectType.edges:
					_outputMap = SelectEdges(_map);
					break;
				case SelectType.fill:
					_outputMap = SelectFill(_map);
					break;
				case SelectType.random:
					_outputMap = SelectRandom(_map);
					break;
				case SelectType.rule:
					_outputMap = SelectByRule(_map);
					break;
			}
		
			
			return _outputMap;
		}
		
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			if (neighbourSelectionRules == null)
			{
				neighbourSelectionRules = new List<SelectionRule>();
			}
			
			using (guiLayout = new TWCGUILayout(_rect))
			{
				guiLayout.Add();
				selectType = (SelectType) EditorGUI.EnumPopup(guiLayout.rect, selectType);
				if (selectType == SelectType.random)
				{
					guiLayout.Add();
					randomSelectionWeight = EditorGUI.Slider(guiLayout.rect, "Random weight", randomSelectionWeight, 0f, 1f);
				}
				
				guiLayout.Add();
				//useSelectionRule = EditorGUI.Toggle(guiLayout.rect, "use selection rule", useSelectionRule);
				
				if (selectType == SelectType.rule)
				{
					guiLayout.Add();
					if (GUI.Button(guiLayout.rect, "Add new rule"))
					{
						neighbourSelectionRules.Add(new SelectionRule());
					}
					
					for (int u = 0; u < neighbourSelectionRules.Count; u ++)
					{
						
						guiLayout.Add();
	
						GUI.Box(new Rect(guiLayout.rect.x, guiLayout.rect.y + 5, guiLayout.rect.width, 67), "");
						
						if (GUI.Button(new Rect(guiLayout.rect.x + guiLayout.rect.width - 20, guiLayout.rect.y + 5, 20, 20), "x"))
						{
							neighbourSelectionRules.RemoveAt(u);
						}
		
						float w = 30;
						float h = 5;
						
						for (int y = 0; y < 3; y++)
						{
							guiLayout.Add();
							for (int x = 0; x < 3; x++)
							{
								Rect r = new Rect(guiLayout.rect.x + (x * w), guiLayout.rect.y + (y * h)  - 10, w, 15);
		
								if (x == 1 && y == 1)
								{
									EditorGUI.Toggle(new Rect(r.x, r.y, 30, 15), "", false, EditorStyles.textArea);
								}
								
								if (neighbourSelectionRules[u].neighbours[x,y])
								{
									GUI.color = Color.green;
								}
								else
								{
									GUI.color = Color.red;
								}
								
								
								if (x == 1 && y == 1)
								{
								}else
								{
									neighbourSelectionRules[u].neighbours[x,y] = EditorGUI.Toggle(r, "", neighbourSelectionRules[u].neighbours[x,y]);
								}
								
								GUI.color = Color.white;
							}
						}
						
					}
				}
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
		
		
		
		bool[,] SelectBorder(bool[,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
	                
					var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
					var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(_map, x, y);
						
						
					if (_neighbours < 9 && _map[x,y])
					{			
						_outputMap[x,y] = true;
					}
				}
			}
			
			
			return _outputMap;
		
		}
		
		bool[,] SelectEdges(bool[,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
	                
					if (_map[x,y])
					{
						var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
						var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(originalMap, x, y);
						
						if (_neighbours == 6)
						{
							// STRAIGHT
							if (_locations.west && _locations.east && _locations.southWest && _locations.south && _locations.southEast)
							{
								
								_outputMap[x,y] = true;
								
							}
							// STRAIGHT
							if (_locations.west && _locations.east && _locations.northWest && _locations.north && _locations.northEast)
							{
								
								_outputMap[x,y] = true;
								
							}
							// STRAIGHT
							if (_locations.west && _locations.northWest && _locations.north && _locations.southWest && _locations.south)
							{
								
								_outputMap[x,y] = true;
								
							}
							// STRAIGHT
							if (_locations.east && _locations.north && _locations.northEast && _locations.south && _locations.southEast)
							{
								
								_outputMap[x,y] = true;
							
							}
						}
					}
				}
			}
			
			
			return _outputMap;
		}
		
		
		
		
		bool[,] SelectCorners(bool [,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
	                if (_map[x,y])
					{
						var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
		                var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(originalMap, x, y);
						
						// CORNER
						if (_neighbours == 4)
						{
							
							_outputMap[x,y] = true;
						}
						// CORNER
						if (_neighbours == 5)
						{
							
							_outputMap[x,y] = true;

						}
						// CORNER
						if (_neighbours == 6)
						{
							// CORNER
							if (_locations.northWest && _locations.north && _locations.northEast && _locations.east && _locations.southEast)
							{					
								
								_outputMap[x,y] = true;
								
							}
							// CORNER
							if (_locations.southWest && _locations.south && _locations.southEast && _locations.east && _locations.northEast)
							{
								
								_outputMap[x,y] = true;
								
							}
							// CORNER
							if (_locations.southWest && _locations.west && _locations.northWest && _locations.north && _locations.northEast)
							{
								
								_outputMap[x,y] = true;
								
							}
							// CORNER
							if (_locations.southWest && _locations.south && _locations.southEast && _locations.west && _locations.northWest)
							{
							
								_outputMap[x,y] = true;
								
							}
						}
					}
				}
			}
			
			return _outputMap;
		}
		
		bool[,] SelectInteriorCorners(bool[,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
					if (_map[x,y])
					{
						var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
						var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(originalMap, x, y);
					
						if (_neighbours == 8)
						{
							
							_outputMap[x,y] = true;
							
						}
					}
				}
			}
			
			
			
			return _outputMap;
		}
		
		bool[,] SelectFill(bool[,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
					if (_map[x,y])
					{
						var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
						var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(originalMap, x, y);
					
						if (_neighbours == 9)
						{
							
							_outputMap[x,y] = true;
							
						}
					}
				}
			}
			
		
			
			return _outputMap;
		}
		
		bool[,] SelectRandom(bool[,] _map)
		{
			bool[,] _outputMap = new bool[_map.GetLength(0), _map.GetLength(1)];
			
			for (int x = 0; x < _map.GetLength(0); x ++)
			{
				for (int y = 0; y < _map.GetLength(1); y ++)
				{
					if (_map[x,y])
					{
						var _neighbours = TileWorldCreatorUtilities.CountNeighbours(x, y, _map);
						var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(originalMap, x, y);
					
						var _rnd = Random.Range(0f, 1f);
						if (_rnd < randomSelectionWeight)
						{
						
							_outputMap[x,y] = true;
							
						}
					}
				}
			}
			
			
			
			return _outputMap;
	                
		}
		
		bool[,] SelectByRule(bool[,] _modifyMap)
		{
					
			bool _ruleAccepted = true;
			bool[,] _finalMap = new bool[_modifyMap.GetLength(0), _modifyMap.GetLength(1)];
				
			for (int r = 0; r < neighbourSelectionRules.Count; r ++)
			{
				neighbourSelectionRules[r].selectionResult = new bool[_modifyMap.GetLength(0), _modifyMap.GetLength(1)];
			
				System.Array.Copy (_modifyMap, neighbourSelectionRules[r].selectionResult, _modifyMap.Length);
				
				for (int x = 0; x < _modifyMap.GetLength(0); x ++)
				{
					for (int y = 0; y < _modifyMap.GetLength(1); y ++)
					{
						if (_modifyMap[x,y])
						{
							var _locations = TileWorldCreatorUtilities.GetNeighboursLocation(_modifyMap, x, y);
							_ruleAccepted = false;
							
							int _ruleCountsPositive = 0;
							int _checkRulesPositive = 0;
							int _ruleCountNegative = 0;
							int _checkRuleNegative = 0;
							
							if (neighbourSelectionRules[r].neighbours[0, 0])
							{
								_ruleCountsPositive ++;
								
								if (_locations.northWest)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.northWest)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[1, 0])
							{
								_ruleCountsPositive ++;
								
								if (_locations.north)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.north)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[2, 0])
							{
								_ruleCountsPositive ++;
								if (_locations.northEast)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.northEast)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[0, 1])
							{
								_ruleCountsPositive ++;
								if (_locations.west)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.west)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[2, 1])
							{
								_ruleCountsPositive ++;
								if (_locations.east)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.east)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[0, 2])
							{
								_ruleCountsPositive ++;
								if (_locations.southWest)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.southWest)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[1, 2])
							{
								_ruleCountsPositive ++;
								if (_locations.south)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.south)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (neighbourSelectionRules[r].neighbours[2, 2])
							{
								_ruleCountsPositive ++;
								if (_locations.southEast)
								{
									_checkRulesPositive ++;
								}
							}
							else
							{
								_ruleCountNegative ++;
								
								if (!_locations.southEast)
								{
									_checkRuleNegative ++;
								}
							}
							
							if (_ruleCountsPositive == _checkRulesPositive && _ruleCountNegative == _checkRuleNegative)
							{
								_ruleAccepted = true;
							}
	
	
							
							neighbourSelectionRules[r].selectionResult[x,y] = _ruleAccepted;
						
						}
					}
				}
			
	
			
			
		
				for (int x = 0; x < neighbourSelectionRules[r].selectionResult.GetLength(0); x ++)
				{
					for (int y = 0; y < neighbourSelectionRules[r].selectionResult.GetLength(1); y ++)
					{
						if (neighbourSelectionRules[r].selectionResult[x,y])
						{
							_finalMap[x,y] = true;
						}
					}
				}
				
			
			}
	
			return _finalMap;
		}
		
	}
}
