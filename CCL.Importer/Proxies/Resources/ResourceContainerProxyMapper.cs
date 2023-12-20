using CCL.Types.Proxies;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies.Resources
{
    [Export(typeof(IProxyReplacer))]
    internal class ResourceContainerProxyMapper : IProxyReplacer
    {
        public void ReplaceProxies(GameObject prefab)
        {
            foreach (var waterDefinition in prefab.GetComponentsInChildren<WaterContainerProxy>())
            {
                var waterContainer = waterDefinition.gameObject.AddComponent<WaterContainerDefinition>();
                Mapper.M.Map(waterDefinition, waterContainer, waterDefinition.GetType(), waterContainer.GetType());
                UnityEngine.Object.Destroy(waterDefinition);
            }
            //TODO: Remaining resource containers
        }
    }
}
