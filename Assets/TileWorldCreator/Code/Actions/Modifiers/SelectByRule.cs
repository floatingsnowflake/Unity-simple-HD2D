/*
 * MIT License
 *
 * Copyright (c) 2022 SparkAflame (Kevin Preece)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;

using TWC;
using TWC.Actions;
using TWC.editor;

using UnityEditor;

using UnityEngine;


namespace TWC.Actions
{
    /// <summary>
    ///    Select cells from a map using rules 
    /// </summary>
    [ActionCategory(Category = ActionCategoryAttribute.CategoryTypes.Modifiers)]
    [ActionName(Name = "Select By Rule")]
    [Serializable]
    public sealed class SelectByRule : TWCBlueprintAction, ITWCAction
    {
        public enum CellState
        {
            Unoccupied, // Cell must be unoccupied for the rule to pass
            Occupied,   // Cell must be occupied for the rule to pass
            DontCare    // Cell may be occupied or unoccupied for the rule to pass
        }


        #region > > > > >   struct CellRule   < < < < <

        [Serializable]
        public struct CellRule
        {
            /// <summary>
            ///    The cell rule.
            /// </summary>
            public CellState State => _state;

            [SerializeField]
            private CellState _state;


            /// <summary>
            ///    Checks whether another cell rule is the same as this one.
            /// </summary>
            /// <param name="other">
            ///    The other cell rule.
            /// </param>
            /// <returns>
            ///    <c>true</c> if the cell rules are the same.
            /// </returns>
            public bool Equals(CellRule other)
            {
                // Method included because it's recommended by the c# spec when implementing operator ==
                return _state == other._state;
            }


            /// <summary>
            ///    Checks whether another cell rule is the same as this one.
            /// </summary>
            /// <param name="obj">
            ///    The other cell rule.
            /// </param>
            /// <returns>
            ///    <c>true</c> if the cell rules are the same.
            /// </returns>
            public override bool Equals(object obj)
            {
                // Method included because it's recommended by the c# spec when implementing operator ==
                return obj is CellRule other && Equals(other);
            }


            /// <summary>
            ///    Gets a hash code for this cell rule.
            /// </summary>
            /// <returns>
            ///    A hash code for this cell rule.
            /// </returns>
            public override int GetHashCode()
            {
                // Method included because it's recommended by the c# spec when implementing operator ==
                return (int)_state;
            }


            /// <summary>
            ///    Initialise a cell rule to a given state.
            /// </summary>
            /// <param name="state">
            ///    The initial state.
            /// </param>
            public CellRule(CellState state)
            {
                _state = state;
            }


            /// <summary>
            ///    Compares whether this cell rule matches the state of the corresponding map cell.
            /// </summary>
            /// <param name="rule">
            ///    The cell rule being checked.
            /// </param>
            /// <param name="isOccupied">
            ///    <c>true</c> if the corresponding map cell is occupied, otherwise <c>false</c>.
            /// </param>
            /// <returns>
            ///    <c>true</c> if (1) this rule is <see cref="CellState.DontCare"/>, (2) this rule is
            ///    <see cref="CellState.Occupied"/> and the map cell is occupied, or (3) this rule is
            ///    <see cref="CellState.Unoccupied"/> and the map cell is unoccupied.  Otherwise returns <c>false</c>.
            /// </returns>
            public static bool operator ==(CellRule rule, bool isOccupied)
            {
                return (rule._state == CellState.DontCare)
                   || ((rule._state == CellState.Occupied) && isOccupied)
                   || ((rule._state == CellState.Unoccupied) && !isOccupied);
            }


            /// <summary>
            ///    Compares whether this cell rule does not match the state of the corresponding map cell.
            /// </summary>
            /// <param name="rule">
            ///    The cell rule being checked.
            /// </param>
            /// <param name="isOccupied">
            ///    <c>true</c> if the corresponding map cell is occupied, otherwise <c>false</c>.
            /// </param>
            /// <returns>
            ///    <c>false</c> if (1) this rule is <see cref="CellState.DontCare"/>, (2) this rule is
            ///    <see cref="CellState.Occupied"/> and the map cell is occupied, or (3) this rule is
            ///    <see cref="CellState.Unoccupied"/> and the map cell is unoccupied.  Otherwise returns <c>true</c>.
            /// </returns>
            public static bool operator !=(CellRule rule, bool isOccupied)
            {
                // Method included because it's required by the c# spec when implementing operator ==
                return !(rule == isOccupied);
            }


            /// <summary>
            ///   Advance to the next state, but wrap round to the beginning when we pass the last item.
            /// </summary>
            /// <param name="rule">
            ///    The cell rule being changed.
            /// </param>
            /// <returns>
            ///    The updated cell rule.
            /// </returns>
            public static CellRule operator ++(CellRule rule)
            {
                rule._state = (CellState)(((int)rule._state + 1) % 3);

                return rule;
            }
        }

        #endregion


        #region > > > > >   Exposed Fields / Classes  < < < < <

        [Serializable]
        public class SelectionRule
        {
            public CellRule[] CellRules = new CellRule[9];

            [SerializeField]
            public bool _rotate90;

            [SerializeField]
            public bool _rotate180;

            [SerializeField]
            public bool _rotate270;


            public SelectionRule()
            {
                _rotate90 = false;
                _rotate180 = false;
                _rotate270 = false;
            }


            public SelectionRule(SelectionRule rule)
            {
                _rotate90 = rule._rotate90;
                _rotate180 = rule._rotate180;
                _rotate270 = rule._rotate270;

                // Since CellRules is an array of value types we can just copy the array to copy the values.  Saves us
                // having to allocate whole new objects.

                Array.Copy(rule.CellRules, CellRules, CellRules.Length);
            }
        }


        [SerializeField]
        private List<SelectionRule> _selectionRules = new List<SelectionRule>();

        #endregion


        // These four arrays hold the indices into SelectionRule.CellRules for the four rule orientations:
        //
        // The order is: (-1,-1), (0,-1), (1,-1), (-1,0), (0,0), (1,0), (-1,1), (0,1), (1,1)
        // (As offsets from the centre cell when the rule cells are axis-aligned with the map.)
        //
        private static int[] _index0 = new int[] { 2, 5, 8, 1, 4, 7, 0, 3, 6 }; // zero degrees.
        private static int[] _index90 = new int[] { 8, 7, 6, 5, 4, 3, 2, 1, 0 }; // 90 degrees clockwise.
        private static int[] _index180 = new int[] { 6, 3, 0, 7, 4, 1, 8, 5, 2 }; // 180 degrees.
        private static int[] _index270 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // 270 degrees clockwise.

        private TWCGUILayout _guiLayout;


        /// <summary>
        ///    Creates a deep-copy clone of this action.
        /// </summary>
        /// <returns>
        ///    A deep clone of this action.
        /// </returns>
        public ITWCAction Clone()
        {
            SelectByRule clone = new SelectByRule();

            foreach (SelectionRule rule in _selectionRules)
            {
                clone._selectionRules.Add(new SelectionRule(rule));
            }

            return clone;
        }


        /// <summary>
        ///    Applies selection rules to decide whether (or not) a selected cell in the base map should remain set in
        ///    the final result map.  Results are cumulative (i.e. a boolean OR) with each other.  So that map cells
        ///    abutting the map edges aren't excluded the algorithm assumes that the map is expanded by one cell in all
        ///    directions and that the match rule for these cells is "DontCare".
        /// </summary>
        /// <param name="map">
        ///    The base map.
        /// </param>
        /// <param name="twc">
        ///    The <see cref="TileWorldCreator"/> instance executing the modifier.
        /// </param>
        /// <returns>
        ///    A map containing only those cells from the base map that have been selected by the rules.
        /// </returns>
        public bool[,] Execute(bool[,] map, TileWorldCreator twc)
        {
            int mapWidth = map.GetLength(0);
            int mapHeight = map.GetLength(1);
            bool[,] finalMap = new bool[mapWidth, mapHeight];
            int maxX = mapWidth - 1;
            int maxY = mapHeight - 1;

            foreach (SelectionRule rule in _selectionRules)
            {
                // Everything in this block could be run in parallel (BURST?) since the input map is read-only and the
                // outcome for any cell in the output map is independent of all other cells.  This could be important for
                // large maps since the algorithm as it stands is O(M*N).

                CellRule[] cellRules = rule.CellRules;

                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        if (!finalMap[x, y]) // No need to evaluate more rules if a prior rule has selected this cell
                        {
                            bool result;

                            if (!(result = TryRule(_index0)))
                            {
                                if (!(result = rule._rotate90 && TryRule(_index90)))
                                {
                                    if (!(result = rule._rotate180 && TryRule(_index180)))
                                    {
                                        result = rule._rotate270 && TryRule(_index270);
                                    }
                                }
                            }

                            finalMap[x, y] = result;
                        }

                        // --- END OF FOR BLOCK ---

                        bool TryRule(int[] index)
                        {
                            return
                               map[x, y]
                               && ((x == 0) || (y == 0) || (cellRules[index[0]] == map[x - 1, y - 1]))
                               && ((y == 0) || (cellRules[index[1]] == map[x, y - 1]))
                               && ((x == maxX) || (y == 0) || (cellRules[index[2]] == map[x + 1, y - 1]))
                               && ((x == 0) || (cellRules[index[3]] == map[x - 1, y]))
                               && ((x == maxX) || (cellRules[index[5]] == map[x + 1, y]))
                               && ((x == 0) || (y == maxY) || (cellRules[index[6]] == map[x - 1, y + 1]))
                               && ((y == maxY) || (cellRules[index[7]] == map[x, y + 1]))
                               && ((x == maxX) || (y == maxY) || (cellRules[index[8]] == map[x + 1, y + 1]));
                        }
                    }
                }
            }

            return finalMap;
        }


#if UNITY_EDITOR


    public override void DrawGUI(Rect rect, int layerIndex, TileWorldCreatorAsset asset, TileWorldCreator twc)
    {
        if (rect.width < 0.0f)
        {
            // Debugging shows this method is called many times in succession and sometimes 'rect' has some highly
            // suspect values.  So just bail out without trying using these dubious values to lay out the UI. 

            return;
        }

        if (null == _selectionRules)
        {
            _selectionRules = new List<SelectionRule>();
        }

        using (_guiLayout = new TWCGUILayout(rect))
        {
            _guiLayout.Add(8);
            _guiLayout.Add();

            if (GUI.Button(_guiLayout.rect, "Add new rule"))
            {
                _selectionRules.Add(new SelectionRule());
            }

            try
            {
                for (int u = 0; u < _selectionRules.Count; u++)
                {

                    _guiLayout.Add();

                    GUI.Box(new Rect(_guiLayout.rect.x, _guiLayout.rect.y + 5, _guiLayout.rect.width, 64), string.Empty);

                    if (GUI.Button(new Rect((_guiLayout.rect.x + _guiLayout.rect.width) - 20, _guiLayout.rect.y + 5, 20, 20), "x"))
                    {
                        _selectionRules.RemoveAt(u);
                    }

                    const float w = 30.0f;
                    const float h = 20.0f;
                    SelectionRule rule = _selectionRules[u];

                    DrawCellRules();
                    DrawRotationCheckboxes();


                    // --- END OF USING BLOCK ---

                    // There's no real benefit to using local methods here except to break up a very long method into more
                    // understandable chunks.


                    void DrawCellRules()
                    {
                        Rect r = new Rect(0.0f, _guiLayout.rect.y - 12.0f, w - 6.0f, 18.0f);

                        for (int y = 0; y < 3; y++)
                        {
                            _guiLayout.Add();

                            r.x = _guiLayout.rect.x + 6;
                            r.y += h;

                            for (int x = 0; x < 3; x++)
                            {
                                //Rect r = new Rect( _guiLayout.rect.x + ( x * w ) + 6, ( _guiLayout.rect.y + ( y * h ) ) - 10, w - 6.0f, 18.0f );

                                if ((x == 1) && (y == 1))
                                {
                                    GUI.color = Color.clear;

                                    GUI.Button(r, string.Empty);
                                }
                                else
                                {
                                    Color colour;  // deliberately not initialised - ask if you don't know why.
                                    string val;     // deliberately not initialised - ask if you don't know why.
                                    string tooltip; // deliberately not initialised - ask if you don't know why.

                                    int index = (x * 3) + y;

                                    switch (rule.CellRules[index].State)
                                    {
                                        case CellState.Occupied:
                                            colour = Color.green;
                                            val = "Y";
                                            tooltip = "The corresponding map cell must be occupied.";

                                            break;

                                        case CellState.Unoccupied:
                                            colour = Color.red;
                                            val = "N";
                                            tooltip = "The corresponding map cell must be unoccupied.";

                                            break;

                                        case CellState.DontCare:
                                        default:
                                            colour = Color.gray;
                                            val = string.Empty;
                                            tooltip = "The corresponding map cell may be occupied or unoccupied.";

                                            break;
                                    }

                                    GUI.color = colour;

                                    if (GUI.Button(r, new GUIContent(val, tooltip)))
                                    {
                                        ++rule.CellRules[index];
                                    }
                                }

                                r.x += w;
                                GUI.color = Color.white;
                            }
                        }
                    }


                    void DrawRotationCheckboxes()
                    {
                        const float rowHeight = 12.0f;
                        const float rowSeparation = 5.0f;
                        const float xOffset = 110.0f;

                        Rect r =
                            new Rect(
                                _guiLayout.rect.x + xOffset,
                                _guiLayout.rect.y - 44.0f,
                                _guiLayout.rect.width - xOffset - 110.0f,
                                rowHeight
                            );

                        rule._rotate90 =
                            EditorGUI.ToggleLeft(
                                r,
                                new GUIContent("Rotate by 90ª", "Also try matching this rule by rotating it 90ª clockwise."),
                                rule._rotate90
                            );

                        r.y += rowHeight + rowSeparation;

                        rule._rotate180 =
                            EditorGUI.ToggleLeft(
                                r,
                                new GUIContent("Rotate by 180ª", "Also try matching this rule by rotating it 180ª."),
                                rule._rotate180
                            );

                        r.y += rowHeight + rowSeparation;

                        rule._rotate270 =
                            EditorGUI.ToggleLeft(
                                r,
                                new GUIContent("Rotate by 270ª", "Also try matching this rule by rotating it 270ª clockwise."),
                                rule._rotate270
                            );
                    }
                }
            }
            catch { }
        }
    }
    

#endif


      public float GetGUIHeight()
      {
         if ( _guiLayout != null )
         {
            return _guiLayout.height;
         }

         return 80.0f;
      }
   }
}
