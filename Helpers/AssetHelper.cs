using System;
using System.IO;

namespace ecosystem.Helpers;

public static class AssetHelper
{
    public static string GetAssetPath(string assetName)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var assetPath = Path.Combine(basePath, "Assets", assetName);
        
        if (!File.Exists(assetPath))
        {
            throw new FileNotFoundException($"Asset not found: {assetPath}");
        }
        
        return assetPath;
    }
}