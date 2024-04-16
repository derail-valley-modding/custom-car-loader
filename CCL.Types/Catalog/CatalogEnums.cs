namespace CCL.Types.Catalog
{
    public enum VehicleType
    {
        Locomotive
    }

    public enum VehicleRole
    {
        None = 0,
        ShuntingLight = 100,
        ShuntingHeavy,
        HaulingLight = 200,
        HaulingHeavy
    }

    public enum ScoreType
    {
        None,
        Score,
        EffectPositive,
        EffectNegative,
        SharedEffectPositive,
        SharedEffectNegative,
    }
}
