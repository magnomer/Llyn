using System;
using System.Collections.Generic;
using System.Globalization;
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
            ["Workspace", "Language", "Transcription", "Listing", "Inflection", "Web", "Layout"],
            shown.CLedgerShownChildren);
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
        List<CLedgerState> shown = TInterfaceConduct.TLedgerShowRead(atelier);
        Assert.Equal(7, shown[0].CLedgerStateShown.CLedgerShownChildren.Count);

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
    public void LedgerChanged_Opened_ShowsEveryPageAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerState state = TInterfaceConduct.TLedgerShowRead(atelier).Single();

        Assert.Equal(
            ["Workspace", "Language", "Transcription", "Listing", "Inflection", "Web", "Layout"],
            state.CLedgerStatePages.Select(static page => page.CLedgerPageChild));
        Assert.Equal(engine.TEngineTextRead("Settings.Web"), state.CLedgerStatePages[5].CLedgerPageTitle);
        Assert.Equal(atelier.CAtelierPathRead(), state.CLedgerStatePath);
    }

    [Fact]
    public void LedgerChanged_WebPage_FormatsTheOnlineTallyOutOfTwo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerState state = TInterfaceConduct.TLedgerShowRead(atelier).Single();

        string expected = string.Format(
            CultureInfo.CurrentCulture,
            engine.TEngineTextRead("Settings.Tally"),
            engine.TEngineSettingsRead().LSettingsOnline,
            2);
        Assert.Equal(expected, state.CLedgerStatePages[5].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerChanged_LayoutPage_LeavesTheSummaryToTheDriver()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        Assert.Equal(
            string.Empty, TInterfaceConduct.TLedgerShowRead(atelier).Single().CLedgerStatePages[6].CLedgerPageMeta);
    }

    [Fact]
    public void LedgerChanged_EpithetSaved_ShowsAgainWithListingOn()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<CLedgerState> shown = TInterfaceConduct.TLedgerShowRead(atelier);

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
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));

        CLedgerState state = TInterfaceConduct.TLedgerShowRead(atelier).Single();

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
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TInterfaceConduct.TLedgerPortCreate([]));

        CLedgerState state = TInterfaceConduct.TLedgerShowRead(atelier).Single();

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
            TInterfaceConduct.TLedgerPortCreate(new Dictionary<string, Func<object?[]?, object?>>
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
    public void LedgerAnalysisSave_Off_SavesAndRepaintsTheInflectionBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        List<string> asked = [];
        int edited = 0;
        int viewed = 0;
        editor.TEditorFixtureTimbre.CTimbreParadigmChanged += () => edited++;
        editor.TEditorFixtureDisplay.CDisplayParadigmChanged += _ => viewed++;
        Assert.True(atelier.CAtelierLedger.CLedgerAnalysisRead());

        atelier.CAtelierLedger.CLedgerAnalysisSave(false, TEnvoyFake.TEnvoyCreate(false, asked));

        Assert.False(atelier.CAtelierLedger.CLedgerAnalysisRead());
        Assert.False(engine.TEngineAnalysisCheck());
        Assert.Equal(1, edited);
        Assert.Equal(1, viewed);
        Assert.Empty(asked);
    }
}
