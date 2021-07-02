using System;
using System.Collections.Generic;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class CarPartNames
    {
        public const string BUFFERS_ROOT = "[buffers]";
        public const string BUFFER_PLATE_FRONT = "HookPlate_F";
        public const string BUFFER_PLATE_REAR = "HookPlate_R";
        public static readonly string[] BUFFER_FRONT_PADS = { "Buffer_FL", "Buffer_FR" };
        public static readonly string[] BUFFER_REAR_PADS = { "Buffer_RL", "Buffer_RR" };
        public const string BUFFER_CHAIN_RIG = "BuffersAndChainRig"; // same name front and rear

        public const string BOGIE_COLLIDERS = "[bogies]";
        public const string COLLIDERS_ROOT = "[colliders]";
        public const string COLLISION_ROOT = "[collision]";

        public const string COUPLER_FRONT = "[coupler front]";
        public const string COUPLER_REAR = "[coupler rear]";
    }

    public static class CarPartOffset
    {
        public static readonly Vector3 HOOK_PLATE_F = new Vector3(0, -0.07841396f, -0.3319998f);
        public static readonly Vector3 HOOK_PLATE_R = new Vector3(0, -0.07841396f, 0.3319998f);

        public static readonly Vector3 BUFFER_PAD_F = new Vector3(0, 0, 0.2150002f);
        public static readonly Vector3 BUFFER_PAD_R = new Vector3(0, 0, -0.2150002f);

        public static readonly Vector3 COUPLER_FRONT = new Vector3(0, 0, 0.2495f);
        public static readonly Vector3 COUPLER_REAR = new Vector3(0, 0, -0.2495f);
    }
}
