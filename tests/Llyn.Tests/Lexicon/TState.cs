using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TState
{
    [Fact]
    public void AnUnreadableFieldIsStoredAsUnreadableWhileAnEmptyOneIsStoredAsNothingRecorded()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [TInterface.TExampleDraftCreate(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [TInterface.TSituationDraftCreate(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [],
                string.Empty,
                ["spoken"], [], 1)],
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

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
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
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [
                    TInterface.TExampleDraftCreate("he said a word"),
                    TInterface.TExampleDraftCreate(
                        LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified),
                    TInterface.TExampleDraftCreate("   "),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void AnUnreadableCitationIsNotTheSameAsCitingNoSourceAtAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [
                    TInterface.TExampleDraftCreate(
                        TInterface.TStateValueCreate("he said a word"),
                        string.Empty,
                        LStateValue.LStateValueUnknown),
                    TInterface.TExampleDraftCreate("not a word was spoken"),
                ],
                [],
                [],
                string.Empty,
                [], [], 1)],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unknown' AND source_id IS NULL;"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE source_state = 'unspecified' AND source_id IS NULL;"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
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
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                LStateValue.LStateValueUnknown,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnknown,
                [],
                [TInterface.TSituationDraftCreate(
                    LStateValue.LStateValueUnknown, string.Empty, LStateValue.LStateValueUnspecified)],
                [],
                string.Empty,
                [], [], 1)],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = loaded.LEntryDraftSenses[0];
        string situationId = card.LCardDraftSituation[0].LSituationDraftId;

        engine.TEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftSenses =
            [
                card with
                {
                    LCardDraftTitle = TInterface.TStateValueCreate("the plain sense"),
                    LCardDraftMeaning = LStateValue.LStateValueUnspecified,
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[0] with
                        {
                            LSituationDraftText = TInterface.TStateValueCreate("in court"),
                        },
                    ],
                },
            ],
        });

        LEntryDraft second = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft written = Assert.Single(second.LEntryDraftSenses);

        Assert.Equal(TInterface.TStateValueCreate("the plain sense"), written.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, written.LCardDraftMeaning);

        LSituationDraft situation = Assert.Single(written.LCardDraftSituation);
        Assert.Equal(situationId, situation.LSituationDraftId);
        Assert.Equal(TInterface.TStateValueCreate("in court"), situation.LSituationDraftText);
    }
}
