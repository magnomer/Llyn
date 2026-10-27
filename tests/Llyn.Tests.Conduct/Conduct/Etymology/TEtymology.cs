using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymology
{
    [Fact]
    public void EtymologyRead_EmptyDesk_AnswersNoRows()
    {
        CEtymology etymology = TTimbre.TTimbreEditorPrepare([]).TEditorFixtureEtymology;

        Assert.Empty(etymology.CEtymologyRead().CEtymologyStateAncestors);
    }

    [Fact]
    public void EtymologyInsert_NoBox_AnswersOneEmptyRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));

        etymology.CEtymologyInsert(null);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal(0, row.CAncestorDepth);
        Assert.Equal(string.Empty, row.CAncestorText);
        Assert.False(row.CAncestorLinked);
        Assert.False(row.CAncestorBorrowed);
        Assert.False(row.CAncestorRestorable);
    }

    [Fact]
    public void EtymologyInsert_AboveBox_AnswersNewBoxOneLevelHigher()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyInsert(first);

        IReadOnlyList<CAncestor> rows = etymology.CEtymologyRead().CEtymologyStateAncestors;
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CAncestorId);
        Assert.Equal([0, 1], rows.Select(static row => row.CAncestorDepth));
    }

    [Fact]
    public void EtymologyAdd_Box_AnswersCoSourceBeside()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyAdd(first);

        IReadOnlyList<CAncestor> rows = etymology.CEtymologyRead().CEtymologyStateAncestors;
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CAncestorId);
        Assert.All(rows, static row => Assert.Equal(0, row.CAncestorDepth));
    }

    [Fact]
    public void EtymologyRemove_BoxWithSource_AnswersSourceInPlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);
        etymology.CEtymologyInsert(first);
        long source = etymology.CEtymologyRead().CEtymologyStateAncestors[1].CAncestorId;

        etymology.CEtymologyRemove(first);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal(source, row.CAncestorId);
        Assert.Equal(0, row.CAncestorDepth);
    }

    [Fact]
    public void EtymologyTextSet_TextBox_AnswersTypedTextAfterPersist()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEtymologyEditorPrepare(engine, TEtymologySave(engine, "water", "English"));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyTextSet(first, "wæter");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal("wæter", row.CAncestorText);
        Assert.False(row.CAncestorLinked);
    }

    [Fact]
    public void EtymologyTextSet_LinkedBox_DropsLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);
        etymology.CEtymologyTargetSet(first, aqua);

        etymology.CEtymologyTextSet(first, "aqua?");

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.False(row.CAncestorLinked);
        Assert.Equal("aqua?", row.CAncestorText);
    }

    [Fact]
    public void EtymologyTargetSet_BoxWithoutSource_LinksWithoutUndo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyTargetSet(first, aqua);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.True(row.CAncestorLinked);
        Assert.Equal("aqua", row.CAncestorText);
        Assert.Equal("Latin", row.CAncestorLanguage);
        Assert.False(row.CAncestorRestorable);
    }

    [Fact]
    public void EtymologyTargetSet_BoxWithSource_KeepsSourceRestorable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);
        etymology.CEtymologyInsert(first);

        etymology.CEtymologyTargetSet(first, aqua);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.True(row.CAncestorLinked);
        Assert.True(row.CAncestorRestorable);
    }

    [Fact]
    public void EtymologyTargetSet_BoxWithSource_SignalsOnceSettled()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);
        etymology.CEtymologyInsert(first);
        List<CEtymologyState> reads = [];
        etymology.CEtymologyChanged += () => reads.Add(etymology.CEtymologyRead());

        etymology.CEtymologyTargetSet(first, aqua);

        CEtymologyState read = Assert.Single(reads);
        Assert.True(Assert.Single(read.CEtymologyStateAncestors).CAncestorRestorable);
        Assert.NotEmpty(read.CEtymologyStateRelations);
        Assert.NotEmpty(read.CEtymologyStateLanguages);
    }

    [Fact]
    public void EtymologyTargetSet_EntryWithMap_AnswersBorrowedBoxes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        TEditorFixture editor = TEtymologyEditorPrepare(engine, aqua);
        long root = TEtymologyBoxCreate(editor.TEditorFixtureEtymology);
        editor.TEditorFixtureEtymology.CEtymologyTextSet(root, "akwa");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        editor.TEditorFixtureSave();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyTargetSet(first, aqua);

        IReadOnlyList<CAncestor> rows = etymology.CEtymologyRead().CEtymologyStateAncestors;
        Assert.Equal(2, rows.Count);
        Assert.False(rows[0].CAncestorBorrowed);
        Assert.Equal("akwa", rows[1].CAncestorText);
        Assert.True(rows[1].CAncestorBorrowed);
        Assert.Equal(1, rows[1].CAncestorDepth);
        Assert.False(rows[1].CAncestorRestorable);
    }

    [Fact]
    public void EtymologyRestore_SetAsideSource_ReturnsTextAndSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long aqua = TEtymologySave(engine, "aqua", "Latin");
        TEditorFixture editor = TEtymologyEditorPrepare(engine, TEtymologySave(engine, "water", "English"));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        long first = TEtymologyBoxCreate(etymology);
        etymology.CEtymologyTextSet(first, "wæter");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();
        etymology.CEtymologyInsert(first);
        etymology.CEtymologyTargetSet(first, aqua);

        etymology.CEtymologyRestore(first);

        IReadOnlyList<CAncestor> rows = etymology.CEtymologyRead().CEtymologyStateAncestors;
        Assert.Equal(2, rows.Count);
        Assert.Equal("wæter", rows[0].CAncestorText);
        Assert.False(rows[0].CAncestorLinked);
        Assert.False(rows[0].CAncestorRestorable);
    }

    [Fact]
    public void EtymologyLanguageSet_CustomTongueName_AnswersTongue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTongueCreate("Proto-Germanic");
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        CAncestorChoice typed = new(null, string.Empty, " proto-germanic ", false, false);

        etymology.CEtymologyLanguageSet(first, typed);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal("Proto-Germanic", row.CAncestorLanguage);
    }

    [Fact]
    public void EtymologyLanguageSet_FreeText_AnswersTrimmedText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        CAncestorChoice typed = new(null, string.Empty, " Old English ", false, false);

        etymology.CEtymologyLanguageSet(first, typed);

        CAncestor row = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors);
        Assert.Equal("Old English", row.CAncestorLanguage);
    }

    [Fact]
    public void EtymologyGlossSet_Box_AnswersGlossAfterPersist()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEtymologyEditorPrepare(engine, TEtymologySave(engine, "water", "English"));
        CEtymology etymology = editor.TEditorFixtureEtymology;
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyGlossSet(first, "water");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("water", Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors).CAncestorGloss);
    }

    [Fact]
    public void EtymologyRelationSet_BuiltinThenNone_SetsThenClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long relation = engine.TEngineRelationRead()[0].LAncestorRelationId;
        CEtymology etymology = TEtymologyPrepare(engine, TEtymologySave(engine, "water", "English"));
        long first = TEtymologyBoxCreate(etymology);

        etymology.CEtymologyRelationSet(first, new CAncestorChoice(relation, string.Empty, string.Empty, false, false));
        long? set = Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors).CAncestorRelation;
        etymology.CEtymologyRelationSet(first, new CAncestorChoice(null, string.Empty, string.Empty, false, false));

        Assert.Equal(relation, set);
        Assert.Null(Assert.Single(etymology.CEtymologyRead().CEtymologyStateAncestors).CAncestorRelation);
    }

    internal static long TEtymologyBoxCreate(CEtymology etymology)
    {
        etymology.CEtymologyInsert(null);
        return etymology.CEtymologyRead().CEtymologyStateAncestors[0].CAncestorId;
    }

    private static CEtymology TEtymologyPrepare(LEngine engine, long entry)
    {
        return TEtymologyEditorPrepare(engine, entry).TEditorFixtureEtymology;
    }

    internal static TEditorFixture TEtymologyEditorPrepare(LEngine engine, long entry)
    {
        return TEtymologyEditorPrepare(engine, entry, TInterfaceEditor.TEditorCreate(engine));
    }

    internal static TEditorFixture TEtymologyEditorPrepare(LEngine engine, long entry, CEditor created)
    {
        TEditorFixture editor = new(created);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(entry);
        return editor;
    }

    internal static long TEtymologySave(LEngine engine, string headword, string language)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, language, string.Empty, string.Empty, [TInterface.TCardCreate("a thing", 1)], [])).LEntryId;
    }
}
