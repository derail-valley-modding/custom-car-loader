namespace CCL.Types.Proxies
{
    public class LocoZoneBlockerProxy : ZoneBlockerProxy
    {
        public CabTeleportDestinationProxy cab;

        private void OnReset()
        {
            if (blockerObjectsParent == null)
            {
                blockerObjectsParent = gameObject;
            }
        }
    }
}
