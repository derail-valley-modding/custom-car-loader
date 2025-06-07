namespace CCL.Types.Catalog
{
    public class TechDescriptionAttribute : StringAndSelectorFieldAttribute
    {
        private static string[] s_techDesc = new[]
        {
            "vc/techdesc/additional_control_interface/be2",
            "vc/techdesc/additional_control_interface/de2",
            "vc/techdesc/additional_control_interface/de6",
            "vc/techdesc/additional_control_interface/de6slug",
            "vc/techdesc/additional_control_interface/dh4",

            "vc/techdesc/air_brake/combined",
            "vc/techdesc/air_brake/non-self-lapping",
            "vc/techdesc/air_brake/nonselflap",
            "vc/techdesc/air_brake/self-lapping",
            "vc/techdesc/air_brake/selflap",
            "vc/techdesc/air_brake/valve",

            "vc/techdesc/cab/central",
            "vc/techdesc/cab/dual-facing",
            "vc/techdesc/cab/frontal",
            "vc/techdesc/cab/in-facing",
            "vc/techdesc/cab/living_quarters",
            "vc/techdesc/cab/out-facing",
            "vc/techdesc/cab/rear",
            "vc/techdesc/cab_closed/be2",
            "vc/techdesc/cab_closed/de2",
            "vc/techdesc/cab_closed/de6",
            "vc/techdesc/cab_closed/dh4",
            "vc/techdesc/cab_closed/dm1u",
            "vc/techdesc/cab_closed/dm3",
            "vc/techdesc/cab_open/s060",
            "vc/techdesc/cab_open/s282a",

            "vc/techdesc/cargo_compartment/dm1u",

            "vc/techdesc/control_interface/dpu",
            "vc/techdesc/control_interface/mu",
            "vc/techdesc/control_interface/remote",
            "vc/techdesc/control_interface/twin",

            "vc/techdesc/crew_quarters/caboose",
            "vc/techdesc/crew_quarters/dm1u",

            "vc/techdesc/delivery/by_dv_crew",
            "vc/techdesc/delivery/on_demand_delivery",

            "vc/techdesc/direct_brake/athermal",
            "vc/techdesc/direct_brake/caboose",
            "vc/techdesc/direct_brake/clasp",
            "vc/techdesc/direct_brake/drum",
            "vc/techdesc/direct_brake/handbrake",
            "vc/techdesc/direct_brake/handcar",
            "vc/techdesc/direct_brake/mechanical",

            "vc/techdesc/dyn_brake/combined",
            "vc/techdesc/dyn_brake/engine",
            "vc/techdesc/dyn_brake/hydraulic",
            "vc/techdesc/dyn_brake/regenerative",
            "vc/techdesc/dyn_brake/rheostatic",

            "vc/techdesc/dynamic_brake/de6",
            "vc/techdesc/dynamic_brake/de6slug",
            "vc/techdesc/dynamic_brake/dh4",
            "vc/techdesc/dynamic_brake/dm3",

            "vc/techdesc/el_power/112v",
            "vc/techdesc/el_power/25kv",
            "vc/techdesc/el_power/48v",
            "vc/techdesc/el_power/lead_acid",
            "vc/techdesc/el_power/pantograph",
            "vc/techdesc/el_trans/ac_tm",
            "vc/techdesc/el_trans/dc_tm",

            "vc/techdesc/electric_power/be2",

            "vc/techdesc/electric_trans/be2",
            "vc/techdesc/electric_trans/de2",
            "vc/techdesc/electric_trans/de6",
            "vc/techdesc/electric_trans/de6slug",

            "vc/techdesc/engine/2-inline",
            "vc/techdesc/engine/2-stroke",
            "vc/techdesc/engine/4-inline",
            "vc/techdesc/engine/4-stroke",
            "vc/techdesc/engine/6-inline",
            "vc/techdesc/engine/8-inline",
            "vc/techdesc/engine/de6",
            "vc/techdesc/engine/dh4",
            "vc/techdesc/engine/diesel",
            "vc/techdesc/engine/dm1u",
            "vc/techdesc/engine/dm3",
            "vc/techdesc/engine/gasoline",
            "vc/techdesc/engine/turbo",
            "vc/techdesc/engine/v10",
            "vc/techdesc/engine/v12",
            "vc/techdesc/engine/v16",
            "vc/techdesc/engine/v18",
            "vc/techdesc/engine/v2",
            "vc/techdesc/engine/v20",
            "vc/techdesc/engine/v4",
            "vc/techdesc/engine/v6",
            "vc/techdesc/engine/v8",

            "vc/techdesc/heat/active",
            "vc/techdesc/heat/forced_air_cooling",
            "vc/techdesc/heat/frontal_radiator",

            "vc/techdesc/heat_management/de2",
            "vc/techdesc/heat_management/de6",
            "vc/techdesc/heat_management/de6slug",
            "vc/techdesc/heat_management/dh4",
            "vc/techdesc/heat_management/dm3",

            "vc/techdesc/hydro_trans/automatic",
            "vc/techdesc/hydro_trans/dh4",
            "vc/techdesc/hydro_trans/torque_converter",

            "vc/techdesc/mech_trans/2-cylinder",
            "vc/techdesc/mech_trans/clutch",
            "vc/techdesc/mech_trans/dm1u",
            "vc/techdesc/mech_trans/dm3",
            "vc/techdesc/mech_trans/fluid_coupling",
            "vc/techdesc/mech_trans/handcar",
            "vc/techdesc/mech_trans/handcrank",
            "vc/techdesc/mech_trans/human",
            "vc/techdesc/mech_trans/manual_gearbox",
            "vc/techdesc/mech_trans/s060",
            "vc/techdesc/mech_trans/s282a",
            "vc/techdesc/mech_trans/speed2",
            "vc/techdesc/mech_trans/speed3",
            "vc/techdesc/mech_trans/speed4",
            "vc/techdesc/mech_trans/speed5",
            "vc/techdesc/mech_trans/speed6",
            "vc/techdesc/mech_trans/speed7",
            "vc/techdesc/mech_trans/speed8",
            "vc/techdesc/mech_trans/valve_gear",

            "vc/techdesc/mult/1",
            "vc/techdesc/mult/2",
            "vc/techdesc/mult/3",
            "vc/techdesc/mult/4",
            "vc/techdesc/mult/5",
            "vc/techdesc/mult/6",
            "vc/techdesc/mult/7",
            "vc/techdesc/mult/8",

            "vc/techdesc/passengers/ac",
            "vc/techdesc/passengers/bed",
            "vc/techdesc/passengers/el_door",
            "vc/techdesc/passengers/heating",
            "vc/techdesc/passengers/pneu_door",
            "vc/techdesc/passengers/seat",
            "vc/techdesc/passengers/storage",

            "vc/techdesc/special/career_manager",
            "vc/techdesc/special/charger",
            "vc/techdesc/special/chemical",
            "vc/techdesc/special/grinder",
            "vc/techdesc/special/signal_booster",
            "vc/techdesc/special/straightener",
            "vc/techdesc/special/train_part",

            "vc/techdesc/specialized_equipment1/caboose",
            "vc/techdesc/specialized_equipment2/caboose",

            "vc/techdesc/steam/s060",
            "vc/techdesc/steam/s282a",

            "vc/techdesc/steam_engine/10bar",
            "vc/techdesc/steam_engine/12bar",
            "vc/techdesc/steam_engine/14bar",
            "vc/techdesc/steam_engine/16bar",
            "vc/techdesc/steam_engine/8bar",
            "vc/techdesc/steam_engine/coal",
            "vc/techdesc/steam_engine/oil",
            "vc/techdesc/steam_engine/superheat",
            "vc/techdesc/steam_engine/wood",

            "vc/techdesc/unit/optional",
            "vc/techdesc/unit/optional_score",
            "vc/techdesc/unit/primary",
            "vc/techdesc/unit/primary_score",
            "vc/techdesc/unit/secondary",
            "vc/techdesc/unit/secondary_score",

            // CCL section.
            "ccl/vc/techdesc/special/articulation",

            "ccl/vc/techdesc/specialized_equipment/steam_mech_stoker",
        };

        public TechDescriptionAttribute(bool customAllowed = true) : base(s_techDesc, customAllowed) { }
    }

    public class TechTypeAttribute : StringAndSelectorFieldAttribute
    {
        private static string[] s_techType = new[]
        {
            "vc/techtype/additional_control_interface",
            "vc/techtype/air_brake",
            "vc/techtype/cab_closed",
            "vc/techtype/cab_open",
            "vc/techtype/cargo_compartment",
            "vc/techtype/crew_quarters",
            "vc/techtype/direct_brake",
            "vc/techtype/dynamic_brake",
            "vc/techtype/electric_power",
            "vc/techtype/electric_trans",
            "vc/techtype/engine",
            "vc/techtype/heat_management",
            "vc/techtype/hydro_trans",
            "vc/techtype/mech_trans",
            "vc/techtype/passenger_compartment",
            "vc/techtype/specialized_equipment",
            "vc/techtype/steam",

            "vc/techdesc/unit/primary_score",
            "vc/techdesc/unit/secondary_score",
            "vc/techdesc/unit/optional_score",
            "vc/techdesc/delivery/by_dv_crew",
        };

        public TechTypeAttribute(bool customAllowed = true) : base(s_techType, customAllowed) { }
    }
}
