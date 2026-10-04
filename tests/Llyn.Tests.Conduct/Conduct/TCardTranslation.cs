using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardTranslation
{
    [Fact]
    public void TranslationAdd_TypedList_LinksResolvedWordsAndKeepsTheUnresolved()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        CProspect rest = card.CCardTranslationAdd(sheet, " chat, zzyzx, , ch", 0);

        Assert.Equal("zzyzx, ch", rest.CProspectText);
        Assert.Equal([chat], TCard.TCardTranslationRead(desk, sheet));
        Assert.Equal("ch", card.CCardTranslationAdd(sheet, "ch", 1).CProspectText);
    }

    [Fact]
    public void TranslationAdd_TypedWord_OffersTheMatchingEntriesWithNoneChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        CProspect typed = card.CCardTranslationAdd(sheet, " chat ", 0);
        CProspect blank = card.CCardTranslationAdd(sheet, "  ", 0);

        Assert.Equal(" chat ", typed.CProspectText);
        Assert.Equal("chat", typed.CProspectWord);
        Assert.Contains(typed.CProspectRows, row => row.CVistaRowId == chat);
        Assert.True(typed.CProspectShown);
        Assert.False(typed.CProspectChosen);
        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
        Assert.False(blank.CProspectShown);
        Assert.Empty(blank.CProspectRows);
    }

    [Fact]
    public void TranslationAdd_TypedWord_OffersAFreshEntryInEveryLanguageWithTheDraftLanguageLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        IReadOnlyList<string> languages = engine.TEngineLanguageRead();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "Klingon"));
        long sheet = TCard.TCardSheetAdd(desk);

        CProspect typed = card.CCardTranslationAdd(sheet, "chat", 0);
        CProspect resolved = card.CCardTranslationResolve(sheet, "chat", 0, true);

        Assert.Equal([.. languages, "Klingon"], typed.CProspectLanguages);
        Assert.Equal(typed.CProspectLanguages, resolved.CProspectLanguages);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, languages[0]));
        Assert.Equal(
            [.. languages.Skip(1), languages[0]], card.CCardTranslationAdd(sheet, "chat", 0).CProspectLanguages);
        Assert.Empty(card.CCardTranslationAdd(sheet, " ", 0).CProspectLanguages);
        Assert.Empty(card.CCardMentionRead("chat").CProspectLanguages);
    }

    [Fact]
    public void TranslationAdd_EditedEntry_LeavesItOutAndKeepsItsTwinNumbered()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long edited = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("water", "English", "", "", [], [])).LEntryId;
        long twin = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("water", "English", "", "", [], [])).LEntryId;
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(edited);
        CCard card = TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, []));

        CProspect typed = card.CCardTranslationAdd(TCard.TCardSheetAdd(desk), "water", 0);

        CVistaRow row = Assert.Single(typed.CProspectRows);
        Assert.Equal(twin, row.CVistaRowId);
        Assert.Equal("water (2)", row.CVistaRowName);
    }

    [Fact]
    public void TranslationInsert_StoredEntry_LinksItOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, chat, string.Empty, string.Empty, 0);
        card.CCardTranslationAdd(sheet, "chat,", 1);

        Assert.Equal([chat], TCard.TCardTranslationRead(desk, sheet));
    }

    [Fact]
    public void TranslationResolve_OfferedWord_AnswersTheProspectsWithTheWholeMatchChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        CProspect offered = card.CCardTranslationResolve(sheet, " chat ", 0, true);
        CProspect blank = card.CCardTranslationResolve(sheet, " ", 0, true);

        Assert.Equal(" chat ", offered.CProspectText);
        Assert.Equal("chat", offered.CProspectWord);
        Assert.Contains(offered.CProspectRows, row => row.CVistaRowId == chat);
        Assert.True(offered.CProspectShown);
        Assert.True(offered.CProspectChosen);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "French"));
        Assert.Contains(card.CCardMentionRead("chat").CProspectRows, row => row.CVistaRowId == chat);
        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
        Assert.False(blank.CProspectShown);
    }

    [Fact]
    public void TranslationResolve_LeftWord_LinksTheWholeMatchAndEmptiesTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        CProspect left = card.CCardTranslationResolve(sheet, "chat", 0, false);
        CProspect kept = card.CCardTranslationResolve(sheet, "zzyzx", 1, false);

        Assert.Equal(string.Empty, left.CProspectText);
        Assert.False(left.CProspectShown);
        Assert.Equal([chat], TCard.TCardTranslationRead(desk, sheet));
        Assert.Equal("zzyzx", kept.CProspectText);
        Assert.False(kept.CProspectShown);
    }

    [Fact]
    public void MentionRead_PaddedWord_FindsTheEntryTheTrimmedWordFinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "French"));

        Assert.Equal(chat, Assert.Single(card.CCardMentionRead("  chat ").CProspectRows).CVistaRowId);
    }

    [Fact]
    public void MentionRead_DraftInEnglish_OffersOnlyEnglishEntriesWithTheFirstChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long english = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        engine.TEngineTranslationCreate("cat", "French");
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "English"));

        CProspect offered = card.CCardMentionRead(" cat ");

        Assert.Equal(english, Assert.Single(offered.CProspectRows).CVistaRowId);
        Assert.Equal("cat", offered.CProspectWord);
        Assert.True(offered.CProspectShown);
        Assert.True(offered.CProspectChosen);
    }

    [Fact]
    public void MentionRead_BlankOrUnmatchedWord_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineTranslationCreate("cat", "English");
        (_, CCard card) = TCard.TCardPrepare(engine);

        CProspect blank = card.CCardMentionRead("  ");
        CProspect unmatched = card.CCardMentionRead("zzyzx");

        Assert.Empty(blank.CProspectRows);
        Assert.False(blank.CProspectShown);
        Assert.Empty(unmatched.CProspectRows);
        Assert.False(unmatched.CProspectShown);
    }

    [Fact]
    public void TranslationInsert_FreshRow_StartsACourtThatRemoveDrops()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, null, "chien", "French", 0);
        long target = Assert.Single(TCard.TCardTranslationRead(desk, sheet));

        Assert.NotNull(engine.TEngineCourtFind(desk.CDeskId, target));
        Assert.Equal("chien", engine.TEngineDraftRead(target)?.LDraftContent.LEntryDraftHeadword);
        Assert.Equal("Input", engine.TEngineDraftRead(target)?.LDraftOrigin);

        card.CCardTranslationRemove(sheet, target);

        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
        Assert.Null(engine.TEngineCourtFind(desk.CDeskId, target));
        Assert.Null(engine.TEngineDraftRead(target));
    }

    [Fact]
    public void TranslationRemove_StoredEntry_UnlinksItAndKeepsTheEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);
        card.CCardTranslationInsert(sheet, chat, string.Empty, string.Empty, 0);

        card.CCardTranslationRemove(sheet, chat);

        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
        desk.TDeskDefer(TInterface.TRequestLanguageCreate(desk.CDeskId, "French"));
        Assert.Contains(card.CCardMentionRead("chat").CProspectRows, row => row.CVistaRowId == chat);
    }

    [Fact]
    public void TranslationInsert_BlankFreshWord_ShowsTheTranslationFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CCard card = TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, asked));
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, null, " ", "French", 0);

        Assert.Equal(["Input.TranslationFailed"], asked);
        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
    }

    [Fact]
    public void TranslationAdd_DuplicateHeadwords_CarriesNamesWithoutChangingStoredText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        IReadOnlyList<CVistaRow> rows = card.CCardTranslationAdd(TCard.TCardSheetAdd(desk), "water", 0).CProspectRows;
        Assert.Equal(["water (1)", "water (2)"], rows.Select(row => row.CVistaRowName));
        Assert.All(rows, row => Assert.Equal("water", row.CVistaRowHeadword));
    }
}
