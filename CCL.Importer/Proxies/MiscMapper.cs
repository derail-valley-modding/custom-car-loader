using CCL.Types.Proxies;
using DV;

namespace CCL.Importer.Proxies
{
    [ProxyMap(typeof(InternalExternalSnapshotSwitcherProxy), typeof(InternalExternalSnapshotSwitcher))]
    internal class MiscMapper : ProxyReplacer { }
}
