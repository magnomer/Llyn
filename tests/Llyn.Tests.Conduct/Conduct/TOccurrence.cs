using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TOccurrence
{
    [Fact]
    public async Task OccurrenceRowsLoad_ChosenSituation_AnswersTheReferencingEntriesAfterTheFill()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation home = TOccurrenceSituationSave(engine, "at home");
        TOccurrenceEntrySave(engine, "hearth", home);
        (COccurrence occurrence, _, _) = TOccurrencePrepare(engine);

        CEnsignSheet<IReadOnlyList<CVistaRow>> sheet =
            await occurrence.COccurrenceRowsLoad(static (_, _) => static () => { });

        Assert.Equal(
            occurrence.COccurrenceRowsRead().Select(static row => row.CVistaRowHeadword),
            sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
        Assert.Contains("hearth", sheet.CEnsignSheetRows.Select(static row => row.CVistaRowHeadword));
    }

    [Fact]
    public void OccurrenceRowsRead_ChosenSituation_ListsTheReferencingEntries()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation home = TOccurrenceSituationSave(engine, "at home");
        LEntry hearth = TOccurrenceEntrySave(engine, "hearth", home);
        TOccurrenceEntrySave(engine, "stone", null);
        (COccurrence occurrence, LVista roll, _) = TOccurrencePrepare(engine);

        Assert.Equal(["hearth", "stone"], occurrence.COccurrenceRowsRead().Select(row => row.CVistaRowHeadword));

        roll.TVistaSelect(home.LSituationId);

        Assert.Equal(hearth.LEntryId, Assert.Single(occurrence.COccurrenceRowsRead()).CVistaRowId);
    }

    [Fact]
    public void OccurrenceRowsRead_EngineFails_ShowsTheLoadFailureAndAnswersNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];

        Assert.Empty(TInterfaceMention.TOccurrenceFailRead(engine, TEnvoyFake.TEnvoyCreate(false, asked)));
        Assert.Equal(["Situation.LoadFailed"], asked);
    }

    [Fact]
    public void OccurrenceQuerySet_UnmatchedText_EmptiesTheRowsAndWordsTheEmptyList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TOccurrenceEntrySave(engine, "stone", null);
        (COccurrence occurrence, _, _) = TOccurrencePrepare(engine);

        Assert.Equal("Situation.Vacant", occurrence.COccurrencePanel.CPanelAperture.CApertureKey);

        occurrence.COccurrencePanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Empty(occurrence.COccurrenceRowsRead());
        Assert.Equal("Situation.Unmatched", occurrence.COccurrencePanel.CPanelAperture.CApertureKey);

        occurrence.COccurrencePanel.CPanelAperture.CApertureQuerySet("  ");

        Assert.Equal("Situation.Vacant", occurrence.COccurrencePanel.CPanelAperture.CApertureKey);
    }

    [Fact]
    public void OccurrenceEmptyKey_NoVista_ReadsVacant()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        COccurrence occurrence = TInterfaceConductPanel.TOccurrenceCreate(engine);

        occurrence.COccurrencePanel.CPanelAperture.CApertureQuerySet("zzz");

        Assert.Equal("Situation.Vacant", occurrence.COccurrencePanel.CPanelAperture.CApertureKey);
        Assert.Equal("entry", occurrence.TOccurrenceFileRead());
    }

    private static (COccurrence, LVista, LVista) TOccurrencePrepare(LEngine engine)
    {
        COccurrence occurrence = TInterfaceConductPanel.TOccurrenceCreate(engine);
        LVista roll = engine.TEngineVistaStart("repertoire", LCatalogOrder.LCatalogOrderName);
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);
        occurrence.TOccurrenceVistaRestore(roll, vista);
        return (occurrence, roll, vista);
    }

    private static LSituation TOccurrenceSituationSave(LEngine engine, string title)
    {
        return engine.TEngineSituationCreate(TInterface.TSituationCreate(0, title, null, null));
    }

    private static LEntry TOccurrenceEntrySave(LEngine engine, string headword, LSituation? situation)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword,
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                string.Empty,
                string.Empty,
                "a meaning",
                [],
                situation is null
                    ? []
                    : [TInterface.TSituationDraftCreate(situation.LSituationTitle, situation.LSituationId)],
                [],
                [],
                [],
                1)],
            []));
    }
}
