using CCL.Types.Components;
using DV.Simulation.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.Components
{
    internal class CoupledAttachmentController : ARefreshableChildrenController<CoupledAttachmentTag>
    {
        private Dictionary<string, CoupledAttachmentTag>? tags = null!;

        public Dictionary<string, CoupledAttachmentTag> Tags
        {
            get
            {
                tags ??= entries.ToDictionary(k => k.Tag, v => v);
                return tags;
            }
        }

        public bool TryGetTag(string tag, out CoupledAttachmentTag comp)
        {
            return Tags.TryGetValue(tag, out comp);
        }
    }
}
