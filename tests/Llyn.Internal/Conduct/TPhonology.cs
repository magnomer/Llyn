using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPhonology
{
    [Fact]
    public void PhonologyRowsRead_NoVistaRestored_AnswersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        CPhonology phonology = CPhonology.CPhonologyCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Empty(phonology.CPhonologyRowsRead());
        Assert.True(phonology.CPhonologyEmpty);
        Assert.False(phonology.CPhonologyFiltered);
    }

    [Fact]
    public void PhonologyQuerySet_MatchingText_NarrowsToMatchingEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        TPhonologyEntrySave(engine, "fire", "faɪə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyQuerySet("wat");

        Assert.Equal(
            ["water"],
            phonology.CPhonologyRowsRead().Select(row => row.CCatalogPronunciationEntry.CVistaRowHeadword));
        Assert.False(phonology.CPhonologyEmpty);
    }

    [Fact]
    public void PhonologyQuerySet_UnmatchedText_ReadsEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyQuerySet("zzz");

        Assert.Empty(phonology.CPhonologyRowsRead());
        Assert.True(phonology.CPhonologyEmpty);
    }

    [Fact]
    public void PhonologyRowsRead_ChosenRow_MarksThatRowAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        LEntry fire = TPhonologyEntrySave(engine, "fire", "faɪə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyPanel.CPanelRowSelect(fire.LEntryId);

        Assert.Equal(["fire"], phonology.CPhonologyRowsRead()
            .Where(row => row.CCatalogPronunciationEntry.CVistaRowChosen)
            .Select(row => row.CCatalogPronunciationEntry.CVistaRowHeadword));
    }

    [Fact]
    public void PhonologyRowsRead_StoredEntry_CarriesEveryField()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        CCatalogPronunciation row = Assert.Single(phonology.CPhonologyRowsRead());

        Assert.Equal(
            new CCatalogPronunciation(
                new CVistaRow(water.LEntryId, "water", "English", string.Empty, "water", false), "ˈwɔːtə"),
            row);
    }

    [Fact]
    public void PhonologyOrderSet_NullAfterAnOrder_KeepsTheChosenOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        TPhonologyEntrySave(engine, "fire", "faɪə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyOrderSet(CCatalogOrder.CCatalogOrderReverse);
        phonology.CPhonologyOrderSet(null);

        Assert.Equal(CCatalogOrder.CCatalogOrderReverse, phonology.CPhonologyPanel.CPanelOrder);
        Assert.Equal(
            ["water", "fire"],
            phonology.CPhonologyRowsRead().Select(row => row.CCatalogPronunciationEntry.CVistaRowHeadword));
    }

    [Fact]
    public void PhonologyFilterSet_HiddenLanguage_MarksThePanelFiltered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyFilterSet(new CCatalogFilter(["English"]));

        Assert.True(phonology.CPhonologyFiltered);
        Assert.Empty(phonology.CPhonologyRowsRead());

        phonology.CPhonologyFilterSet(new CCatalogFilter([]));

        Assert.False(phonology.CPhonologyFiltered);
    }

    [Fact]
    public async Task PhonologyPortraitExport_ChosenEntry_WritesItAndNothingBefore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        string path = Path.Combine(workspace.TWorkspaceFolder, "water.md");
        CPhonology phonology = TPhonologyPrepare(
            atelier, TEnvoyFake.TEnvoyFileCreate(path, CPortraitMedium.CPortraitMediumMarkdown, []));

        await phonology.CPhonologyPortraitExport();

        Assert.False(File.Exists(path));

        phonology.CPhonologyPanel.CPanelRowSelect(water.LEntryId);
        await phonology.CPhonologyPortraitExport();

        Assert.Contains("water", File.ReadAllText(path), StringComparison.Ordinal);
        Assert.Contains("water", phonology.TPhonologyFileRead(), StringComparison.Ordinal);
    }

    [Fact]
    public void PhonologyPanelRowOpen_ScribeOn_OpensTheEntryInTheEditorUntilClosed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LEntry water = TPhonologyEntrySave(engine, "water", "ˈwɔːtə");
        CPhonology phonology = TPhonologyPrepare(atelier);

        phonology.CPhonologyPanel.CPanelRowOpen(water.LEntryId);
        phonology.CPhonologyPanel.CPanelScribeToggle(true);

        Assert.Equal(water.LEntryId, phonology.CPhonologyEditor.CEditorDesk.CDeskStoredRead());

        phonology.CPhonologyPanel.CPanelEntryClose();

        Assert.Null(phonology.CPhonologyEditor.CEditorDesk.CDeskStoredRead());
    }

    private static CPhonology TPhonologyPrepare(CAtelier atelier)
    {
        return TPhonologyPrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
    }

    private static CPhonology TPhonologyPrepare(CAtelier atelier, CEnvoy envoy)
    {
        CPhonology phonology = CPhonology.CPhonologyCreate(atelier, static () => true, envoy);
        phonology.CPhonologyVistaRestore();
        return phonology;
    }

    private static LEntry TPhonologyEntrySave(LEngine engine, string headword, string sound)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", sound, string.Empty, [TInterface.TCardCreate("a meaning", 1)], []));
    }
}
