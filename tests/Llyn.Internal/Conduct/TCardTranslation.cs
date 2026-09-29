using System.Collections.Generic;
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
    public void TranslationInsert_StoredEntry_LinksItOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, chat, "chat", "French", 0);
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
        Assert.Contains(card.CCardProspectFind("chat"), row => row.CVistaRowId == chat);
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
    public void ProspectFind_PaddedWord_FindsTheEntryTheTrimmedWordFinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        (_, CCard card) = TCard.TCardPrepare(engine);

        Assert.Equal(chat, Assert.Single(card.CCardProspectFind("  chat ")).CVistaRowId);
    }

    [Fact]
    public void TranslationInsert_FreshRow_StartsACourtThatRemoveDrops()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, 0, "chien", "French", 0);
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
        card.CCardTranslationInsert(sheet, chat, "chat", "French", 0);

        card.CCardTranslationRemove(sheet, chat);

        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
        Assert.Contains(card.CCardProspectFind("chat"), row => row.CVistaRowId == chat);
    }

    [Fact]
    public void TranslationInsert_BlankFreshWord_ShowsTheTranslationFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        List<string> asked = [];
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Input", TInterfaceConduct.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        CCard card = TInterfaceConduct.TCardCreate(engine, desk, TInterfaceConduct.TEnvoyCreate(false, asked));
        long sheet = TCard.TCardSheetAdd(desk);

        card.CCardTranslationInsert(sheet, 0, " ", "French", 0);

        Assert.Equal(["Input.TranslationFailed"], asked);
        Assert.Empty(TCard.TCardTranslationRead(desk, sheet));
    }
}
