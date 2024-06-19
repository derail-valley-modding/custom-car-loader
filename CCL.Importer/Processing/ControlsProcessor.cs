using DV.CabControls;
using DV.KeyboardInput;
using System.ComponentModel.Composition;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    [RequiresStep(typeof(ProxyScriptProcessor))]
    [RequiresStep(typeof(ExternalInteractableProcessor))]
    internal class ControlsProcessor : ModelProcessorStep
    {
        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var item in context.Car.AllPrefabs)
            {
                var keyboardControls = item.GetComponentsInChildren<AKeyboardInput>();

                if (keyboardControls.Length > 0)
                {
                    if (!item.TryGetComponent(out InteractablesKeyboardControl controller))
                    {
                        controller = item.AddComponent<InteractablesKeyboardControl>();
                    }

                    controller.RefreshChildren();
                }
            }
        }
    }
}
