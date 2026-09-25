using System;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymology
{
    [Fact]
    public void EtymologyCommit_ProseBesideLinks_StoresTheProseAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source, 0));
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineDraftCommit(held.LDraftId);

        LEtymologyDraft stored = TEtymologyRead(engine, entryId);
        Assert.Equal("From rinnan.", stored.LEtymologyDraftText);
        Assert.Empty(stored.LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymologyCommit_ProseCleared_LeavesTheLinksAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long first = TEtymologyEntryCreate(engine, "rinnan");
        long second = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineDraftCommit(held.LDraftId);

        LDraft again = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(again.LDraftId, string.Empty));
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(again.LDraftId, first, 0));
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(again.LDraftId, second, 1));
        engine.TEngineDraftCommit(again.LDraftId);

        LEtymologyDraft stored = TEtymologyRead(engine, entryId);
        Assert.Equal(string.Empty, stored.LEtymologyDraftText);
        Assert.Equal([first, second], stored.LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymonShift_MovedLink_StoresTheNewOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long first = TEtymologyEntryCreate(engine, "rinnan");
        long second = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, first, 0));
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, second, 1));
        engine.TEngineRequestApply(TInterface.TEtymonShiftCreate(held.LDraftId, second, 0));
        engine.TEngineDraftCommit(held.LDraftId);

        Assert.Equal([second, first], TEtymologyRead(engine, entryId).LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymonRemoval_LastLink_ClearsTheEtymology()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source, 0));
        engine.TEngineDraftCommit(held.LDraftId);

        LDraft again = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonRemovalCreate(again.LDraftId, source));
        engine.TEngineDraftCommit(again.LDraftId);

        Assert.True(TEtymologyRead(engine, entryId).LEtymologyDraftEmpty);
    }

    [Fact]
    public void EtymonAddition_UnknownOrRepeatedEntry_RefusesTheLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source, 0));

        LRefusal unknown = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source + 900, 0)));
        LRefusal repeated = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TEtymonAdditionCreate(held.LDraftId, source, 1)));

        Assert.Equal(LRefusal.LRefusalLink, unknown.LRefusalReason);
        Assert.Equal(LRefusal.LRefusalLink, repeated.LRefusalReason);
        Assert.Single(TEtymologyHeldRead(engine, held.LDraftId).LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymologyMention_TextOutsideTheBasicPlane_CountsInCodePoints()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "𝄞 from rinnan"));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 7, 6, source));
        engine.TEngineDraftCommit(held.LDraftId);

        LMentionDraft mention = Assert.Single(TEtymologyRead(engine, entryId).LEtymologyDraftMentions);
        Assert.Equal((7, 6, source), (
            mention.LMentionDraftOffset, mention.LMentionDraftLength, mention.LMentionDraftEntry));
    }

    [Fact]
    public void EtymologyMention_SpanPastTheText_RefusesTheSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));

        LRefusal refusal = Assert.Throws<LRefusal>(
            () => engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 40, source)));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
        Assert.Empty(TEtymologyHeldRead(engine, held.LDraftId).LEtymologyDraftMentions);
    }

    [Fact]
    public void EtymologyText_Shortened_DropsTheSpanItCutAway()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 6, source));
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From."));

        Assert.Empty(TEtymologyHeldRead(engine, held.LDraftId).LEtymologyDraftMentions);
    }

    [Fact]
    public void EtymologyMention_OverAStandingSpan_ReplacesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long first = TEtymologyEntryCreate(engine, "rinnan");
        long second = TEtymologyEntryCreate(engine, "iernan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 6, first));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 0, 11, second));

        LMentionDraft mention = Assert.Single(TEtymologyHeldRead(engine, held.LDraftId).LEtymologyDraftMentions);
        Assert.Equal(second, mention.LMentionDraftEntry);
    }

    [Fact]
    public void EtymologyMention_NamingNoEntry_ClearsTheSpanUnderIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long source = TEtymologyEntryCreate(engine, "rinnan");
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 6, source));
        engine.TEngineRequestApply(TInterface.TEtymologyMentionCreate(held.LDraftId, 5, 6, 0));

        Assert.Empty(TEtymologyHeldRead(engine, held.LDraftId).LEtymologyDraftMentions);
    }

    [Fact]
    public void EtymologyCommit_ChangedEtymology_RecordsOneRevisionChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long entryId = TEtymologyEntryCreate(engine, "run");

        LDraft held = engine.TEngineDraftStart("Input", entryId);
        engine.TEngineRequestApply(TInterface.TEtymologyTextCreate(held.LDraftId, "From rinnan."));
        engine.TEngineDraftCommit(held.LDraftId);

        long revision = Assert.IsType<long>(engine.TEngineRevisionRead());
        LRevisionChange change = Assert.Single(
            workspace.TRevisionChangeRead(revision),
            static row => string.Equals(row.LRevisionChangeSubject, "etymology", StringComparison.Ordinal));
        Assert.Equal("create", change.LRevisionChangeKind);
        Assert.Equal("From rinnan.", change.LRevisionChangeSummary);
    }

    private static long TEtymologyEntryCreate(LEngine engine, string headword)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            headword, "English", string.Empty, string.Empty, [], [])).LEntryId;
    }

    private static LEtymologyDraft TEtymologyRead(LEngine engine, long entryId)
    {
        return Assert.IsType<LEntryDraft>(engine.TEngineEntryLoad(entryId)).LEntryDraftEtymology;
    }

    private static LEtymologyDraft TEtymologyHeldRead(LEngine engine, long draftId)
    {
        return Assert.IsType<LDraft>(engine.TEngineDraftRead(draftId)).LDraftContent.LEntryDraftEtymology;
    }
}
