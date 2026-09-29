using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexRow
{
    [Fact]
    public void ReflexLanguageKey_NamedOrBlank_PrefixesTheReflexKey()
    {
        CReflex named = TReflexRowCreate("Wu", "Go-on", false);
        CReflex blank = TReflexRowCreate(string.Empty, string.Empty, false);

        Assert.Equal("Reflex.Wu", named.CReflexLanguageKey);
        Assert.Equal("Reflex.Go-on", named.CReflexKindKey);
        Assert.Equal("Reflex.", blank.CReflexLanguageKey);
        Assert.Equal("Reflex.", blank.CReflexKindKey);
    }

    [Fact]
    public void ReflexLeadRead_ThreeRuns_LeadsEachRunOnce()
    {
        Assert.Equal([true, false, true, true], TInterfaceConduct.TReflexLeadRead(["Wu", "Wu", "Jin", "Wu"]));
        Assert.Equal([true, true], TInterfaceConduct.TReflexLeadRead(["Wu", "Wu "]));
        Assert.Empty(TInterfaceConduct.TReflexLeadRead([]));
    }

    [Fact]
    public void EditorLeadRead_TypedLanguage_LeadsByTheOverlaidRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "水",
            "Chinese",
            string.Empty,
            string.Empty,
            [],
            [],
            reflexes:
            [
                TInterface.TReflexDraftCreate("Wu", string.Empty, "sy"),
                TInterface.TReflexDraftCreate("Wu", string.Empty, "si"),
                TInterface.TReflexDraftCreate("Jin", string.Empty, "sui"),
            ]));
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        Assert.Empty(editor.CEditorLeadRead(1, "Wu"));

        editor.CEditorEntryOpen(entry.LEntryId);
        IReadOnlyList<long> ids =
            editor.CEditorDraftRead()!.CEntryDraftReflexes.Select(static row => row.CReflexDraftId).ToList();

        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], false), new CReflexHead(ids[2], true)],
            editor.CEditorLeadRead(ids[1], "Wu"));
        Assert.Equal(
            [new CReflexHead(ids[0], true), new CReflexHead(ids[1], true), new CReflexHead(ids[2], false)],
            editor.CEditorLeadRead(ids[1], "Jin"));
    }

    [Fact]
    public void ReflexHiddenCheck_FoldAndOpening_HidesAFoldedRowWhileClosed()
    {
        Assert.True(TReflexRowCreate("Jin", string.Empty, true).CReflexHiddenCheck(false));
        Assert.False(TReflexRowCreate("Jin", string.Empty, true).CReflexHiddenCheck(true));
        Assert.False(TReflexRowCreate("Wu", string.Empty, false).CReflexHiddenCheck(false));
    }

    private static CReflex TReflexRowCreate(string language, string kind, bool folded)
    {
        return new CReflex(
            1, language, kind, "ipa", string.Empty, string.Empty, string.Empty, false, string.Empty, [],
            new CRespellingMark(false, string.Empty, string.Empty), folded, true);
    }
}
