using CCL.Types.Proxies.Ports;
using System.ComponentModel.Composition;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(GrabberProcessor))]
    [RequiresStep(typeof(VFXProcessor))]
    internal class ProxyScriptProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var prefab in context.Car.AllPrefabs)
            {
                FixEmptyPortIds(prefab);
                Mapper.ProcessConfigs(prefab);
            }
        }

        private void FixEmptyPortIds(GameObject prefab)
        {
            foreach (var component in prefab.GetComponentsInChildren<Component>())
            {
                const BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                foreach (var field in component.GetType().GetFields(ALL_INSTANCE))
                {
                    if (field.GetCustomAttribute<PortIdAttribute>() != null ||
                        field.GetCustomAttribute<FuseIdAttribute>() != null)
                    {
                        if (field.FieldType.IsArray)
                        {
                            if (field.GetValue(component) is not string[] array) continue;

                            for (int i = 0; i < array.Length; i++)
                            {
                                if (string.IsNullOrWhiteSpace(array[i]))
                                {
                                    CCLPlugin.LogVerbose($"Fix empty id {component.GetType().Name}.{field.Name}[{i}]");
                                    array[i] = null!;
                                }
                            }
                        }
                        else if ((field.FieldType == typeof(string)) && string.IsNullOrWhiteSpace((string?)field.GetValue(component)))
                        {
                            CCLPlugin.LogVerbose($"Fix empty id {component.GetType().Name}.{field.Name}");
                            field.SetValue(component, null);
                        }
                    }
                }
            }
        }
    }
}
