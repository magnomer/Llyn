using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequest
{
    [Fact]
    public void RequestApply_HeadwordText_PersistsInDraftFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindle"));
        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.Equal("kindle", answered.LDraftContent.LEntryDraftHeadword);
        Assert.NotNull(held);
        Assert.Equal("kindle", held.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void RequestApply_CardAddition_MintsNegativeIdAndRaisesBulletin()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindMeaning, 0, 0));

        LCardDraft card = Assert.Single(answered.LDraftContent.LEntryDraftMeanings);
        Assert.True(card.LCardDraftId < 0);
        Assert.Equal(1, card.LCardDraftPosition);
        LBulletin bulletin = Assert.Single(bulletins);
        Assert.Equal(LSubject.LSubjectDraft, bulletin.LBulletinSubject);
        Assert.Equal(started.LDraftId, bulletin.LBulletinId);
    }

    [Fact]
    public void RequestApply_CardShift_ReordersAndRenumbersCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);
        long third = TRequestCardAdd(engine, started.LDraftId, 2);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestShiftCreate(started.LDraftId, third, 0, 0));

        IReadOnlyList<LCardDraft> cards = answered.LDraftContent.LEntryDraftMeanings;
        Assert.Equal([third, first, second], cards.Select(card => card.LCardDraftId));
        Assert.Equal([1, 2, 3], cards.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void RequestApply_CardRemoval_DropsCardAndRenumbersRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestRemovalCreate(started.LDraftId, first));

        LCardDraft kept = Assert.Single(answered.LDraftContent.LEntryDraftMeanings);
        Assert.Equal(second, kept.LCardDraftId);
        Assert.Equal(1, kept.LCardDraftPosition);
    }

    [Fact]
    public void RequestApply_CardTitle_ChangesOnlyThatCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        long first = TRequestCardAdd(engine, started.LDraftId, 0);
        long second = TRequestCardAdd(engine, started.LDraftId, 1);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestTitleCreate(started.LDraftId, second, TInterface.TStateValueCreate("set alight")));

        IReadOnlyList<LCardDraft> cards = answered.LDraftContent.LEntryDraftMeanings;
        Assert.Equal(first, cards[0].LCardDraftId);
        Assert.True(cards[0].LCardDraftTitle.LStateValueEmpty);
        Assert.Equal("set alight", cards[1].LCardDraftTitle.TStateValueShow());
    }

    [Fact]
    public void RequestApply_UnknownCard_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestTitleCreate(started.LDraftId, TInterface.TIdentityCreate(), "set alight")));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_ZeroCardId_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestRemovalCreate(started.LDraftId, 0)));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_CollocationParent_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindCollocation, 0, 0));
        long parent = answered.LDraftContent.LEntryDraftCollocations[0].LCardDraftId;

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindCollocation, parent, 0)));

        Assert.Equal(LRefusal.LRefusalCollocation, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_DraftNotHeld_Refuses()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TRequestHeadwordCreate(TInterface.TIdentityCreate(), "kindle")));

        Assert.Equal(LRefusal.LRefusalDraft, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_HeadwordFreshAudio_Clears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindle"));
        engine.TEngineRequestApply(TInterface.TRequestAudioCreate(started.LDraftId, "kindle.mp3", "Wiktionary"));

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindles"));

        LPronunciationDraft spoken = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal(string.Empty, spoken.LPronunciationDraftAudio);
        Assert.Null(spoken.LPronunciationDraftSource);
    }

    [Fact]
    public void RequestApply_HeadwordStoredAudio_Stays()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string file = TRequestRecordingSave(workspace);
        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [], [], file, "Wiktionary"));
        LDraft started = engine.TEngineDraftStart("Input", stored.LEntryId);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindles"));

        LPronunciationDraft spoken = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal(file, spoken.LPronunciationDraftAudio);
        Assert.Equal("Wiktionary", spoken.LPronunciationDraftSource);
    }

    [Fact]
    public void RequestApply_HeadwordReplacedAudio_Clears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string file = TRequestRecordingSave(workspace);
        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [], [], file, "Wiktionary"));
        LDraft started = engine.TEngineDraftStart("Input", stored.LEntryId);
        engine.TEngineRequestApply(TInterface.TRequestAudioCreate(started.LDraftId, "other.mp3", "Forvo"));

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestHeadwordCreate(started.LDraftId, "kindles"));

        LPronunciationDraft spoken = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal(string.Empty, spoken.LPronunciationDraftAudio);
    }

    [Fact]
    public void RequestApply_LanguageFreshAudio_Clears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "English"));
        engine.TEngineRequestApply(TInterface.TRequestAudioCreate(started.LDraftId, "kindle.mp3", "Wiktionary"));
        LDraft same = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "English"));

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "German"));

        Assert.Equal("kindle.mp3", same.LDraftContent.LEntryDraftAudio);
        Assert.Equal(string.Empty, answered.LDraftContent.LEntryDraftAudio);
    }

    [Fact]
    public void RequestApply_LanguageStoredAudio_Clears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string file = TRequestRecordingSave(workspace);
        LEntry stored = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [], [], file, "Wiktionary"));
        LDraft started = engine.TEngineDraftStart("Input", stored.LEntryId);

        LDraft answered = engine.TEngineRequestApply(TInterface.TRequestLanguageCreate(started.LDraftId, "German"));

        LPronunciationDraft spoken = Assert.Single(answered.LDraftContent.LEntryDraftPronunciations);
        Assert.Equal(string.Empty, spoken.LPronunciationDraftAudio);
        Assert.Null(spoken.LPronunciationDraftSource);
    }

    private static string TRequestRecordingSave(TWorkspace workspace)
    {
        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, "kindle.mp3");
        File.WriteAllBytes(file, [0]);
        return file;
    }

    private static long TRequestCardAdd(LEngine engine, long draftId, int position)
    {
        LDraft answered = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(draftId, LCardKind.LCardKindMeaning, 0, position));
        return answered.LDraftContent.LEntryDraftMeanings[position].LCardDraftId;
    }

    [Fact]
    public void RequestApply_UnchangedText_WritesNothingAndRaisesNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineExampleStart("Example", null);
        LDraft changed = engine.TEngineRequestApply(
            TInterface.TExampleTextCreate(started.LDraftId, TInterface.TStateValueCreate("a line")));
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TExampleTextCreate(started.LDraftId, TInterface.TStateValueCreate("a line")));

        Assert.Equal(changed.LDraftVersion, answered.LDraftVersion);
        Assert.Equal(changed.LDraftExample, answered.LDraftExample);
        Assert.Equal(started.LDraftExample!.LExampleId, answered.LDraftExample!.LExampleId);
        Assert.Equal("a line", answered.LDraftExample.LExampleText.TStateValueShow());
        Assert.Empty(bulletins);
    }

    [Fact]
    public void RequestApply_SituationBody_ChangesThePanelSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineSituationStart("Situation", null);
        long id = started.LDraftSituation!.LSituationId;
        LSituation sent = TInterface.TSituationCreate(
            0,
            TInterface.TStateValueCreate("around a hearth"),
            TInterface.TStateValueCreate("a fireside"),
            TInterface.TStateValueCreate("place"));

        LDraft answered = engine.TEngineRequestApply(TInterface.TSituationBodyCreate(started.LDraftId, id, sent));

        Assert.Equal(id, answered.LDraftSituation!.LSituationId);
        Assert.Equal("around a hearth", answered.LDraftSituation.LSituationTitle.TStateValueShow());
        Assert.Equal("a fireside", answered.LDraftSituation.LSituationDescription.TStateValueShow());
        Assert.Equal("place", answered.LDraftSituation.LSituationKind.TStateValueShow());
    }
}
