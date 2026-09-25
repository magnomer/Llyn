using System.Collections.Generic;
using System.IO;
using System.Text;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupImport
{
    [Fact]
    public void MarkupImport_TwoEntriesLinked_StoresBothWithLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TInterface.TMarkupSave(workspace, TInterface.TMarkupPair);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(2));

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        Assert.Equal(2, outcome.TMarkupOutcomeEntry.Count);
        LEntry ember = outcome.TMarkupOutcomeEntry[0];
        LEntry braise = outcome.TMarkupOutcomeEntry[1];

        LEntryDraft emberDraft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        LEntryDraft braiseDraft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(braise.LEntryId));
        LCardDraft emberCard = Assert.Single(emberDraft.LEntryDraftMeanings);
        LCardDraft braiseCard = Assert.Single(braiseDraft.LEntryDraftMeanings);
        Assert.Equal(braise.LEntryId, Assert.Single(emberCard.LCardDraftTranslation));
        Assert.Equal(ember.LEntryId, Assert.Single(braiseCard.LCardDraftTranslation));
        Assert.Equal("a glowing coal", emberCard.LCardDraftMeaning.TStateValueShow());
    }

    [Fact]
    public void MarkupImport_AbsentTranslation_StoresEntryAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        LEntry ember = Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.Contains("braise", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Empty(Assert.Single(draft.LEntryDraftMeanings).LCardDraftTranslation);
    }

    [Fact]
    public void MarkupImport_TwinTargets_DropsLinkAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("braise", "French", string.Empty, string.Empty, [], []));
        engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("Braise", "french", string.Empty, string.Empty, [], []));

        string path = TInterface.TMarkupSave(workspace, TInterface.TMarkupLone);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        LEntry ember = Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.Contains("braise", Assert.Single(outcome.TMarkupOutcomeOmission).LMarkupOmissionText);
        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(ember.LEntryId));
        Assert.Empty(Assert.Single(draft.LEntryDraftMeanings).LCardDraftTranslation);
    }

    [Fact]
    public void MarkupImport_MalformedFile_RefusesAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = TInterface.TMarkupSave(workspace, "<llyn><entry><headword>ember</headword></llyn>");
        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1)));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.LRefusalReason);
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void MarkupImport_BlankSecondHeadword_StoresNeither()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>English</language>
              </entry>
              <entry>
                <headword> </headword>
                <language>English</language>
              </entry>
            </llyn>
            """;
        string path = TInterface.TMarkupSave(workspace, text);
        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(2)));

        Assert.Equal(LRefusal.LRefusalHeadword, refusal.LRefusalReason);
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void MarkupImport_ExportedFile_RoundTrips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "glow", "English", string.Empty, string.Empty,
            [TInterface.TCardCreate("shine", 1) with { LCardDraftChild = [TInterface.TCardCreate("shine softly", 1)] }],
            []));
        long senseId = Assert.Single(
            Assert.Single(Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(glow.LEntryId)).LEntryDraftMeanings)
                .LCardDraftChild).LCardDraftId;

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("A Dictionary"),
            TInterface.TStateValueCreate("1998"),
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LAuthor kim = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Kim"));
        engine.TRequestCreditApply(reference.LReferenceId, kim.LAuthorId, 0);

        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            TInterface.TStateValueCreate("The ember glows."),
            0,
            TInterface.TStateAnchorRead(reference.LReferenceId));
        sentence = sentence with
        {
            LSentenceDraftExample = Assert.IsType<LExampleDraft>(sentence.LSentenceDraftExample) with
            {
                LExampleDraftMention = [TInterface.TMentionDraftCreate(0, 10, 5, glow.LEntryId, senseId)],
            },
        };

        LEntry ember = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "ember", "English", string.Empty, "a note",
            [
                TInterface.TCardDraftCreate(
                    TInterface.TStateValueCreate("glowing coal"),
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("a small piece of burning coal"),
                    [sentence], [], [glow.LEntryId], ["fire"], [], 1) with
                {
                    LCardDraftChild = [TInterface.TCardCreate("a fading one", 1)],
                },
                TInterface.TCardCreate("a remnant", 2),
            ],
            [
                TInterface.TCardDraftCreate(
                    LStateValue.LStateValueUnspecified,
                    TInterface.TStateValueCreate("dying ember"),
                    LStateValue.LStateValueUnspecified,
                    [], [], [], [], [], 1),
            ]));

        string path = Path.Combine(workspace.TWorkspaceFolder, "export.llx");
        engine.TEngineMarkupExport([ember.LEntryId], path);
        engine.TEngineEntryDelete(ember.LEntryId);

        TMarkupOutcome outcome = engine.TEngineMarkupImport(engine.TEngineMarkupRead(path), TMarkupIntakeCreate(1));

        Assert.Empty(outcome.TMarkupOutcomeOmission);
        LEntry imported = Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.NotEqual(ember.LEntryId, imported.LEntryId);
        Assert.Equal("ember", imported.LEntryHeadword);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(imported.LEntryId));
        Assert.Equal("a note", draft.LEntryDraftNote);
        Assert.Equal(2, draft.LEntryDraftMeanings.Count);
        LCardDraft first = draft.LEntryDraftMeanings[0];
        Assert.Equal("glowing coal", first.LCardDraftTitle.TStateValueShow());
        Assert.Equal("a fading one", Assert.Single(first.LCardDraftChild).LCardDraftMeaning.TStateValueShow());
        Assert.Equal(glow.LEntryId, Assert.Single(first.LCardDraftTranslation));
        Assert.Equal("fire", Assert.Single(first.LCardDraftTag).LTagDraftText);
        Assert.Equal(
            "dying ember",
            Assert.Single(draft.LEntryDraftCollocations).LCardDraftExpression.TStateValueShow());

        LExampleDraft example = Assert.IsType<LExampleDraft>(
            Assert.Single(first.LCardDraftSentence).LSentenceDraftExample);
        Assert.Equal("The ember glows.", example.LExampleDraftText.TStateValueShow());
        LMentionDraft mention = Assert.Single(example.LExampleDraftMention);
        Assert.Equal(glow.LEntryId, mention.LMentionDraftEntry);
        Assert.Equal(senseId, mention.LMentionDraftSense);
        Assert.Equal(reference.LReferenceId, example.LExampleDraftReference.LStateAnchorId);
        Assert.Equal(2, engine.TEngineReferenceRead().Count);
    }

    private static IReadOnlyList<LMarkupIntake> TMarkupIntakeCreate(int count)
    {
        List<LMarkupIntake> intakes = new(count);
        for (int index = 0; index < count; index++)
        {
            intakes.Add(TInterface.TMarkupIntakeCreate(index, LMarkupMode.LMarkupModeNew));
        }

        return intakes;
    }
}
