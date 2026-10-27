using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardSentence
{
    [Fact]
    public void SentenceAdd_BelowFirstRow_PlacesNewRowSecond()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceAdd(false, sheet, 0);

        LCardDraft card = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        Assert.Equal(2, card.LCardDraftSentence.Count);
        Assert.Equal(first, card.LCardDraftSentence[0].LSentenceDraftId);
    }

    [Fact]
    public void SentenceRemove_HeldRow_DropsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceRemove(false, sheet, first);

        Assert.Empty(TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftSentence);
    }

    [Fact]
    public void TextSet_TypedFields_WritesTextParticleAndDependence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceTextSet(false, sheet, first, "a cat sleeps");
        sentence.CSentenceParticleSet(false, sheet, first, "of");
        sentence.CSentenceDependenceSet(false, sheet, first, "Something");

        LSentenceDraft row = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(held => held.LSentenceDraftId == first);
        Assert.Equal("a cat sleeps", row.LSentenceDraftExample!.LExampleDraftText.TStateValueShow());
        Assert.Equal("of", row.LSentenceDraftParticle.TStateValueShow());
        Assert.Equal("Something", row.LSentenceDraftDependence.TStateValueShow());
    }

    [Fact]
    public void CitationSet_PickedSource_CitesSourceById()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceCitationSet(false, sheet, first, notes.LReferenceId);

        LSentenceDraft row = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(held => held.LSentenceDraftId == first);
        Assert.Equal(notes.LReferenceId, row.LSentenceDraftExample!.LExampleDraftReference.LStateAnchorShown);
    }

    [Fact]
    public void FrameRead_LanguageWithoutFrame_AnswersEmptyLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (_, CSentence sentence) = TSentencePrepare(engine);

        CSentenceFrame frame = sentence.CSentenceFrameRead();

        Assert.Empty(frame.CSentenceFrameParticle);
        Assert.Empty(frame.CSentenceFrameDependence);
    }

    [Fact]
    public void SentenceAdd_NoHeldDraft_LeavesNothingBehind()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        CSentence sentence = TInterfaceConductCard.TSentenceCreate(engine, desk);

        sentence.CSentenceAdd(false, 1, 0);

        Assert.False(desk.CDeskHeld);
    }

    [Fact]
    public void SentenceMentionAdd_SelectedWord_LinksSpanToEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long row) = TSentenceCardAdd(desk);
        sentence.CSentenceTextSet(false, sheet, row, "a cat sat");

        sentence.CSentenceMentionAdd(false, sheet, row, "a cat sat", 2, 3, cat);

        LExampleDraft? example = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence[0].LSentenceDraftExample;
        LMentionDraft linked = Assert.Single(example!.LExampleDraftMention);
        Assert.Equal(cat, linked.LMentionDraftEntry);
        Assert.Equal(2, linked.LMentionDraftOffset);
        Assert.Equal(3, linked.LMentionDraftLength);
    }

    private static (CDesk TSentenceDesk, CSentence TSentenceGate) TSentencePrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        return (desk, TInterfaceConductCard.TSentenceCreate(engine, desk));
    }

    private static (long TSentenceSheet, long TSentenceRow) TSentenceCardAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long sheet = desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        LCardDraft carded = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        return (sheet, carded.LCardDraftSentence[0].LSentenceDraftId);
    }
}
