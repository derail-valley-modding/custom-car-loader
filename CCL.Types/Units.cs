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
        public const float FromKilo = 1000f;

        // Length.
        public const float InchToMetre = 0.0254f;
        public const float FootToMetre = 0.3048f;
        public const float MileToMetre = 1609.344f;

        // Speed.
        public const float MStoKMH = 3.6f;
        public const float KMHtoMS = 1.0f / MStoKMH;
        public const float MPHtoKMH = 1.609344f;

        // Force.
        public const float LBFtoNewton = 4.4482216152605f;
        public const float KGFtoNewton = 9.806650f;

        // Pressure.
        public const float BarToPascal = 100000.0f;
        public const float PSItoPascal = 6894.757f;
        public const float PSItoBar = 0.06895f;
        public const float KGFCM2toPascal = 98066.5f;
        public const float ATMtoPascal = 101325.0f;

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

        /// <summary>
        /// Only for values below 1T.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string RoundAndFormat(float value, string? unit = null)
        {
            // 1 234 567 890 ->    1.23 G
            //   123 456 789 ->  123.45 M
            //    12 345 678 ->   12.34 M
            //     1 234 567 ->    1.23 M
            //       123 456 ->  123.45 k
            //        12 345 ->   12.34 k
            //         1 234 -> 1234.000

            unit ??= string.Empty;

            if (unit != "kg")
            {
                if (value >= 1000000000 || value <= -1000000000)
                {
                    return $"{value:0,,,.## G}{unit}";
                }

                if (value >= 1000000 || value <= -1000000)
                {
                    return $"{value:0,,.## M}{unit}";
                }

                if (value >= 10000 || value <= -10000)
                {
                    return $"{value:0,.## k}{unit}";
                }
            }

            // 3 decimal spaces looks ugly, so only keep them for very small values.
            return (value < 10 || value > -10) ? $"{value:0.###} {unit}" : $"{value:0.##} {unit}";
        }
    }
}
