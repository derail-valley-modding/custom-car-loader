using CCL.Importer.Proxies;
using CCL.Types.Proxies;
using DV;
using DV.CabControls.Spec;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ProxyScriptProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            // Standard proxy scripts
            if (context.Car.interiorPrefab)
            {
                ProxyWrangler.Instance.MapProxiesOnPrefab(context.Car.interiorPrefab);
            }

            if (context.Car.externalInteractablesPrefab)
            {
                ProxyWrangler.Instance.MapProxiesOnPrefab(context.Car.externalInteractablesPrefab);
            }

            ProxyWrangler.Instance.MapProxiesOnPrefab(context.Car.prefab);
        }
    }
}
