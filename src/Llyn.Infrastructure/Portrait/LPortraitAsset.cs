using System;
using System.IO;

namespace Llyn.Infrastructure;

public sealed record LPortraitAsset(
    byte[] LPortraitAssetData,
    string LPortraitAssetMedia,
    string LPortraitAssetSuffix)
{
    private const long LPortraitAssetCeiling = 24L * 1024L * 1024L;

    public string LPortraitAssetAddress =>
        "data:" + LPortraitAssetMedia + ";base64," + Convert.ToBase64String(LPortraitAssetData);

    public static LPortraitAsset? LPortraitAssetLoad(string location)
    {
        string trimmed = (location ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return null;
        }

        if (trimmed.StartsWith("data:", StringComparison.Ordinal))
        {
            return LPortraitAssetParse(trimmed);
        }

        string path = trimmed;
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? address))
        {
            if (!address.IsFile)
            {
                return null;
            }

            path = address.LocalPath;
        }

        try
        {
            FileInfo file = new FileInfo(path);
            if (!file.Exists || file.Length > LPortraitAssetCeiling)
            {
                return null;
            }

            string suffix = file.Extension.ToLowerInvariant();
            string media = suffix switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".tif" or ".tiff" => "image/tiff",
                ".svg" => "image/svg+xml",
                _ => "image/jpeg",
            };

            return new LPortraitAsset(File.ReadAllBytes(file.FullName), media, suffix);
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static LPortraitAsset? LPortraitAssetParse(string address)
    {
        int comma = address.IndexOf(',', StringComparison.Ordinal);
        if (comma < 0)
        {
            return null;
        }

        string head = address.Substring(5, comma - 5);
        int semicolon = head.IndexOf(';', StringComparison.Ordinal);
        string media = semicolon < 0 ? head : head.Substring(0, semicolon);
        if (!head.EndsWith(";base64", StringComparison.Ordinal))
        {
            return null;
        }

        byte[] data;
        try
        {
            data = Convert.FromBase64String(address.Substring(comma + 1));
        }
        catch (FormatException)
        {
            return null;
        }

        string suffix = media switch
        {
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/webp" => ".webp",
            "image/tiff" => ".tif",
            "image/svg+xml" => ".svg",
            _ => ".jpg",
        };

        return new LPortraitAsset(data, media, suffix);
    }
}
