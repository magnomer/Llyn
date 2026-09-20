using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Infrastructure;

public static class LFolioBond
{
    public static string LFolioBondRead(IReadOnlyList<LPortraitAsset> plates)
    {
        ArgumentNullException.ThrowIfNull(plates);

        StringBuilder bonds = new StringBuilder();

        bonds.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
            .Append("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">")
            .Append("<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/")
            .Append("officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>");

        for (int place = 0; place < plates.Count; place++)
        {
            bonds.Append("<Relationship Id=\"rId")
                .Append((place + 101).ToString(CultureInfo.InvariantCulture))
                .Append("\" Type=\"http://schemas.openxmlformats.org/")
                .Append("officeDocument/2006/relationships/image\" Target=\"media/image")
                .Append((place + 1).ToString(CultureInfo.InvariantCulture))
                .Append(plates[place].LPortraitAssetSuffix)
                .Append("\"/>");
        }

        bonds.Append("</Relationships>");
        return bonds.ToString();
    }

    public static string LFolioBondCreate(IReadOnlyList<LPortraitAsset> plates)
    {
        ArgumentNullException.ThrowIfNull(plates);

        HashSet<string> suffixes = new HashSet<string>(StringComparer.Ordinal);
        foreach (LPortraitAsset plate in plates)
        {
            suffixes.Add(plate.LPortraitAssetSuffix.TrimStart('.'));
        }

        StringBuilder types = new StringBuilder();

        types.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
            .Append("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">")
            .Append("<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.")
            .Append("relationships+xml\"/>")
            .Append("<Default Extension=\"xml\" ContentType=\"application/xml\"/>");

        foreach (string suffix in suffixes)
        {
            string media = suffix switch
            {
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "webp" => "image/webp",
                "tif" or "tiff" => "image/tiff",
                "svg" => "image/svg+xml",
                _ => "image/jpeg",
            };

            types.Append("<Default Extension=\"").Append(suffix)
                .Append("\" ContentType=\"").Append(media).Append("\"/>");
        }

        types.Append("<Override PartName=\"/word/document.xml\" ContentType=\"application/vnd.")
            .Append("openxmlformats-officedocument.wordprocessingml.document.main+xml\"/>")
            .Append("<Override PartName=\"/word/styles.xml\" ContentType=\"application/vnd.")
            .Append("openxmlformats-officedocument.wordprocessingml.styles+xml\"/>")
            .Append("</Types>");

        return types.ToString();
    }
}
