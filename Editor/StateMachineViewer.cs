using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Stateforge.Editor
{
    public class StateMachineViewer : EditorWindow
    {
        private GameObject _selectedGameObject;
        private Vector2 _scrollPosition;
        private GUIStyle _labelStyle;
        private double _lastUpdateTime;

        private const float NodeHeight = 60f;
        private const float HorizontalSpacing = 20f;
        private const float VerticalSpacing = 60f;
        

        [MenuItem("Window/Stateforge/State Viewer")]
        public static void ShowWindow()
        {
            GetWindow<StateMachineViewer>("Stateforge State Viewer");
        }

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            var currentTime = EditorApplication.timeSinceStartup;
            if (EditorApplication.isPlaying && currentTime - _lastUpdateTime > 1.0 / 60.0)
            {
                Repaint();
                _lastUpdateTime = currentTime;
            }
        }

        private void InitStyles()
        {
            _labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
        }

        private void OnGUI()
        {
            InitStyles();

            var currentSelection = Selection.activeGameObject;
            object stateFactory;
            if (currentSelection != _selectedGameObject)
            {
                _selectedGameObject = currentSelection;
                if (_selectedGameObject == false)
                {
                    stateFactory = StateMachineData.GetStateFactory(_selectedGameObject);
                    if (stateFactory != null)
                    {
                        StateMachineData.PopulateStatesInEditMode(stateFactory);
                    }
                }
                Repaint();
            }

            if (_selectedGameObject == false)
            {
                EditorGUILayout.HelpBox("Select a GameObject with a StateFactory component.", MessageType.Info);
                return;
            }

            stateFactory = StateMachineData.GetStateFactory(_selectedGameObject);
            if (stateFactory == null)
            {
                EditorGUILayout.HelpBox("No StateFactory found on the selected GameObject.", MessageType.Warning);
                return;
            }

            GUILayout.Label($"GameObject: {_selectedGameObject.gameObject.name}", EditorStyles.boldLabel);

            HashSet<Type> activeStates = new();
            if (EditorApplication.isPlaying)
            {
                var stateMachine = StateMachineData.GetStateMachine(stateFactory);
                if (stateMachine != null)
                {
                    StateMachineData.GetActiveStateBranch(stateMachine, activeStates);
                }
            }
            else
            {
                StateMachineData.PopulateStatesInEditMode(stateFactory);
            }

            EditorGUILayout.Space();

            List<Type> rootStates = StateMachineData.GetRootStates(stateFactory);
            IReadOnlyDictionary<Type, List<Type>> stateMap = StateMachineData.GetStateMap(stateFactory);

            if (rootStates.Count == 0)
            {
                EditorGUILayout.HelpBox("No states found. Ensure SetStates is implemented and you are in Play Mode, or that the viewer can access it in Edit Mode.", MessageType.Info);
            }
            else
            {
                DrawStateGraph(rootStates, stateMap, activeStates);
            }
        }

         private void DrawStateGraph(List<Type> rootStates, IReadOnlyDictionary<Type, List<Type>> stateMap, ICollection<Type> activeStates)
        {
            Dictionary<Type, Rect> nodeRects = new();
            Dictionary<int, List<Type>> statesByDepth = new();
            HashSet<Type> allStates = new();

            Queue<(Type state, int depth)> queue = new();
            foreach (var rootState in rootStates.Where(rootState => allStates.Add(rootState)))
            {
                queue.Enqueue((rootState, 0));
            }

            while (queue.Count > 0)
            {
                var (currentState, depth) = queue.Dequeue();

                if (!statesByDepth.ContainsKey(depth))
                {
                    statesByDepth[depth] = new List<Type>();
                }
                statesByDepth[depth].Add(currentState);

                if (stateMap.TryGetValue(currentState, out List<Type> children))
                {
                    foreach (var child in children.Where(child => allStates.Add(child)))
                    {
                        queue.Enqueue((child, depth + 1));
                    }
                }
            }

            Dictionary<Type, float> nodeWidths = new();
            const float padding = 40f;
            const float minWidth = 80f;
            foreach (var stateType in allStates)
            {
                var width = _labelStyle.CalcSize(new GUIContent(stateType.Name)).x + padding;
                nodeWidths[stateType] = Mathf.Max(width, minWidth);
            }

            float totalHeight = statesByDepth.Keys.Count * (NodeHeight + VerticalSpacing) + VerticalSpacing;
            float maxWidth = 0;

            foreach (var depth in statesByDepth.Keys)
            {
                List<Type> statesInLevel = statesByDepth[depth];
                var levelWidth = statesInLevel.Sum(state => nodeWidths[state]) + Mathf.Max(0, statesInLevel.Count - 1) * HorizontalSpacing;
                if (levelWidth > maxWidth)
                {
                    maxWidth = levelWidth;
                }

                float currentX = (position.width - levelWidth) / 2;
                float y = depth * (NodeHeight + VerticalSpacing) + VerticalSpacing;

                foreach (var stateType in statesInLevel)
                {
                    var nodeWidth = nodeWidths[stateType];
                    nodeRects[stateType] = new Rect(currentX, y, nodeWidth, NodeHeight);
                    currentX += nodeWidth + HorizontalSpacing;
                }
            }

            var viewRect = new Rect(0, 0, Mathf.Max(maxWidth, position.width), Mathf.Max(totalHeight, position.height));
            var scrollViewRect = GUILayoutUtility.GetRect(0, 10000, 0, 10000);

            StateMachineDrawer.DrawGrid(scrollViewRect, _scrollPosition);

            _scrollPosition = GUI.BeginScrollView(scrollViewRect, _scrollPosition, viewRect, false, false, GUIStyle.none, GUIStyle.none);

            StateMachineDrawer.DrawLines(nodeRects, stateMap, activeStates);
            StateMachineDrawer.DrawNodes(nodeRects, activeStates, _labelStyle);
            
            GUI.EndScrollView();
        }

        private void OnSelectionChange()
        {
            _selectedGameObject = Selection.activeGameObject;
            if (_selectedGameObject != null)
            {
                var stateFactory = StateMachineData.GetStateFactory(_selectedGameObject);
                if (stateFactory != null)
                {
                    StateMachineData.PopulateStatesInEditMode(stateFactory);
                }
            }
            Repaint();
        }
    }
}