using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSpeechRetirement
{
    private const string TSpeechFixture = "Fixture";

    [Fact]
    public void SpeechRetirementApply_Subtype_MovesLinkToHead()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LSpeechValue> values = TSpeechPartsCreate(workspace);
        LEntry entry = engine.TEngineEntrySave(TSpeechEntryCreate("Head, sub"));

        TSpeechRetirementRun(workspace, 2, 1, LUnit.LUnitEmpty);

        LSpeech link = Assert.Single(TSpeechLinksRead(workspace, entry.LEntryId));
        Assert.Equal(values[0].LSpeechValueId, link.LSpeechValueId);
        Assert.Equal(0, workspace.TWorkspaceCountRead(
            $"SELECT COUNT(*) FROM speech_value WHERE speech_value_id = {values[1].LSpeechValueId};"));
    }

    [Fact]
    public void SpeechRetirementApply_EntryHoldingBoth_KeepsOneLinkInOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LSpeechValue> values = TSpeechPartsCreate(workspace);
        LEntry entry = engine.TEngineEntrySave(TSpeechEntryCreate("Head", "Head, sub", "Saying"));

        TSpeechRetirementRun(workspace, 2, 1, LUnit.LUnitEmpty);

        IReadOnlyList<LSpeech> links = TSpeechLinksRead(workspace, entry.LEntryId);
        Assert.Equal(
            [values[0].LSpeechValueId, values[3].LSpeechValueId],
            [links[0].LSpeechValueId, links[1].LSpeechValueId]);
        Assert.Equal(
            ["0", "1"],
            workspace.TWorkspaceRowRead(
                $"SELECT position FROM part_of_speech WHERE entry_parent = {entry.LEntryId} ORDER BY position;"));
    }

    [Fact]
    public void SpeechRetirementApply_Morpheme_SetsUnitKeepingChosenOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TSpeechPartsCreate(workspace);
        LEntry affix = engine.TEngineEntrySave(TSpeechEntryCreate("Affix"));
        LEntry chosen = engine.TEngineEntrySave(
            TSpeechEntryCreate("Affix") with { LEntryDraftUnit = LUnit.LUnitWord });

        TSpeechRetirementRun(workspace, 3, 0, LUnit.LUnitMorpheme);

        Assert.Equal(LUnit.LUnitMorpheme, engine.TEngineEntryRead(affix.LEntryId)!.LEntryUnit);
        Assert.Equal(LUnit.LUnitWord, engine.TEngineEntryRead(chosen.LEntryId)!.LEntryUnit);
        Assert.Empty(TSpeechLinksRead(workspace, affix.LEntryId));
    }

    [Fact]
    public void SpeechRetirementApply_NoTarget_DropsLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TSpeechPartsCreate(workspace);
        LEntry entry = engine.TEngineEntrySave(TSpeechEntryCreate("Saying"));

        TSpeechRetirementRun(workspace, 4, 0, LUnit.LUnitEmpty);

        Assert.Empty(TSpeechLinksRead(workspace, entry.LEntryId));
        Assert.Equal(LUnit.LUnitEmpty, engine.TEngineEntryRead(entry.LEntryId)!.LEntryUnit);
    }

    [Fact]
    public void SpeechRetirementApply_Again_ChangesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<LSpeechValue> values = TSpeechPartsCreate(workspace);
        LEntry entry = engine.TEngineEntrySave(TSpeechEntryCreate("Head, sub"));
        TSpeechRetirementRun(workspace, 2, 1, LUnit.LUnitEmpty);

        TSpeechRetirementRun(workspace, 2, 1, LUnit.LUnitEmpty);

        Assert.Equal(
            values[0].LSpeechValueId, Assert.Single(TSpeechLinksRead(workspace, entry.LEntryId)).LSpeechValueId);
    }

    [Fact]
    public void LanguageImport_RetiredPack_KeepsUserPartAndDropsSubtypes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            Assert.NotNull(engine.TEngineSpeechAdd("English", "Verb, ergative"));
        }

        using LEngine reopened = workspace.TWorkspaceEngineStart();

        Assert.NotNull(reopened.TSpeechValueFind("English", "Verb, ergative"));
        Assert.Null(reopened.TSpeechValueFind("English", "Verb, transitive"));
        Assert.Null(reopened.TSpeechValueFind("English", "Prefix"));
        Assert.Null(reopened.TSpeechValueFind("English", "Idiom"));
    }

    [Fact]
    public void SpeechPackLoad_EnglishPack_ReadsRetiredRows()
    {
        LSpeechPack pack = TInterface.TSpeechPackLoad("English");

        Assert.Contains(TInterface.TSpeechRetirementCreate(7, 6, LUnit.LUnitEmpty), pack.LSpeechPackRetirements);
        Assert.Contains(
            pack.LSpeechPackRetirements,
            static row => row.LSpeechRetirementTarget == 0 && row.LSpeechRetirementUnit == LUnit.LUnitMorpheme);
        Assert.Contains(
            pack.LSpeechPackRetirements,
            static row => row.LSpeechRetirementTarget == 0 && row.LSpeechRetirementUnit == LUnit.LUnitEmpty);
    }

    private static IReadOnlyList<LSpeechValue> TSpeechPartsCreate(TWorkspace workspace)
    {
        LSpeechArchive speeches = TInterface.TSpeechArchiveCreate(workspace.TWorkspaceDatabase);
        return
        [
            speeches.TSpeechValueCreate(TInterface.TSpeechValueCreate(TSpeechFixture, 1, "Head", 0)),
            speeches.TSpeechValueCreate(TInterface.TSpeechValueCreate(TSpeechFixture, 2, "Head, sub", 1)),
            speeches.TSpeechValueCreate(TInterface.TSpeechValueCreate(TSpeechFixture, 3, "Affix", 2)),
            speeches.TSpeechValueCreate(TInterface.TSpeechValueCreate(TSpeechFixture, 4, "Saying", 3)),
        ];
    }

    private static void TSpeechRetirementRun(TWorkspace workspace, long code, long target, LUnit unit)
    {
        TInterface.TSpeechArchiveCreate(workspace.TWorkspaceDatabase).TSpeechRetirementApply(
            TSpeechFixture, TInterface.TSpeechRetirementCreate(code, target, unit));
    }

    private static IReadOnlyList<LSpeech> TSpeechLinksRead(TWorkspace workspace, long entry)
    {
        return TInterface.TEntryArchiveCreate(workspace.TWorkspaceDatabase).TEntrySpeechRead(entry);
    }

    private static LEntryDraft TSpeechEntryCreate(params string[] speeches)
    {
        return TInterface.TEntryDraftCreate(
            "word",
            TSpeechFixture,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1)],
            [],
            speeches: speeches);
    }
}
