using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCardEtymology
{
    [Fact]
    public void EtymologySet_TextTyped_WritesTheNarrative()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);

        card.CCardEtymologySet("from Latin");

        Assert.Equal("from Latin", TCardEtymologyRead(desk).LEtymologyDraftText);
    }

    [Fact]
    public void EtymonAdd_StoredEntry_AppendsTheSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);

        card.CCardEtymonAdd(cat);

        Assert.Equal([cat], TCardEtymologyRead(desk).LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymonRemove_AddedSource_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cat", "English").LEntryId;
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
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
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
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
        (CDesk desk, CCard card) = TCard.TCardPrepare(engine);
        card.CCardEtymologySet("from cattus");
        card.CCardMentionSave("from cattus", 5, 6, cat);

        card.CCardMentionDelete("from cattus", 6, 2);

        Assert.Empty(TCardEtymologyRead(desk).LEtymologyDraftMentions);
        Assert.False(card.CCardMentionCheck("from cattus", 6, 2));
    }

    [Fact]
    public void CardEtymologyRead_LinkedSpan_NamesTheWordAndTheHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long pattern = engine.TEngineTranslationCreate("pattern", "English").LEntryId;
        (_, CCard card) = TCard.TCardPrepare(engine);
        card.CCardEtymologySet("a pattern here");
        card.CCardMentionSave("a pattern here", 2, 7, pattern);

        IReadOnlyList<CMentionLabel> labels = card.CCardEtymologyRead();

        CMentionLabel label = Assert.Single(labels);
        Assert.Equal(new CMentionLabel(label.CMentionLabelId, "pattern", "pattern", string.Empty, true), label);
        Assert.Null(label.CMentionLabelKey);
    }

    [Fact]
    public void CardEtymologyRead_EmptyDesk_AnswersNoChips()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        CCard card = TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Empty(card.CCardEtymologyRead());
    }

    private static LEtymologyDraft TCardEtymologyRead(CDesk desk)
    {
        return desk.TDeskRead()!.LDraftContent.LEntryDraftEtymology;
    }
}
