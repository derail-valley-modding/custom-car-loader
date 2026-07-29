using CCL.Importer.Components;
using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using UnityEngine;
using static DV.UI.ATutorialsMenuProvider;

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
            Help = "Calculates the spawn chance of a locomotive spawning at a station\nUse \"ALL\" in place of the station ID to show chances on all stations",
            Hint = "CCL.SpawnChance HB LocoS282B T",
            MinArgCount = 2, MaxArgCount = 3)]
        public static void CalculateSpawnChance(CommandArg[] args)
        {
            if (args.Length < 2)
            {
                Debug.LogError("Missing arguments!");
                return;
            }

            string id = args[1].String;

            if (!DV.Globals.G.Types.TryGetLivery(id, out var livery))
            {
                Debug.LogWarning($"Could not find livery '{id}'");
                return;
            }

            int sort = 0;

            if (args.Length > 2)
            {
                switch (args[2].String.ToLower())
                {
                    case "l":
                        sort = 1;
                        break;
                    case "t":
                        sort = 2;
                        break;
                    default:
                        Debug.LogWarning($"Unknown sort type '{args[2].String}', default to alphabetical");
                        break;
                }
            }

            var station = args[0].String.ToUpper();

            if (station == "ALL")
            {
                var sb = new StringBuilder($"Listing all spawn chances for livery '{id}':");
                var list = new List<(string S, float CL, float CT)>();
                var flag = true;

                foreach (var data in StationSpawnChanceData.Data)
                {
                    var chanceT = data.Value.GetChance(livery.parentType);
                    var chanceL = data.Value.GetChance(livery);

                    if (chanceT <= 0) continue;
                    if (chanceT != chanceL) flag = false;

                    list.Add((data.Key, chanceL, chanceT));
                }

                if (list.Count > 0)
                {
                    sb.Append(flag ? "\n Station  |   Type" : "\n Station  |  Livery  |   Type");
                    list.Sort();

                    switch (sort)
                    {
                        case 1:
                            list = list.OrderBy(x => -x.CL).ToList();
                            break;
                        case 2:
                            list = list.OrderBy(x => -x.CT).ToList();
                            break;
                        default:
                            break;
                    }

                    foreach (var (s, cl, ct) in list)
                    {
                        sb.Append(flag ? $"\n {s,-8} | {ct,8:P1}" : $"\n {s,-8} | {cl,8:P1} | {ct,8:P1}");
                    }

                    Debug.Log(sb.ToString());
                }
                else
                {
                    Debug.LogWarning($"Livery '{id}' does not spawn at any station");
                }
            }
            else if (StationSpawnChanceData.Data.TryGetValue(station, out var data))
            {
                var chanceT = data.GetChance(livery.parentType);
                var chanceL = data.GetChance(livery);

                if (chanceT > 0)
                {
                    Debug.Log($"Spawn chance for livery '{id}' at '{station}' is: {chanceL:P1} / {chanceT:P1}");
                }
                else
                {
                    Debug.LogWarning($"Livery '{id}' does not spawn at {station}");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find station '{station}'");
            }
        }
    }
}
