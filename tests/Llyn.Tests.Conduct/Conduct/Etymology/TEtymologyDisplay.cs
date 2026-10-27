using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymologyDisplay
{
    [Fact]
    public void EtymologyRead_TwoBoxesLinkOneEntry_AnswersUniqueKeys()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologyAquaSave(engine);
        TEditorFixture editor = TEtymology.TEtymologyEditorPrepare(
            engine, TEtymology.TEtymologySave(engine, "water", "English"));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        long first = TEtymology.TEtymologyBoxCreate(etymology);
        etymology.CEtymologyAdd(first);
        IReadOnlyList<CAncestor> boxes = etymology.CEtymologyRead().CEtymologyStateAncestors;
        long second = boxes.Single(row => row.CAncestorId != first).CAncestorId;
        etymology.CEtymologyTargetSet(first, aqua);
        etymology.CEtymologyTargetSet(second, aqua);

        IReadOnlyList<CAncestor> rows = etymology.CEtymologyRead().CEtymologyStateAncestors;

        Assert.Equal(4, rows.Count);
        IReadOnlyList<CAncestor> borrowed = [.. rows.Where(static row => row.CAncestorBorrowed)];
        Assert.Equal(2, borrowed.Count);
        Assert.Equal(borrowed[0].CAncestorId, borrowed[1].CAncestorId);
        Assert.Equal(rows.Count, rows.Select(static row => row.CAncestorKey).Distinct().Count());
        Assert.Equal(first + "/" + borrowed[0].CAncestorId, borrowed[0].CAncestorKey);
    }

    [Fact]
    public void EtymologyRead_EngineFails_ShowsFailureNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        long water = TEtymology.TEtymologySave(engine, "water", "English");
        CEtymology etymology = TEtymology.TEtymologyEditorPrepare(
            engine, water, TEtymologyFaultPrepare(engine, asked)).TEditorFixtureEtymology;
        etymology.CEtymologyInsert(null);

        Assert.Empty(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal(["Input.AncestryFailed"], asked);
    }

    [Fact]
    public void DisplayEtymologyRead_StoredEntry_AnswersRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologyAquaSave(engine);

        CAncestor row = Assert.Single(TEtymologyList.TEtymologyStoredRead(engine, aqua));

        Assert.Equal("akwa", row.CAncestorText);
        Assert.Equal(row.CAncestorId.ToString(System.Globalization.CultureInfo.InvariantCulture), row.CAncestorKey);
        Assert.False(row.CAncestorBorrowed);
        Assert.False(row.CAncestorRestorable);
    }

    [Fact]
    public void DisplayEtymologyRead_EngineFails_ShowsFailureNoRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologyAquaSave(engine);
        List<string> asked = [];
        CDisplay display = new TEditorFixture(TEtymologyFaultPrepare(engine, asked)).TEditorFixtureDisplay;
        display.LDisplayRule.CVoice.TDisplaySoundShow(aqua, engine.TEngineEntryLoad(aqua)!);

        Assert.Empty(display.CDisplayCard.CDisplayEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal(["Display.AncestryFailed"], asked);
    }

    private static long TEtymologyAquaSave(LEngine engine)
    {
        long aqua = TEtymology.TEtymologySave(engine, "aqua", "Latin");
        TEditorFixture editor = TEtymology.TEtymologyEditorPrepare(engine, aqua);
        long root = TEtymology.TEtymologyBoxCreate(editor.TEditorFixtureEtymology);
        editor.TEditorFixtureEtymology.CEtymologyTextSet(root, "akwa");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        editor.TEditorFixtureSave();
        return aqua;
    }

    private static CEditor TEtymologyFaultPrepare(LEngine engine, List<string> asked)
    {
        return TInterfaceEditor.TEditorCreate(
            engine,
            TInterfaceConduct.TEntryBundleCreate(engine, "LAncestorPort.LEngineLineageRead", true),
            TEnvoyFake.TEnvoyCreate(false, asked));
    }
}
