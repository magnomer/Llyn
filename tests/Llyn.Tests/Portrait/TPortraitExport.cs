using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitExport
{
    [Fact]
    public async Task PortraitExport_Exemplar_HtmlCarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.html");
        await engine.TEnginePortraitExport(
            id, path, LPortraitFormat.LPortraitFormatHtml, TInterface.TPortraitLabelRead());

        TPortraitExportMatch(engine, id, File.ReadAllText(path), TInterface.TSheetNormalize);
    }

    [Fact]
    public async Task PortraitExport_Exemplar_MarkdownCarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.md");
        await engine.TEnginePortraitExport(
            id, path, LPortraitFormat.LPortraitFormatMarkdown, TInterface.TPortraitLabelRead());

        TPortraitExportMatch(engine, id, File.ReadAllText(path), TInterface.TOutlineNormalize);
    }

    [Fact]
    public async Task PortraitExport_Exemplar_DocxCarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.docx");
        await engine.TEnginePortraitExport(
            id, path, LPortraitFormat.LPortraitFormatDocx, TInterface.TPortraitLabelRead());

        string document;
        using (ZipArchive package = ZipFile.OpenRead(path))
        using (Stream stream = package.GetEntry("word/document.xml")!.Open())
        using (StreamReader reader = new(stream))
        {
            document = reader.ReadToEnd();
        }

        TPortraitExportMatch(engine, id, document, TInterface.TFolioLineFormat);
    }

    [Fact]
    public void PortraitRead_Exemplar_CarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntryDraft draft = TExemplar.TExemplarCreate(engine);
        long id = TExemplar.TExemplarSave(engine, draft)[0];

        string shown = string.Join(
            "\n", TInterface.TPortraitTextRead(engine.TEnginePortraitRead(id, TInterface.TPortraitLabelRead())));

        List<string> missing = [];
        List<string> stale = [];
        foreach (string text in TExemplar.TExemplarTextRead(draft))
        {
            bool hidden = System.Array.IndexOf(TExemplar.TExemplarHidden, text) >= 0;
            bool carried = shown.Contains(text, System.StringComparison.Ordinal);
            if (!hidden && !carried)
            {
                missing.Add(text);
            }
            else if (hidden && carried)
            {
                stale.Add(text);
            }
        }

        Assert.True(missing.Count == 0, "Dropped by the reader: " + string.Join(" | ", missing));
        Assert.True(stale.Count == 0, "Hidden but shown: " + string.Join(" | ", stale));
    }

    [Fact]
    public void PortraitExport_Exemplar_ChildCardNested()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        LPortraitPage page = engine.TEnginePortraitRead(id, TInterface.TPortraitLabelRead());

        LPortraitSection parent = Assert.Single(
            TPortraitExportFind(page.LPortraitPageSection, "exemplar-title"));
        LPortraitSection child = Assert.Single(
            TPortraitExportFind(parent.LPortraitSectionChild, "exemplar-child-title"));
        Assert.Same(child, Assert.Single(TPortraitExportFind(page.LPortraitPageSection, "exemplar-child-title")));
    }

    private static void TPortraitExportMatch(
        LEngine engine, long id, string written, System.Func<string?, string> normalize)
    {
        IReadOnlyList<string> shown = TInterface.TPortraitTextRead(
            engine.TEnginePortraitRead(id, TInterface.TPortraitLabelRead()));

        Assert.Contains("exemplar-child-meaning", shown);
        List<string> missing = [];
        foreach (string text in shown)
        {
            if (!written.Contains(normalize(text), System.StringComparison.Ordinal))
            {
                missing.Add(text);
            }
        }

        Assert.True(missing.Count == 0, "Dropped: " + string.Join(" | ", missing));
    }

    private static IReadOnlyList<LPortraitSection> TPortraitExportFind(
        IReadOnlyList<LPortraitSection> sections, string heading)
    {
        List<LPortraitSection> found = [];
        foreach (LPortraitSection section in sections)
        {
            if (section.LPortraitSectionHeading == heading)
            {
                found.Add(section);
            }
            else
            {
                found.AddRange(TPortraitExportFind(section.LPortraitSectionChild, heading));
            }
        }

        return found;
    }
}
