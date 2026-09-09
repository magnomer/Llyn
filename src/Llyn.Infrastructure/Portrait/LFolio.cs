using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolio
{
    private const string LFolioRoot =
        "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
        + "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">"
        + "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/"
        + "relationships/officeDocument\" Target=\"word/document.xml\"/></Relationships>";

    public static void LFolioSave(LPortrait portrait, LTheme theme, string path)
    {
        ArgumentNullException.ThrowIfNull(portrait);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        List<LPortraitAsset> plates = new List<LPortraitAsset>();
        string document = LFolioBody.LFolioBodyFormat(portrait, theme, plates);

        using FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);
        using ZipArchive package = new ZipArchive(file, ZipArchiveMode.Create);

        LFolioTextSave(package, "[Content_Types].xml", LFolioBond.LFolioBondCreate(plates));
        LFolioTextSave(package, "_rels/.rels", LFolioRoot);
        LFolioTextSave(package, "word/document.xml", document);
        LFolioTextSave(package, "word/styles.xml", LFolioStyle.LFolioStyleRead(theme));
        LFolioTextSave(package, "word/_rels/document.xml.rels", LFolioBond.LFolioBondRead(plates));

        for (int place = 0; place < plates.Count; place++)
        {
            string name = "word/media/image"
                + (place + 1).ToString(CultureInfo.InvariantCulture)
                + plates[place].LPortraitAssetSuffix;

            ZipArchiveEntry entry = package.CreateEntry(name, CompressionLevel.Fastest);
            using Stream stream = entry.Open();
            stream.Write(plates[place].LPortraitAssetData, 0, plates[place].LPortraitAssetData.Length);
        }
    }

    private static void LFolioTextSave(ZipArchive package, string name, string text)
    {
        ZipArchiveEntry entry = package.CreateEntry(name, CompressionLevel.Optimal);
        using Stream stream = entry.Open();
        using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false));
        writer.Write(text);
    }
}
