using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLibraryMarkup
{
    [Fact]
    public async Task LibraryMarkupImport_NoFile_AsksNothingMore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(null, static _ => true, asked));

        await library.CLibraryMarkupImport();

        Assert.Equal(["Markup"], asked);
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public async Task LibraryMarkupImport_FreshRows_StoresTheEntriesAndReportsTheOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<CMarkupEntry> shown = [];
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(
            TInterface.TMarkupSave(workspace, TInterface.TMarkupLone),
            customs =>
            {
                shown.AddRange(customs.CSCustomsEntry);
                return true;
            },
            asked));
        int refreshed = 0;
        library.CLibraryPanel.CPanelAperture.CApertureRowsChanged += () => refreshed++;

        await library.CLibraryMarkupImport();

        CMarkupEntry entry = Assert.Single(shown);
        Assert.Equal("ember", entry.CMarkupEntryName);
        Assert.Empty(entry.CMarkupEntryTarget);
        Assert.Equal(1, refreshed);
        Assert.Equal(["Markup", "Customs", "Omission"], asked.Select(static question => question.Split(':')[0]));
        Assert.Contains("braise", asked[2], StringComparison.Ordinal);
        Assert.Equal(["ember"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public async Task LibraryMarkupImport_MergeRow_AppendsToTheTargetEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry ember = TLibrary.TLibraryEntrySave(engine, "ember", "English");
        List<CMarkupEntry> shown = [];
        CSCustomsRow? replaced = null;
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(
            TInterface.TMarkupSave(workspace, TInterface.TMarkupLone),
            customs =>
            {
                shown.AddRange(customs.CSCustomsEntry);
                customs.CSCustomsModeSet(0, CSCustomsMode.CSCustomsModeReplace);
                replaced = customs.CSCustomsRowRead(0);
                customs.CSCustomsModeSet(0, CSCustomsMode.CSCustomsModeMerge);
                return customs.CSCustomsTargetSet(0, ember.LEntryId);
            },
            []));

        await library.CLibraryMarkupImport();

        Assert.Equal([ember.LEntryId], Assert.Single(shown).CMarkupEntryTarget);
        Assert.Equal(
            new CSCustomsRow(CSCustomsMode.CSCustomsModeReplace, ember.LEntryId, true, "Customs.Loss", 1, 0), replaced);
        Assert.Equal(ember.LEntryId, Assert.Single(library.CLibraryRowsRead()).CVistaRowId);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Equal(2, draft.LEntryDraftMeanings.Count);
    }

    [Fact]
    public async Task LibraryMarkupImport_CleanFile_ReportsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        string file = TInterface.TMarkupSave(
            workspace, "<llyn><entry><headword>ember</headword><language>English</language></entry></llyn>");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(file, static _ => true, asked));

        await library.CLibraryMarkupImport();

        Assert.Equal(["Markup", "Customs:1"], asked);
        Assert.Equal(["ember"], library.CLibraryRowsRead().Select(row => row.CVistaRowHeadword));
    }

    [Fact]
    public async Task LibraryMarkupImport_Declined_StoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        string file = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(file, static _ => false, asked));
        int refreshed = 0;
        library.CLibraryPanel.CPanelAperture.CApertureRowsChanged += () => refreshed++;

        await library.CLibraryMarkupImport();

        Assert.Equal(["Markup", "Customs:1"], asked);
        Assert.Equal(0, refreshed);
        Assert.Empty(library.CLibraryRowsRead());
    }

    [Fact]
    public async Task LibraryMarkupImport_MalformedFile_ShowsTheImportFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        string file = TInterface.TMarkupSave(workspace, "<llyn><entry><headword>ember</headword></llyn>");
        CLibrary library = TLibrary.TLibraryPrepare(atelier, TLibraryEnvoyCreate(file, static _ => true, asked));

        await library.CLibraryMarkupImport();

        Assert.Equal(["Markup", "List.ImportFailed"], asked);
        Assert.Empty(library.CLibraryRowsRead());
    }


    private static CEnvoy TLibraryEnvoyCreate(
        string? file, Func<CSCustoms, bool> customs, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyMarkupRead"] = _ =>
            {
                asked.Add("Markup");
                return file;
            },
            ["CEnvoyCustomsRead"] = args =>
            {
                CSCustoms declared = (CSCustoms)args![0]!;
                asked.Add($"Customs:{declared.CSCustomsEntry.Count}");
                return customs(declared);
            },
            ["CEnvoyOmissionShow"] = args =>
            {
                IReadOnlyList<CMarkupOmission> omissions = (IReadOnlyList<CMarkupOmission>)args![0]!;
                asked.Add(
                    "Omission:" + string.Join("|", omissions.Select(static omission => omission.CMarkupOmissionText)));
                return null;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

}
