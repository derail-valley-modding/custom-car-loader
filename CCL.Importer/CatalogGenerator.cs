using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Types.Catalog;
using CCL.Types.Catalog.Diagram;
using DV.Booklets;
using DV.Localization;
using DV.RenderTextureSystem.BookletRender;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCL.Importer
{
    internal static class CatalogGenerator
    {
        public static Dictionary<CCL_CarVariant, CatalogPage> PageInfos = new();
        public static List<VehicleCatalogPageTemplatePaper> NewCatalogPages = new();
        public static Dictionary<string, Dictionary<string, float>> SpawnChances = new();
        private static VehicleCatalogPageTemplatePaper PageDE2 { get; set; } = null!;
        private static Transform TransformDE2 { get; set; } = null!;

        public static void GeneratePages(VehicleCatalogRender original)
        {
            PageDE2 = original.vehiclePages[0];
            TransformDE2 = PageDE2.transform;

            NewCatalogPages.Clear();

            foreach (var item in PageInfos)
            {
                var result = ProcessPage(item.Key, item.Value);

                if (result == null) continue;

                result.transform.parent = original.transform;
                result.transform.localPosition = Vector3.zero;
                result.transform.localRotation = Quaternion.identity;
                NewCatalogPages.Add(result);
            }

            ClearCache();
        }

        private static VehicleCatalogPageTemplatePaper? ProcessPage(CCL_CarVariant livery, CatalogPage layout)
        {
            CCLPlugin.Log($"Generating catalog page '{layout.PageName}'...");

            var page = ModelProcessor.CreateModifiablePrefab(TransformDE2.gameObject).transform;
            page.gameObject.SetActive(true);
            var paper = page.GetComponent<VehicleCatalogPageTemplatePaper>();
            paper.carLivery = livery;

            UpdateReferences(page, paper, livery);
            ProcessHeader(page, livery, layout);
            ProcessRoles(page, layout);
            ProcessDiagram(page, paper, layout);
            ProcessTechList(page, layout);
            ProcessAllScoreLists(page, layout);

            return paper;
        }

        private static void UpdateReferences(Transform page, VehicleCatalogPageTemplatePaper paper, CCL_CarVariant livery)
        {
            if (!livery.UnlockableAsWorkTrain) return;

            paper.garage.icon = Paths.GetImage(page, Paths.GarageLockIcon);
            paper.garage.price = Paths.GetTextUGUI(page, Paths.GaragePrice);

            paper.summon.icon = Paths.GetImage(page, Paths.SummonIcon);
            paper.summon.price = Paths.GetTextUGUI(page, Paths.SummonPrice);
        }

        private static void ProcessHeader(Transform page, CCL_CarVariant livery, CatalogPage layout)
        {
            Paths.GetImage(page, Paths.PageColor).color = layout.HeaderColour;
            Paths.GetText(page, Paths.PageName).gameObject.AddComponent<Localize>().SetKeyAndUpdate(livery.CatalogPageNameTranslationKey);
            Paths.GetText(page, Paths.Units).SetTextAndUpdate(layout.ConsistUnits);

            var nick = Paths.GetLocalize(page, Paths.Nickname);

            if (layout.HasNickname)
            {
                nick.SetKeyAndUpdate(livery.CatalogNicknameTranslationKey);
                nick.gameObject.SetActive(true);
            }
            else
            {
                nick.gameObject.SetActive(false);
            }

            Paths.GetImage(page, Paths.Icon).sprite = livery.icon;

            ProcessSpawnLocations(page.Find(Paths.Locations), livery);

            if (!string.IsNullOrEmpty(layout.ProductionYears))
            {
                var text = Paths.GetText(page, Paths.ProductionYears);
                text.gameObject.SetActive(true);
                text.SetTextAndUpdate(layout.ProductionYears);
            }
            else
            {
                page.Find(Paths.ProductionYears).gameObject.SetActive(false);
            }

            page.Find(Paths.GarageIcon).gameObject.SetActive(livery.UnlockableAsWorkTrain);
            page.Find(Paths.Summonable).gameObject.SetActive(livery.UnlockableAsWorkTrain);

            #region Load Ratings

            if (layout.ShowLoadRatings)
            {
                page.Find(Paths.LoadRatingText).gameObject.SetActive(true);

                ProcessLoadRating(page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconFlat)), layout.LoadFlat);
                ProcessLoadRating(page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconIncline)), layout.LoadIncline);
                ProcessLoadRating(page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconInclineWet)), layout.LoadInclineWet);
            }
            else
            {
                page.Find(Paths.LoadRatingText).gameObject.SetActive(false);
                page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconFlat)).gameObject.SetActive(false);
                page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconIncline)).gameObject.SetActive(false);
                page.Find(Paths.Combine(Paths.LoadRating, Paths.LoadRatings.IconInclineWet)).gameObject.SetActive(false);
            }

            #endregion
        }

        private static void ProcessSpawnLocations(Transform locations, CCL_CarVariant livery)
        {
            LocoSpawnRateRenderer spawner = locations.gameObject.GetComponent<LocoSpawnRateRenderer>();

            // If unlocked by a garage, don't show spawn bar.
            if (livery.UnlockableAsWorkTrain)
            {
                Object.DestroyImmediate(spawner);
                // Move icons up to avoid blank space.
                // Which doesn't work who knows why.
                var rect = (RectTransform)locations.GetChild(0);
                rect.sizeDelta = Vector2.zero;
                Object.DestroyImmediate(rect.GetComponent<Image>());
                rect.localPosition += new Vector3(0, -34, 0);
                return;
            }

            // Cache the children so they can be deleted without causing loop issues.
            // These are the original spawn chance icons.
            List<GameObject> children = new();

            for (int i = 0; i < spawner.transform.childCount; i++)
            {
                children.Add(spawner.transform.GetChild(i).gameObject);
            }

            foreach (var item in children)
            {
                if (item.name != "YearPriceContainer")
                {
                    Object.DestroyImmediate(item);
                }
            }

            var og = PageDE2.GetComponentInChildren<LocoSpawnRateRenderer>();
            spawner.loco = livery.parentType;
            spawner.spawnRateIndicatorPrefab = og.spawnRateIndicatorPrefab;
            spawner.stationData = og.stationData;

            foreach (var item in spawner.stationData.stationsData)
            {
                if (SpawnChances.TryGetValue(item.id, out var chances))
                {
                    // Get the chance for this ID.
                    // Need to invert since it's storing the chance to NOT spawn. Math.
                    if (chances.TryGetValue(livery.parentType.id, out var chance))
                    {
                        item.locoSpawnChances.Add(new(livery.parentType, chance));
                    }
                }
            }
        }

        private static void ProcessLoadRating(Transform root, LoadRating rating)
        {
            var bad = root.Find(Paths.LoadRatings.Bad).gameObject;
            var medium = root.Find(Paths.LoadRatings.Medium).gameObject;
            var good = root.Find(Paths.LoadRatings.Good).gameObject;

            switch (rating.Rating)
            {
                case TonnageRating.Good:
                    bad.SetActive(false);
                    medium.SetActive(false);
                    good.SetActive(true);
                    break;
                case TonnageRating.Medium:
                    bad.SetActive(false);
                    medium.SetActive(true);
                    good.SetActive(false);
                    break;
                case TonnageRating.Bad:
                    bad.SetActive(true);
                    medium.SetActive(false);
                    good.SetActive(false);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(rating.Rating));
            }

            Paths.GetLocalizedNumber(root, Paths.LoadRatings.Tonnage).value = rating.Tonnage;
        }

        private static void ProcessRoles(Transform page, CatalogPage layout)
        {
            Paths.GetLocalize(page, Paths.VehicleType).SetKeyAndUpdate(GetVehicleTypeKey(layout.Type));

            var role1 = Paths.GetLocalize(page, Paths.VehicleRole1);
            var role2 = Paths.GetLocalize(page, Paths.VehicleRole2);

            if (layout.Role1 != VehicleRole.None)
            {
                role1.gameObject.SetActive(true);
                role1.SetKeyAndUpdate(GetVehicleRoleKey(layout.Role1));
            }
            else
            {
                role1.gameObject.SetActive(false);
            }

            if (layout.Role2 != VehicleRole.None)
            {
                role2.gameObject.SetActive(true);
                role2.SetKeyAndUpdate(GetVehicleRoleKey(layout.Role2));
            }
            else
            {
                role2.gameObject.SetActive(false);
            }
        }

        #region Diagram

        private static void ProcessDiagram(Transform page, VehicleCatalogPageTemplatePaper paper, CatalogPage layout)
        {
            var parent = page.Find(Paths.DiagramParent);

            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }

            // If there's no layout, don't draw a diagram at all.
            if (layout.DiagramLayout == null)
            {
                return;
            }

            var diagram = parent.Find(Paths.Diagrams.Generic);
            diagram.gameObject.SetActive(true);

            ProcessDiagramExtras(diagram, paper, layout.DiagramExtras);
            ProcessDiagramIcons(diagram, layout.DiagramLayout.transform);
        }

        private static void ProcessDiagramExtras(Transform root, VehicleCatalogPageTemplatePaper paper, VehicleDiagramExtras extras)
        {
            if (extras.IsThinVehicle)
            {
                root.Find(Paths.Diagrams.TallVehicle).gameObject.SetActive(false);
                root.Find(Paths.Diagrams.ThinVehicle).gameObject.SetActive(true);
            }

            root.Find(Paths.Diagrams.BufferL).gameObject.SetActive(extras.HasFrontBumper);
            root.Find(Paths.Diagrams.BufferR).gameObject.SetActive(extras.HasRearBumper);

            Paths.GetText(root, Paths.Diagrams.XText).SetTextAndUpdate($"{extras.Length} mm");
            Paths.GetText(root, Paths.Diagrams.YText).SetTextAndUpdate($"{extras.Height} mm");
            Paths.GetText(root, Paths.Diagrams.ZText).SetTextAndUpdate($"{extras.Width} mm");

            paper.price = Paths.GetTextUGUI(root, Paths.Diagrams.Price);
            paper.massEmpty = Paths.GetTextUGUI(root, Paths.Diagrams.MassEmpty);
            paper.massFull = Paths.GetTextUGUI(root, Paths.Diagrams.MassFull);
        }

        private static void ProcessDiagramIcons(Transform root, Transform layout)
        {
            layout = Object.Instantiate(layout, root);
            layout.localPosition = new Vector3(DiagramComponent.WIDTH, DiagramComponent.HEIGHT, 0);

            foreach (var bogie in layout.GetComponentsInChildren<BogieLayout>())
            {
                if (bogie.Wheels.Length == 0)
                {
                    continue;
                }

                int length = bogie.Wheels.Length;
                var middleSpace = length % 2 == 0;
                var position = new Vector3(-BogieLayout.RADIUS * (length - 1) - (middleSpace ? BogieLayout.MIDDLE_SPACE / 2 : 0), 0, 0);
                List<Vector3> connectors = new();

                for (int i = 0; i < length; i++)
                {
                    Transform wheel;
                    if (bogie.Wheels[i])
                    {
                        wheel = Object.Instantiate(Icons.PoweredWheel, bogie.transform);
                    }
                    else
                    {
                        wheel = Object.Instantiate(Icons.Wheel, bogie.transform);
                    }

                    wheel.localPosition = position;
                    wheel.localRotation = Quaternion.identity;

                    position += Vector3.right * BogieLayout.RADIUS;
                    connectors.Add(position);
                    position += Vector3.right * BogieLayout.RADIUS;

                    if (middleSpace && i + 1 == length / 2)
                    {
                        position += Vector3.right * BogieLayout.MIDDLE_SPACE;
                    }
                }

                if (bogie.Pivots)
                {
                    Transform pivot;

                    if (middleSpace)
                    {
                        pivot = Object.Instantiate(Icons.PivotLong, bogie.transform);
                        pivot.localPosition = new Vector3(0, 15, 0);
                    }
                    else
                    {
                        pivot = Object.Instantiate(Icons.PivotShort, bogie.transform);
                        pivot.localPosition = new Vector3(0, 22, 0);
                    }

                    pivot.localRotation = Quaternion.identity;
                }

                connectors.RemoveAt(length - 1);

                foreach (var connector in connectors)
                {
                    var frame = Object.Instantiate(Icons.Bogie, bogie.transform);
                    frame.localPosition = connector;
                    frame.localRotation = Quaternion.identity;
                    frame.sizeDelta = new Vector2(Mathf.Abs(position.x) < 1 ? BogieLayout.MIDDLE_SPACE + 10 : 10, 10);
                }
            }

            foreach (var tech in layout.GetComponentsInChildren<TechnologyIcon>())
            {
                var icon = Object.Instantiate(Icons.GetIcon(tech.Icon), tech.transform);
                icon.localPosition = Vector3.zero;
                icon.localRotation = Quaternion.identity;
                icon.gameObject.SetActive(true);

                if (tech.Flip)
                {
                    icon.localScale = new Vector3(-1, 1, 1);
                }
            }
        }

        #endregion

        #region Tech List

        private static void ProcessTechList(Transform page, CatalogPage layout)
        {
            ProcessTech(page.Find(Paths.TechnologyItem), layout.TechList.Tech1);
            ProcessTech(page.Find(Paths.TechnologyItem + " (1)"), layout.TechList.Tech2);
            ProcessTech(page.Find(Paths.TechnologyItem + " (2)"), layout.TechList.Tech3);
            ProcessTech(page.Find(Paths.TechnologyItem + " (3)"), layout.TechList.Tech4);
            ProcessTech(page.Find(Paths.TechnologyItem + " (4)"), layout.TechList.Tech5);
            ProcessTech(page.Find(Paths.TechnologyItem + " (5)"), layout.TechList.Tech6);
            ProcessTech(page.Find(Paths.TechnologyItem + " (6)"), layout.TechList.Tech7);
        }

        private static void ProcessTech(Transform slot, TechEntry tech)
        {
            slot.gameObject.SetActive(true);

            foreach (Transform child in slot)
            {
                child.gameObject.SetActive(false);
            }

            switch (tech.Icon)
            {
                case TechIcon.Generic:
                    slot.Find(Paths.TechItems.Generic).gameObject.SetActive(true);
                    break;
                case TechIcon.ClosedCab:
                    slot.Find(Paths.TechItems.ClosedCab).gameObject.SetActive(true);
                    break;
                case TechIcon.OpenCab:
                    slot.Find(Paths.TechItems.OpenCab).gameObject.SetActive(true);
                    break;
                case TechIcon.CrewCompartment:
                    slot.Find(Paths.TechItems.CrewCompartment).gameObject.SetActive(true);
                    break;
                case TechIcon.CompressedAirBrakeSystem:
                    slot.Find(Paths.TechItems.CompressedAirBrakeSystem).gameObject.SetActive(true);
                    break;
                case TechIcon.DirectBrakeSystem:
                    slot.Find(Paths.TechItems.DirectBrakeSystem).gameObject.SetActive(true);
                    break;
                case TechIcon.DynamicBrakeSystem:
                    slot.Find(Paths.TechItems.DynamicBrakeSystem).gameObject.SetActive(true);
                    break;
                case TechIcon.ElectricPowerSupplyAndTransmission:
                    slot.Find(Paths.TechItems.ElectricPowerSupplyAndTransmission).gameObject.SetActive(true);
                    break;
                case TechIcon.ExternalControlInterface:
                    slot.Find(Paths.TechItems.ExternalControlInterface).gameObject.SetActive(true);
                    break;
                case TechIcon.HeatManagement:
                    slot.Find(Paths.TechItems.HeatManagement).gameObject.SetActive(true);
                    break;
                case TechIcon.HydraulicTransmission:
                    slot.Find(Paths.TechItems.HydraulicTransmission).gameObject.SetActive(true);
                    break;
                case TechIcon.InternalCombustionEngine:
                    slot.Find(Paths.TechItems.InternalCombustionEngine).gameObject.SetActive(true);
                    break;
                case TechIcon.MechanicalTransmission:
                    slot.Find(Paths.TechItems.MechanicalTransmission).gameObject.SetActive(true);
                    break;
                case TechIcon.PassengerCompartment:
                    slot.Find(Paths.TechItems.PassengerCompartment).gameObject.SetActive(true);
                    break;
                case TechIcon.SpecializedEquipment:
                    slot.Find(Paths.TechItems.SpecializedEquipment).gameObject.SetActive(true);
                    break;
                case TechIcon.SteamEngine:
                    slot.Find(Paths.TechItems.SteamEngine).gameObject.SetActive(true);
                    break;
                case TechIcon.UnitEffect:
                    slot.Find(Paths.TechItems.UnitEffect).gameObject.SetActive(true);
                    break;
                case TechIcon.CrewDelivery:
                    slot.Find(Paths.TechItems.CrewDelivery).gameObject.SetActive(true);
                    break;
                default:
                    slot.gameObject.SetActive(false);
                    return;
            }

            var desc = Paths.GetLocalize(slot, Paths.TechItems.TechnologyDesc);
            desc.gameObject.SetActive(!string.IsNullOrEmpty(tech.Description));
            desc.SetKeyAndUpdate(tech.Description);

            var type = Paths.GetLocalize(slot, Paths.TechItems.TechnologyType);
            type.gameObject.SetActive(!string.IsNullOrEmpty(tech.Type));
            type.SetKeyAndUpdate(tech.Type);
        }

        #endregion

        #region Score Lists

        private static void ProcessAllScoreLists(Transform page, CatalogPage layout)
        {
            ProcessScoreList(page.Find(Paths.EaseOfOperationScore), layout.EaseOfOperation);
            ProcessScoreList(page.Find(Paths.MaintenanceScore), layout.Maintenance);
            ProcessScoreList(page.Find(Paths.HaulingScore), layout.Hauling);
            ProcessScoreList(page.Find(Paths.ShuntingScore), layout.Shunting);
        }

        private static void ProcessScoreList(Transform root, ScoreList list)
        {
            ProcessTotalScore(root, list);

            Transform item;
            int i = 0;

            foreach (var score in list.AllScores)
            {
                if (i == 0)
                {
                    item = root.Find(Paths.ScoreLists.ScoreItem);
                }
                else
                {
                    item = root.Find($"{Paths.ScoreLists.ScoreItem} ({i})");
                }

                ProcessScoreItem(item, score);
                i++;
            }
        }

        private static void ProcessTotalScore(Transform root, ScoreList list)
        {
            foreach (Transform child in root.Find(Paths.ScoreLists.Total))
            {
                child.gameObject.SetActive(false);
            }

            var total = Paths.GetLocalizedNumber(root, Paths.ScoreLists.TotalScore);

            switch (list.TotalScoreDisplay)
            {
                case TotalScoreDisplay.Average:
                    switch (list.Total)
                    {
                        case >= 4.0f:
                            root.Find(Paths.ScoreLists.Bg40).gameObject.SetActive(true);
                            break;
                        case >= 3.5f:
                            root.Find(Paths.ScoreLists.Bg35).gameObject.SetActive(true);
                            break;
                        case >= 3.0f:
                            root.Find(Paths.ScoreLists.Bg30).gameObject.SetActive(true);
                            break;
                        default:
                            root.Find(Paths.ScoreLists.Bg10).gameObject.SetActive(true);
                            break;
                    }

                    total.value = list.Total;
                    break;
                case TotalScoreDisplay.NotApplicable:
                    root.Find(Paths.ScoreLists.BgDisqualified).gameObject.SetActive(true);
                    Object.Destroy(total);
                    Paths.GetText(root, Paths.ScoreLists.TotalScore).SetTextAndUpdate("-");
                    break;
                default:
                    return;
            }

            total.gameObject.SetActive(true);
        }

        private static void ProcessScoreItem(Transform item, CatalogScore score)
        {
            foreach (Transform child in item)
            {
                child.gameObject.SetActive(false);
            }

            item.Find(Paths.ScoreLists.BarBg).gameObject.SetActive(true);
            item.Find(Paths.ScoreLists.ScoreName).gameObject.SetActive(true);

            switch (score.ScoreType)
            {
                case ScoreType.NotApplicable:
                    item.Find(Paths.ScoreLists.BarScore0).gameObject.SetActive(true);
                    break;
                case ScoreType.Score:
                    item.Find(Paths.WithNumber(Paths.ScoreLists.BarScore, score.Value)).gameObject.SetActive(true);
                    break;
                case ScoreType.PositiveEffect:
                    item.Find(Paths.WithNumber(Paths.ScoreLists.BarEffectPositive, score.Value)).gameObject.SetActive(true);
                    break;
                case ScoreType.NegativeEffect:
                    item.Find(Paths.WithNumber(Paths.ScoreLists.BarEffectNegative, score.Value)).gameObject.SetActive(true);
                    break;
                case ScoreType.PositiveSharedEffect:
                    item.Find(Paths.WithNumber(Paths.ScoreLists.BarSharedEffectPositive, score.Value)).gameObject.SetActive(true);
                    break;
                case ScoreType.NegativeSharedEffect:
                    item.Find(Paths.WithNumber(Paths.ScoreLists.BarSharedEffectNegative, score.Value)).gameObject.SetActive(true);
                    break;
                case ScoreType.EffectOn:
                    item.Find(Paths.ScoreLists.BarEffectOn).gameObject.SetActive(true);
                    break;
                case ScoreType.EffectOff:
                    item.Find(Paths.ScoreLists.BarEffectOff).gameObject.SetActive(true);
                    break;
                default:
                    return;
            }
        }

        #endregion

        private static string GetVehicleTypeKey(VehicleType type) => type switch
        {
            VehicleType.Locomotive => "vc/vehicletype/locomotive",
            VehicleType.Tender => "vc/vehicletype/tender",
            VehicleType.Slug => "vc/vehicletype/slug",
            VehicleType.Draisine => "vc/vehicletype/draisine",
            VehicleType.Car => "vc/vehicletype/car",
            VehicleType.Booster => "vc/vehicletype/booster",
            VehicleType.Railcar => "vc/vehicletype/railcar",
            VehicleType.ControlCar => "vc/vehicletype/control_car",
            VehicleType.Support => "vc/vehicletype/support",
            VehicleType.Special => "vc/vehicletype/special",
            _ => throw new System.ArgumentOutOfRangeException(nameof(type)),
        };

        private static string GetVehicleRoleKey(VehicleRole role) => role switch
        {
            VehicleRole.LightShunting => "vc/role/shunting_light",
            VehicleRole.HeavyShunting => "vc/role/shunting_heavy",
            VehicleRole.LightHauling => "vc/role/hauling_light",
            VehicleRole.HeavyHauling => "vc/role/hauling_heavy",
            VehicleRole.FuelSupply => "vc/role/fuel_supply",
            VehicleRole.CrewTransport => "vc/role/crew_transport",
            VehicleRole.CrewSupport => "vc/role/crew_support",
            VehicleRole.PassengerTransport => "vc/role/passenger_transport",
            VehicleRole.FreightTransport => "vc/role/freight",
            VehicleRole.UtilityTransport => "vc/role/utility",
            VehicleRole.TrackMaintenance => "vc/role/track_maintenance",
            _ => throw new System.ArgumentOutOfRangeException(nameof(role)),
        };

        private static void ClearCache()
        {
            CCLPlugin.Log("Cleaning up...");
            PageDE2 = null!;
            PageDE2 = null!;
            TransformDE2 = null!;
            SpawnChances.Clear();
            Icons.ClearCache();
        }

        public static string FormatPrice(float price) => "$" + price.ToString("N0", LocalizationAPI.CC);

        private static class Paths
        {
            public const string Header = "Template Canvas/BackgroundImage/OuterWrapper/VCHeader";
            public const string Content = "Template Canvas/BackgroundImage/OuterWrapper/Content";

            public const string PageColor = Header + "/LocoColorBg";
            public const string PageName = Header + "/LocoName";
            public const string Units = Header + "/LocoUnits";
            public const string Nickname = Header + "/LocoNickname";
            public const string Icon = Header + "/LocoIcon";

            public const string GarageIcon = Header + "/InfoGroup/Licenses/VCGarageLock";
            public const string License1 = Header + "/InfoGroup/Licenses/License";
            public const string License2 = Header + "/InfoGroup/Licenses/License (1)";
            public const string License3 = Header + "/InfoGroup/Licenses/License (2)";
            public const string Locations = Header + "/InfoGroup/Locations";
            public const string ProductionYears = Locations + "/YearPriceContainer/LocoYears";
            public const string LicenseContainer = Locations + "/YearPriceContainer/License";
            public const string LicenseLock = "-LockIcon";
            public const string LicenseValue = "-PriceValue";
            public const string SummonIcon = Locations + "/YearPriceContainer/SummonIcon";
            public const string SummonPrice = Locations + "/YearPriceContainer/SummonPrice";
            public const string GarageLockIcon = Locations + "/YearPriceContainer/Garage-LockIcon";
            public const string GaragePrice = Locations + "/YearPriceContainer/Garage-PriceValue";

            public const string VehicleType = Content + "/ColumnLeft/VCVehicleType/VehicleType";
            public const string VehicleRole1 = VehicleType + "/VehicleRoles/VehicleRole1";
            public const string VehicleRole2 = VehicleType + "/VehicleRoles/VehicleRole2";

            public const string DiagramParent = Content + "/ColumnLeft/VCDiagram/Bg/VehicleDiagrams";

            public const string LoadRating = Content + "/ColumnRight/VCLoadRating";
            public const string LoadRatingText = LoadRating + "/LoadRatingText";
            public const string Summonable = LoadRating + "/HorLayout/VCSummon";

            public const string TechList = Content + "/ColumnRight/VCTechList";
            public const string TechnologyItem = TechList + "/VCTechItem";

            public const string EaseOfOperationScore = Content + "/ColumnLeft/VCScoreList";
            public const string MaintenanceScore = Content + "/ColumnLeft/VCScoreList (1)";
            public const string HaulingScore = Content + "/ColumnRight/VCScoreList";
            public const string ShuntingScore = Content + "/ColumnRight/VCScoreList (1)";

            public static class LoadRatings
            {
                public const string IconFlat = "HorLayout/VCLoadRating";
                public const string IconIncline = "HorLayout/VCLoadRating (1)";
                public const string IconInclineWet = "HorLayout/VCLoadRating (2)";

                public const string Bad = "RatingBad";
                public const string Medium = "RatingMedium";
                public const string Good = "RatingGood";
                public const string Tonnage = "TonnageText";
            }

            public static class Diagrams
            {
                public const string Generic = "DiagramVehicle";
                public const string BE2 = "DiagramVehicle_BE2-260";
                public const string DE2 = "DiagramVehicle_DE2-480";
                public const string DE6 = "DiagramVehicle_DE6-960";
                public const string DE6Slug = "VCDiag_DE6-860S";
                public const string DH4 = "DiagramVehicle_DH4-670";
                public const string DM1P = "DiagramVehicle_DM1P-150";
                public const string DM3 = "DiagramVehicle_DM3-540";
                public const string H1 = "DiagramVehicle_H1-020";
                public const string S060 = "DiagramVehicle_S060-440";
                public const string S282A = "DiagramVehicle_S282-730";
                public const string S282B = "VCDiag_S282-730B";
                public const string Caboose = "VCDiag_Caboose";

                public const string TallVehicle = "TallVehicle";
                public const string ThinVehicle = "ThinVehicle";
                public const string BufferL = "BufferL";
                public const string BufferR = "BufferR";
                public const string XText = "XText";
                public const string YText = "YText";
                public const string ZText = "ZText";
                public const string MassFull = "Mass/MassFull/Text (TMP)";
                public const string MassEmpty = "Mass/MassEmpty/Text (TMP)";
                public const string Price = "VehiclePrice";
            }

            public static class TechItems
            {
                public const string Generic = "VCTechIcon";
                public const string ClosedCab = "VCTechIcon_CabClosed";
                public const string OpenCab = "VCTechIcon_CabOpen";
                public const string CrewCompartment = "VCTechIcon_CrewCompartment";
                public const string CompressedAirBrakeSystem = "VCTechIcon_CompressedAirBrakeSystem";
                public const string DirectBrakeSystem = "VCTechIcon_DirectBrakeSystem";
                public const string DynamicBrakeSystem = "VCTechIcon_DynamicBrakeSystem";
                public const string ElectricPowerSupplyAndTransmission = "VCTechIcon_ElectricPowerSupplyAndTransmission";
                public const string ExternalControlInterface = "VCTechIcon_ExternalControlInterface";
                public const string HeatManagement = "VCTechIcon_HeatManagement";
                public const string HydraulicTransmission = "VCTechIcon_HydraulicTransmission";
                public const string InternalCombustionEngine = "VCTechIcon_InternalCombustionEngine";
                public const string MechanicalTransmission = "VCTechIcon_MechanicalTransmission";
                public const string PassengerCompartment = "VCTechIcon_PassengerCompartment";
                public const string SpecializedEquipment = "VCTechIcon_SpecializedEquipment";
                public const string SteamEngine = "VCTechIcon_SteamEngine";
                public const string UnitEffect = "VCTechIcon_UnitEffect";
                public const string CrewDelivery = "VCTechIcon_CrewDelivery";

                public const string TechnologyDesc = "TechnologyDesc";
                public const string TechnologyType = "TechnologyType";
            }

            public static class ScoreLists
            {
                public const string Total = "ScoreListTotalScore";
                public const string BgDisqualified = Total + "/ScoreListTotalScoreBgDisqualified";
                public const string Bg10 = Total + "/ScoreListTotalScoreBg10+";
                public const string Bg30 = Total + "/ScoreListTotalScoreBg30+";
                public const string Bg35 = Total + "/ScoreListTotalScoreBg35+";
                public const string Bg40 = Total + "/ScoreListTotalScoreBg40+";
                public const string TotalScore = Total + "/ScoreListTotalScore";

                public const string ScoreItem = "ScoreList/VCScoreItem";
                public const string BarBg = "BarBg";
                public const string BarScore0 = "BarScore0";
                public const string BarEffectOn = "BarEffectOn";
                public const string BarEffectOff = "BarEffectOff";
                public const string BarSharedEffectPositive = "BarSharedEffect+";
                public const string BarSharedEffectNegative = "BarSharedEffect-";
                public const string BarEffectPositive = "BarEffect+";
                public const string BarEffectNegative = "BarEffect-";
                public const string BarScore = "BarScore";
                public const string ScoreName = "ScoreName";
            }

            public static string Combine(params string[] paths) => string.Join("/", paths);

            public static string WithNumber(string path, int number) => $"{path}{number}";

            public static TMP_Text GetText(Transform root, string path) => TMPHelper.GetTMP(root.Find(path));

            public static TextMeshProUGUI GetTextUGUI(Transform root, string path) => TMPHelper.GetTextMeshProUGUI(root.Find(path));

            public static Localize GetLocalize(Transform root, string path) => root.Find(path).GetComponent<Localize>();

            public static LocalizedNumber GetLocalizedNumber(Transform root, string path) => root.Find(path).GetComponent<LocalizedNumber>();

            public static Image GetImage(Transform root, string path) => root.Find(path).GetComponent<Image>();
        }

        private static class Icons
        {
            private static Transform? s_generic;
            private static Transform? s_closedCab;
            private static Transform? s_openCab;
            private static Transform? s_crewCompartment;
            private static Transform? s_compressedAirBrakeSystem;
            private static Transform? s_directBrakeSystem;
            private static Transform? s_dynamicBrakeSystem;
            private static Transform? s_electricPowerSupplyAndTransmission;
            private static Transform? s_externalControlInterface;
            private static Transform? s_heatManagement;
            private static Transform? s_hydraulicTransmission;
            private static Transform? s_internalCombustionEngine;
            private static Transform? s_mechanicalTransmission;
            private static Transform? s_passengerCompartment;
            private static Transform? s_specializedEquipment;
            private static Transform? s_steamEngine;
            private static Transform? s_unitEffect;
            private static Transform? s_crewDelivery;

            private static Transform? s_wheel;
            private static Transform? s_poweredWheel;
            private static Transform? s_pivotShort;
            private static Transform? s_pivotLong;
            private static RectTransform? s_bogie;

            public static Transform Generic => Extensions.GetCached(ref s_generic,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.Generic)));
            public static Transform ClosedCab => Extensions.GetCached(ref s_closedCab,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.ClosedCab)));
            public static Transform OpenCab => Extensions.GetCached(ref s_openCab,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.OpenCab)));
            public static Transform CrewCompartment => Extensions.GetCached(ref s_crewCompartment,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.CrewCompartment)));
            public static Transform CompressedAirBrakeSystem => Extensions.GetCached(ref s_compressedAirBrakeSystem,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.CompressedAirBrakeSystem)));
            public static Transform DirectBrakeSystem => Extensions.GetCached(ref s_directBrakeSystem,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.DirectBrakeSystem)));
            public static Transform DynamicBrakeSystem => Extensions.GetCached(ref s_dynamicBrakeSystem,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.DynamicBrakeSystem)));
            public static Transform ElectricPowerSupplyAndTransmission => Extensions.GetCached(ref s_electricPowerSupplyAndTransmission,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.ElectricPowerSupplyAndTransmission)));
            public static Transform ExternalControlInterface => Extensions.GetCached(ref s_externalControlInterface,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.ExternalControlInterface)));
            public static Transform HeatManagement => Extensions.GetCached(ref s_heatManagement,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.HeatManagement)));
            public static Transform HydraulicTransmission => Extensions.GetCached(ref s_hydraulicTransmission,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.HydraulicTransmission)));
            public static Transform InternalCombustionEngine => Extensions.GetCached(ref s_internalCombustionEngine,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.InternalCombustionEngine)));
            public static Transform MechanicalTransmission => Extensions.GetCached(ref s_mechanicalTransmission,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.MechanicalTransmission)));
            public static Transform PassengerCompartment => Extensions.GetCached(ref s_passengerCompartment,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.PassengerCompartment)));
            public static Transform SpecializedEquipment => Extensions.GetCached(ref s_specializedEquipment,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.SpecializedEquipment)));
            public static Transform SteamEngine => Extensions.GetCached(ref s_steamEngine,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.SteamEngine)));
            public static Transform UnitEffect => Extensions.GetCached(ref s_unitEffect,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.UnitEffect)));
            public static Transform CrewDelivery => Extensions.GetCached(ref s_crewDelivery,
                () => TransformDE2.Find(Paths.Combine(Paths.TechnologyItem, Paths.TechItems.CrewDelivery)));

            public static Transform Wheel => Extensions.GetCached(ref s_wheel,
                () => TransformDE2.Find(Paths.Combine(Paths.DiagramParent, Paths.Diagrams.DM1P, "VCBogie_1x0/Wheel")));
            public static Transform PoweredWheel => Extensions.GetCached(ref s_poweredWheel,
                () => TransformDE2.Find(Paths.Combine(Paths.DiagramParent, Paths.Diagrams.DM1P, "VCBogie_1x1/Wheel")));
            public static Transform PivotShort => Extensions.GetCached(ref s_pivotShort,
                () => TransformDE2.Find(Paths.Combine(Paths.DiagramParent, Paths.Diagrams.DM1P, "VCBogie_1x0/Bogie (1)")));
            public static Transform PivotLong => Extensions.GetCached(ref s_pivotLong,
                () => TransformDE2.Find(Paths.Combine(Paths.DiagramParent, Paths.Diagrams.DH4, "VCBogie_2x2/Bogie (1)")));
            public static RectTransform Bogie => Extensions.GetCached(ref s_bogie,
                () => TransformDE2.Find(Paths.Combine(Paths.DiagramParent, Paths.Diagrams.DH4, "VCBogie_2x2/Bogie")).GetComponent<RectTransform>());

            public static Transform GetIcon(TechIcon icon) => icon switch
            {
                TechIcon.Generic => Generic,
                TechIcon.ClosedCab => ClosedCab,
                TechIcon.OpenCab => OpenCab,
                TechIcon.CrewCompartment => CrewCompartment,
                TechIcon.CompressedAirBrakeSystem => CompressedAirBrakeSystem,
                TechIcon.DirectBrakeSystem => DirectBrakeSystem,
                TechIcon.DynamicBrakeSystem => DynamicBrakeSystem,
                TechIcon.ElectricPowerSupplyAndTransmission => ElectricPowerSupplyAndTransmission,
                TechIcon.ExternalControlInterface => ExternalControlInterface,
                TechIcon.HeatManagement => HeatManagement,
                TechIcon.HydraulicTransmission => HydraulicTransmission,
                TechIcon.InternalCombustionEngine => InternalCombustionEngine,
                TechIcon.MechanicalTransmission => MechanicalTransmission,
                TechIcon.PassengerCompartment => PassengerCompartment,
                TechIcon.SpecializedEquipment => SpecializedEquipment,
                TechIcon.SteamEngine => SteamEngine,
                TechIcon.UnitEffect => UnitEffect,
                TechIcon.CrewDelivery => CrewDelivery,
                _ => null!,
            };

            public static void ClearCache()
            {
                s_generic = null!;
                s_closedCab = null!;
                s_openCab = null!;
                s_crewCompartment = null!;
                s_compressedAirBrakeSystem = null!;
                s_directBrakeSystem = null!;
                s_dynamicBrakeSystem = null!;
                s_electricPowerSupplyAndTransmission = null!;
                s_externalControlInterface = null!;
                s_heatManagement = null!;
                s_hydraulicTransmission = null!;
                s_internalCombustionEngine = null!;
                s_mechanicalTransmission = null!;
                s_passengerCompartment = null!;
                s_specializedEquipment = null!;
                s_steamEngine = null!;
                s_unitEffect = null!;
                s_crewDelivery = null!;

                s_wheel = null!;
                s_poweredWheel = null!;
                s_pivotShort = null!;
                s_pivotLong = null!;
                s_bogie = null!;
            }
        }
    }
}
