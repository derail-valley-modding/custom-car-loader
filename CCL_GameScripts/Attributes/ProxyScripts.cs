using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL_GameScripts.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ProxyFieldAttribute : Attribute
    {
        public string TargetName;
        public int OverrideFlag;

        public ProxyFieldAttribute( string proxyField = null, int overrideFlag = -1 )
        {
            TargetName = proxyField;
            OverrideFlag = overrideFlag;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ProxyComponentAttribute : ProxyFieldAttribute
    {
        public string ComponentType;

        public ProxyComponentAttribute( string proxyField, string compName, int overrideFlag = -1 ) :
            base(proxyField, overrideFlag)
        {
            ComponentType = compName;
        }
    }

    public interface IProxyScript
    {
        string TargetTypeName { get; }
        bool IsOverrideSet( int index );
    }

    public static class ProxyScriptExtensions
    {
        public static void ApplyProxyFields( this IProxyScript source, object target, Func<string, Type> findTypeFunc, Action<string> logAction = null )
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach( FieldInfo sourceField in sourceType.GetFields() )
            {
                var proxies = sourceField.GetCustomAttributes().OfType<ProxyFieldAttribute>();
                foreach( var proxy in proxies )
                {
                    if( (proxy.OverrideFlag > 0) && !source.IsOverrideSet(proxy.OverrideFlag) )
                    {
                        // flag isn't set, so we skip this field
                        continue;
                    }

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
                            GameObject componentParent = sourceField.GetValue(source) as GameObject;
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
                            assignValue = sourceField.GetValue(source);
                        }

                        if( targetField.FieldType.IsAssignableFrom(assignValueType) )
                        {
                            targetField.SetValue(target, assignValue);
                        }
                        else if (targetField.FieldType.IsEnum && assignValueType.IsEnum)
                        {
                            object converted = Convert.ChangeType(assignValue, assignValueType.GetEnumUnderlyingType());
                            targetField.SetValue(target, converted);
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
        }
    }
}
