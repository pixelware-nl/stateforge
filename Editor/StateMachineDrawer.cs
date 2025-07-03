using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Stateforge.Editor
{
    public class StateMachineDrawer : EditorWindow
    {
        public static void DrawNodes(Dictionary<Type, Rect> nodeRects, ICollection<Type> activeStates, GUIStyle labelStyle)
        {
            foreach (var stateType in nodeRects.Keys)
            {
                var rect = nodeRects[stateType];
                var isActive = activeStates.Contains(stateType);

                if (isActive)
                {
                    var activeRect = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
                    EditorGUI.DrawRect(activeRect, Color.yellow);
                }
                
                EditorGUI.DrawRect(rect, Color.gray1);
                GUI.Label(rect, stateType.Name, labelStyle);
            }
        }
        
        public static void DrawGrid(Rect clipRect, Vector2 scrollPosition)
        {
            var minorGridColor = new Color(0, 0, 0, 0.08f);
            var majorGridColor = new Color(0, 0, 0, 0.12f);
            
            EditorGUI.DrawRect(clipRect, new Color(0, 0, 0, 0));

            Handles.BeginGUI();
            Handles.color = minorGridColor;

            float minorGridSize = 20f;
            float majorGridSize = 100f;

            for (float x = -scrollPosition.x % minorGridSize; x < clipRect.width; x += minorGridSize)
            {
                Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, clipRect.height, 0));
            }
            for (float y = -scrollPosition.y % minorGridSize; y < clipRect.height; y += minorGridSize)
            {
                Handles.DrawLine(new Vector3(0, y, 0), new Vector3(clipRect.width, y, 0));
            }
            
            Handles.color = majorGridColor;
            
            for (float x = -scrollPosition.x % majorGridSize; x < clipRect.width; x += majorGridSize)
            {
                Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, clipRect.height, 0));
            }
            for (float y = -scrollPosition.y % majorGridSize; y < clipRect.height; y += majorGridSize)
            {
                Handles.DrawLine(new Vector3(0, y, 0), new Vector3(clipRect.width, y, 0));
            }

            Handles.EndGUI();
        }

        public static void DrawLines(Dictionary<Type, Rect> nodeRects, IReadOnlyDictionary<Type, List<Type>> stateMap, ICollection<Type> activeStates)
        {
            Handles.BeginGUI();
            foreach (var parentState in stateMap.Keys)
            {
                if (!nodeRects.TryGetValue(parentState, out var parentRect)) continue;

                foreach (var childState in stateMap[parentState])
                {
                    if (!nodeRects.TryGetValue(childState, out var childRect)) continue;

                    var isConnectionActive = activeStates.Contains(parentState) && activeStates.Contains(childState);
                    Vector2 start = parentRect.center + new Vector2(0, parentRect.height / 2);
                    Vector2 end = childRect.center - new Vector2(0, childRect.height / 2);
                    Vector2 startTan = start + Vector2.up * 50;
                    Vector2 endTan = end - Vector2.up * 50;

                    var originalHandlesColor = Handles.color;
                    try
                    {
                        if (isConnectionActive)
                        {
                            DrawFlowingDashedBezier(start, end, startTan, endTan, Color.yellow, 4f);
                        }
                        else
                        {
                            Handles.color = Color.gray1;
                            Handles.DrawBezier(start, end, startTan, endTan, Handles.color, null, 2f);
                        }
                    }
                    finally
                    {
                        Handles.color = originalHandlesColor;
                    }
                }
            }
            Handles.EndGUI();
        }
        
        private static void DrawFlowingDashedBezier(Vector3 startPos, Vector3 endPos, Vector3 startTan, Vector3 endTan, Color color, float width)
        {
            const int segmentCount = 40;
            const float dashLength = 5f;
            const float gapLength = 5f;
            const float patternLength = dashLength + gapLength;
            const float lineAnimationSpeed = 30f;

            Vector3[] points = Handles.MakeBezierPoints(startPos, endPos, startTan, endTan, segmentCount + 1);

            float timeOffset = ((float)EditorApplication.timeSinceStartup * lineAnimationSpeed) % patternLength;
            float currentDistance = 0f;

            Handles.color = color;
            
            for (int i = 0; i < segmentCount; i++)
            {
                float segmentLength = Vector3.Distance(points[i], points[i + 1]);
                float startDistance = currentDistance;

                float currentPatternPos = (startDistance - timeOffset + patternLength) % patternLength;

                if (currentPatternPos < dashLength)
                {
                    float dashEnd = dashLength - currentPatternPos;
                    if (dashEnd >= segmentLength) 
                    {
                        Handles.DrawAAPolyLine(width, points[i], points[i + 1]);
                    }
                    else
                    {
                        float t = dashEnd / segmentLength;
                        Vector3 midPoint = Vector3.Lerp(points[i], points[i + 1], t);
                        Handles.DrawAAPolyLine(width, points[i], midPoint);
                    }
                }
                else
                {
                    float gapEnd = patternLength - currentPatternPos;
                    if (gapEnd < segmentLength)
                    {
                        float t = gapEnd / segmentLength;
                        Vector3 midPoint = Vector3.Lerp(points[i], points[i + 1], t);
                        Handles.DrawAAPolyLine(width, midPoint, points[i + 1]);
                    }
                }
                currentDistance += segmentLength;
            }
        }
    }
    
    
}