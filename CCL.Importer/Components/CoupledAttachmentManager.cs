using CCL.Types.Components;
using DV.Simulation.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.Components
{
    internal class CoupledAttachmentManager : ARefreshableChildrenController<CoupledAttachmentTag>
    {
        public Dictionary<string, CoupledAttachmentTag> Tags = null!;

        private void Start()
        {
            Tags = entries.ToDictionary(k => k.Tag, v => v);
        }

        public bool TryGetTag(string tag, out CoupledAttachmentTag comp)
        {
            return Tags.TryGetValue(tag, out comp);
        }
    }
}
