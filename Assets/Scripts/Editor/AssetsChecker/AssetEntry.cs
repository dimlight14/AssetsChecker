using System.Collections.Generic;

namespace DefaultNamespace
{
    public class AssetEntry
    {
        public string Path { get; }
        public string Type { get; }
        public int ErrorCount { get; private set; }
        public List<string> ErrorStrings { get; private set; }
        public bool MarkedAsResolved = false;
        public bool FoldoutOpen = false;

        public AssetEntry(string path, string type, string missingType)
        {
            Path = path;
            Type = type;
            ErrorCount = 1;
            ErrorStrings = new List<string>() { missingType };
        }

        public void AddMissingReference(string missingType)
        {
            ErrorCount++;
            ErrorStrings.Add(missingType);
        }
    }
}