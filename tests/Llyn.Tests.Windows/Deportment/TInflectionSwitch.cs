using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TInflectionSwitch
{
    [Fact]
    public void LedgerChanged_Opened_ListsTheInflectionPageAfterListing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);

        CLedgerState state = TInterfaceConduct.TLedgerShowRead(atelier).Single();
        List<string> children = state.CLedgerStatePages.Select(static page => page.CLedgerPageChild).ToList();
        CLedgerPage page = state.CLedgerStatePages[children.IndexOf("Inflection")];

        Assert.Equal(children.IndexOf("Listing") + 1, children.IndexOf("Inflection"));
        Assert.Equal(engine.TEngineTextRead("Settings.Inflection"), page.CLedgerPageTitle);
        Assert.Equal(engine.TEngineTextRead("Settings.On"), page.CLedgerPageMeta);
        Assert.Contains(
            "Inflection",
            atelier.CAtelierLedger.CLedgerFind(engine.TEngineTextRead("Analysis.Switch")).CLedgerShownChildren);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void InflectionRefine_StoredSetting_PaintsTheSwitch(bool stored)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        atelier.CAtelierLedger.CLedgerAnalysisSave(stored, envoy);
        bool? shown = null;

        TWindow.TWindowRun(() =>
        {
            ToggleButton toggle = new() { Name = "PSettingsAnalysis", IsChecked = !stored };
            StackPanel settings = new();
            settings.Children.Add(toggle);
            object inflection = TInterfaceDeportment.TInflectionCreate(settings, atelier.CAtelierLedger, envoy);

            TInterfaceDeportment.TInflectionRefine(inflection);
            shown = toggle.IsChecked;
        });

        Assert.Equal(stored, shown);
        Assert.Equal(stored, engine.TEngineAnalysisCheck());
        Assert.Empty(asked);
    }

    [Fact]
    public void InflectionObserve_SwitchClicked_SavesTheRawSwitch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        Assert.True(engine.TEngineAnalysisCheck());
        bool afterOff = true;

        TWindow.TWindowRun(() =>
        {
            ToggleButton toggle = new() { Name = "PSettingsAnalysis" };
            StackPanel settings = new();
            settings.Children.Add(toggle);
            TInterfaceDeportment.TInflectionCreate(settings, atelier.CAtelierLedger, envoy);

            toggle.IsChecked = false;
            toggle.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            afterOff = engine.TEngineAnalysisCheck();
            toggle.IsChecked = true;
            toggle.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        });

        Assert.False(afterOff);
        Assert.True(engine.TEngineAnalysisCheck());
        Assert.Empty(asked);
    }
}
