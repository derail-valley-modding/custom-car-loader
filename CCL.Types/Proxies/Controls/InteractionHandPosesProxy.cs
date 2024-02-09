using System;

namespace CCL.Types.Proxies.Controls
{
    [Serializable]
    public class InteractionHandPosesProxy
    {
        public DVHandPoseState nearTouchPose = DVHandPoseState.Generic;
        public DVHandPoseState touchPose = DVHandPoseState.Generic;
        public DVHandPoseState grabPose = DVHandPoseState.Generic;

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