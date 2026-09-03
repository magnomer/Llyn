using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TState
{
    [Fact]
    public void AnUnreadableFieldIsStoredAsUnreadableWhileAnEmptyOneIsStoredAsNothingRecorded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueCreate("a unit of language"),
                [new LExampleDraft(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [new LSituationDraft(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [],
                string.Empty,
                ["spoken"], [])],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense WHERE title_state = 'unknown' AND title IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE text_state = 'unknown' AND text IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM situation WHERE title_state = 'unknown' AND title IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);

        Assert.Equal(LStateValue.LStateValueUnknown, card.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, card.LCardDraftExpression);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            Assert.Single(card.LCardDraftExample).LExampleDraftText);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            Assert.Single(card.LCardDraftSituation).LSituationDraftText);
        Assert.Equal(["spoken"], card.LCardDraftTag);
    }

    [Fact]
    public void ARowWhoseFieldWasNeverWrittenIsNotStoredWhileAnUnreadableOneIs()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueCreate("a unit of language"),
                [
                    LExampleDraft.LExampleDraftCreate("he said a word"),
                    new LExampleDraft(
                        LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified),
                    LExampleDraft.LExampleDraftCreate("   "),
                ],
                [],
                [],
                string.Empty,
                [], [])],
            []));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void AnUnreadableCitationIsNotTheSameAsCitingNoSourceAtAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueCreate("a unit of language"),
                [
                    new LExampleDraft(
                        LStateValue.LStateValueCreate("he said a word"),
                        string.Empty,
                        LStateValue.LStateValueUnknown),
                    LExampleDraft.LExampleDraftCreate("not a word was spoken"),
                ],
                [],
                [],
                string.Empty,
                [], [])],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unknown' AND source_id IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unspecified' AND source_id IS NULL;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);

        Assert.Equal(
            LStateValue.LStateValueUnknown, card.LCardDraftExample[0].LExampleDraftReference);
        Assert.Equal(
            LStateValue.LStateValueUnspecified, card.LCardDraftExample[1].LExampleDraftReference);
    }

    [Fact]
    public void RewritingAnUnreadableFieldReplacesItAndClearingItSaysNothingWasWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LEntry stored = engine.LEngineEntrySave(new LEntryDraft(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [new LCardDraft(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnknown,
                [],
                [new LSituationDraft(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [],
                string.Empty,
                [], [])],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(stored.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];
        string situationId = card.LCardDraftSituation[0].LSituationDraftId;

        engine.LEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftTitle = LStateValue.LStateValueCreate("the plain sense"),
                    LCardDraftMeaning = LStateValue.LStateValueUnspecified,
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[0] with
                        {
                            LSituationDraftText = LStateValue.LStateValueCreate("in court"),
                        },
                    ],
                },
            ],
        });

        LEntryDraft second = Assert.IsType<LEntryDraft>(engine.LEngineEntryLoad(stored.LEntryId));
        LCardDraft written = Assert.Single(second.LEntryDraftSenses);

        Assert.Equal(LStateValue.LStateValueCreate("the plain sense"), written.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, written.LCardDraftMeaning);

        LSituationDraft situation = Assert.Single(written.LCardDraftSituation);
        Assert.Equal(situationId, situation.LSituationDraftId);
        Assert.Equal(LStateValue.LStateValueCreate("in court"), situation.LSituationDraftText);
    }
}
