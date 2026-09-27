using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelier
{
    [Fact]
    public void AtelierModeSave_Name_Matches()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        atelier.CAtelierModeSave("Corpus");

        Assert.True(atelier.CAtelierModeMatch("Corpus"));
        Assert.False(atelier.CAtelierModeMatch("Library"));
    }

    [Fact]
    public void AtelierRead_FreshEngine_ReadsDefaultVolumeAndSplit()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal(1, atelier.CAtelierVolumeRead());
        Assert.False(atelier.CAtelierSplitRead());
    }

    [Fact]
    public void AtelierVolumeSet_Unsettled_PlaysWithoutWriting()
    {
        List<double> played = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TAtelierMediaCreate(played));

        atelier.CAtelierVolumeSet(0.4, false);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        Assert.Equal(0.4, atelier.CAtelierVolumeRead());
        Assert.Equal(1, reopened.CAtelierVolumeRead());
        Assert.Equal([0.4], played);
    }

    [Fact]
    public void AtelierVolumeSet_Settled_WritesTheLevel()
    {
        List<double> played = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TAtelierMediaCreate(played));

        atelier.CAtelierVolumeSet(0.3, false);
        atelier.CAtelierVolumeSet(0.6, true);

        using CAtelier reopened = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        Assert.Equal(0.6, reopened.CAtelierVolumeRead());
        Assert.Equal([0.3, 0.6], played);
    }

    [Fact]
    public void AtelierWorkspaceChange_Path_MovesEngineThenWritesPointer()
    {
        List<string> pointed = [];
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        string chosen = "  " + second.TWorkspaceFolder + "  ";

        Assert.NotNull(atelier.CAtelierWorkspaceChange(chosen, TAtelierEnvoyCreate(true)));

        Assert.Equal([second.TWorkspaceFolder], pointed);
    }

    [Fact]
    public void AtelierWorkspaceChange_FolderFails_WritesNoPointer()
    {
        List<string> pointed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            _ => throw new IOException("unreadable"),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Throws<IOException>(() => atelier.CAtelierWorkspaceChange("fake-broken", TAtelierEnvoyCreate(true)));

        Assert.Equal("fake", engine.TEngineWorkspaceRead());
        Assert.Empty(pointed);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(" fake ")]
    public void AtelierWorkspaceChange_BlankOrSamePath_AsksNothingAndStays(string chosen)
    {
        List<string> pointed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            workspace => TRigFake.TRigFakeBuild(new TVaultFake(), workspace),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Null(atelier.CAtelierWorkspaceChange(chosen, TEngineFake.TEngineStubCreate<CEnvoy>()));

        Assert.Equal("fake", atelier.CAtelierPathRead());
        Assert.Empty(pointed);
    }

    [Fact]
    public void AtelierWorkspaceChange_DiscardDeclined_Stays()
    {
        List<string> pointed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            workspace => TRigFake.TRigFakeBuild(new TVaultFake(), workspace),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Null(atelier.CAtelierWorkspaceChange("fake-next", TAtelierEnvoyCreate(false)));

        Assert.Equal("fake", atelier.CAtelierPathRead());
        Assert.Empty(pointed);
    }

    [Fact]
    public void AtelierWorkspaceChange_Moved_ReadsTheNewWorkspaceState()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        CWorkspaceState? state = atelier.CAtelierWorkspaceChange(second.TWorkspaceFolder, TAtelierEnvoyCreate(true));

        Assert.Equal(atelier.CAtelierStateRead(), state);
    }

    [Fact]
    public void AtelierEstablishmentAttach_Attached_ShowsTheStatusAtOnceAndStopsOnDetach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CEstablishment> shown = [];

        Action detach = atelier.CAtelierEstablishmentAttach(shown.Add);
        detach();
        atelier.CAtelierLedger.CLedgerEpithetSave(!engine.TEngineSettingsRead().LSettingsEpithet);

        Assert.Equal([atelier.CAtelierEstablishmentRead()], shown);
    }

    private static CEnvoy TAtelierEnvoyCreate(bool discard)
    {
        return TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyDiscardConfirm"] = _ => discard,
        });
    }

    private static LMediaPort TAtelierMediaCreate(List<double> played)
    {
        return TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineVolumeSet"] = args =>
            {
                played.Add((double)args![0]!);
                return null;
            },
        });
    }
}
