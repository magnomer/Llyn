using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineTenureSubject
{
    [Fact]
    public void TenureFinish_Changed_CommitsExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("ember")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("ember", engine.TEngineExampleRead(stored.Value)?.LExampleText.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
        tenure.TTenureRequestDefer(TInterface.TRequestHeadwordCreate(tenure.LTenureId, "kindle"));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("kindle", engine.TEngineEntryRead(stored.Value)?.LEntryHeadword);
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsSituation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectSituation, null);
        tenure.TTenureRequestDefer(TInterface.TSituationTitleCreate(
            tenure.LTenureId,
            tenure.TTenureRead()!.LDraftSituation!.LSituationId,
            TInterfaceState.TStateValueCreate("around a hearth")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("around a hearth", engine.TEngineSituationRead(stored.Value)?.LSituationTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectReference, null);
        tenure.TTenureRequestDefer(
            TInterface.TReferenceTitleCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("the evening news")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("the evening news", engine.TEngineReferenceRead(stored.Value)?.LReferenceTitle.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureFinish_Changed_CommitsAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);
        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada Lovelace"));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("Ada Lovelace", engine.TEngineAuthorRead(stored.Value)?.LAuthorName);
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureReadyCheck_AuthorNamedLater_ReadyOnceTheNameIsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TEngineTenure.TTenureHold);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);

        Assert.False(tenure.TTenureReadyCheck());

        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada"));

        Assert.True(tenure.TTenureReadyCheck());
        Assert.Equal("Ada", tenure.TTenureRead()?.LDraftAuthorHeld?.LAuthorName);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Renamed_UpdatesAuthor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, author.LAuthorId);
        Assert.Equal("Ada", tenure.TTenureRead()?.LDraftAuthorHeld?.LAuthorName);
        Assert.False(tenure.TTenureStateRead().LTenureStateChanged);

        tenure.TTenureRequestDefer(TInterface.TAuthorNameCreate(tenure.LTenureId, "Ada Lovelace"));
        tenure.TTenurePersist();
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);

        Assert.Equal(author.LAuthorId, tenure.TTenureFinish(true));
        Assert.Equal("Ada Lovelace", engine.TEngineAuthorRead(author.LAuthorId)?.LAuthorName);
        Assert.Single(engine.TEngineAuthorFind(string.Empty));
    }
}
