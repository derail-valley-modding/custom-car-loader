using CCL.Importer.Processing;
using CCL.Types.Catalog;
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
        public static List<CatalogPage> PageInfos = new();
        public static List<StaticPageTemplatePaper> NewCatalogPages = new();

        private static StaticPageTemplatePaper[] s_original = System.Array.Empty<StaticPageTemplatePaper>();
        private static Transform PageDE2 => s_original[1].transform;
        private static Transform PageS060 => s_original[2].transform;
        private static Transform PageDM3 => s_original[3].transform;
        private static Transform PageDH4 => s_original[4].transform;
        private static Transform PageS282A => s_original[5].transform;
        private static Transform PageS282B => s_original[6].transform;
        private static Transform PageDE6 => s_original[7].transform;
        private static Transform PageBE2 => s_original[8].transform;
        private static Transform PageDE6Slug => s_original[9].transform;
        private static Transform PageH1 => s_original[10].transform;
        private static Transform PageCaboose => s_original[11].transform;

        public static void GeneratePages(StaticPagesRender original)
        {
            s_original = original.staticPages.ToArray();
            NewCatalogPages.Clear();

            foreach (var item in PageInfos)
            {
                var result = ProcessPage(item);
                result.transform.parent = original.transform;
                result.transform.localPosition = Vector3.zero;
                result.transform.localRotation = Quaternion.identity;
                NewCatalogPages.Add(result);
            }
        }

        private static StaticPageTemplatePaper ProcessPage(CatalogPage layout)
        {
            CCLPlugin.Log($"Generating catalog page '{layout.PageName}'");

            var page = ModelProcessor.CreateModifiablePrefab(PageDE2.gameObject).transform;
            page.gameObject.SetActive(true);

            ProcessHeader(page, layout);
            ProcessDiagram(page, layout);
            ProcessTechList(page, layout);
            ProcessAllScoreLists(page, layout);

            return page.GetComponent<StaticPageTemplatePaper>();
        }

        private static void ProcessHeader(Transform page, CatalogPage layout)
        {
            page.Find(Paths.PageColor).GetComponent<Image>().color = layout.HeaderColour;
            Paths.GetText(page, Paths.PageName).text = layout.PageName;
            Paths.GetText(page, Paths.Units).text = layout.ConsistUnits;

            if (string.IsNullOrEmpty(layout.Nickname))
            {
                page.Find(Paths.Nickname).gameObject.SetActive(false);
            }
            else
            {
                var nick = Paths.GetText(page, Paths.Nickname);
                nick.gameObject.SetActive(true);
                nick.text = layout.Nickname;
            }

            page.Find(Paths.Icon).GetComponent<Image>().sprite = layout.Icon;

            ProcessSpawnLocations(page.GetComponentInChildren<LocoSpawnRateRenderer>(), layout);

            #region Licenses

            if (layout.UnlockedByGarage)
            {
                page.Find(Paths.GarageIcon).gameObject.SetActive(true);
                page.Find(Paths.LicenseOther + "1" + Paths.LicenseLock).gameObject.SetActive(true);

                var text = Paths.GetText(page, Paths.LicenseOther + "1" + Paths.LicenseValue);
                text.gameObject.SetActive(true);
                text.text = layout.GaragePrice.ToString();

                page.Find(Paths.License1).gameObject.SetActive(false);
                page.Find(Paths.License2).gameObject.SetActive(false);
                page.Find(Paths.License3).gameObject.SetActive(false);

                page.Find(Paths.LicenseOther + "2" + Paths.LicenseLock).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "2" + Paths.LicenseValue).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "3" + Paths.LicenseLock).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "3" + Paths.LicenseValue).gameObject.SetActive(false);
            }
            else
            {
                // Licenses disabled for now.
                page.Find(Paths.License1).gameObject.SetActive(false);
                page.Find(Paths.License2).gameObject.SetActive(false);
                page.Find(Paths.License3).gameObject.SetActive(false);

                page.Find(Paths.LicenseOther + "1" + Paths.LicenseLock).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "1" + Paths.LicenseValue).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "2" + Paths.LicenseLock).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "2" + Paths.LicenseValue).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "3" + Paths.LicenseLock).gameObject.SetActive(false);
                page.Find(Paths.LicenseOther + "3" + Paths.LicenseValue).gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(layout.ProductionYears))
            {
                var text = Paths.GetText(page, Paths.ProductionYears);
                text.gameObject.SetActive(true);
                text.text = layout.ProductionYears;
            }
            else
            {
                page.Find(Paths.ProductionYears).gameObject.SetActive(false);
            }

            #endregion

            if (layout.SummonableByRemote)
            {
                page.Find(Paths.Summonable).gameObject.SetActive(true);
                page.Find(Paths.SummonIcon).gameObject.SetActive(true);
                var text = Paths.GetText(page, Paths.SummonPrice);
                text.gameObject.SetActive(true);
                text.text = layout.SummonPrice.ToString();
            }
            else
            {
                page.Find(Paths.Summonable).gameObject.SetActive(false);
            }

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

        private static void ProcessSpawnLocations(LocoSpawnRateRenderer spawner, CatalogPage layout)
        {
            // Always disable spawn locations while we figure what to do.
            spawner.enabled = false;
            spawner.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }

        private static bool SetLicenseIcon(Image image, string license)
        {
            if (!DV.Globals.G.Types.TryGetGeneralLicense(license, out var v2))
            {
                CCLPlugin.Warning($"No license with ID '{license}' found.");
                return false;
            }

            image.sprite = v2.icon;
            return true;
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

            Paths.GetText(root, Paths.LoadRatings.Tonnage).text = rating.Tonnage.ToString();
        }

        private static void ProcessDiagram(Transform page, CatalogPage layout)
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

            // Disable diagram for now.
            page.Find(Paths.Diagram).gameObject.SetActive(false);
        }

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

            var desc = Paths.GetText(slot, Paths.TechItems.TechnologyDesc);
            desc.gameObject.SetActive(true);
            desc.text = tech.Description;
            var type = Paths.GetText(slot, Paths.TechItems.TechnologyType);
            type.gameObject.SetActive(true);
            type.text = tech.Type;
        }

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

            TMP_Text text = Paths.GetText(root, Paths.ScoreLists.TotalScore);

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

                    text.text = list.FormattedTotal;
                    break;
                case TotalScoreDisplay.NotApplicable:
                    root.Find(Paths.ScoreLists.BgDisqualified).gameObject.SetActive(true);
                    text.text = "-";
                    break;
                default:
                    return;
            }
            text.gameObject.SetActive(true);
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
            _ => throw new System.ArgumentOutOfRangeException(nameof(role)),
        };

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
            public const string LicenseOther = Locations + "/YearPriceContainer/License";
            public const string LicenseLock = "-LockIcon";
            public const string LicenseValue = "-PriceValue";
            public const string SummonIcon = Locations + "/YearPriceContainer/SummonIcon";
            public const string SummonPrice = Locations + "/YearPriceContainer/SummonPrice";

            public const string VehicleType = Content + "/ColumnLeft/VCVehicleType/VehicleType";
            public const string VehicleRole1 = VehicleType + "/VehicleRoles/VehicleRole1";
            public const string VehicleRole2 = VehicleType + "/VehicleRoles/VehicleRole2";

            public const string Diagram = Content + "/ColumnLeft/VCDiagram/Bg/VehicleDiagrams";

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

            public static class TechItems
            {
                public const string Generic = "VCTechIcon";
                public const string ClosedCab = "VCTechIcon_ClosedCab";
                public const string OpenCab = "VCTechIcon_OpenCab";
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

            public static Localize GetLocalize(Transform root, string path) => root.Find(path).GetComponent<Localize>();
        }
    }
}
