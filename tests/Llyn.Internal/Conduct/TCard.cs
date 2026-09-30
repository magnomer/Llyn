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

        CEntryDraft shaped = TInterfaceConduct.TCardEntryRead(
            draft, new Dictionary<long, IReadOnlyList<LTranslationTarget>>());

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
    public void TagAdd_TypedPrefix_OffersTheStoredTagSplitAroundTheWord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (_, CCard card) = TCardPrepare(engine);

        CSlate slate = card.CCardTagAdd(0, " ani", 0, false);

        Assert.Equal(" ani", slate.CSlateText);
        Assert.True(slate.CSlateShown);
        Assert.Contains(new CSlateRow(tag.LTagId, string.Empty, "ani", "mal"), slate.CSlateRows);
    }

    [Fact]
    public void TagAdd_BlankOrUnmatchedText_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTagCreate("animal");
        (_, CCard card) = TCardPrepare(engine);

        CSlate blank = card.CCardTagAdd(0, "   ", 0, false);
        CSlate unmatched = card.CCardTagAdd(0, "zz", 0, false);

        Assert.Equal("   ", blank.CSlateText);
        Assert.False(blank.CSlateShown);
        Assert.Empty(blank.CSlateRows);
        Assert.False(unmatched.CSlateShown);
        Assert.Empty(unmatched.CSlateRows);
    }

    [Fact]
    public void TagAdd_NineMatchingTags_OffersTheFirstEight()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        for (int index = 1; index <= 9; index++)
        {
            engine.TEngineTagCreate("tag " + index);
        }

        (_, CCard card) = TCardPrepare(engine);

        CSlate slate = card.CCardTagAdd(0, "tag", 0, false);

        Assert.Equal(8, slate.CSlateRows.Count);
        Assert.Equal("tag 1", slate.CSlateRows[0].CSlateRowMark + slate.CSlateRows[0].CSlateRowTail);
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

        string kept = card.CCardTagAdd(sheet, " ani", 0, false).CSlateText;
        string rest = card.CCardTagAdd(sheet, "animal, , plant,  wi", 0, false).CSlateText;

        Assert.Equal(" ani", kept);
        Assert.Equal("wi", rest);
        Assert.Equal(["animal", "plant"], TCardTagRead(desk, sheet).Select(static tag => tag.LTagDraftText));
    }

    [Fact]
    public void TagAdd_TagHeldOnTheCard_LeavesItOutOfTheOffer()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("animal");
        (CDesk desk, CCard card) = TCardPrepare(engine);
        long sheet = TCardSheetAdd(desk);
        long other = TCardSheetAdd(desk);
        card.CCardTagInsert(sheet, tag.LTagId, 0);

        CSlateRow row = new(tag.LTagId, string.Empty, "ani", "mal");
        Assert.DoesNotContain(row, card.CCardTagAdd(sheet, "ani", 1, false).CSlateRows);
        Assert.Contains(row, card.CCardTagAdd(other, "ani", 0, false).CSlateRows);
    }

    [Fact]
    public void ReferenceFind_NoWord_ListsEveryStoredSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        (_, CCard card) = TCardPrepare(engine);

        Assert.Contains(card.CCardReferenceRead(), row => row.CCatalogReferenceId == notes.LReferenceId);
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

    internal static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        return (desk, TInterfaceConduct.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, [])));
    }

    private static LEtymologyDraft TCardEtymologyRead(CDesk desk)
    {
        return desk.TDeskRead()!.LDraftContent.LEntryDraftEtymology;
    }

    internal static long TCardSheetAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
    }

    private static IReadOnlyList<LTagDraft> TCardTagRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTag;
    }

    internal static IReadOnlyList<long> TCardTranslationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTranslation;
    }

    internal static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long sheet = desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        LCardDraft carded = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        return (sheet, carded.LCardDraftSentence[0].LSentenceDraftId);
    }
}
