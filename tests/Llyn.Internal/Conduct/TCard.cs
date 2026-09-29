using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCard
{
    [Fact]
    public void CardStateRead_UnknownValue_CarriesTheUncertainVerdict()
    {
        Assert.Equal(
            new CStateValue(string.Empty, true, false),
            TInterfaceConduct.TCardStateRead(LStateValue.LStateValueUnknown));
    }

    [Fact]
    public void CardEntryRead_ThreePronunciations_SplitsThePrimaryFromTheAccents()
    {
        LEntryDraft draft = TInterface.TEntryDraftCreate("colour", "English", "ˈkʌlə", "a note", [], []) with
        {
            LEntryDraftPronunciations =
            [
                TInterface.TPronunciationDraftCreate("ˈkʌlə", "RP"),
                TInterface.TPronunciationDraftCreate("ˈkʌlɚ", "GA"),
                TInterface.TPronunciationDraftCreate(string.Empty, "AU"),
            ],
        };

        CEntryDraft shaped = TInterfaceConduct.TCardEntryRead(draft);

        Assert.Equal("RP", shaped.CEntryDraftPronunciation?.CPronunciationDraftVariety);
        Assert.Equal(["GA", "AU"], shaped.CEntryDraftAccents.Select(spoken => spoken.CPronunciationDraftVariety));
        Assert.False(shaped.CEntryDraftReflected);
    }

    [Fact]
    public void PanelRowRead_EngineRow_CopiesEveryFieldAndTheMark()
    {
        CVistaRow row = TInterfaceConduct.TPanelRowRead(TInterfaceConduct.TVistaRowCreate(4, "gloss", true));

        Assert.Equal(new CVistaRow(4, "aqua", "Latin", "gloss", "aqua (1)", true), row);
    }

    [Fact]
    public void EtymologySet_TextTyped_WritesTheNarrative()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);

        card.CCardEtymologySet("from Latin");

        Assert.Equal("from Latin", TCardEtymologyRead(desk).LEtymologyDraftText);
    }

    [Fact]
    public void EtymonAdd_StoredEntry_AppendsTheSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);

        card.CCardEtymonAdd(cat);

        Assert.Equal([cat], TCardEtymologyRead(desk).LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymonRemove_AddedSource_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);
        card.CCardEtymonAdd(cat);

        card.CCardEtymonRemove(cat);

        Assert.Empty(TCardEtymologyRead(desk).LEtymologyDraftEtymons);
    }

    [Fact]
    public void MentionSave_SelectionOverEntry_LinksTheSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cattus", "Latin").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);
        card.CCardEtymologySet("from cattus");

        card.CCardMentionSave("from cattus", 5, 6, cat);

        LMentionDraft linked = Assert.Single(TCardEtymologyRead(desk).LEtymologyDraftMentions);
        Assert.Equal(cat, linked.LMentionDraftEntry);
        Assert.True(card.CCardMentionCheck("from cattus", 6, 2));
        Assert.False(card.CCardMentionCheck("from cattus", 0, 4));
    }

    [Fact]
    public void MentionDelete_SelectionInsideSpan_DropsTheSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cattus", "Latin").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);
        card.CCardEtymologySet("from cattus");
        card.CCardMentionSave("from cattus", 5, 6, cat);

        card.CCardMentionDelete("from cattus", 6, 2);

        Assert.Empty(TCardEtymologyRead(desk).LEtymologyDraftMentions);
        Assert.False(card.CCardMentionCheck("from cattus", 6, 2));
    }

    [Fact]
    public void CitationSet_TypedTitle_CitesTheResolvedSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        (long sheet, long sentence) = TCardSentenceAdd(desk);

        card.CCardCitationSet(sheet, sentence, "Field notes");

        LExampleDraft? example = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet)
            .LCardDraftSentence.Single(row => row.LSentenceDraftId == sentence).LSentenceDraftExample;
        long? cited = example?.LExampleDraftReference.LStateAnchorShown;
        Assert.NotNull(cited);
        Assert.Equal("Field notes", engine.TEngineCitationRead()[cited!.Value]);
    }

    [Fact]
    public void TagFind_TypedPrefix_FindsTheStoredTag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (_, CCard card) = TCardPrepare(engine);

        IReadOnlyList<CTag> found = card.CCardTagFind(0, "ani");

        Assert.Contains(new CTag(tag.LTagId, "animal"), found);
    }

    [Fact]
    public void TagAdd_PaddedText_AddsTheTrimmedTagOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        card.CCardTagAdd(sheet, "  animal ", 0, true);
        card.CCardTagAdd(sheet, "animal", 1, true);
        card.CCardTagAdd(sheet, "   ", 1, true);

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal("animal", held.LTagDraftText);
    }

    [Fact]
    public void TagInsert_StoredTag_LinksItUnderItsOwnId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag stored = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        card.CCardTagInsert(sheet, stored.LTagId, 0);
        card.CCardTagAdd(sheet, "animal", 1, true);

        LTagDraft held = Assert.Single(TCardTagRead(desk, sheet));
        Assert.Equal(stored.LTagId, held.LTagDraftId);
    }

    [Fact]
    public void TagRemove_AddedTag_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);
        card.CCardTagAdd(sheet, "animal", 0, true);

        card.CCardTagRemove(sheet, TCardTagRead(desk, sheet)[0].LTagDraftId);

        Assert.Empty(TCardTagRead(desk, sheet));
    }

    [Fact]
    public void TagAdd_TypedList_AddsTheCompletedTagsAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        string kept = card.CCardTagAdd(sheet, " ani", 0, false);
        string rest = card.CCardTagAdd(sheet, "animal, , plant,  wi", 0, false);

        Assert.Equal(" ani", kept);
        Assert.Equal("wi", rest);
        Assert.Equal(["animal", "plant"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
    }

    [Fact]
    public void TagFind_TagHeldOnTheCard_LeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);
        long other = TCardSheetAdd(desk);
        card.CCardTagInsert(sheet, tag.LTagId, 0);

        Assert.DoesNotContain(new CTag(tag.LTagId, "animal"), card.CCardTagFind(sheet, "ani"));
        Assert.Contains(new CTag(tag.LTagId, "animal"), card.CCardTagFind(other, "ani"));
    }

    [Fact]
    public void SituationAdd_TypedList_AddsTheCompletedTitlesOnceAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        string rest = card.CCardSituationAdd(sheet, "Hearth, ,  Kitchen, Ga", 0, false);
        string settled = card.CCardSituationAdd(sheet, " Hearth ", 2, true);
        string blank = card.CCardSituationAdd(sheet, "   ", 2, true);

        Assert.Equal("Ga", rest);
        Assert.Equal(string.Empty, settled);
        Assert.Equal(string.Empty, blank);
        Assert.Equal(
            ["Hearth", "Kitchen"],
            TCardSituationRead(desk, sheet).Select(static row => row.LSituationDraftTitle.LStateValueShown));
    }

    [Fact]
    public void SituationInsert_StoredSituation_LinksItOnceAndRemoveDropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Hearth", null, null));
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        card.CCardSituationInsert(sheet, hearth.LSituationId, 0);
        card.CCardSituationInsert(sheet, hearth.LSituationId, 1);

        Assert.Equal(hearth.LSituationId, Assert.Single(TCardSituationRead(desk, sheet)).LSituationDraftId);

        card.CCardSituationRemove(sheet, hearth.LSituationId);

        Assert.Empty(TCardSituationRead(desk, sheet));
    }

    [Fact]
    public void SituationFind_SituationHeldOnTheCard_LeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, "Hearth", null, null));
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);
        long other = TCardSheetAdd(desk);
        card.CCardSituationInsert(sheet, hearth.LSituationId, 0);

        Assert.DoesNotContain(
            card.CCardSituationFind(sheet, "Hear"), row => row.CCatalogSituationId == hearth.LSituationId);
        Assert.Contains(card.CCardSituationFind(other, "Hear"), row => row.CCatalogSituationId == hearth.LSituationId);
    }

    [Fact]
    public void RegisterAdd_TypedList_AddsTheCompletedNamesOnceAndKeepsTheRest()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        string rest = card.CCardRegisterAdd(sheet, "formal,  slang, fo", 0, false);
        string settled = card.CCardRegisterAdd(sheet, "slang", 2, true);

        Assert.Equal("fo", rest);
        Assert.Equal(string.Empty, settled);
        Assert.Equal(
            ["formal", "slang"],
            TCardRegisterRead(desk, sheet).Select(static row => row.LRegisterDraftName.LStateValueShown));
    }

    [Fact]
    public void RegisterInsert_StoredRegister_LinksItOnceAndFindLeavesItOut()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister formal = engine.TEngineRegisterCreate("formal");
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);
        long other = TCardSheetAdd(desk);

        card.CCardRegisterInsert(sheet, formal.LRegisterId, 0);
        card.CCardRegisterInsert(sheet, formal.LRegisterId, 1);

        Assert.Equal(formal.LRegisterId, Assert.Single(TCardRegisterRead(desk, sheet)).LRegisterDraftId);
        Assert.DoesNotContain(
            card.CCardRegisterFind(sheet, "form", "English"), row => row.CRegisterId == formal.LRegisterId);
        Assert.Contains(card.CCardRegisterFind(other, "form", "English"), row => row.CRegisterId == formal.LRegisterId);
    }

    [Fact]
    public void TranslationAdd_TypedList_LinksResolvedWordsAndKeepsTheUnresolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        string rest = card.CCardTranslationAdd(sheet, " chat, zzyzx, , ch", 0);

        Assert.Equal("zzyzx, ch", rest);
        Assert.Equal([chat], TCardTranslationRead(desk, sheet));
        Assert.Equal("ch", card.CCardTranslationAdd(sheet, "ch", 1));
    }

    [Fact]
    public void TranslationInsert_StoredEntry_LinksItOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, chat, 0);
        card.CCardTranslationAdd(sheet, "chat,", 1);

        Assert.Equal([chat], TCardTranslationRead(desk, sheet));
    }

    [Fact]
    public void ReferenceFind_NoWord_ListsEveryStoredSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (_, CCard card) = TCardPrepare(engine);

        Assert.Contains(card.CCardReferenceFind(), row => row.CCatalogReferenceId == notes.LReferenceId);
        Assert.Contains(card.CCardReferenceFind("Field"), row => row.CCatalogReferenceId == notes.LReferenceId);
    }

    [Fact]
    public void SituationFind_TypedTitle_FindsTheStoredSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSituation hearth = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        (_, CCard card) = TCardPrepare(engine);

        Assert.Contains(card.CCardSituationFind(0, "Hear"), row => row.CCatalogSituationId == hearth.LSituationId);
    }

    [Fact]
    public void TranslationResolve_StoredHeadword_AnswersItsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (_, CCard card) = TCardPrepare(engine);

        Assert.Equal(chat, card.CCardTranslationResolve("chat", null));
        Assert.Contains(card.CCardProspectFind("chat"), row => row.CVistaRowId == chat);
    }

    [Fact]
    public void CourtDelete_StartedCourt_DropsTheLinkAndItsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCardPrepare(engine);

        long target = card.CCardCourtStart(desk.CDeskId, "Input", "chien", "French");
        card.CCardCourtDelete(desk.CDeskId, target);

        Assert.Null(engine.TEngineCourtFind(desk.CDeskId, target));
        Assert.Null(engine.TEngineDraftRead(target));
    }

    [Fact]
    public void OrderRead_TrailingOrder_WritesTheRoleBeforeTheMarker()
    {
        CStateValue of = new("of", false, true);
        CStateValue thing = new("Something", false, true);
        CStateValue text = new("an example", false, true);

        Assert.Equal("(+of Something) an example", CCard.CCardOrderRead(null, of, thing, text, "?"));
        Assert.Equal(
            "(+Something of) an example",
            CCard.CCardOrderRead(new CSentenceOrder(1, 0), of, thing, text, "?"));
        Assert.Equal(
            "(+?)",
            CCard.CCardOrderRead(
                null,
                new CStateValue(string.Empty, true, false),
                CStateValue.CStateValueEmpty,
                CStateValue.CStateValueEmpty,
                "?"));
    }

    [Fact]
    public void OrderRead_EmptyFields_DropsTheHeadOrTheSpace()
    {
        CStateValue to = new("to", false, true);
        CStateValue text = new("text", false, true);
        CStateValue none = CStateValue.CStateValueEmpty;

        Assert.Equal("(+to)", CCard.CCardOrderRead(null, to, none, none, "?"));
        Assert.Equal(string.Empty, CCard.CCardOrderRead(null, none, none, none, "?"));
        Assert.Equal("text", CCard.CCardOrderRead(null, none, none, text, "?"));
    }

    private static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TInterfaceConduct.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        return (desk, TInterfaceConduct.TCardCreate(engine, desk, TInterfaceConduct.TEnvoyCreate(false, [])));
    }

    private static LEtymologyDraft TCardEtymologyRead(CDesk desk)
    {
        return desk.TDeskRead()!.LDraftContent.LEntryDraftEtymology;
    }

    private static long TCardSheetAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
    }

    private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTag;
    }

    private static IReadOnlyList<LSituationDraft> TCardSituationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftSituation;
    }

    private static IReadOnlyList<LRegisterDraft> TCardRegisterRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftRegister;
    }

    private static IReadOnlyList<long> TCardTranslationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTranslation;
    }

    private static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long sheet = desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        LCardDraft carded = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        return (sheet, carded.LCardDraftSentence[0].LSentenceDraftId);
    }
}
