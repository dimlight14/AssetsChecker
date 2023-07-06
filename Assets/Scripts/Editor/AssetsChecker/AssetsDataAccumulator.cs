using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace DefaultNamespace
{
    public class AssetsDataAccumulator
    {
        private const string SCRIPTS_FOLDER_NAME = "Scripts/";
        private const string PACKAGES_FOLDER_NAME = "Packages/";
        private const string SCENES_FOLDER_NAME = "Scenes/";

        private List<string> _directoriesFilter;
        private AssetEntry _currentAssetEntry;
        private List<AssetEntry> _missingReferencesAssets;

        public void AccumulateMissingReferences(List<AssetEntry> missingReferencesAssets, bool includeScriptsFolder, bool includePackages, bool includeScenes)
        {
            _missingReferencesAssets = missingReferencesAssets;
            SetDirectoriesFilter(includePackages, includeScriptsFolder, includeScenes);

            var assetPaths = AssetDatabase.GetAllAssetPaths().ToList();
            var count = 0;
            foreach (var assetPath in assetPaths)
            {
                count++;
                EditorUtility.DisplayProgressBar("Missing References", "Searching for missing references",
                    (float)count / assetPaths.Count);
                if (!FilterByDirectories(assetPath))
                {
                    continue;
                }

                var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (type == null)
                {
                    AddAssetEntry(assetPath, "Unidentified", "Unidentified asset, possibly a missing script");
                    continue;
                }

                if (!IsValidType(type))
                {
                    continue;
                }

                if (includeScenes && type == typeof(SceneAsset))
                {
                    CheckSceneObject(assetPath);
                    continue;
                }

                var obj = AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (type == typeof(GameObject))
                {
                    CheckGameObject(obj as GameObject, assetPath, false);
                    continue;
                }

                using var serializedObject = new SerializedObject(obj);
                var serializedProperty = serializedObject.GetIterator();

                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (serializedProperty.objectReferenceValue == null && serializedProperty.objectReferenceInstanceIDValue != 0)
                        {
                            AddAssetEntry(assetPath, type.ToString(), serializedProperty.displayName);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private void CheckSceneObject(string assetPath)
        {
            Scene openScene;
            try
            {
                openScene = EditorSceneManager.OpenScene(assetPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not open scene at path {assetPath}. Error: {ex.Message}");
                return;
            }

            var rootObjects = openScene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                CheckGameObject(rootObject, openScene.path, true);
            }
        }

        private void AddAssetEntry(string assetPath, string type, string missingReference)
        {
            if (_currentAssetEntry == null || _currentAssetEntry.Path != assetPath)
            {
                var shortType = type.Substring(type.LastIndexOf('.') + 1);
                _currentAssetEntry = new AssetEntry(assetPath, shortType, missingReference);
                _missingReferencesAssets.Add(_currentAssetEntry);
            }
            else
            {
                _currentAssetEntry.AddMissingReference(missingReference);
            }
        }

        private void CheckGameObject(GameObject gameObject, string path, bool partOfScene)
        {
            if (PrefabUtility.IsPrefabAssetMissing(gameObject) || gameObject.name.Contains("Missing Prefab with guid"))
            {
                AddAssetEntry(path, partOfScene ? "Scene" : "Prefab", gameObject.name);
                return;
            }

            var components = gameObject.GetComponents<Component>();
            for (var j = 0; j < components.Length; j++)
            {
                var component = components[j];
                if (!component)
                {
                    AddAssetEntry(path, partOfScene ? "Scene" : "Prefab", gameObject.name + "/" + "Missing script");
                    continue;
                }

                var serializedObject = new SerializedObject(component);
                var serializedProperty = serializedObject.GetIterator();

                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (serializedProperty.objectReferenceValue == null && serializedProperty.objectReferenceInstanceIDValue != 0)
                        {
                            var typeVal = component.GetType().ToString();
                            var shortType = typeVal.Substring(typeVal.LastIndexOf('.') + 1);
                            var refName = GetFullObjectPath(gameObject.transform) + "/" + shortType + "/" + serializedProperty.displayName;
                            AddAssetEntry(path, partOfScene ? "Scene" : "Prefab", refName);
                        }
                    }
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                CheckGameObject(child.gameObject, path, partOfScene);
            }
        }

        private void SetDirectoriesFilter(bool includePackages, bool includeScriptsFolder, bool includeScenes)
        {
            if (includePackages && includeScriptsFolder && includeScenes)
            {
                _directoriesFilter = null;
                return;
            }
            
            if (_directoriesFilter != null)
            {
                _directoriesFilter.Clear();
            }
            else
            {
                _directoriesFilter = new List<string>(3);
            }

            if (!includePackages)
            {
                _directoriesFilter.Add(PACKAGES_FOLDER_NAME);
            }

            if (!includeScriptsFolder)
            {
                _directoriesFilter.Add(SCRIPTS_FOLDER_NAME);
            }

            if (!includeScenes)
            {
                _directoriesFilter.Add(SCENES_FOLDER_NAME);
            }
        }

        private bool FilterByDirectories(string path)
        {
            if (_directoriesFilter == null)
            {
                return true;
            }
            
            foreach (var directory in _directoriesFilter)
            {
                if (path.Contains(directory))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidType(Type type)
        {
            if (type == null || type == typeof(DefaultAsset) || type == typeof(MonoScript) || type == typeof(TextAsset))
                return false;

            return true;
        }

        private string GetFullObjectPath(Transform gameObjectTransform)
        {
            if (gameObjectTransform.parent == null)
                return gameObjectTransform.name;

            return GetFullObjectPath(gameObjectTransform.parent) + "/" + gameObjectTransform.name;
        }
    }
}