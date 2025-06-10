namespace CCL.Types
{
    public static class CarPartNames
    {
        public static class Buffers
        {
            public const string ROOT = "[buffers]";
            public const string PLATE_FRONT = "HookPlate_F";
            public const string PLATE_REAR = "HookPlate_R";
            public static readonly string[] STEMS = { "BufferStems", "CabooseExteriorBufferStems" };

            public const string PAD_FL = "Buffer_FL";
            public const string PAD_FR = "Buffer_FR";
            public const string PAD_RL = "Buffer_RL";
            public const string PAD_RR = "Buffer_RR";

            public static readonly string[] FRONT_PADS = { PAD_FL, PAD_FR };
            public static readonly string[] REAR_PADS = { PAD_RL, PAD_RR };

            public static readonly string[] ANCHORS = { "buffer anchor left", "buffer anchor right" };

            public const string CHAIN_REGULAR = "BuffersAndChainRig";
            public const string CHAIN_MU = "BuffersAndChainRigMU";
            public static readonly string[] CHAIN_RIGS = { CHAIN_REGULAR, CHAIN_MU }; // same name front and rear
        }

        public static class Couplers
        {
            public const string CHAIN_ROOT = "ChainCoupler";
            public const string HOSES_ROOT = "hoses";
            public const string AIR_HOSE = "CouplingHoseRig";
            public const string MU_CONNECTOR = "CouplingHoseRigMU";

            public const string RIG_FRONT = "[coupler_rig_front]";
            public const string RIG_REAR = "[coupler_rig_rear]";
            public const string COUPLER_FRONT = "[coupler front]";
            public const string COUPLER_REAR = "[coupler rear]";
        }

        public static class Colliders
        {
            public const string ROOT = "[colliders]";
            public const string COLLISION = "[collision]";
            public const string WALKABLE = "[walkable]";
            public const string ITEMS = "[items]";
            public const string BOGIES = "[bogies]";
            public const string CAMERA_DAMPENING = "[camera dampening]";
            public const string FALL_SAFETY = "[fall safety]";
        }

        public static class Bogies
        {
            public const string FRONT = "BogieF";
            public const string REAR = "BogieR";
            public const string BOGIE_CAR = "bogie_car";
            public const string BRAKE_ROOT = "bogie2brakes";
            public const string BRAKE_PADS = "Bogie2BrakePads";
            public const string CONTACT_POINTS = "ContactPoints";
            public const string AXLE = "[axle]";
        }

        // Info Plates
        public static readonly string[] INFO_PLATES = { "[car plate anchor1]", "[car plate anchor2]" };

        public static class Cab
        {
            public const string TELEPORT_ROOT = "[cab]";
            public const string HIGHLIGHT_GLOW = "[cab]/CabHighlightGlow/Quad";
        }

        public static class Particles
        {
            public const string ROOT = "[particles]";
            public const string WHEEL_SPARKS = "[wheel sparks]";
            public const string SMOKE_EMITTER = "[smoke emitter]";
            public const string EXHAUST_SMOKE = "ExhaustEngineSmoke";
            public const string HIGH_TEMP_SMOKE = "HighTempEngineSmoke";
            public const string DAMAGED_SMOKE = "DamagedEngineSmoke";

            public static class Steam
            {
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
            }
        }

        public static class Firebox
        {
            public const string ROOT = "I firebox";
            public const string COAL = "firebox_coal_pivot";
            public const string FLAMES = "firebox_coal_pivot/fire";
            public const string SPARKS = "sparks";
            public const string MESH = "Firebox";
        }

        public static class Caboose
        {
            public const string CAREER_MANAGER = "CareerManagerTrainInterior";
            public const string REMOTE_CHARGER = "RemoteControllerCharger";
            public const string REMOTE_ANTENNA = "RemoteControllerSignalBooster";
        }

        public static class Interactables
        {
            public const string HANDBRAKE_SMALL = "HandbrakeSmall";
            public const string BRAKE_CYL_RELEASE = "BrakeCylinderRelease";
            public const string HANDBRAKE_HOPPER = "HandbrakeLargeEU";
            public const string HANDBRAKE_S060 = "Back/Handbrake";
            public const string HANDBRAKE_S282 = "HandbrakeEU3-TenderOverride";
            public const string HANDBRAKE_DE2 = "Handbrake";
            public const string HANDBRAKE_DM3 = "Handbrake_04";
            public const string HANDBRAKE_DH4 = "RearCluster/HandbrakeLargeEU";
            public const string HANDBRAKE_MICROSHUNTER = "HandbrakeLargeEU";
            public const string HANDBRAKE_DM1U = "RightCluster/HandbrakeEU3";

            public const string DUMMY_HANDBRAKE_SMALL = "[brake small]";
            public const string DUMMY_BRAKE_RELEASE = "[brake release]";
            public const string DUMMY_HANDBRAKE_LARGE = "[brake large]";
            public const string DUMMY_HANDBRAKE_S060 = "[brake s060]";
            public const string DUMMY_HANDBRAKE_S282 = "[brake s282]";
            public const string DUMMY_HANDBRAKE_DE2 = "[brake de2]";
            public const string DUMMY_HANDBRAKE_DM3 = "[brake dm3]";
            public const string DUMMY_HANDBRAKE_DH4 = "[brake dh4]";
            public const string DUMMY_HANDBRAKE_MICROSHUNTER = "[brake microshunter]";
            public const string DUMMY_HANDBRAKE_DM1U = "[brake dm1u]";
        }

        public static class FuelPorts
        {
            public const string FUEL_CAP_ROOT = "[fuel tank cap]";
            public const string FUEL_CAP_DE2 = "FuelTankCap";

            public const string CHARGE_PORT_ROOT = "[battery charge port]";
            public const string CHARGE_PORT_BE2 = "BatteryChargePort";

            public const string DUMMY_FUEL_CAP_DE2 = "[fuel de2]";
            public const string DUMMY_CHARGE_PORT_BE2 = "[charge be2]";
        }

        public static class Audio
        {
            public const string RAIN_MODULE = "RainModule";
            public const string RAIN_DUMMY_TRANSFORM = "[rain]";
            
            public const string WHEELS_MODULE = "WheelsModule";
            public const string WHEELS_FRONT = "WheelsFront";
            public const string WHEELS_REAR = "WheelsRear";
            public const string WHEELS_LAYERS_WHEELSLIP = "WheelslipLayers";
            public const string WHEELS_LAYERS_DAMAGED = "WheelDamagedLayers";

            public const string BRAKE_MODULE = "BrakeModule";
            public const string CYLINDER_EXHAUST = "Brake/CylinderExhaust_Layered";
            public const string AIRFLOW = "Airflow/AirflowLayers";
        }

        public const string CENTER_OF_MASS = "[center of mass]";
        public const string LICENSE_BLOCKER = "LocoLicenseBlocker";
        public const string GRABPASS_HACK = "[grabpass hack]";
    }
}
