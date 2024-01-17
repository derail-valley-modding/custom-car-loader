using System.ComponentModel.Composition;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ProxyScriptProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var prefab in context.Car.AllPrefabs)
            {
                Mapper.ProcessConfigs(prefab);
            }
        }
    }
}
