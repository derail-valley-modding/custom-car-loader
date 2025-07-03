using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/Internal \u2215 External Snapshot Switcher Proxy")]
    public class InternalExternalSnapshotSwitcherProxy : MonoBehaviour
    {
        public CameraTriggerProxy trigger = null!;
    }

    [AddComponentMenu("CCL/Proxies/Internal \u2215 External Snapshot Switcher Doors And Windows Proxy")]
    public class InternalExternalSnapshotSwitcherDoorsAndWindowsProxy : InternalExternalSnapshotSwitcherProxy { }
}
