using CCL.Types.Proxies;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyMapper))]
    internal class ResourceContainerProxyMapper : IProxyMapper
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
