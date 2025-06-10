using CCL.Importer.Components;
using CommandTerminal;
using System.Text;
using UnityEngine;

namespace CCL.Importer
{
    internal static class Console
    {
        [RegisterCommand("CCL.AllLoadedTypes",
            Help = "Prints all custom car types added by CCL, optionally with liveries",
            MinArgCount = 0, MaxArgCount = 1)]
        public static void PrintLoadedTypes(CommandArg[] args)
        {
            if (CarManager.CustomCarTypes.Count == 0)
            {
                Debug.Log("No car types loaded!");
                return;
            }

            StringBuilder sb = new();
            bool liveries = args.Length > 0 && args[0].String.ToLowerInvariant() == "l";

            foreach (var type in CarManager.CustomCarTypes)
            {
                sb.AppendLine(type.id);

                if (liveries)
                {
                    foreach (var livery in type.liveries)
                    {
                        sb.Append("    ");
                        sb.AppendLine(livery.id);
                    }
                }
            }

            Debug.Log(sb.ToString());
        }

        [RegisterCommand("CCL.LoadFailures",
            Help = "Prints all car load failures",
            MinArgCount = 0, MaxArgCount = 0)]
        public static void PrintLoadFailures(CommandArg[] args)
        {
            if (CarManager.LoadFailures.Count == 0)
            {
                Debug.Log("No failures detected!");
                return;
            }

            StringBuilder sb = new();

            foreach (var failure in CarManager.LoadFailures)
            {
                sb.AppendLine(failure);
            }

            Debug.Log(sb.ToString());
        }

        [RegisterCommand("CCL.AddPortPlotter",
            Help = "Adds or gets a SimPortPlotter to the vehicle with the provided ID or, if no ID is provided, the last used locomotive",
            Hint = "CCL.AddPortPlotter L-017",
            MinArgCount = 0, MaxArgCount = 1)]
        public static void AddPortPlotterToCar(CommandArg[] args)
        {
            if (args.Length > 0)
            {
                SimPortPlotterInternal.GetOrAddToCarId(args[0].String);
            }
            else
            {
                SimPortPlotterInternal.GetOrAddToLastLoco();
            }
        }
    }
}
