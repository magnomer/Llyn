using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitExporting
{
    [Fact]
    public async Task PortraitExport_Exemplar_HtmlCarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.html");
        LPortraitLabel label = TInterface.TPortraitLabelRead();
        await engine.TEnginePortraitExport(id, path, LPortraitMedium.LPortraitMediumHtml, label);

        string[] unshown =
        [
            label.LPortraitLabelGlyph,
            label.LPortraitLabelFrequency,
            label.LPortraitLabelForm,
            label.LPortraitLabelParadigm,
            "Korean exemplar-kind exemplar-region ●",
            "exemplar-reflex-reading exemplar-reflex-romanization exemplar-reflex-meaning exemplar-reflex-note",
            "English pattern",
            "Korean exemplar-ancestor-text ‘exemplar-ancestor-gloss’",
        ];
        TPortraitExportMatch(engine, id, File.ReadAllText(path), TInterface.TSheetNormalize, unshown);
    }

    [Fact]
    public async Task PortraitExport_ExemplarPdf_HandsTheEntrySheetToThePress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];
        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.pdf");

        LPortraitMedium medium = LPortraitMedium.LPortraitMediumPdf;
        await engine.TEnginePortraitExport(id, path, medium, TInterface.TPortraitLabelRead());

        TPress press = workspace.TWorkspacePress;
        Assert.Equal(path, press.TPressPath);
        Assert.Contains("<main class=\"entry\">", press.TPressHtml, System.StringComparison.Ordinal);
        Assert.DoesNotContain("<main class=\"portrait\">", press.TPressHtml, System.StringComparison.Ordinal);
    }

    [Fact]
    public async Task PortraitPrint_StoredEntry_HandsTheEntrySheetToThePress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry water = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));

        await using LGalleyRelay? galley = await engine.TEnginePressOpen(
            water.LEntryId, TInterface.TPortraitLabelRead());

        TPress press = workspace.TWorkspacePress;
        Assert.True(press.TPressOpened);
        Assert.Contains("<main class=\"entry\">", press.TPressHtml, System.StringComparison.Ordinal);
        Assert.Contains("<h1 class=\"headword\"", press.TPressHtml, System.StringComparison.Ordinal);
        Assert.DoesNotContain("<main class=\"portrait\">", press.TPressHtml, System.StringComparison.Ordinal);
    }

    [Fact]
    public async Task PortraitExport_Exemplar_MarkdownCarriesEveryText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TExemplar.TExemplarCreate(engine))[0];

        string path = Path.Combine(workspace.TWorkspaceFolder, "exemplar.md");
        await engine.TEnginePortraitExport(
            id, path, LPortraitMedium.LPortraitMediumMarkdown, TInterface.TPortraitLabelRead());

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
            id, path, LPortraitMedium.LPortraitMediumDocx, TInterface.TPortraitLabelRead());

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
        foreach (string text in TExemplarText.TExemplarTextRead(draft))
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
        LEngine engine,
        long id,
        string written,
        System.Func<string?, string> normalize,
        IReadOnlyList<string>? unshown = null)
    {
        unshown ??= [];
        LPortraitPage page = engine.TEnginePortraitRead(id, TInterface.TPortraitLabelRead());
        page = page with
        {
            LPortraitPageSection =
            [
                .. page.LPortraitPageSection.Where(section => !unshown.Contains(section.LPortraitSectionHeading)),
            ],
        };
        IReadOnlyList<string> shown = TInterface.TPortraitTextRead(page);

        Assert.Contains("exemplar-child-meaning", shown);
        List<string> missing = [];
        foreach (string text in shown)
        {
            if (!unshown.Contains(text) && !written.Contains(normalize(text), System.StringComparison.Ordinal))
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
