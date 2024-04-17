using CCL.Creator.Utility;
using CCL.Types.Catalog;
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
                    case nameof(CatalogPage.UnlockedByGarage):
                        GUI.enabled = _prop.boolValue;
                        break;
                    case nameof(CatalogPage.GaragePrice):
                        //GUI.enabled = !GUI.enabled;
                        GUI.enabled = true;
                        break;
                    case nameof(CatalogPage.SummonableByRemote):
                        GUI.enabled = _prop.boolValue;
                        break;
                    case nameof(CatalogPage.SummonPrice):
                        GUI.enabled = true;
                        break;
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
        }

        private static string GetScoreListLabelText(SerializedProperty prop, TotalScoreDisplay scoreDisplay, string total) => scoreDisplay switch
        {
            TotalScoreDisplay.None => $"{prop.displayName}",
            TotalScoreDisplay.Average => $"{prop.displayName} [{total}]",
            TotalScoreDisplay.NotApplicable => $"{prop.displayName} [ - ]",
            _ => throw new System.ArgumentOutOfRangeException(nameof(scoreDisplay)),
        };
    }
}
