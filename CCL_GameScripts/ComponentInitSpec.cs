using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL_GameScripts
{
    public abstract class ComponentInitSpec : MonoBehaviour
    {
        protected abstract string TargetTypeName { get; }
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

            foreach( FieldInfo sourceField in sourceType.GetFields() )
            {
                var proxies = sourceField.GetCustomAttributes().OfType<ProxyFieldAttribute>();
                foreach( var proxy in proxies )
                {
                    string targetName = proxy.TargetName ?? sourceField.Name;
                    FieldInfo targetField = targetType.GetField(targetName);

                    if( targetField != null )
                    {
                        Type assignValueType;
                        object assignValue;

                        if( proxy is ProxyComponentAttribute proxyComp )
                        {
                            // create a component on the source field gameobject
                            if( !typeof(GameObject).Equals(sourceField.FieldType) )
                            {
                                // can't create a component on non-gameobject
                                logAction($"{sourceType.Name}.{sourceField.Name} is not of type GameObject, can't proxy a component on it!");
                                continue;
                            }

                            // get the root object from the source field
                            GameObject componentParent = sourceField.GetValue(this) as GameObject;
                            if( componentParent == null || !componentParent )
                            {
                                logAction($"{sourceType.Name}.{sourceField.Name} is null, can't proxy a component on it!");
                                continue;
                            }

                            // create the actual component that will be sent to the target
                            assignValueType = findTypeFunc(proxyComp.ComponentType);
                            if( (assignValueType != null) && typeof(Component).IsAssignableFrom(assignValueType) )
                            {
                                assignValue = componentParent.AddComponent(assignValueType);
                            }
                            else
                            {
                                logAction($"{sourceType.Name}.{sourceField.Name} component type {proxyComp.ComponentType} not found, or not a component");
                                continue;
                            }
                        }
                        else
                        {
                            // direct assignment
                            assignValueType = sourceField.FieldType;
                            assignValue = sourceField.GetValue(this);
                        }

                        if( targetField.FieldType.IsAssignableFrom(assignValueType) )
                        {
                            targetField.SetValue(realComp, assignValue);
                        }
                        else
                        {
                            logAction($"Proxy {targetType.Name}.{targetName} is not assignable from {sourceType.Name}.{sourceField.Name}");
                        }
                    }
                    else
                    {
                        logAction($"From spec type {sourceType.Name} - target {targetName} not found on {targetType.Name}");
                    }
                }
            }

            if( DestroyAfterCreation )
            {
                Destroy(this);
            }

            return realComp;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ProxyFieldAttribute : Attribute
    {
        public string TargetName;

        public ProxyFieldAttribute( string proxyField = null )
        {
            TargetName = proxyField;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ProxyComponentAttribute : ProxyFieldAttribute
    {
        public string ComponentType;

        public ProxyComponentAttribute( string proxyField, string compName ) :
            base(proxyField)
        {
            ComponentType = compName;
        }
    }
}
