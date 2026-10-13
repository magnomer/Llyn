using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierWorkspace
{
    [Fact]
    public void AtelierWorkspaceChange_Path_MovesEngineThenWritesPointer()
    {
        List<string> pointed = [];
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        string chosen = "  " + second.TWorkspaceFolder + "  ";

        string shown = atelier.CAtelierWorkspace.CWorkspaceChange(chosen, TEngineFake.TEngineStubCreate<CEnvoy>());

        Assert.Equal([second.TWorkspaceFolder], pointed);
        Assert.Equal(second.TWorkspaceFolder, shown);
    }

    [Fact]
    public void AtelierOpen_EstablishmentFails_ShowsTheFailureAndRaisesNoStatus()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineWorkspaceStart"] = _ => TInterfaceEngineWorkspace.TWorkspaceStateCreate(),
                ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
            }));
        List<string> asked = [];
        List<CEstablishment> shown = [];
        atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged += shown.Add;

        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Workspace.EstablishmentFailed"], asked);
        Assert.Empty(shown);
    }

    [Fact]
    public void AtelierOpen_SecondEnvoy_ShowsALaterEstablishmentFailureThroughTheLatestEnvoy()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<Action<LBulletin>> observers = [];
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineWorkspaceStart"] = _ => TInterfaceEngineWorkspace.TWorkspaceStateCreate(),
                ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
            }),
            TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineLeftoverSweep"] = _ => null,
                ["LEngineObserverAttach"] = args =>
                {
                    observers.Add((Action<LBulletin>)args![0]!);
                    return null;
                },
                ["LEngineObserverDetach"] = _ => null,
            }));
        List<string> first = [];
        List<string> second = [];
        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, first));
        atelier.TAtelierOpen(TEnvoyFake.TEnvoyCreate(false, second));
        first.Clear();
        second.Clear();

        foreach (Action<LBulletin> observer in observers.ToArray())
        {
            observer(TInterfaceEngineWorkspace.TBulletinCreate(default, 1));
        }

        Assert.Empty(first);
        Assert.Equal(["Workspace.EstablishmentFailed"], second);
    }

    [Fact]
    public void AtelierWorkspaceChange_FolderFails_ShowsTheFailureAndWritesNoPointer()
    {
        List<string> pointed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            _ => throw new IOException("unreadable"),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<string> asked = [];

        string shown = atelier.CAtelierWorkspace.CWorkspaceChange(
            "fake-broken", TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.Equal("fake", shown);
        Assert.Equal(["Workspace.OpenFailed"], asked);
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
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => true, static _ => true);
        List<string> asked = [];
        List<string> opened = [];
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => opened.Add("Opened");

        atelier.CAtelierWorkspace.CWorkspaceChange(chosen, TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.Equal("fake", atelier.CAtelierPathRead());
        Assert.Empty(pointed);
        Assert.Empty(asked);
        Assert.Empty(opened);
    }

    [Fact]
    public void AtelierWorkspaceChange_LeaveCancelled_AsksOnceAndStays()
    {
        List<string> pointed = [];
        List<bool> closed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            workspace => TRigFake.TRigFakeBuild(new TVaultFake(), workspace),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => true, store => { closed.Add(store); return true; });
        List<string> asked = [];

        string shown = atelier.CAtelierWorkspace.CWorkspaceChange(
            "fake-next", TEnvoyFake.TEnvoyCreate(null, asked));

        Assert.Equal("fake", shown);
        Assert.Equal("fake", atelier.CAtelierPathRead());
        Assert.Empty(pointed);
        Assert.Equal(["Leave"], asked);
        Assert.Empty(closed);
    }

    [Fact]
    public void AtelierWorkspaceChange_UnsavedStored_AsksOnceAndStoresEveryArea()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<bool> closed = [];
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => true, store => { closed.Add(store); return true; });
        List<string> asked = [];

        atelier.CAtelierWorkspace.CWorkspaceChange(
            second.TWorkspaceFolder, TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.Equal(["Leave"], asked);
        Assert.Equal([true], closed);
    }

    [Fact]
    public void AtelierWorkspaceChange_Moved_OpensTheNewWorkspaceState()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> heard = [];
        CWorkspaceState? state = null;
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => heard.Add("Opened");
        atelier.CAtelierWorkspace.TWorkspaceStateAdd(opened =>
        {
            heard.Add("State");
            state = opened;
        });

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, TEngineFake.TEngineStubCreate<CEnvoy>());

        Assert.Equal(["Opened", "State"], heard);
        Assert.Equal(atelier.TAtelierStateOpen(), state);
    }

    [Fact]
    public void AtelierWorkspaceChange_Moved_RestoresEveryAreaVistaBeforeTheViews()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CLibrary library = CLibrary.CLibraryCreate(
            atelier, static () => true, TEnvoyFake.TEnvoyCreate(false, []), static run => run());
        LVista? held = library.CLibraryPanel.TPanelVistaRead();
        LVista? restored = null;
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => restored = library.CLibraryPanel.TPanelVistaRead();

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, TEngineFake.TEngineStubCreate<CEnvoy>());

        Assert.NotNull(restored);
        Assert.NotSame(held, restored);
    }

    [Fact]
    public void AtelierWorkspaceChange_FolderAnswered_AsksFromTheFolderInUseAndMoves()
    {
        List<string> pointed = [];
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        string held = atelier.CAtelierPathRead();

        atelier.CAtelierWorkspace.CWorkspaceChange(TAtelierEnvoyCreate(_ => second.TWorkspaceFolder, asked));

        Assert.Equal([held], asked);
        Assert.Equal([second.TWorkspaceFolder], pointed);
    }

    [Fact]
    public void AtelierWorkspaceChange_FolderDeclined_MovesNothing()
    {
        List<string> pointed = [];
        using LEngine engine = new(
            TRigFake.TRigFakeBuild(),
            workspace => TRigFake.TRigFakeBuild(new TVaultFake(), workspace),
            pointed.Add);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<string> asked = [];

        string shown = atelier.CAtelierWorkspace.CWorkspaceChange(TAtelierEnvoyCreate(static _ => null, asked));

        Assert.Equal("fake", shown);
        Assert.Equal(["fake"], asked);
        Assert.Equal("fake", atelier.CAtelierPathRead());
        Assert.Empty(pointed);
    }

    [Fact]
    public void AtelierWorkspaceChange_FolderQuestionFails_ShowsTheFailure()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<string> asked = [];

        atelier.CAtelierWorkspace.CWorkspaceChange(
            TAtelierEnvoyCreate(static _ => throw new InvalidOperationException("dialog"), asked));

        Assert.Equal(["fake", "Workspace.OpenFailed"], asked);
        Assert.Equal("fake", atelier.CAtelierPathRead());
    }

    private static CEnvoy TAtelierEnvoyCreate(Func<string, string?> folder, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyWorkspaceRead"] = args =>
            {
                string workspace = (string)args![0]!;
                asked.Add(workspace);
                return folder(workspace);
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });
}
