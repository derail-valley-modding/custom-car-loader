using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCL_GameScripts;
using CCL_GameScripts.Attributes;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class InitSpecManager
    {
        public static Component CreateRealComponent(ComponentInitSpec spec)
        {
            // *All* the reflection
            Type sourceType = spec.GetType();
            Type targetType = AccessTools.TypeByName(spec.TargetTypeName);

            if (targetType == null)
            {
                Main.Warning($"Target of spec {sourceType.Name} ({spec.TargetTypeName}) not found!");
                return null;
            }

            var realComp = spec.gameObject.AddComponent(targetType);
            if (realComp == null || !realComp)
            {
                Main.Warning($"Failed to instantiate component of type {spec.TargetTypeName} (from spec {sourceType.Name})");
                return null;
            }

            ApplyProxyFields(spec, realComp);

            // apply post-processing from real script side
            Type finalizerType = typeof(IInitSpecFinalizer<>).MakeGenericType(sourceType);
            if (finalizerType.IsAssignableFrom(targetType))
            {
                MethodInfo finalizeMethod = finalizerType.GetMethod("FinalizeFromSpec").MakeGenericMethod(sourceType);
                finalizeMethod.Invoke(realComp, new object[] { spec });
            }

            if (spec.DestroyAfterCreation)
            {
                UnityEngine.Object.Destroy(spec);
            }

            return realComp;
        }

        public static TReal CreateRealComponent<TSpec, TReal>(TSpec spec)
            where TSpec : ComponentInitSpec
            where TReal : Component
        {
            // *All* the reflection
            Type sourceType = spec.GetType();
            Type targetType = typeof(TReal);

            if (targetType.FullName != spec.TargetTypeName)
            {
                Main.Warning($"Target of spec {sourceType.Name} does not match desired real type {targetType.Name}");
                return null;
            }

            var realComp = spec.gameObject.AddComponent<TReal>();
            if (realComp == null || !realComp)
            {
                Main.Warning($"Failed to instantiate component of type {spec.TargetTypeName} (from spec {sourceType.Name})");
                return null;
            }

            ApplyProxyFields(spec, realComp);

            // apply post-processing from real script side
            if (realComp is IInitSpecFinalizer<TSpec> finalizer)
            {
                finalizer.FinalizeFromSpec(spec);
            }

            if (spec.DestroyAfterCreation)
            {
                UnityEngine.Object.Destroy(spec);
            }

            return realComp;
        }

        public static void ApplyProxyFields(IProxyScript source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (FieldInfo sourceField in sourceType.GetFields())
            {
                var proxies = sourceField.GetCustomAttributes().OfType<ProxyFieldAttribute>();
                foreach (var proxy in proxies)
                {
                    if ((proxy.OverrideFlag > 0) && !source.IsOverrideSet(proxy.OverrideFlag))
                    {
                        // flag isn't set, so we skip this field
                        continue;
                    }

                    string targetName = proxy.TargetName ?? sourceField.Name;
                    FieldInfo targetField = targetType.GetField(targetName);

                    if (targetField != null)
                    {
                        Type assignValueType;
                        object assignValue;

                        if (proxy is ProxyComponentAttribute proxyComp)
                        {
                            // create a component on the source field gameobject
                            if (!typeof(GameObject).Equals(sourceField.FieldType))
                            {
                                // can't create a component on non-gameobject
                                Main.Warning($"{sourceType.Name}.{sourceField.Name} is not of type GameObject, can't proxy a component on it!");
                                continue;
                            }

                            // get the root object from the source field
                            GameObject componentParent = sourceField.GetValue(source) as GameObject;
                            if (componentParent == null || !componentParent)
                            {
                                Main.Warning($"{sourceType.Name}.{sourceField.Name} is null, can't proxy a component on it!");
                                continue;
                            }

                            // create the actual component that will be sent to the target
                            assignValueType = AccessTools.TypeByName(proxyComp.ComponentType);
                            if ((assignValueType != null) && typeof(Component).IsAssignableFrom(assignValueType))
                            {
                                assignValue = componentParent.AddComponent(assignValueType);
                            }
                            else
                            {
                                Main.Warning($"{sourceType.Name}.{sourceField.Name} component type {proxyComp.ComponentType} not found, or not a component");
                                continue;
                            }
                        }
                        else
                        {
                            // direct assignment
                            assignValueType = sourceField.FieldType;
                            assignValue = sourceField.GetValue(source);
                        }

                        if (targetField.FieldType.IsAssignableFrom(assignValueType))
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
                            Main.Warning($"Proxy {targetType.Name}.{targetName} is not assignable from {sourceType.Name}.{sourceField.Name}");
                        }
                    }
                    else
                    {
                        Main.Warning($"From spec type {sourceType.Name} - target {targetName} not found on {targetType.Name}");
                    }
                }
            }
        }
    }

    public interface IInitSpecFinalizer<TSpec>
    {
        void FinalizeFromSpec(TSpec spec);
    }
}
