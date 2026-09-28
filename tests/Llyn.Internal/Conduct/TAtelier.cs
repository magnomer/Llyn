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

        CEstablishment establishment = atelier.CAtelierEstablishmentRead();

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
    public void AtelierScreenRead_HostedOrOtherLocation_AnswersAddressAndEngineFilmId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TMediaCreate(engine));

        Assert.Equal(
            new CScreen(new Uri("https://youtu.be/dQw4w9WgXcQ"), "dQw4w9WgXcQ"),
            atelier.CAtelierScreenRead(" https://youtu.be/dQw4w9WgXcQ "));
        Assert.Equal(
            new CScreen(new Uri("https://example.com/reel.mp4"), null),
            atelier.CAtelierScreenRead("https://example.com/reel.mp4"));
        Assert.Null(atelier.CAtelierScreenRead("media/dQw4w9WgXcQ.mp4"));
        Assert.Null(atelier.CAtelierScreenRead(null));
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
