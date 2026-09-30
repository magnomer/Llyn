using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelier
{
    [Fact]
    public void AtelierAboutRead_AnySession_ReadsVersionKey()
    {
        Assert.Equal("Headquarter.Version", CAtelier.CAtelierAboutRead());
    }

    [Fact]
    public void AtelierRefusalRead_BusyOrNot_ReadsItsOwnKey()
    {
        Assert.Equal("Workspace.Busy", CAtelier.CAtelierRefusalRead(true));
        Assert.Equal("Workspace.OpenFailed", CAtelier.CAtelierRefusalRead(false));
    }

    [Fact]
    public void AtelierRescueRead_SetAsideOrNot_ReadsTheKeyOnlyWhenDone()
    {
        Assert.Equal("Workspace.DatabaseReset", CAtelier.CAtelierRescueRead(true));
        Assert.Null(CAtelier.CAtelierRescueRead(false));
    }

    [Fact]
    public void AtelierVistaStart_EverySubjectAndOrder_ReachesTheEngineByName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        foreach (CCatalogOrder order in Enum.GetValues<CCatalogOrder>())
        {
            LVista vista = atelier.CAtelierVistaStart("order" + order, CSubject.CSubjectReference, order);

            Assert.Equal(order.ToString()[1..], vista.LVistaOrder.ToString()[1..]);
        }

        foreach (CSubject subject in Enum.GetValues<CSubject>())
        {
            LVista vista = atelier.CAtelierVistaStart("subject" + subject, subject, CCatalogOrder.CCatalogOrderName);

            Assert.Equal(subject.ToString()[1..], vista.LVistaSubject?.ToString()[1..]);
        }

        Assert.Null(atelier.CAtelierVistaStart("bare", null, CCatalogOrder.CCatalogOrderName).LVistaSubject);
    }

    [Fact]
    public void AtelierRead_FreshEngine_ReadsDefaultVolumeAndSplit()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        Assert.Equal(1, atelier.CAtelierVolumeRead());
        Assert.False(TInterfaceConduct.TAtelierSplitRead(atelier));
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
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        string chosen = "  " + second.TWorkspaceFolder + "  ";

        string shown = atelier.CAtelierWorkspace.CWorkspaceChange(chosen, TEngineFake.TEngineStubCreate<CEnvoy>());

        Assert.Equal([second.TWorkspaceFolder], pointed);
        Assert.Equal(second.TWorkspaceFolder, shown);
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

    [Fact]
    public void WorkspaceEstablishmentChanged_Opened_ShowsTheStatusAtOnceAndStopsOnDetach()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CEstablishment> shown = [];

        atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged += shown.Add;
        atelier.CAtelierOpen();
        atelier.CAtelierWorkspace.CWorkspaceEstablishmentChanged -= shown.Add;
        atelier.CAtelierLedger.CLedgerEpithetSave(!engine.TEngineSettingsRead().LSettingsEpithet);

        Assert.Equal([atelier.TAtelierEstablishmentRead()], shown);
    }

    [Theory]
    [InlineData(0, 0L, 3000L, "Establishment.Entry", "Establishment.Kilobyte", 3d)]
    [InlineData(2, 1L, 1024L * 1024, "Establishment.EntryOne", "Establishment.Megabyte", 1d)]
    [InlineData(0, 7L, 3L * 1024 * 1024 / 2, "Establishment.Entry", "Establishment.Megabyte", 1.5d)]
    public void AtelierEstablishmentRead_EngineVerdicts_ChoosesWordingKeysAndAmount(
        int unsaved, long entry, long size, string entryKey, string sizeKey, double amount)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineEstablishmentRead"] = _ => TInterface.TEstablishmentCreate(unsaved, entry, size),
            }));

        CEstablishment establishment = atelier.TAtelierEstablishmentRead();

        Assert.Equal(unsaved, establishment.CEstablishmentUnsaved);
        Assert.Equal(entry, establishment.CEstablishmentEntry);
        Assert.Equal(unsaved > 0, establishment.CEstablishmentPending);
        Assert.Equal(entryKey, establishment.CEstablishmentEntryKey);
        Assert.Equal(sizeKey, establishment.CEstablishmentSizeKey);
        Assert.Equal(
            amount.ToString(size >= 1024L * 1024 ? "0.0" : "0", CultureInfo.CurrentCulture),
            establishment.CEstablishmentAmount);
    }

    [Fact]
    public void AtelierOpen_Opened_RaisesTheViewsBeforeTheirState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> heard = [];
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => heard.Add("Opened");
        atelier.CAtelierWorkspace.TWorkspaceStateAdd(
            state => heard.Add(state == new CWorkspaceState(null, null) ? "Blank" : "Kept"));

        atelier.CAtelierOpen();

        Assert.Equal(["Opened", "Blank"], heard);
    }

    [Fact]
    public void AtelierOpen_BlankLeftoverDraft_SweepsItBeforeTheViewsRestore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        long blank;
        using (LEngine earlier = workspace.TWorkspaceEngineStart())
        {
            blank = earlier.TEngineDraftStart("Input", null).LDraftId;
        }

        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        bool? swept = null;
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => swept = engine.TEngineDraftRead(blank) is null;

        atelier.CAtelierOpen();

        Assert.True(swept);
    }

    [Fact]
    public void AtelierOpen_Reopened_HearsEachLedgerChangeOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = [];
        atelier.CAtelierLedger.CLedgerChanged += shown.Add;
        atelier.CAtelierOpen();
        atelier.CAtelierOpen();
        int opened = shown.Count;

        atelier.CAtelierLedger.CLedgerEpithetSave(!engine.TEngineSettingsRead().LSettingsEpithet);

        Assert.Equal(opened + 1, shown.Count);
    }

    [Fact]
    public void AtelierWorkspaceChange_InputHeld_OpenLeavesTheInputOnABlankEntry()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspacePrepare();
        using LEngine engine = new(first.TWorkspaceRigCreate(), _ => second.TWorkspaceRigCreate(), _ => { });
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyLeaveConfirm"] = _ =>
            {
                asked.Add("Leave");
                return false;
            },
        });
        CEditor editor = atelier.CAtelierInputCreate(envoy);
        editor.CEditorEntryOpen(null);
        editor.CEditorHeadwordSet("water");

        atelier.CAtelierWorkspace.CWorkspaceChange(second.TWorkspaceFolder, envoy);

        Assert.Equal(["Leave"], asked);
        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.TDeskChangeCheck());
        Assert.Equal(string.Empty, editor.TEditorDraftRead()?.CEntryDraftHeadword);
    }

    [Fact]
    public void AtelierQuitConfirm_InputUnsaved_AsksOnceAndDiscardsEveryArea()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        List<bool> closed = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CEditor editor = atelier.CAtelierInputCreate(envoy);
        editor.CEditorEntryOpen(null);
        editor.CEditorHeadwordSet("water");
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => false, store => { closed.Add(store); return true; });

        bool quit = atelier.CAtelierQuitConfirm(envoy);

        Assert.True(quit);
        Assert.Equal(["Leave"], asked);
        Assert.Equal([false], closed);
        Assert.False(editor.CEditorDesk.CDeskHeld);
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
