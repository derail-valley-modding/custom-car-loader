using CCL.Importer.Proxies;
using CCL.Types.Effects;
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
            var newFab = context.Car.prefab;
            Windows(newFab);

            // TODO: merge into ProxyReplacer workflow
            foreach (var item in newFab.GetComponentsInChildren<InternalExternalSnapshotSwitcherProxy>())
            {
                Mapper.MapComponent(item, out InternalExternalSnapshotSwitcher _);
            }

            foreach (var item in newFab.GetComponentsInChildren<PlayerDistanceGameObjectsDisablerProxy>())
            {
                Mapper.MapComponent(item, out PlayerDistanceGameObjectsDisabler _);
            }

            // KEEP LAST
            foreach (var item in newFab.GetComponentsInChildren<PlayerDistanceMultipleGameObjectsOptimizerProxy>())
            {
                Mapper.MapComponent(item, out PlayerDistanceMultipleGameObjectsOptimizer _);
            }

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

        private static void Windows(GameObject newFab)
        {
            WindowProxy[] windows = newFab.GetComponentsInChildren<WindowProxy>();
            DV.Rain.Window[] newWindows = new DV.Rain.Window[windows.Length];

            for (int i = 0; i < windows.Length; i++)
            {
                Mapper.MapComponent(windows[i], out DV.Rain.Window temp);
                newWindows[i] = temp;
            }

            for (int i = 0; i < windows.Length; i++)
            {
                newWindows[i].duplicates = windows[i].duplicates.Select(x => x.GetComponent<DV.Rain.Window>()).ToArray();
            }
        }
    }
}
