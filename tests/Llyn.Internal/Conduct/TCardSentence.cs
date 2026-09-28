using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardSentence
{
    [Fact]
    public void SentenceAdd_BelowTheFirstRow_PlacesTheNewRowSecond()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceAdd(sheet, 0);

        LCardDraft card = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        Assert.Equal(2, card.LCardDraftSentence.Count);
        Assert.Equal(first, card.LCardDraftSentence[0].LSentenceDraftId);
    }

    [Fact]
    public void SentenceRemove_HeldRow_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceRemove(sheet, first);

        Assert.Empty(TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftSentence);
    }

    [Fact]
    public void TextSet_TypedFields_WritesTextParticleAndDependence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CSentence sentence) = TSentencePrepare(engine);
        (long sheet, long first) = TSentenceCardAdd(desk);

        sentence.CSentenceTextSet(sheet, first, "a cat sleeps");
        sentence.CSentenceParticleSet(sheet, first, "of");
        sentence.CSentenceDependenceSet(sheet, first, "Something");

        LSentenceDraft row = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(held => held.LSentenceDraftId == first);
        Assert.Equal("a cat sleeps", row.LSentenceDraftExample!.LExampleDraftText.TStateValueShow());
        Assert.Equal("of", row.LSentenceDraftParticle.TStateValueShow());
        Assert.Equal("Something", row.LSentenceDraftDependence.TStateValueShow());
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
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TInterfaceConduct.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        CSentence sentence = TInterfaceConduct.TSentenceCreate(engine, desk);

        sentence.CSentenceAdd(1, 0);

        Assert.False(desk.CDeskHeld);
    }

    private static (CDesk TSentenceDesk, CSentence TSentenceGate) TSentencePrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TInterfaceConduct.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        return (desk, TInterfaceConduct.TSentenceCreate(engine, desk));
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
