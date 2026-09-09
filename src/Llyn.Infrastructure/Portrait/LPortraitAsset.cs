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
}
