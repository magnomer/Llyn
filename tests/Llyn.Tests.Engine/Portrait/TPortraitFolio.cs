using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitFolio
{
    [Fact]
    public void FolioSave_FoldedRow_PrintsRowOpen()
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".docx");

        try
        {
            TInterface.TFolioSave(TPortraitSample.TPortraitRowCreate(), TInterface.TThemeLoad(), path);

            using ZipArchive package = ZipFile.OpenRead(path);
            using Stream stream = package.GetEntry("word/document.xml")!.Open();
            using StreamReader reader = new StreamReader(stream);
            string document = reader.ReadToEnd();

            int caption = document.IndexOf("Collocations", System.StringComparison.Ordinal);
            int phrase = document.IndexOf("set off", System.StringComparison.Ordinal);
            int meaning = document.IndexOf("to start a journey", System.StringComparison.Ordinal);
            int tone = document.IndexOf("informal", System.StringComparison.Ordinal);
            Assert.True(caption >= 0);
            Assert.True(caption < phrase);
            Assert.True(phrase < meaning);
            Assert.True(meaning < tone);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FolioSave_FullPortrait_WritesReadablePackage()
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".docx");

        try
        {
            TInterface.TFolioSave(
                TPortraitSample.TPortraitSampleCreate(), TInterface.TThemeLoad(), path);

            using ZipArchive package = ZipFile.OpenRead(path);

            Assert.NotNull(package.GetEntry("[Content_Types].xml"));
            Assert.NotNull(package.GetEntry("_rels/.rels"));
            Assert.NotNull(package.GetEntry("word/document.xml"));
            Assert.NotNull(package.GetEntry("word/styles.xml"));
            Assert.NotNull(package.GetEntry("word/_rels/document.xml.rels"));

            foreach (ZipArchiveEntry entry in package.Entries.Where(
                row => row.FullName.EndsWith(".xml", System.StringComparison.Ordinal)
                    || row.FullName.EndsWith(".rels", System.StringComparison.Ordinal)))
            {
                using Stream stream = entry.Open();
                XDocument.Load(stream);
            }
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FolioSave_FullPortrait_CarriesDisplayedFields()
    {
        string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".docx");

        try
        {
            TInterface.TFolioSave(
                TPortraitSample.TPortraitSampleCreate(), TInterface.TThemeLoad(), path);

            using ZipArchive package = ZipFile.OpenRead(path);
            using Stream stream = package.GetEntry("word/document.xml")!.Open();
            using StreamReader reader = new StreamReader(stream);
            string document = reader.ReadToEnd();

            Assert.Contains("kindle", document);
            Assert.Contains("Meanings", document);
            Assert.Contains("Collocations", document);
            Assert.Contains("set alight", document);
            Assert.Contains("kindle interest", document);
            Assert.Contains("불붙이다", document);
            Assert.Contains(
                "Chiefly </w:t></w:r><w:r><w:rPr><w:i/></w:rPr><w:t xml:space=\"preserve\">literary",
                document);
            Assert.Contains(
                "<w:t>•</w:t></w:r><w:r><w:tab/></w:r><w:r><w:rPr></w:rPr><w:t xml:space=\"preserve\">poetic",
                document);
            Assert.Contains("around a hearth &lt;cold&gt;", document);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
