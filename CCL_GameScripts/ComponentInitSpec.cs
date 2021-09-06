using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CCL_GameScripts.Attributes;

namespace CCL_GameScripts
{
    public abstract class ComponentInitSpec : MonoBehaviour, IProxyScript
    {
        public abstract string TargetTypeName { get; }
        public abstract bool IsOverrideSet( int index );
        protected abstract bool DestroyAfterCreation { get; }

        // use dependency injection so that this class can be used in Unity without references to Harmony or car loader
        public object CreateRealComponent( Func<string, Type> findTypeFunc, Action<string> logAction = null )
        {
            // *All* the reflection
            if( logAction == null ) logAction = Debug.LogWarning;
            Type sourceType = GetType();
            Type targetType = findTypeFunc(TargetTypeName);

            if( targetType == null )
            {
                logAction($"Target of spec {sourceType.Name} ({TargetTypeName}) not found!");
                return null;
            }

            var realComp = gameObject.AddComponent(targetType);
            if( realComp == null || !realComp )
            {
                logAction($"Failed to instantiate component of type {TargetTypeName} (from spec {sourceType.Name})");
                return null;
            }

            this.ApplyProxyFields(realComp, findTypeFunc, logAction);

            if( DestroyAfterCreation )
            {
                Destroy(this);
            }

            return realComp;
        }
    }
}
