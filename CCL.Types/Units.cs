using UnityEngine;

namespace CCL.Types
{
    public enum MetricPrefix
    {
        [InspectorName("\u00B5")]
        Micro = -6,
        [InspectorName("m")]
        Mili = -3,
        [InspectorName("c")]
        Centi = -2,
        [InspectorName("d")]
        Deci = -1,
        [InspectorName("(None)")]
        None = 0,
        [InspectorName("da")]
        Deca = 1,
        [InspectorName("h")]
        Hexa = 2,
        [InspectorName("k")]
        Kilo = 3,
        [InspectorName("M")]
        Mega = 6,
        [InspectorName("G")]
        Giga = 9,
    }

    public class Units
    {
        public const float Gravity = 9.81f;

        public const float ToKilo = 0.001f;
        public const float MStoKMH = 3.6f;
        public const float KMHtoMS = 1.0f / MStoKMH;

        public const float BarToPascal = 100000.0f;

        public static float FromPrefix(float value, MetricPrefix prefix)
        {
            return prefix switch
            {
                MetricPrefix.Micro => value * 0.000001f,
                MetricPrefix.Mili => value * 0.001f,
                MetricPrefix.Centi => value * 0.01f,
                MetricPrefix.Deci => value * 0.1f,
                MetricPrefix.Deca => value * 10.0f,
                MetricPrefix.Hexa => value * 100.0f,
                MetricPrefix.Kilo => value * 1000.0f,
                MetricPrefix.Mega => value * 1000000.0f,
                MetricPrefix.Giga => value * 1000000000.0f,
                _ => value,
            };
        }

        public static float ToPrefix(float value, MetricPrefix prefix)
        {
            return prefix switch
            {
                MetricPrefix.Micro => value * 1000000.0f,
                MetricPrefix.Mili => value * 1000.0f,
                MetricPrefix.Centi => value * 100.0f,
                MetricPrefix.Deci => value * 10.0f,
                MetricPrefix.Deca => value * 0.1f,
                MetricPrefix.Hexa => value * 0.01f,
                MetricPrefix.Kilo => value * 0.001f,
                MetricPrefix.Mega => value * 0.000001f,
                MetricPrefix.Giga => value * 0.000000001f,
                _ => value,
            };
        }
    }
}
