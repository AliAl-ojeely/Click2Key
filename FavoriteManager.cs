using System;
using System.Collections.Generic;
using System.IO;

namespace Click2Key
{
    public static class FavoriteManager
    {
        private static readonly string favFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Click2Key",
            "favorites.txt");

        // Returns the keys in the order they were saved (first starred = first entry)
        public static List<string> LoadFavorites()
        {
            if (!File.Exists(favFilePath))
                return new List<string>();
            return new List<string>(File.ReadAllLines(favFilePath));
        }

        // Saves the keys in the given order (preserves starring order)
        public static void SaveFavorites(IEnumerable<string> favoriteKeys)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(favFilePath));
            File.WriteAllLines(favFilePath, favoriteKeys);
        }
    }
}