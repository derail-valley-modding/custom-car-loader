using CCL.Importer.Processing;
using CCL.Types.Catalog;
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

            var page = ModelProcessor.CreateModifiablePrefab(PageDE6Slug.gameObject).transform;
            page.gameObject.SetActive(true);

            ProcessHeader(page, layout);

            // Disable diagram for now.
            page.Find(Paths.Diagram).gameObject.SetActive(false);

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

            Paths.GetText(page, Paths.Nickname).text = layout.ConsistUnits;
            page.Find(Paths.Icon).GetComponent<Image>().sprite = layout.Icon;
        }

        private static class Paths
        {
            public const string Header = "Template Canvas/BackgroundImage/OuterWrapper/VCHeader";
            public const string Content = "Template Canvas/BackgroundImage/OuterWrapper/Content";

            public const string PageColor = Header + "/LocoColorBg";
            public const string PageName = Header + "/LocoName";
            public const string Units = Header + "/LocoUnits";
            public const string Nickname = Header + "/LocoNickname";
            public const string Icon = Header + "/LocoIcon";

            public const string Diagram = Content + "/ColumnLeft/VCDiagram/Bg/VehicleDiagrams";

            public static string Combine(params string[] paths)
            {
                return string.Join("/", paths);
            }

            public static TMP_Text GetText(Transform root, string path) => TMPHelper.GetTMP(root.Find(path));
        }
    }
}
