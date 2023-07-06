using System.Collections.Generic;

namespace DefaultNamespace
{
    public class AssetEntry
    {
        public string Path { get; }
        public string Type { get; }
        public int MissingReferencesCount { get; private set; }
        public List<string> MissingReferencesStrings { get; private set; }
        public bool MarkedAsResolved = false;
        public bool FoldoutOpen = false;

        public AssetEntry(string path, string type, string missingType)
        {
            Path = path;
            Type = type;
            MissingReferencesCount = 1;
            MissingReferencesStrings = new List<string>() { missingType };
        }

        public void AddMissingReference(string missingType)
        {
            MissingReferencesCount++;
            MissingReferencesStrings.Add(missingType);
        }
    }
}