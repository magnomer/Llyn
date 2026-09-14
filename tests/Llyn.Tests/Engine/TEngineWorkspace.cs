using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineWorkspace
{
    [Fact]
    public void WorkspaceOpen_TargetHasSettings_KeepsTargetSettings()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        TInterface.TSettingsSave(second.TWorkspaceFolder, TInterface.TSettingsCreate("ko", respelled: true));
        using LEngine engine = first.TWorkspaceEngineStart();
        engine.TEngineLocalizationSave("en");

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        LSettings settings = engine.TEngineSettingsRead();

        Assert.Equal(second.TWorkspaceFolder, engine.TEngineWorkspaceRead());
        Assert.Equal("ko", settings.LSettingsLocalization);
        Assert.True(settings.LSettingsRespelled);
    }

    [Fact]
    public void WorkspaceOpen_TargetEmpty_InheritsCurrentSettings()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.True(TInterface.TSettingsExist(second.TWorkspaceFolder));
        Assert.True(TInterface.TSettingsLoad(second.TWorkspaceFolder).LSettingsRespelled);
        Assert.True(engine.TEngineSettingsRead().LSettingsRespelled);
    }

    [Fact]
    public void WorkspaceOpen_RelativePath_RefusesAndKeepsWorkspace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string relative = "llyn-test-relative-" + Guid.NewGuid().ToString("n");

        Assert.Throws<ArgumentException>(() => engine.TEngineWorkspaceOpen(relative));

        Assert.Equal(workspace.TWorkspaceFolder, engine.TEngineWorkspaceRead());
        Assert.False(Directory.Exists(relative));
    }

    [Fact]
    public void WorkspaceOpen_TargetUnopenable_KeepsEverything()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        Directory.CreateDirectory(Path.Combine(second.TWorkspaceFolder, "llyn.db"));
        using LEngine engine = first.TWorkspaceEngineStart();
        engine.TEngineRespellingSave(true);
        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.ThrowsAny<Exception>(() => engine.TEngineWorkspaceOpen(second.TWorkspaceFolder));

        Assert.Equal(first.TWorkspaceFolder, engine.TEngineWorkspaceRead());
        Assert.True(engine.TEngineSettingsRead().LSettingsRespelled);
        Assert.False(TInterface.TSettingsExist(second.TWorkspaceFolder));
        Assert.NotNull(engine.TEngineStateRead());
        Assert.NotNull(engine.TEngineDraftRead(started.LDraftId));
    }

    [Fact]
    public void WorkspaceOpen_HeldDraft_RefusesLaterCommitAsStale()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("editor", null);

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineDraftCommit(started.LDraftId));

        Assert.Equal(LRefusal.LRefusalStale, refusal.LRefusalReason);
    }

    [Fact]
    public void WorkspaceOpen_Observer_RaisesWorkspaceBulletinOnce()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        TWorkspaceObserver observer = new();
        engine.TEngineObserverAttach(observer);

        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        Assert.Single(observer.TWorkspaceObserverRaised);
        Assert.Equal(LSubject.LSubjectWorkspace, observer.TWorkspaceObserverRaised[0].LBulletinSubject);
    }

    [Fact]
    public void WorkspaceOpen_ThenSave_ReopensOnTarget()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();

        using (LEngine engine = first.TWorkspaceEngineStart())
        {
            engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);
            engine.TEngineLocalizationSave("ko");
        }

        using LEngine reopened = second.TWorkspaceEngineStart();

        Assert.Equal("ko", reopened.TEngineSettingsRead().LSettingsLocalization);
        Assert.Equal("en", TInterface.TSettingsLoad(first.TWorkspaceFolder).LSettingsLocalization);
    }

    private sealed class TWorkspaceObserver : LObserver
    {
        private readonly List<LBulletin> _tWorkspaceObserverRaised = [];

        internal IReadOnlyList<LBulletin> TWorkspaceObserverRaised => _tWorkspaceObserverRaised;

        public void LObserverBulletinHandle(LBulletin bulletin)
        {
            _tWorkspaceObserverRaised.Add(bulletin);
        }
    }
}
