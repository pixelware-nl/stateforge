using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Stateforge.Editor
{
    public static class StateMachineData
    {
        public static void GetActiveStateBranch(object stateMachine, ISet<Type> activeStates)
        {
            activeStates.Clear();
            var currentState = GetCurrentState(stateMachine);

            if (currentState == null) return;

            var leafState = currentState;
            while (true) 
            {
                var childState = GetChildState(leafState);
                if (childState == null) break;
                leafState = childState;
            }

            var parentTraversalState = leafState;
            while (parentTraversalState != null)
            {
                activeStates.Add(parentTraversalState.GetType());
                parentTraversalState = GetParentState(parentTraversalState);
            }
        }
        
        public static object GetStateFactory(GameObject go)
        {
            MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
            return components.FirstOrDefault(component =>
            {
                if (component == false) return false;
                var type = component.GetType();
                while (type != null && type != typeof(MonoBehaviour) && type != typeof(object))
                {
                    if (type.IsGenericType && type.GetGenericTypeDefinition().FullName?.StartsWith("Stateforge.Runtime.StateFactory`1") == true)
                    {
                        return true;
                    }
                    type = type.BaseType;
                }
                return false;
            });
        }

        public static object GetStateMachine(object stateFactory)
        {
            var type = stateFactory.GetType();
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition().FullName?.StartsWith("Stateforge.Runtime.StateFactory`1") == true)
                {
                    var field = type.GetField("_stateMachine", BindingFlags.NonPublic | BindingFlags.Instance);
                    return field?.GetValue(stateFactory);
                }
                type = type.BaseType;
            }
            return null;
        }

        public static object GetCurrentState(object stateMachine)
        {
            var prop = stateMachine.GetType().GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(stateMachine);
        }

        public static object GetChildState(object state)
        {
            var prop = state.GetType().GetProperty("ChildState", BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(state);
        }

        public static object GetParentState(object state)
        {
            var prop = state.GetType().GetProperty("ParentState", BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(state);
        }

        public static List<Type> GetRootStates(object stateFactory)
        {
            var property = stateFactory.GetType().GetProperty("RootStates", BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(stateFactory) as List<Type> ?? new List<Type>();
        }

        public static IReadOnlyDictionary<Type, List<Type>> GetStateMap(object stateFactory)
        {
            var property = stateFactory.GetType().GetProperty("Map", BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(stateFactory) as IReadOnlyDictionary<Type, List<Type>> ?? new Dictionary<Type, List<Type>>();
        }
        
        public static void PopulateStatesInEditMode(object stateFactory)
        {
            var stateFactoryType = stateFactory.GetType();

            GetRootStates(stateFactory);
            GetStateMap(stateFactory);

            var statesField = stateFactoryType.GetField("_states", BindingFlags.NonPublic | BindingFlags.Instance);
            (statesField?.GetValue(stateFactory) as System.Collections.IDictionary)?.Clear();

            var setStatesMethod = stateFactoryType.GetMethod("SetStates", BindingFlags.NonPublic | BindingFlags.Instance);
            setStatesMethod?.Invoke(stateFactory, null);
        }
    }
}