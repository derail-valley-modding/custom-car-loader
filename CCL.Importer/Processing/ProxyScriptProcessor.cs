using CCL.Importer.Proxies;
using System.ComponentModel.Composition;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(GrabberProcessor))]
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
