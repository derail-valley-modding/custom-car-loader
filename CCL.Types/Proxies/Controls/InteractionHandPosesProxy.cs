namespace CCL.Types.Proxies.Controls
{
    public class InteractionHandPosesProxy
    {
        public InteractionHandPosesProxy.DVHandPoseState nearTouchPose;
        public InteractionHandPosesProxy.DVHandPoseState touchPose;
        public InteractionHandPosesProxy.DVHandPoseState grabPose;

        public enum DVHandPoseState
        {
            Generic = -1,
            Idle,
            Point,
            PreGrab,
            Grab,
            Pinch,
            Brick,
            Stick,
            Booklet,
            Lighter,
            Watch,
            Valve,
            Cup,
            WatchDown,
            Scanner
        }
    }
}