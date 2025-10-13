using CCL.Importer.Components;
using CommandTerminal;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CCL.Importer
{
    internal static class Console
    {
        private static bool StringEqualsAny(string source, params string[] values) => values.Contains(source, StringComparer.OrdinalIgnoreCase);

        [RegisterCommand("CCL.AllLoadedTypes",
            Help = "Prints all custom car types added by CCL, optionally with liveries",
            Hint = "CCL.AllLoadedTypes L",
            MinArgCount = 0, MaxArgCount = 1)]
        public static void PrintLoadedTypes(CommandArg[] args)
        {
            if (CarManager.CustomCarTypes.Count == 0)
            {
                Debug.Log("No car types loaded!");
                return;
            }

            StringBuilder sb = new();
            bool liveries = args.Length > 0 && StringEqualsAny(args[0].String, "l", "liveries");

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

        [RegisterCommand("CCL.Trainset",
            Help = "Prints the trainset of the vehicle with the provided ID or, if no ID is provided, the current vehicle",
            Hint = "CCL.Trainset L-017",
            MinArgCount = 0, MaxArgCount = 1)]
        public static void PrintTrainset(CommandArg[] args)
        {
            TrainCar car;

            if (args.Length > 0)
            {
                if (!CarSpawner.Instance.TryGetTraincar(args[0].String, out car))
                {
                    Debug.LogWarning($"Could not find car with ID {args[0].String}");
                    return;
                }
            }
            else
            {
                car = PlayerManager.Car;

                if (car == null)
                {
                    Debug.LogWarning($"Could not find player's car");
                    return;
                }
            }

            switch (CarManager.TryGetInstancedTrainset(car, out var trainset))
            {
                case CarManager.TrainSetCompleteness.NotCCL:
                    Debug.LogWarning($"Car is not a CCL car, ignoring command");
                    return;
                case CarManager.TrainSetCompleteness.NotPartOfTrainset:
                    Debug.Log($"Car {car.ID} does not belong to a trainset");
                    return;
                case CarManager.TrainSetCompleteness.NotComplete:
                    Debug.Log($"The set for car {car.ID} is incomplete");
                    return;
                case CarManager.TrainSetCompleteness.Complete:
                    StringBuilder sb = new();

                    foreach (var instance in trainset)
                    {
                        sb.Append($"{instance.ID} ({instance.carLivery.id})");

                        if (instance.ID == car.ID)
                        {
                            sb.AppendLine(" [#]");
                        }
                        else
                        {
                            sb.AppendLine();
                        }
                    }

                    Debug.Log(sb.ToString());
                    return;
                default:
                    return;
            }
        }

        [RegisterCommand("CCL.ResourceCaches",
            Help = "Prints CCL's resource caches",
            MinArgCount = 0, MaxArgCount = 0)]
        public static void PrintResourceCaches(CommandArg[] args)
        {
            Processing.GrabberProcessor.PrintCaches();
            Debug.Log("Done!");
        }

        [RegisterCommand("CCL.SpawnChance",
            Help = "Calculates the spawn chance of a locomotive spawning at a station",
            Hint = "CCL.SpawnChance HB LocoS282B",
            MinArgCount = 2, MaxArgCount = 2)]
        public static void CalculateSpawnChance(CommandArg[] args)
        {
            if (args.Length < 2)
            {
                Debug.LogError("Missing arguments!");
                return;
            }

            if (StationSpawnChanceData.Data.TryGetValue(args[0].String, out var data))
            {
                if (DV.Globals.G.Types.TryGetLivery(args[1].String, out var livery))
                {
                    Debug.Log($"Spawn chance for livery '{args[1].String}' at '{args[0].String}' is {data.GetChance(livery):P1}");
                }
                else
                {
                    Debug.LogWarning($"Could not find livery '{args[1].String}'");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find station '{args[0].String}'");
            }
        }
    }
}
