namespace CCL.Types
{
    public static class CarPartNames
    {
        // Buffers
        public const string BUFFERS_ROOT = "[buffers]";
        public const string BUFFER_PLATE_FRONT = "HookPlate_F";
        public const string BUFFER_PLATE_REAR = "HookPlate_R";
        public static readonly string[] BUFFER_STEMS = { "BufferStems", "CabooseExteriorBufferStems" };

        public const string BUFFER_PAD_FL = "Buffer_FL";
        public const string BUFFER_PAD_FR = "Buffer_FR";
        public const string BUFFER_PAD_RL = "Buffer_RL";
        public const string BUFFER_PAD_RR = "Buffer_RR";

        public static readonly string[] BUFFER_FRONT_PADS = { BUFFER_PAD_FL, BUFFER_PAD_FR };
        public static readonly string[] BUFFER_REAR_PADS = { BUFFER_PAD_RL, BUFFER_PAD_RR };

        public static readonly string[] BUFFER_ANCHORS = { "buffer anchor left", "buffer anchor right" };

        public const string BUFFER_CHAIN_REGULAR = "BuffersAndChainRig";
        public const string BUFFER_CHAIN_MU = "BuffersAndChainRigMU";
        public static readonly string[] BUFFER_CHAIN_RIGS = { BUFFER_CHAIN_REGULAR, BUFFER_CHAIN_MU }; // same name front and rear

        // Chains & Hoses
        public const string CHAIN_ROOT = "ChainCoupler";
        public const string HOSES_ROOT = "hoses";
        public const string AIR_HOSE = "CouplingHoseRig";
        public const string MU_CONNECTOR = "CouplingHoseRigMU";

        // Collider parts
        public const string COLLIDERS_ROOT = "[colliders]";
        public const string COLLISION_ROOT = "[collision]";
        public const string WALKABLE_COLLIDERS = "[walkable]";
        public const string ITEM_COLLIDERS = "[items]";
        public const string BOGIE_COLLIDERS = "[bogies]";
        public const string CAMERA_DAMP_COLLIDERS = "[camera dampening]";

        // Couplers
        public const string COUPLER_RIG_FRONT = "[coupler_rig_front]";
        public const string COUPLER_RIG_REAR = "[coupler_rig_rear]";
        public const string COUPLER_FRONT = "[coupler front]";
        public const string COUPLER_REAR = "[coupler rear]";

        // Bogies
        public const string BOGIE_FRONT = "BogieF";
        public const string BOGIE_REAR = "BogieR";
        public const string BOGIE_CAR = "bogie_car";
        public const string BOGIE_BRAKE_ROOT = "bogie2brakes";
        public const string BOGIE_BRAKE_PADS = "Bogie2BrakePads";
        public const string BOGIE_CONTACT_POINTS = "ContactPoints";
        public const string AXLE = "[axle]";

        // Info Plates
        public static readonly string[] INFO_PLATES = { "[car plate anchor1]", "[car plate anchor2]" };

        public const string CAB_TELEPORT_ROOT = "[cab]";
        
        // Particle Effects
        public const string PARTICLE_ROOT = "[particles]";
        public const string WHEEL_SPARKS = "[wheel sparks]";
        public const string SMOKE_EMITTER = "[smoke emitter]";
        public const string EXHAUST_SMOKE = "ExhaustEngineSmoke";
        public const string HIGH_TEMP_SMOKE = "HighTempEngineSmoke";
        public const string DAMAGED_SMOKE = "DamagedEngineSmoke";

        // Steam Loco Particles
        public const string CHIMNEY_STEAM = "steam_lit";
        public const string STEAM_CHUFF_L = "SteamChuff L";
        public const string STEAM_CHUFF_R = "SteamChuff R";
        public const string STEAM_CHUFF_NAME = "SteamChuffParticles";

        public const string STEAM_RELEASE_L = "SteamRelease L";
        public const string STEAM_RELEASE_R = "SteamRelease R";
        public const string STEAM_RELEASE_SAFETY = "SteamSafetyRelease";
        public const string STEAM_RELEASE_NAME = "SteamReleaseParticles";

        public const string STEAM_WHISTLE_M = "Whistle Mid";
        public const string STEAM_WHISTLE_F = "Whistle F";
        public const string STEAM_WHISTLE_NAME = "SteamWhistleParticles";

        // Firebox
        public const string FIREBOX_ROOT = "I firebox";
        public const string FIREBOX_COAL = "firebox_coal_pivot";
        public const string FIREBOX_FLAMES = "firebox_coal_pivot/fire";
        public const string FIREBOX_SPARKS = "sparks";
        public const string FIREBOX_MESH = "Firebox";

        // Caboose
        public const string CABOOSE_CAREER_MANAGER = "CareerManagerTrainInterior";
        public const string CABOOSE_REMOTE_CHARGER = "RemoteControllerCharger";
        public const string CABOOSE_REMOTE_ANTENNA = "RemoteControllerSignalBooster";

        // Interactables
        public const string HANDBRAKE_SMALL = "HandbrakeSmall";
        public const string BRAKE_CYL_RELEASE = "BrakeCylinderRelease";

        public const string DUMMY_HANDBRAKE_SMALL = "[brake small]";
        public const string DUMMY_BRAKE_RELEASE = "[brake release]";

        public static class Audio
        {
            public const string RAIN_MODULE = "RainModule";
            public const string RAIN_DUMMY_TRANSFORM = "[rain]";

            public const string WHEELS_MODULE = "WheelsModule";
            public const string WHEELS_FRONT = "WheelsFront";
            public const string WHEELS_REAR = "WheelsRear";
            public const string WHEELS_LAYERS_WHEELSLIP = "WheelslipLayers";
            public const string WHEELS_LAYERS_DAMAGED = "WheelDamagedLayers";
        }
    }
}
