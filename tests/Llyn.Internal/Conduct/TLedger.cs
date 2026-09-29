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

        Assert.Equal(
            ["Workspace", "Language", "Transcription", "Listing", "Web", "Layout"],
            atelier.CAtelierLedger.CLedgerFind("  "));
    }

    [Fact]
    public void LedgerFind_SwitchLabel_ReadsOnlyItsPage()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        string label = engine.TEngineTextRead("Morphology.Switch");

        Assert.Contains("Web", atelier.CAtelierLedger.CLedgerFind(label.ToUpperInvariant()));
        Assert.DoesNotContain("Language", atelier.CAtelierLedger.CLedgerFind(label));
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

        atelier.CAtelierLedger.CLedgerEpithetSave(false);
        atelier.CAtelierLedger.CLedgerEpithetSave(true);

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
        atelier.CAtelierOpen();
        atelier.CAtelierLedger.CLedgerChanged -= shown.Add;

        atelier.CAtelierLedger.CLedgerEpithetSave(!shown[0].CLedgerStateSettings.CSettingsEpithet);

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

        atelier.CAtelierLedger.CLedgerEpithetSave(false);
        atelier.CAtelierLedger.CLedgerEpithetSave(true);

        Assert.Equal([false, true], saved);
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
        atelier.CAtelierOpen();
        return shown;
    }

    private static LSettingsPort TLedgerPortCreate(Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers.TryAdd("LEngineLocalizationRead", _ => "ko");
        answers.TryAdd("LEngineLocalizationLoad", _ => new Dictionary<string, string>());
        answers.TryAdd("LEngineLocalizationScan", _ => new List<string> { "de" });
        answers.TryAdd("LEngineWorkspaceRead", _ => "fake");
        answers.TryAdd("LEngineWorkspaceStart", _ => TInterface.TWorkspaceStateCreate());
        answers.TryAdd("LEngineWorkspaceFormat", _ => "fake");
        answers.TryAdd("LEngineTextRead", args => (string)args![0]!);
        answers.TryAdd(
            "LEngineSettingsRead",
            _ => TInterface.TSettingsCreate("ko", respelled: true, frequency: true, morphology: true, epithet: false));
        return TEngineFake.TEngineCreate<LSettingsPort>(answers);
    }
}
