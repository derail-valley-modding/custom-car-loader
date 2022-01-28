using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCL_GameScripts;
using CCL_GameScripts.Attributes;
using CCL_GameScripts.CabControls;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader
{
    using SF = KeyValuePair<Type, MethodInfo>;

    public static class InitSpecManager
    {
        public static List<SF> StaticAfterInitMethods =
            new List<SF>();

        public static void OnStartup()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    if (m.GetCustomAttribute<InitSpecAfterInitAttribute>() is InitSpecAfterInitAttribute attribute)
                    {
#if DEBUG
                        Main.Log($"Static After Init Found: {t.Name}.{m.Name}, spec = {attribute.SpecType.Name}");
#endif
                        var parameters = m.GetParameters();
                        if ((parameters.Length == 2) && 
                            (parameters[0].ParameterType == attribute.SpecType) &&
                            (typeof(Component).IsAssignableFrom(parameters[1].ParameterType)))
                        {
                            // this is not a place of honor
                            StaticAfterInitMethods.Add(new SF(attribute.SpecType, m));
                        }
                        else
                        {
                            Main.Error($"Method {t.Name}.{m.Name} has wrong # or type of params for {attribute.SpecType.Name} finalizer");
                        }
                    }
                    else if (m.GetCustomAttribute<CopySpecAfterInitAttribute>() is CopySpecAfterInitAttribute copyAfterInit)
                    {
#if DEBUG
                        Main.Log($"Static After Copy Found: {t.Name}.{m.Name}, spec = {copyAfterInit.SpecType.Name}");
#endif
                        var parameters = m.GetParameters();
                        if ((parameters.Length == 2) &&
                            (parameters[0].ParameterType == copyAfterInit.SpecType) &&
                            (parameters[1].ParameterType == typeof(GameObject)))
                        {
                            // this is not a place of honor
                            StaticAfterInitMethods.Add(new SF(copyAfterInit.SpecType, m));
                        }
                        else
                        {
                            Main.Error($"Method {t.Name}.{m.Name} has wrong # or type of params for {copyAfterInit.SpecType.Name} finalizer");
                        }
                    }
                }
            }
        }

        public static void ExecuteStaticAfterInit(ComponentInitSpec spec, Component realComp)
        {
            foreach (var sf in StaticAfterInitMethods)
            {
                if (sf.Key.IsAssignableFrom(spec.GetType()))
                {
#if DEBUG
                    Main.Log($"StaticAfterInit {sf.Key.Name} ({spec.GetType().Name}) - {realComp.name}");
#endif
                    sf.Value.Invoke(null, new object[] { spec, realComp });
                }
            }
        }

        public static void ExecuteStaticAfterCopy(CopiedCabDevice spec, GameObject newObject)
        {
            foreach (var sf in StaticAfterInitMethods)
            {
                if (sf.Key.IsAssignableFrom(spec.GetType()))
                {
#if DEBUG
                    Main.Log($"StaticAfterCopy {sf.Key.Name} ({spec.GetType().Name}) - {newObject.name}");
#endif
                    sf.Value.Invoke(null, new object[] { spec, newObject });
                }
            }
        }

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
                MethodInfo finalizeMethod = finalizerType.GetMethod("FinalizeFromSpec");
                finalizeMethod.Invoke(realComp, new object[] { spec });
            }

            ExecuteStaticAfterInit(spec, realComp);

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

            ExecuteStaticAfterInit(spec, realComp);

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
                    FieldInfo targetField = AccessTools.Field(targetType, targetName);

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

    [AttributeUsage(AttributeTargets.Method)]
    public class InitSpecAfterInitAttribute : Attribute
    {
        public Type SpecType;

        public InitSpecAfterInitAttribute(Type specType)
        {
            SpecType = specType;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CopySpecAfterInitAttribute : Attribute
    {
        public Type SpecType;

        public CopySpecAfterInitAttribute(Type specType)
        {
            SpecType = specType;
        }
    }
}
