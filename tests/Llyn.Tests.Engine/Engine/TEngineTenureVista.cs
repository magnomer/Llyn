using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineTenureVista
{
    [Fact]
    public void OccurrenceStart_SituationGiven_StartsAFreshEntryAlreadyLinked()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LStateValue text = TInterfaceState.TStateValueCreate("at the market");
        LSituation market = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineOccurrenceStart(vista, market.LSituationId);

        LEntryDraft? content = tenure.TTenureRead()?.LDraftContent;
        Assert.Null(tenure.TTenureRead()?.LDraftStored);
        Assert.Contains(
            content?.LEntryDraftMeanings[0].LCardDraftSituation ?? [],
            row => row.LSituationDraftId == market.LSituationId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void OccurrenceStart_NoSituation_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineOccurrenceStart(vista, null);

        Assert.False(tenure.TTenureStateRead().LTenureStateChanged);
        Assert.All(
            tenure.TTenureRead()?.LDraftContent.LEntryDraftMeanings ?? [],
            card => Assert.Empty(card.LCardDraftSituation));
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureStorable_UnnamedOccurrence_AnswersFalseUntilTheHeadwordIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TEngineTenure.TTenureHold);
        LStateValue text = TInterfaceState.TStateValueCreate("at the market");
        LSituation market = engine.TEngineSituationCreate(TInterface.TSituationCreate(0, text, text, text));
        LVista vista = engine.TEngineVistaStart("occurrence", LCatalogOrder.LCatalogOrderHeadword);
        LTenure tenure = engine.TEngineOccurrenceStart(vista, market.LSituationId);

        Assert.True(tenure.TTenureChangeCheck());
        Assert.False(tenure.TTenureStorableRead());

        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "stall"));

        Assert.False(tenure.TTenureStorableRead());
        Assert.True(tenure.TTenureChangeCheck());
        Assert.True(tenure.TTenureStorableRead());
        Assert.Equal("stall", tenure.TTenureRead()?.LDraftContent.LEntryDraftHeadword);
        tenure.TTenureCancel();
    }

    [Fact]
    public void QuotationStart_ExampleGiven_StartsAFreshEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LExample example = engine.TEngineExampleCreate(TInterfaceExample.TExampleCreate(
            0,
            "English",
            TInterfaceState.TStateValueCreate("Water is wet."),
            null,
            LStateAnchor.LStateAnchorUnspecified));
        LVista vista = engine.TEngineVistaStart("quotation", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineQuotationStart(vista, example.LExampleId);

        LExampleDraft? cited = tenure.TTenureRead()?.LDraftContent
            .LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(example.LExampleId, cited?.LExampleDraftId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void FootnoteStart_SourceGiven_StartsAFreshEntryCitingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference book = engine.TEngineCitationCreate("Book");
        LVista vista = engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineFootnoteStart(vista, book.LReferenceId);

        LExampleDraft? cited = tenure.TTenureRead()?.LDraftContent
            .LEntryDraftMeanings[0].LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(book.LReferenceId, cited?.LExampleDraftReference.LStateAnchorShown);
        tenure.TTenureCancel();
    }

    [Fact]
    public void FootnoteStart_NoSource_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("footnote", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineFootnoteStart(vista, null);

        Assert.False(tenure.TTenureChangeCheck());
        tenure.TTenureCancel();
    }

    [Fact]
    public void MembershipStart_TagGiven_StartsAFreshEntryCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTag tag = engine.TEngineTagCreate("botany");
        LVista vista = engine.TEngineVistaStart("membership", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineMembershipStart(vista, tag.LTagId);

        Assert.Contains(
            tenure.TTenureRead()?.LDraftContent.LEntryDraftMeanings[0].LCardDraftTag ?? [],
            row => row.LTagDraftId == tag.LTagId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void MembershipStart_NoTag_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("membership", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineMembershipStart(vista, null);

        Assert.False(tenure.TTenureChangeCheck());
        tenure.TTenureCancel();
    }

    [Fact]
    public void CohortStart_RegisterGiven_StartsAFreshEntryCarryingIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LRegister register = engine.TEngineRegisterCreate("formal");
        LVista vista = engine.TEngineVistaStart("cohort", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineCohortStart(vista, register.LRegisterId);

        Assert.Contains(
            tenure.TTenureRead()?.LDraftContent.LEntryDraftMeanings[0].LCardDraftRegister ?? [],
            row => row.LRegisterDraftId == register.LRegisterId);
        tenure.TTenureCancel();
    }

    [Fact]
    public void CohortStart_NoRegister_StartsABlankEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LVista vista = engine.TEngineVistaStart("cohort", LCatalogOrder.LCatalogOrderHeadword);

        LTenure tenure = engine.TEngineCohortStart(vista, null);

        Assert.False(tenure.TTenureChangeCheck());
        tenure.TTenureCancel();
    }
}
