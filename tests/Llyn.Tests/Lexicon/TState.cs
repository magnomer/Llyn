using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TState
{
    [Fact]
    public void EntrySave_UnknownAndEmptyFields_StoresEachState()
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
                [TInterface.TSentenceDraftCreate(
                    LStateValue.LStateValueUnknown, 0, LStateAnchor.LStateAnchorUnspecified)],
                [TInterface.TSituationDraftCreate(LStateValue.LStateValueUnknown, 0)],
                [],
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
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);

        Assert.Equal(LStateValue.LStateValueUnknown, card.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, card.LCardDraftExpression);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            TExampleDraftRead(Assert.Single(card.LCardDraftSentence)).LExampleDraftText);
        Assert.Equal(
            LStateValue.LStateValueUnknown,
            Assert.Single(card.LCardDraftSituation).LSituationDraftTitle);
        Assert.Equal(["spoken"], TInterface.TTagDraftRead(card.LCardDraftTag));
    }

    [Fact]
    public void EntrySave_FieldNeverWritten_StoresNoRow()
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
                    TInterface.TSentenceDraftCreate("he said a word"),
                    TInterface.TSentenceDraftCreate(
                        LStateValue.LStateValueUnknown, 0, LStateAnchor.LStateAnchorUnspecified),
                    TInterface.TSentenceDraftCreate("   "),
                ],
                [],
                [],
                [], [], 1)],
            []));

        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));
        Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_example;"));
    }

    [Fact]
    public void EntrySave_UnknownSourceCited_LinksTheSeededRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long unknown = workspace.TWorkspaceCountRead(
            "SELECT reference_id FROM reference WHERE title = 'Unknown';");

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
                    TInterface.TSentenceDraftCreate(
                        TInterface.TStateValueCreate("he said a word"),
                        0,
                        TInterface.TStateAnchorRead(unknown)),
                    TInterface.TSentenceDraftCreate("not a word was spoken"),
                ],
                [],
                [],
                [], [], 1)],
            []));

        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                $"SELECT COUNT(*) FROM example WHERE reference_state = 'specified' AND reference_ref = {unknown};"));
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM example WHERE reference_state = 'unspecified' AND reference_ref IS NULL;"));
        Assert.Equal(
            0,
            workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example WHERE reference_state = 'unknown';"));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);

        Assert.Equal(
            TInterface.TStateAnchorRead(unknown),
            TExampleDraftRead(card.LCardDraftSentence[0]).LExampleDraftReference);
        Assert.Equal(
            LStateAnchor.LStateAnchorUnspecified,
            TExampleDraftRead(card.LCardDraftSentence[1]).LExampleDraftReference);
    }

    [Fact]
    public void ExampleRead_UnknownSourceWording_ReadsUnreadable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        workspace.TWorkspaceScriptRun(
            "INSERT INTO example (language, text_state, text, reference_state) " +
            "VALUES ('English', 'specified', 'he said a word', 'unknown');");
        long id = workspace.TWorkspaceCountRead("SELECT MAX(example_id) FROM example;");

        LExample read = Assert.IsType<LExample>(engine.TEngineExampleRead(id));

        Assert.True(read.LExampleSource.LStateAnchorUnreadable);
        Assert.Equal(LState.LStateUnspecified, read.LExampleSource.LStateAnchorState);
    }

    [Fact]
    public void EntryUpdate_ClearedUnknownField_RecordsNothing()
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
                [TInterface.TSituationDraftCreate(LStateValue.LStateValueUnknown, 0)],
                [],
                [], [], 1)],
            []));

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = loaded.LEntryDraftMeanings[0];
        long situationId = card.LCardDraftSituation[0].LSituationDraftId;

        engine.TEngineEntryUpdate(stored.LEntryId, loaded with
        {
            LEntryDraftMeanings =
            [
                card with
                {
                    LCardDraftTitle = TInterface.TStateValueCreate("the plain meaning"),
                    LCardDraftMeaning = LStateValue.LStateValueUnspecified,
                    LCardDraftSituation =
                    [
                        card.LCardDraftSituation[0] with
                        {
                            LSituationDraftTitle = TInterface.TStateValueCreate("in court"),
                        },
                    ],
                },
            ],
        });

        LEntryDraft second = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft written = Assert.Single(second.LEntryDraftMeanings);

        Assert.Equal(TInterface.TStateValueCreate("the plain meaning"), written.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, written.LCardDraftMeaning);

        LSituationDraft situation = Assert.Single(written.LCardDraftSituation);
        Assert.Equal(situationId, situation.LSituationDraftId);
        Assert.Equal(TInterface.TStateValueCreate("in court"), situation.LSituationDraftTitle);
    }

    [Theory]
    [InlineData("a unit of language", false)]
    [InlineData("a unit of language", true)]
    [InlineData("", false)]
    [InlineData("", true)]
    [InlineData("   ", false)]
    [InlineData("   ", true)]
    [InlineData(null, false)]
    [InlineData(null, true)]
    public void StateValueResolve_TextAndMark_MapsOneStateEach(string? text, bool unknown)
    {
        LStateValue resolved = TInterface.TStateValueResolve(text, unknown);

        if (unknown)
        {
            Assert.Equal(LStateValue.LStateValueUnknown, resolved);
            Assert.Null(resolved.LStateValueText);
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            Assert.Equal(LStateValue.LStateValueUnspecified, resolved);
            return;
        }

        Assert.Equal(TInterface.TStateValueCreate(text), resolved);
    }

    [Fact]
    public void StateValueResolve_WhitespaceRoundTrip_ReportsNoChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TRequestContentApply(started.LDraftId, TInterface.TEntryDraftCreate(
                "word",
                "English",
                string.Empty,
                string.Empty,
                [TInterface.TCardDraftCreate(
                    TInterface.TStateValueResolve(string.Empty, true),
                    TInterface.TStateValueResolve("   ", false),
                    TInterface.TStateValueResolve("a unit of language", false),
                    [],
                    [],
                    [],
                    [],
                    [],
                    0)],
                []));

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LDraft opened = engine.TEngineDraftStart("editor", stored.LEntryId);
        LCardDraft card = Assert.Single(opened.LDraftContent.LEntryDraftMeanings);

        Assert.Equal(LStateValue.LStateValueUnknown, card.LCardDraftTitle);
        Assert.Equal(LStateValue.LStateValueUnspecified, card.LCardDraftExpression);
        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));

        engine.TEngineRequestApply(TInterface.TRequestTitleCreate(
            opened.LDraftId,
            card.LCardDraftId,
            TInterface.TStateValueResolve(
                card.LCardDraftTitle.TStateValueShow(),
                card.LCardDraftTitle.LStateValueState == LState.LStateUnknown)));
        engine.TEngineRequestApply(TInterface.TRequestExpressionCreate(
            opened.LDraftId,
            card.LCardDraftId,
            TInterface.TStateValueResolve(
                card.LCardDraftExpression.TStateValueShow(),
                card.LCardDraftExpression.LStateValueState == LState.LStateUnknown)));
        engine.TEngineRequestApply(TInterface.TRequestMeaningCreate(
            opened.LDraftId,
            card.LCardDraftId,
            TInterface.TStateValueResolve(
                card.LCardDraftMeaning.TStateValueShow(),
                card.LCardDraftMeaning.LStateValueState == LState.LStateUnknown)));

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void EntryLoad_BrokenStateWord_ReadsUnreadableAndRefusesSave()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                TInterface.TStateValueCreate("a title"),
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [], [], [], [], [], 1)],
            []));

        workspace.TWorkspaceScriptRun("UPDATE sense SET title_state = 'broken', title = NULL;");

        LEntryDraft loaded = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(stored.LEntryId));
        LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);
        Assert.True(card.LCardDraftTitle.LStateValueUnreadable);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
        Assert.Equal("broken", card.LCardDraftTitle.TStateValueShow());

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineEntryUpdate(stored.LEntryId, loaded));
        Assert.Equal(LRefusal.LRefusalUnreadable, refusal.LRefusalReason);
    }

    [Fact]
    public void DraftNormalize_UnreadableField_DropsItAndCommits()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "word",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(
                TInterface.TStateValueCreate("a title"),
                LStateValue.LStateValueUnspecified,
                TInterface.TStateValueCreate("a unit of language"),
                [], [], [], [], [], 1)],
            []));

        workspace.TWorkspaceScriptRun("UPDATE sense SET title_state = 'broken', title = NULL;");

        LDraft draft = engine.TEngineDraftStart("test", stored.LEntryId);
        Assert.True(Assert.Single(draft.LDraftContent.LEntryDraftMeanings).LCardDraftTitle.LStateValueUnreadable);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineDraftCommit(draft.LDraftId));
        Assert.Equal(LRefusal.LRefusalUnreadable, refusal.LRefusalReason);

        engine.TEngineDraftSweep(draft.LDraftId);
        LDraft cleared = Assert.IsType<LDraft>(engine.TEngineDraftRead(draft.LDraftId));
        Assert.Equal(
            LStateValue.LStateValueUnspecified,
            Assert.Single(cleared.LDraftContent.LEntryDraftMeanings).LCardDraftTitle);

        engine.TEngineDraftCommit(draft.LDraftId);
        Assert.Equal(
            1,
            workspace.TWorkspaceCountRead(
                "SELECT COUNT(*) FROM sense WHERE title_state = 'unspecified' AND title IS NULL;"));
    }

    private static LExampleDraft TExampleDraftRead(LSentenceDraft sentence)
    {
        return Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample);
    }
}
