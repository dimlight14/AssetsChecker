using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class MissingReferencesFinder : EditorWindow
    {
        private const int ENTRIES_PER_PAGE = 50;

        private bool _includeScriptsFolder;
        private bool _includePackages;
        private bool _includeScenes = true;
        private bool _settingsFoldout = true;
        private Vector2 _entriesScrollVector = Vector2.zero;
        private Vector2 _pagesScroll = Vector2.zero;

        private readonly AssetsDataAccumulator _assetsDataAccumulator = new AssetsDataAccumulator();
        private readonly List<AssetEntry> _missingReferencesAssets = new List<AssetEntry>();
        private List<AssetEntry> _filteredAssets;
        private bool _hasResult = false;
        private int _pageSelected = 0;
        private string _pathFilter;

        [MenuItem("Tools/Find Missing References")]
        public static void OpenAssetsCheckerWindow()
        {
            GetWindow<MissingReferencesFinder>("MissingReferencesFinder");
        }

        private void DrawLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(5);
            DrawSettings();
            EditorGUILayout.Space(30);
            DrawMainButton();

            if (!_hasResult)
                return;

            DrawLine();

            EditorGUILayout.Space(10);
            if (_missingReferencesAssets.Count == 0)
            {
                GUI.color = Color.green;
                EditorGUILayout.LabelField("No missing references found!");
                GUI.color = Color.white;
            }
            else
            {
                DrawFilterAndFilterAssets();
                EditorGUILayout.Space(10);
                if (_filteredAssets.Count > ENTRIES_PER_PAGE)
                {
                    DrawPages();
                    EditorGUILayout.Space(15);
                }

                DrawAssetsScroll();
            }
        }

        private void DrawSettings()
        {
            _settingsFoldout = EditorGUILayout.Foldout(_settingsFoldout, "Settings");
            if (_settingsFoldout)
            {
                EditorGUILayout.BeginVertical();
                _includeScriptsFolder = EditorGUILayout.Toggle("Include Scripts folder", _includeScriptsFolder);
                _includePackages = EditorGUILayout.Toggle("Include Packages folder", _includePackages);
                _includeScenes = EditorGUILayout.Toggle("Include Scenes", _includeScenes);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMainButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = Color.green;
            if (GUILayout.Button("Find missing references", GUILayout.Width(200f)))
            {
                Clear();
                _hasResult = true;
                _assetsDataAccumulator.AccumulateMissingReferences(_missingReferencesAssets, _includeScriptsFolder, _includePackages, _includeScenes);
            }

            GUI.color = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFilterAndFilterAssets()
        {
            var oldFilterValue = _pathFilter;
            _pathFilter = EditorGUILayout.TextField("Path Filter:", _pathFilter, GUILayout.Width(400f));

            if (_filteredAssets == null || oldFilterValue != _pathFilter)
            {
                if (!string.IsNullOrEmpty(_pathFilter))
                {
                    _pageSelected = 0;
                    _filteredAssets = _missingReferencesAssets.Where(x => x.Path.Contains(_pathFilter)).ToList();
                }
                else
                {
                    _filteredAssets = _missingReferencesAssets;
                }
            }
        }

        private void DrawPages()
        {
            _pagesScroll = EditorGUILayout.BeginScrollView(_pagesScroll);
            EditorGUILayout.BeginHorizontal();

            var assetsCount = _filteredAssets.Count;
            var pagesCount = assetsCount / ENTRIES_PER_PAGE + (assetsCount % ENTRIES_PER_PAGE > 0 ? 1 : 0);

            for (var i = 0; i < pagesCount; i++)
            {
                GUI.color = _pageSelected == i ? Color.green : Color.white;

                if (GUILayout.Button((i + 1).ToString(), GUILayout.Width(30f)))
                {
                    _pageSelected = i;
                }
            }

            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        private void DrawAssetsScroll()
        {
            _entriesScrollVector = GUILayout.BeginScrollView(_entriesScrollVector);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("#", GUILayout.Width(30f));
            EditorGUILayout.LabelField("Asset path", GUILayout.Width(320f));
            EditorGUILayout.LabelField("Asset type", GUILayout.Width(100f));
            EditorGUILayout.LabelField("Marked Resolved", GUILayout.Width(110f));
            EditorGUILayout.LabelField("Mis. Ref. Count", GUILayout.Width(100f));
            EditorGUILayout.LabelField("Field names or GameObject/Component/FieldName");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);

            var selectionEnd = Mathf.Min(_filteredAssets.Count, (_pageSelected + 1) * ENTRIES_PER_PAGE);
            for (var i = _pageSelected * ENTRIES_PER_PAGE; i < selectionEnd; i++)
            {
                var assetEntry = _filteredAssets[i];
                DrawAssetEntry(i, assetEntry);

                EditorGUILayout.Space(2);
            }

            GUILayout.EndScrollView();
        }

        private void DrawAssetEntry(int number, AssetEntry assetEntry)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(number.ToString(), GUILayout.Width(30f));

            if (!assetEntry.MarkedAsResolved)
            {
                GUI.color = Color.red;
            }

            if (GUILayout.Button(assetEntry.Path, GUILayout.Width(320f), GUILayout.Height(20f)))
            {
                var obj = AssetDatabase.LoadMainAssetAtPath(assetEntry.Path);
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }

            GUI.color = Color.white;

            EditorGUILayout.LabelField(assetEntry.Type, GUILayout.Width(100f));

            assetEntry.MarkedAsResolved = EditorGUILayout.ToggleLeft("Resolved?", assetEntry.MarkedAsResolved, GUILayout.Width(110f));

            EditorGUILayout.LabelField(assetEntry.MissingReferencesCount.ToString(), GUILayout.Width(100f));
            if (assetEntry.MissingReferencesCount == 1)
            {
                GUILayout.Label(assetEntry.MissingReferencesStrings[0]);
            }
            else if (assetEntry.MissingReferencesCount > 1)
            {
                EditorGUILayout.BeginVertical();
                assetEntry.FoldoutOpen = EditorGUILayout.Foldout(assetEntry.FoldoutOpen, assetEntry.MissingReferencesStrings[0]);
                if (assetEntry.FoldoutOpen)
                {
                    for (var index = 1; index < assetEntry.MissingReferencesStrings.Count; index++)
                    {
                        GUILayout.Label(assetEntry.MissingReferencesStrings[index]);
                    }
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void Clear()
        {
            _pageSelected = 0;
            _missingReferencesAssets.Clear();
            _filteredAssets = null;
        }
    }
}