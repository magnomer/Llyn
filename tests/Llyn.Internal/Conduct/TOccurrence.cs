using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TOccurrence
{
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
    public void OccurrenceQuerySet_UnmatchedText_EmptiesTheRowsAndWordsTheEmptyList()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TOccurrenceEntrySave(engine, "stone", null);
        (COccurrence occurrence, _, _) = TOccurrencePrepare(engine);

        Assert.Equal("Situation.Vacant", occurrence.COccurrenceEmptyKey);

        occurrence.COccurrenceQuerySet("zzz");

        Assert.Empty(occurrence.COccurrenceRowsRead());
        Assert.Equal("Situation.Unmatched", occurrence.COccurrenceEmptyKey);

        occurrence.COccurrenceQuerySet("  ");

        Assert.Equal("Situation.Vacant", occurrence.COccurrenceEmptyKey);
    }

    [Fact]
    public void OccurrenceEmptyKey_NoVista_ReadsVacant()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        COccurrence occurrence = TInterfaceConduct.TOccurrenceCreate(engine);

        occurrence.COccurrenceQuerySet("zzz");

        Assert.Equal("Situation.Vacant", occurrence.COccurrenceEmptyKey);
        Assert.Equal("entry", occurrence.COccurrenceFileRead());
    }

    private static (COccurrence, LVista, LVista) TOccurrencePrepare(LEngine engine)
    {
        COccurrence occurrence = TInterfaceConduct.TOccurrenceCreate(engine);
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
