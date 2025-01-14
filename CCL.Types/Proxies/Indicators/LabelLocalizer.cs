using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [DisallowMultipleComponent]
    public class LabelLocalizer : MonoBehaviour
    {
        public int selectedDefaultIdx;
        [Delayed]
        public string key;

        public LabelModelType ModelType = LabelModelType.None;

        private void OnDrawGizmos()
        {
            if (ModelType == LabelModelType.None) return;

            if (ModelType.HasFlag(LabelModelType.Offset))
            {
                const float WIDTH = 0.065f;
                const float HEIGHT = 0.015f;
                const float DEPTH = 0.001f;

                const float STICK_WIDTH = 0.008f;
                const float STICK_HEIGHT = 0.007f;
                const float STICK_DEPTH = 0.016f;

                Gizmos.color = Color.cyan;
                GizmoUtil.DrawLocalPrism(transform, new Vector3(0, 0, DEPTH / 2), new Vector3(WIDTH, HEIGHT, DEPTH));
                GizmoUtil.DrawLocalPrism(transform, new Vector3(0, 0, DEPTH + STICK_DEPTH / 2), new Vector3(STICK_WIDTH, STICK_HEIGHT, STICK_DEPTH));
            }
            else if (ModelType.HasFlag(LabelModelType.Flush))
            {
                const float WIDTH = 0.065f;
                const float HEIGHT = 0.015f;
                const float BACK_RAY_LENGTH = 0.008f;

                Gizmos.color = Color.cyan;
                GizmoUtil.DrawLocalPrism(transform, Vector3.zero, new Vector3(WIDTH, HEIGHT, 0));
                GizmoUtil.DrawLocalRay(transform, Vector3.zero, Vector3.forward, BACK_RAY_LENGTH);
            }
        }

        public static readonly string[] DefaultOptions =
        {
            "CUSTOM",
            "car/air_pump",
            "car/alerter",
            "car/amperage",
            "car/ash_pan",
            "car/battery",
            "car/bell",
            "car/blower",
            "car/boiler_pressure",
            "car/br_electrics",
            "car/br_starter",
            "car/br_tm",
            "car/brake_cutout",
            "car/brake_cyl",
            "car/brake_pipe",
            "car/breakers",
            "car/cab_fan",
            "car/cab_heater",
            "car/cab_lights",
            "car/cab_orient",
            "car/chest_pressure",
            "car/coal",
            "car/cutoff",
            "car/cyl_cock",
            "car/damper",
            "car/dash_light",
            "car/dyn_brake",
            "car/dynamo",
            "car/engine_brake",
            "car/fire_door",
            //"car/fire_out",
            "car/fire_temp",
            "car/firebox",
            "car/fuel",
            "car/fuel_cutoff",
            "car/gear_light",
            "car/gearbox_a",
            "car/gearbox_b",
            "car/handbrake",
            "car/headlights",
            "car/headlights_1",
            "car/headlights_1_front",
            "car/headlights_1_rear",
            "car/headlights_2",
            "car/headlights_2_front",
            "car/headlights_2_rear",
            "car/headlights_dir",
            "car/headlights_front",
            "car/headlights_rear",
            "car/headlights_type_front",
            "car/headlights_type_rear",
            "car/horn",
            "car/ignite_fire",
            "car/ind_brake",
            "car/injector",
            "car/lubricator",
            "car/main_res",
            "car/oil",
            "car/oil_temp",
            "car/pantograph",
            //"car/plate_caboose",
            //"car/plate_cargo",
            //"car/plate_locomotive",
            //"car/plate_tender",
            //"car/plate_vehicle",
            "car/power",
            //"car/primer",
            "car/regulator",
            "car/release_cyl",
            "car/reverser",
            "car/rpm",
            "car/sand",
            "car/sander",
            "car/shovel",
            "car/speedometer",
            "car/starter",
            "car/steam",
            //"car/steam_dump",
            "car/tachometer",
            "car/throttle",
            "car/tm_offline",
            "car/tm_temp",
            "car/train_brake",
            "car/turbine_rpm",
            "car/voltage",
            "car/water",
            "car/water_dump",
            "car/wheelslip",
            "car/whistle",
            "car/wiper",
            "car/wipers_1",
            "car/wipers_2",
        };
    }

    public enum LabelModelType
    {
        None,

        CustomText = 1,

        // Label_01
        Offset = 2,
        OffsetDefaultText = 2,
        OffsetCustomText = 3,

        // Label_02
        Flush = 4,
        FlushDefaultText = 4,
        FlushCustomText = 5,
    }
}
