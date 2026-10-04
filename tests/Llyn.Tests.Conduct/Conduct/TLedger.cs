using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TLedger
{
    [Fact]
    public void LedgerFind_BlankText_ReadsEveryPageInOrder()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        CLedgerShown shown = atelier.CAtelierLedger.CLedgerFind("  ");

        Assert.Equal(
            ["Workspace", "Language", "Transcription", "Listing", "Web", "Layout"], shown.CLedgerShownChildren);
        Assert.False(shown.CLedgerShownEmpty);
    }

    [Fact]
    public void LedgerFind_SwitchLabel_ReadsOnlyItsPage()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        string label = engine.TEngineTextRead("Morphology.Switch");

        Assert.Contains("Web", atelier.CAtelierLedger.CLedgerFind(label.ToUpperInvariant()).CLedgerShownChildren);
        Assert.DoesNotContain("Language", atelier.CAtelierLedger.CLedgerFind(label).CLedgerShownChildren);
    }

    [Fact]
    public void LedgerFind_NothingReads_AnswersEmpty()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        CLedgerShown shown = atelier.CAtelierLedger.CLedgerFind("qqqzzzxxx");

        Assert.Empty(shown.CLedgerShownChildren);
        Assert.True(shown.CLedgerShownEmpty);
    }

    [Fact]
    public void LedgerChanged_AfterFind_KeepsTheCatalogNarrowed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = TLedgerShowRead(atelier);
        Assert.Equal(6, shown[0].CLedgerStateShown.CLedgerShownChildren.Count);

        CLedgerShown found = atelier.CAtelierLedger.CLedgerFind(engine.TEngineTextRead("Epithet.Switch"));
        atelier.CAtelierLedger.CLedgerEpithetSave(
            !shown[^1].CLedgerStateSettings.CSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Contains("Listing", found.CLedgerShownChildren);
        Assert.Equal(found.CLedgerShownChildren, shown[^1].CLedgerStateShown.CLedgerShownChildren);
        Assert.False(shown[^1].CLedgerStateShown.CLedgerShownEmpty);
    }

    [Theory]
    [InlineData(true, "Layout.LinkedMeta")]
    [InlineData(false, "Layout.FreeMeta")]
    public void LedgerMetaRead_LinkedFlag_WordsTheLayoutPage(bool linked, string key)
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());

        CLedgerPage page = atelier.CAtelierLedger.CLedgerMetaRead(linked);

        Assert.Equal("Layout", page.CLedgerPageChild);
        Assert.Equal(engine.TEngineTextRead("Settings.Layout"), page.CLedgerPageTitle);
        Assert.Equal(engine.TEngineTextRead(key), page.CLedgerPageMeta);
    }

    [Fact]
    public void LedgerFolderOpen_Pressed_OpensTheWorkspaceFolderAndAsksNothing()
    {
        int opened = 0;
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ =>
                {
                    opened++;
                    return null;
                },
            }));

        atelier.CAtelierLedger.CLedgerFolderOpen(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(1, opened);
        Assert.Empty(asked);
    }

    [Fact]
    public void LedgerFolderOpen_ShellFails_ShowsTheFolderFailure()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ => throw new IOException("moved away"),
            }));

        atelier.CAtelierLedger.CLedgerFolderOpen(TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Settings.FolderFailed"], asked);
    }

    [Fact]
    public void LedgerFolderOpen_ShellFailsTwice_ShowsTwoNotices()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFolderOpen"] = _ => throw new IOException("moved away"),
            }));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.CAtelierLedger.CLedgerFolderOpen(envoy);
        atelier.CAtelierLedger.CLedgerFolderOpen(envoy);

        Assert.Equal(["Settings.FolderFailed", "Settings.FolderFailed"], asked);
    }

    [Fact]
    public void LedgerFailureShow_FailsTwice_ShowsTwoNotices()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerFailureShow(envoy, "Export.Failed", new IOException("locked"));
        atelier.TLedgerFailureShow(envoy, "Export.Failed", new IOException("locked"));

        Assert.Equal(["Export.Failed", "Export.Failed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_FailsTwice_ShowsOneNotice()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_SameKeyThenOtherKey_ShowsEachKeyOnce()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.CitationFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.CitationFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_OtherAtelier_ShowsTheKeyAgain()
    {
        List<string> asked = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));
        using CAtelier other = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        other.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerRepaintShow_SettingsSavedBetween_ShowsTheKeyAgain()
    {
        List<string> asked = [];
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = TLedgerShowRead(atelier);
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);

        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));
        atelier.CAtelierLedger.CLedgerEpithetSave(
            !shown[^1].CLedgerStateSettings.CSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));
        atelier.TLedgerRepaintShow(envoy, "Display.OrderFailed", new IOException("locked"));

        Assert.Equal(["Display.OrderFailed", "Display.OrderFailed"], asked);
    }

    [Fact]
    public void LedgerChanged_Opened_ShowsEveryPageAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerState state = TLedgerShowRead(atelier).Single();

        Assert.Equal(
            ["Workspace", "Language", "Transcription", "Listing", "Web", "Layout"],
            state.CLedgerStatePages.Select(static page => page.CLedgerPageChild));
        Assert.Equal(engine.TEngineTextRead("Settings.Web"), state.CLedgerStatePages[4].CLedgerPageTitle);
        Assert.Equal(atelier.CAtelierPathRead(), state.CLedgerStatePath);
    }

    [Fact]
    public void LedgerChanged_WebPage_FormatsTheOnlineTallyOutOfTwo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerState state = TLedgerShowRead(atelier).Single();

        string expected = string.Format(
            CultureInfo.CurrentCulture,
            engine.TEngineTextRead("Settings.Tally"),
            engine.TEngineSettingsRead().LSettingsOnline,
            2);
        Assert.Equal(expected, state.CLedgerStatePages[4].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerChanged_LayoutPage_LeavesTheSummaryToTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        Assert.Equal(string.Empty, TLedgerShowRead(atelier).Single().CLedgerStatePages[5].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerChanged_EpithetSaved_ShowsAgainWithListingOn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = TLedgerShowRead(atelier);

        atelier.CAtelierLedger.CLedgerEpithetSave(false, TEnvoyFake.TEnvoyCreate(false, []));
        atelier.CAtelierLedger.CLedgerEpithetSave(true, TEnvoyFake.TEnvoyCreate(false, []));

        CLedgerState last = shown[^1];
        Assert.True(shown.Count >= 2);
        Assert.True(last.CLedgerStateSettings.CSettingsEpithet);
        Assert.Equal(engine.TEngineTextRead("Settings.On"), last.CLedgerStatePages[3].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerChanged_Detached_ShowsNothingMore()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = [];
        atelier.CAtelierLedger.CLedgerChanged += shown.Add;
        atelier.TAtelierStubOpen();
        atelier.CAtelierLedger.CLedgerChanged -= shown.Add;

        atelier.CAtelierLedger.CLedgerEpithetSave(
            !shown[0].CLedgerStateSettings.CSettingsEpithet, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Single(shown);
    }

    [Fact]
    public void LedgerChanged_FakePort_ReadsTheNormalisedLanguageAndOnlineCount()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));

        CLedgerState state = TLedgerShowRead(atelier).Single();

        Assert.Equal("ko", state.CLedgerStateLocalization);
        Assert.Equal("ko", state.CLedgerStateSettings.CSettingsLocalization);
        Assert.True(state.CLedgerStateSettings.CSettingsRespelled);
        Assert.False(state.CLedgerStateSettings.CSettingsEpithet);
        Assert.Equal(2, state.CLedgerStateSettings.CSettingsOnline);
    }

    [Fact]
    public void LedgerChanged_ScannedLanguage_ReadsItsNativeNameAndAnUnscannedOneItsCode()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TLedgerPortCreate([]));

        CLedgerState state = TLedgerShowRead(atelier).Single();

        Assert.Equal(
            [new KeyValuePair<string, string>("de", CultureInfo.GetCultureInfo("de").NativeName)],
            state.CLedgerStateLanguages);
        Assert.Equal("ko", state.CLedgerStatePages[1].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerEpithetSave_Toggle_ReachesPort()
    {
        List<bool> saved = [];
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineEpithetSave"] = args =>
                {
                    saved.Add((bool)args![0]!);
                    return null;
                },
            }));

        atelier.CAtelierLedger.CLedgerEpithetSave(false, TEnvoyFake.TEnvoyCreate(false, []));
        atelier.CAtelierLedger.CLedgerEpithetSave(true, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Equal([false, true], saved);
    }

    [Fact]
    public void LedgerSave_RefusedSaves_ShowsEachSaveFailureAndRepaints()
    {
        List<string> asked = [];
        Func<object?[]?, object?> refuse = _ => throw new IOException("The disk is full.");
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineLocalizationSave"] = refuse,
                ["LEngineEpithetSave"] = refuse,
                ["LEngineFrequencySave"] = refuse,
                ["LEngineMorphologySave"] = refuse,
                ["LEngineRespellingSave"] = refuse,
            }));
        List<CLedgerState> shown = TLedgerShowRead(atelier);
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CLedger ledger = atelier.CAtelierLedger;

        ledger.CLedgerLocalizationSave("de", envoy);
        ledger.CLedgerEpithetSave(true, envoy);
        ledger.CLedgerFrequencySave(false, envoy);
        ledger.CLedgerMorphologySave(false, envoy);
        ledger.CLedgerRespellingSave(false, envoy);

        Assert.Equal(Enumerable.Repeat("Settings.SaveFailed", 5), asked);
        Assert.Equal(6, shown.Count);
        Assert.Equal(shown[0].CLedgerStateSettings, shown[^1].CLedgerStateSettings);
    }

    [Fact]
    public void LedgerSave_FaultingStore_ShowsTheSaveFailureAndRecordsTheFaultOnce()
    {
        List<string> asked = [];
        List<Exception> recorded = [];
        Dictionary<string, Func<object?[]?, object?>> stored = new()
        {
            ["LSettingsRead"] = _ => TInterface.TSettingsCreate("en"),
            ["LSettingsSave"] = _ => throw TInterface.TVaultFaultCreate("The disk is full."),
        };
        Dictionary<string, Func<object?[]?, object?>> audited = new()
        {
            ["LAuditRecord"] = args =>
            {
                recorded.Add((Exception)args![0]!);
                return null;
            },
        };
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild() with
        {
            LRigSettings = TEngineFake.TEngineCreate<LSettingsVault>(stored),
            LRigAudit = TEngineFake.TEngineCreate<LAuditVault>(audited),
        });
        LSettingsPort settings = TInterfaceConduct.TSettingsOutletCreate(engine);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, settings);
        bool epithet = engine.TEngineSettingsRead().LSettingsEpithet;

        atelier.CAtelierLedger.CLedgerEpithetSave(!epithet, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.Equal(["Settings.SaveFailed"], asked);
        Assert.Equal(epithet, engine.TEngineSettingsRead().LSettingsEpithet);
        Assert.IsType<LVaultFault>(Assert.Single(recorded));
    }

    [Fact]
    public void LedgerNoticeRead_WrappedRefusal_AnswersItsReasonAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        Exception wrapped = new InvalidOperationException("outer", TInterface.TRefusalCreate(LRefusal.LRefusalStale));

        Assert.Equal(
            new CLedgerNotice(LRefusal.LRefusalStale, null, null), atelier.CAtelierLedger.CLedgerNoticeRead(wrapped));
    }

    [Fact]
    public void LedgerNoticeRead_Fault_AnswersTheUnexpectedKeyAndItsAuditFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerNotice notice = atelier.CAtelierLedger.CLedgerNoticeRead(new InvalidOperationException("bare"));

        Assert.Equal("Notice.Unexpected", notice.CLedgerNoticeKey);
        Assert.Equal("Notice.Recorded", notice.CLedgerNoticeLabel);
        Assert.True(File.Exists(notice.CLedgerNoticePath));
    }

    private static List<CLedgerState> TLedgerShowRead(CAtelier atelier)
    {
        List<CLedgerState> shown = [];
        atelier.CAtelierLedger.CLedgerChanged += shown.Add;
        atelier.TAtelierStubOpen();
        return shown;
    }

    private static LSettingsPort TLedgerPortCreate(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers.TryAdd("LEngineLocalizationRead", _ => "ko");
        answers.TryAdd("LEngineLocalizationLoad", _ => new Dictionary<string, string>());
        answers.TryAdd("LEngineLocalizationScan", _ => new List<string> { "de" });
        answers.TryAdd("LEngineWorkspaceRead", _ => "fake");
        answers.TryAdd("LEngineWorkspaceStart", _ => TInterfaceEngineWorkspace.TWorkspaceStateCreate());
        answers.TryAdd("LEngineWorkspaceFormat", _ => "fake");
        answers.TryAdd("LEngineEstablishmentRead", _ => TInterfaceEngineWorkspace.TEstablishmentCreate(0, 0, 0));
        answers.TryAdd("LEngineTextRead", args => (string)args![0]!);
        answers.TryAdd("LEngineGroupFind", args => ((IReadOnlyList<(string, IReadOnlyList<string>)>)args![0]!)
            .Select(static group => group.Item1)
            .ToList());
        answers.TryAdd("LEngineFailureRead", args => ((string)args![1]!, (string?)null, (string?)null));
        answers.TryAdd(
            "LEngineSettingsRead",
            _ => TInterface.TSettingsCreate("ko", respelled: true, frequency: true, morphology: true, epithet: false));
        return TEngineFake.TEngineCreate<LSettingsPort>(answers);
    }
}
