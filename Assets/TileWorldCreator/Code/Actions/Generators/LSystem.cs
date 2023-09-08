using System.Text;
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
	[ActionNameAttribute(Name="L-System")]
	public class LSystem : TWCBlueprintAction, ITWCAction
	{
		
		public class KeyPosition
		{
			public Vector3 position, direction;
			public int length;
		}
		
		
		public class Rule
		{
			public string letter = "F";
			
			//[+F][-F] 
			public List<string> results = new List<string>();
		
			public string GetResult()
			{
				int randomIndex = UnityEngine.Random.Range(0, results.Count);
				return results[randomIndex];
			}
			
			public Rule()
			{
				// Add three basic rules
				results.Add("[+F][-F]");
				results.Add("[+F]F[-F]");
				results.Add("[-F]F[+F]");
			}
		}
		
		
		public Rule rule = new Rule();
		
		public string rootSentence = "[F]--F";
		
		public bool randomIgnoreRuleModifier = false;
		public float chanceToIgnoreRule = 0.3f;
		
		public int iterations = 3;
		public int length = 4;
		public bool shortenLength;
	
		List<Vector3> positions = new List<Vector3>();
		TWCGUILayout guiLayout;
		
		public int Length
		{
			get 
			{
				if (length > 0)
				{
					return length;
				}
				else
				{
					return 1;
				}
			}
			set
			{
				length = value;
			}
		}
		
		
		public enum EncodingLetters
		{
			unknown = '1',
			save = '[',
			load = ']',
			draw = 'F',
			turnRight = '+',
			turnLeft = '-'
		}
		
		public ITWCAction Clone()
		{
			var _r = new LSystem();
			
			_r.rule = new Rule();
			_r.rule.results = new List<string>();
			
			for (int r = 0; r < this.rule.results.Count; r ++)
			{
				_r.rule.results.Add(this.rule.results[r]);
			}
			
			_r.iterations = this.iterations;
			_r.length = this.length;
			_r.shortenLength = this.shortenLength;
			_r.randomIgnoreRuleModifier = this.randomIgnoreRuleModifier;
			
			return _r;
			
		}
		
		#if UNITY_EDITOR
		public override void DrawGUI(Rect _rect, int _layerIndex, TileWorldCreatorAsset _asset, TileWorldCreator _twc)
		{
			using (guiLayout = new TWCGUILayout(_rect))
			{
				
				guiLayout.Add();
				if (GUI.Button(guiLayout.rect, "Add rule"))
				{
					rule.results.Add("");
				}
				for (int r = 0; r < rule.results.Count; r ++)
				{
					guiLayout.Add();
					rule.results[r] = EditorGUI.TextField(new Rect(guiLayout.rect.x, guiLayout.rect.y, guiLayout.rect.width-20, guiLayout.rect.height), "Rule", rule.results[r]);
					if (GUI.Button(new Rect(guiLayout.rect.x + (guiLayout.rect.width - 20), guiLayout.rect.y, 20, guiLayout.rect.height), "x"))
					{
						rule.results.RemoveAt(r);
					}
				}
				
				guiLayout.Add();
				iterations = EditorGUI.IntField(guiLayout.rect, "Iterations", iterations);
				guiLayout.Add();
				length = EditorGUI.IntField(guiLayout.rect, "Length", length);
				guiLayout.Add();
				shortenLength = EditorGUI.Toggle(guiLayout.rect, "Shorten length on each iteration", shortenLength);
				guiLayout.Add();
				randomIgnoreRuleModifier = EditorGUI.Toggle(guiLayout.rect, "Random ignore rules", randomIgnoreRuleModifier);
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
			
			// Make sure to set the seed from TileWorldCreator
			UnityEngine.Random.InitState(_twc.currentSeed);
			
			var _sequence = GenerateSentence();
				
			Stack<KeyPosition> savePoints = new Stack<KeyPosition>();
			positions = new List<Vector3>();
			var tmpLength = length;
			var tmpAngle = 90;
			var currentPosition = new Vector3(map.GetLength(0) / 2, 0, map.GetLength(1) / 2);
			Vector3 direction = Vector3.forward;
			Vector3 tempPosition = Vector3.zero;
			
			
			foreach(var letter in _sequence)
			{
				
				EncodingLetters encoding = (EncodingLetters)letter;
				
				switch (encoding)
				{
					case EncodingLetters.save:
						savePoints.Push(new KeyPosition
						{
							position = currentPosition,
							direction = direction,
							length = Length
						});
						break;
					case EncodingLetters.load:
						if (savePoints.Count > 0)
						{
							var keyPosition = savePoints.Pop();
							currentPosition = keyPosition.position;
							direction = keyPosition.direction;
							Length = keyPosition.length;
						}
						else
						{}
						break;
					case EncodingLetters.draw:
						tempPosition = currentPosition;
						currentPosition += direction * tmpLength;
						
						var _maxX = 0;
						var _maxZ = 0;
						var _startX = 0;
						var _startZ = 0;
						
						if (currentPosition.x >= tempPosition.x)
						{
							_maxX = (int)currentPosition.x;
							_startX = (int)tempPosition.x;
						}
						else
						{
							_maxX = (int)tempPosition.x;
							_startX = (int)currentPosition.x;
						}
						
						if (currentPosition.z >= tempPosition.z)
						{
							_maxZ = (int)currentPosition.z;
							_startZ = (int)tempPosition.z;
						}
						else
						{
							_maxZ = (int)tempPosition.z;
							_startZ = (int)currentPosition.z;
						}
						
					
						
					
						
						for (int x = _startX; x <= _maxX; x ++)
						{
							for (int y = _startZ; y <= _maxZ; y ++)
							{
								
								
								
								positions.Add(new Vector3(x, 0, y));
							}
						}
						
						if (shortenLength)
						{
							//Shorten the path length				
							if (tmpLength - 2 <= 0)
							{
								tmpLength = 1;
							}
							else
							{
								tmpLength -= 2;
							}
						}
						
						break;
					case EncodingLetters.turnLeft:
						direction = Quaternion.AngleAxis(tmpAngle, Vector3.up) * direction;
						break;
					case EncodingLetters.turnRight:
						direction = Quaternion.AngleAxis(-tmpAngle, Vector3.up) * direction;
						break;
					default:
						break;
				}
			}
			
			var _boolMap = new bool[map.GetLength(0), map.GetLength(1)];
			
			// Cleanup map
			List<Vector3> _removePositions = new List<Vector3>();
			foreach ( var p in positions)
			{
				
				var _nb = 0;
				var _dg1 = 0;
				var _dg2 = 0;
				var _dg3 = 0;
				var _dg4 = 0;
				
				for (int x = -1; x < 2; x ++)
				{
					for (int y = -1; y < 2; y ++)
					{
						var _p2 = new Vector3(p.x + x, p.y, p.z + y);
						
						
						if (positions.Contains(_p2) && !_removePositions.Contains(_p2))
						{
							_nb ++;
							if (x == -1 && y == 0)
							{
								_dg1 ++;
								_dg4 ++;
							}
							
							if (x == -1 && y == 1)
							{
								_dg1 ++;
							}
							
							if (x == 0 && y == 1)
							{
								_dg1 ++;
								_dg2 ++;
							}
							
							if (x == 1 && y == 1)
							{
								_dg2 ++;
							}
							if (x == 1 && y == 0)
							{
								_dg2 ++;
								_dg3 ++;
							}
							if (x == 1 && y == -1)
							{
								_dg3 ++;
							}
							if (x == 0 && y == -1)
							{
								_dg3 ++;
								_dg4 ++;
							}
							if (x == -1 && y == -1)
							{
								_dg4 ++;
							}
						}
						
						
					}
				}
				
				if (_nb < 6)
				{
					if (_nb == 4 && (_dg1 == 3 || _dg2 == 3 || _dg3 == 3 || _dg4 == 3))
					{
						_removePositions.Add(p);
					}
				}
				else
				{
					_removePositions.Add(p);
				}
			}
	
			foreach(var _rm in _removePositions)
			{
				positions.Remove(_rm);
			}
			
			
			foreach (var position in positions)
			{
				if (position.x >= 0 && position.x < _boolMap.GetLength(0) && position.z >= 0 && position.z < _boolMap.GetLength(1))
				{
					_boolMap[(int)position.x, (int)position.z] = true;
				}
			}
			
			
			return TileWorldCreatorUtilities.MergeMap(map, _boolMap);
		}
		
		public string GenerateSentence(string _word = null)
		{
			if (_word == null)
			{
				_word = rootSentence;
			}
			
			return GrowRecursive(_word);
		}
		
		public string GrowRecursive(string _word, int _iterationIndex = 0)
		{
			if (_iterationIndex >= iterations)
			{
				return _word;
			}
			
			StringBuilder _newWord = new StringBuilder();
			
			foreach (var c in _word)
			{
				_newWord.Append(c);
				ProcessRulesRecursivally(_newWord, c, _iterationIndex);
			}
			
			return _newWord.ToString();
		}
		
		private void ProcessRulesRecursivally(StringBuilder _newWord, char _c, int _iterationIndex)
		{
		
			if (rule.letter == _c.ToString())
			{
				if (randomIgnoreRuleModifier && _iterationIndex > 1)
				{
					if (UnityEngine.Random.value < chanceToIgnoreRule)
					{
						return;
					}
				}
				
				_newWord.Append(GrowRecursive(rule.GetResult(), _iterationIndex + 1));
					
			}
	
		}
	}

}
