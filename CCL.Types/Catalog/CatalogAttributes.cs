namespace CCL.Types.Catalog
{
    public class TechDescriptionAttribute : StringAndSelectorFieldAttribute
    {
        private static string[] s_techDesc = new[]
        {
            "vc/techdesc/mult/1",
            "vc/techdesc/mult/2",
            "vc/techdesc/mult/3",
            "vc/techdesc/mult/4",
            "vc/techdesc/mult/5",
            "vc/techdesc/mult/6",
            "vc/techdesc/mult/7",
            "vc/techdesc/mult/8",

            "vc/techdesc/engine/2-inline",
            "vc/techdesc/engine/v2",
            "vc/techdesc/engine/4-inline",
            "vc/techdesc/engine/v4",
            "vc/techdesc/engine/6-inline",
            "vc/techdesc/engine/v6",
            "vc/techdesc/engine/8-inline",
            "vc/techdesc/engine/v8",
            "vc/techdesc/engine/v10",
            "vc/techdesc/engine/v12",
            "vc/techdesc/engine/v16",
            "vc/techdesc/engine/v18",
            "vc/techdesc/engine/v20",
            "vc/techdesc/engine/turbo",
            "vc/techdesc/engine/2-stroke",
            "vc/techdesc/engine/4-stroke",
            "vc/techdesc/engine/gasoline",
            "vc/techdesc/engine/diesel",

            "vc/techdesc/steam_engine/8bar",
            "vc/techdesc/steam_engine/10bar",
            "vc/techdesc/steam_engine/12bar",
            "vc/techdesc/steam_engine/14bar",
            "vc/techdesc/steam_engine/16bar",
            "vc/techdesc/steam_engine/coal",
            "vc/techdesc/steam_engine/wood",
            "vc/techdesc/steam_engine/oil",
            "vc/techdesc/steam_engine/superheat",

            "vc/techdesc/el_power/lead_acid",
            "vc/techdesc/el_power/pantograph",
            "vc/techdesc/el_power/48v",
            "vc/techdesc/el_power/112v",
            "vc/techdesc/el_power/25kv",

            "vc/techdesc/el_trans/dc_tm",
            "vc/techdesc/el_trans/ac_tm",

            "vc/techdesc/hydro_trans/torque_converter",
            "vc/techdesc/hydro_trans/automatic",

            "vc/techdesc/mech_trans/fluid_coupling",
            "vc/techdesc/mech_trans/manual_gearbox",
            "vc/techdesc/mech_trans/clutch",
            "vc/techdesc/mech_trans/human",
            "vc/techdesc/mech_trans/speed2",
            "vc/techdesc/mech_trans/speed3",
            "vc/techdesc/mech_trans/speed4",
            "vc/techdesc/mech_trans/speed5",
            "vc/techdesc/mech_trans/speed6",
            "vc/techdesc/mech_trans/speed7",
            "vc/techdesc/mech_trans/speed8",
            "vc/techdesc/mech_trans/2-cylinder",
            "vc/techdesc/mech_trans/valve_gear",
            "vc/techdesc/mech_trans/handcrank",

            "vc/techdesc/heat/frontal_radiator",
            "vc/techdesc/heat/forced_air_cooling",
            "vc/techdesc/heat/active",

            "vc/techdesc/air_brake/self-lapping",
            "vc/techdesc/air_brake/non-self-lapping",
            "vc/techdesc/air_brake/valve",
            "vc/techdesc/air_brake/combined",

            "vc/techdesc/direct_brake/handbrake",
            "vc/techdesc/direct_brake/drum",
            "vc/techdesc/direct_brake/mechanical",
            "vc/techdesc/direct_brake/clasp",
            "vc/techdesc/direct_brake/athermal",

            "vc/techdesc/dyn_brake/rheostatic",
            "vc/techdesc/dyn_brake/regenerative",
            "vc/techdesc/dyn_brake/engine",
            "vc/techdesc/dyn_brake/hydraulic",
            "vc/techdesc/dyn_brake/combined",

            "vc/techdesc/cab/out-facing",
            "vc/techdesc/cab/in-facing",
            "vc/techdesc/cab/dual-facing",
            "vc/techdesc/cab/frontal",
            "vc/techdesc/cab/central",
            "vc/techdesc/cab/rear",
            "vc/techdesc/cab/living_quarters",

            "vc/techdesc/control_interface/twin",
            "vc/techdesc/control_interface/mu",
            "vc/techdesc/control_interface/dpu",
            "vc/techdesc/control_interface/remote",

            "vc/techdesc/passengers/storage",
            "vc/techdesc/passengers/bed",
            "vc/techdesc/passengers/seat",
            "vc/techdesc/passengers/pneu_door",
            "vc/techdesc/passengers/el_door",
            "vc/techdesc/passengers/ac",
            "vc/techdesc/passengers/heating",

            "vc/techdesc/special/straightener",
            "vc/techdesc/special/grinder",
            "vc/techdesc/special/chemical",
            "vc/techdesc/special/train_part",

            "vc/techdesc/special/signal_booster",
            "vc/techdesc/special/charger",
            "vc/techdesc/special/career_manager",

            "vc/techdesc/unit/primary",
            "vc/techdesc/unit/secondary",
            "vc/techdesc/unit/optional",

            "vc/techdesc/delivery/on_demand_delivery",
        };

        public TechDescriptionAttribute(bool customAllowed = true) : base(s_techDesc, customAllowed) { }
    }

    public class TechTypeAttribute : StringAndSelectorFieldAttribute
    {
        private static string[] s_techType = new[]
        {
            "vc/techtype/engine",
            "vc/techtype/steam",
            "vc/techtype/electric_power",
            "vc/techtype/electric_trans",
            "vc/techtype/hydro_trans",
            "vc/techtype/mech_trains",
            "vc/techtype/heat_management",
            "vc/techtype/air_brake",
            "vc/techtype/direct_brake",
            "vc/techtype/dynamic_brake",
            "vc/techtype/cab_closed",
            "vc/techtype/cab_open",
            "vc/techtype/additional_control_interface",
            "vc/techtype/passenger_compartment",
            "vc/techtype/specialized_equipment",
            "vc/techtype/crew_quarters",
            "vc/techtype/cargo_compartment",

            "vc/techdesc/unit/primary_score",
            "vc/techdesc/unit/secondary_score",
            "vc/techdesc/unit/optional_score",
            "vc/techdesc/delivery/by_dv_crew",
        };

        public TechTypeAttribute(bool customAllowed = true) : base(s_techType, customAllowed) { }
    }
}
