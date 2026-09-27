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
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            workspace => TRigFake.TRigFakeBuild(new TVaultFake(), workspace),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        atelier.CAtelierWorkspaceChange("fake-next");

        Assert.Equal("fake-next", engine.TEngineWorkspaceRead());
        Assert.Equal(["fake-next"], pointed);
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

        Assert.Throws<IOException>(() => atelier.CAtelierWorkspaceChange("fake-broken"));

        Assert.Equal("fake", engine.TEngineWorkspaceRead());
        Assert.Empty(pointed);
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
