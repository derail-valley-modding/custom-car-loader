namespace CCL.Types
{
    public enum ExplosionPrefab
    {
        Boiler = 0,
        Electric,
        Hydraulic,
        Mechanical,
        TMOverspeed,

        Fire = 1000,

        DieselLocomotive = 2000,
    }

    public enum DVLayer
    {
        Default = 0,
        TransparentFX = 1,
        //Ignore Raycast = 2,
        //,
        Water = 4,
        UI = 5,
        //,
        //,
        Terrain = 8,
        Player = 9,
        Train_Big_Collider = 10,
        Train_Walkable = 11,
        Train_Interior = 12,
        Interactable = 13,
        Teleport_Destination = 14,
        Laser_Pointer_Target = 15,
        Camera_Dampening = 16,
        Culling_Sleepers = 17,
        Culling_Anchors = 18,
        Culling_Rails = 19,
        Render_Elements = 20,
        No_Teleport_Interaction = 21,
        Inventory = 22,
        Controller = 23,
        Hazmat = 24,
        PostProcessing = 25,
        Grabbed_Item = 26,
        World_Item = 27,
        Reflection_Probe_Only = 28,
    }
}
