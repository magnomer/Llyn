using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineObserver
{
    [Fact]
    public void ObserverAttach_SettingsChanged_CountsOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        int count = 0;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
            {
                count++;
            }
        });

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Equal(1, count);
    }

    [Fact]
    public void ObserverDetach_SameDelegate_CountStays()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);
        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        engine.TEngineObserverDetach(bulletins.Add);
        engine.TEngineEpithetSave(settings.LSettingsEpithet);

        LBulletin bulletin = Assert.Single(bulletins);
        Assert.Equal(LSubject.LSubjectSettings, bulletin.LBulletinSubject);
    }

    [Fact]
    public void ObserverAttach_SameDelegateTwice_CountsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);
        engine.TEngineObserverAttach(bulletins.Add);

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Single(bulletins);
    }
}
