using CCL.Creator.Utility;
using CCL.Types.Catalog;
using CCL.Types.Catalog.Diagram;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    [CustomEditor(typeof(CatalogPage))]
    internal class CatalogPageEditor : Editor
    {
        private CatalogPage _page = null!;
        private SerializedProperty _prop = null!;
        private ScoreType _scoreType = ScoreType.Score;

        public override void OnInspectorGUI()
        {
            _page = (CatalogPage)target;
            _prop = serializedObject.FindProperty(nameof(CatalogPage.HeaderColour));

            do
            {
                EditorGUILayout.PropertyField(_prop);

                switch (_prop.name)
                {
                    case nameof(CatalogPage.ShowLoadRatings):
                        GUI.enabled = _prop.boolValue;
                        break;
                    case nameof(CatalogPage.LoadInclineWet):
                        GUI.enabled = true;
                        break;
                    case nameof(CatalogPage.TechList):
                        if (_prop.isExpanded)
                        {
                            EditorGUILayout.Space();

                            if (GUILayout.Button(new GUIContent("Try to set appropriate types",
                                "This will try to set the 'Type' fields to an appropriate default value")))
                            {
                                foreach (var tech in _page.TechList.AllTechs)
                                {
                                    TechEntry.TryToSetAppropriateType(tech, _page.Type);
                                }

                                AssetHelper.SaveAsset(_page);
                                serializedObject.UpdateIfRequiredOrScript();
                            }
                        }

                        EditorGUILayout.Space();

                        if (GUILayout.Button(new GUIContent("Create diagram",
                            "Creates a diagram layout from the information above\n" +
                            "Please fill out the tech list first for better results")))
                        {
                            _page.DiagramLayout = CreateDiagram(_page);
                            AssetHelper.SaveAsset(_page);
                            serializedObject.UpdateIfRequiredOrScript();
                        }
                        break;
                    default:
                        break;
                }

            } while (_prop.Next(false) && _prop.name != nameof(CatalogPage.EaseOfOperation));

            for (int i = 0; i < 4; i++)
            {
                var list = _page.GetScoreList(i);
                var label = new GUIContent(GetScoreListLabelText(_prop, list.TotalScoreDisplay, list.FormattedTotal));
                EditorGUILayout.PropertyField(_prop, label);
                _prop.Next(false);
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Change all score types"))
                {
                    var showTotal = _scoreType == ScoreType.Score ? TotalScoreDisplay.Average : TotalScoreDisplay.None;

                    foreach (var list in _page.AllScoreLists)
                    {
                        list.TotalScoreDisplay = showTotal;

                        foreach (var score in list.AllScores)
                        {
                            score.ScoreType = _scoreType;
                        }
                    }

                    AssetHelper.SaveAsset(_page);
                    serializedObject.UpdateIfRequiredOrScript();
                }

                _scoreType = (ScoreType)EditorGUILayout.EnumPopup(_scoreType);
            }

            bool flag = false;

            foreach (var item in _page.AllScoreLists)
            {
                flag |= item.ValidateDisplay();
            }

            if (flag)
            {
                serializedObject.UpdateIfRequiredOrScript();
                Repaint();
            }
        }

        private static string GetScoreListLabelText(SerializedProperty prop, TotalScoreDisplay scoreDisplay, string total) => scoreDisplay switch
        {
            TotalScoreDisplay.None => $"{prop.displayName}",
            TotalScoreDisplay.Average => $"{prop.displayName} [{total}]",
            TotalScoreDisplay.NotApplicable => $"{prop.displayName} [ - ]",
            _ => throw new System.ArgumentOutOfRangeException(nameof(scoreDisplay)),
        };

        private static GameObject CreateDiagram(CatalogPage page)
        {
            var diagram = new GameObject($"{page.name}_Diagram");

            CreateBogie(diagram.transform, "Bogie F");
            CreateBogie(diagram.transform, "Bogie R");

            BogieLayout.TryToAlignBogies(diagram.transform);

            int i = 0;

            foreach (var item in page.TechList.AllTechs)
            {
                i++;

                if (SkipTech(item.Icon))
                {
                    continue;
                }

                var tech = new GameObject($"Tech {i}")
                    .AddComponent<TechnologyIcon>();

                tech.transform.SetParent(diagram.transform);
                tech.transform.localPosition = Vector3.zero;
                tech.Icon = item.Icon;
            }

            string path = $"{System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(page))}/{diagram.name}.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(diagram, path, InteractionMode.AutomatedAction);

            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        private static bool SkipTech(TechIcon icon)
        {
            switch (icon)
            {
                case TechIcon.None:
                case TechIcon.UnitEffect:
                case TechIcon.CrewDelivery:
                    return true;
                default:
                    return false;
            }
        }

        private static BogieLayout CreateBogie(Transform parent, string name)
        {
            var bogie = new GameObject(name)
                .AddComponent<BogieLayout>();

            bogie.transform.SetParent(parent);
            bogie.transform.localPosition = Vector3.zero;

            return bogie;
        }
    }
}
