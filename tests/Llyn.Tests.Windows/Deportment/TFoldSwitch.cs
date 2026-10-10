using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TFoldSwitch
{
    [Fact]
    public void FoldSwitchClick_NoStoredEntryHeld_PutsBothSwitchesBackAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        bool? fanqieShown = null;
        bool? scriptShown = null;

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            StackPanel surface = TFoldSwitchCreate(editor, atelier);
            QFanqie fanqie = (QFanqie)surface.Children[0];
            QScript script = (QScript)surface.Children[1];

            TFoldSwitchToggle(fanqie.TFanqieSwitchRead(), true);
            TFoldSwitchToggle(script.TScriptSwitchRead(), true);
            fanqieShown = fanqie.TFanqieSwitchRead().IsChecked;
            scriptShown = script.TScriptSwitchRead().IsChecked;
        });

        Assert.False(fanqieShown);
        Assert.False(scriptShown);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void FoldSwitchClick_OpenSwitchNoStoredEntryHeld_PutsBothSwitchesBackOpen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        bool? fanqieShown = null;
        bool? scriptShown = null;

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            StackPanel surface = TFoldSwitchCreate(editor, atelier);
            ToggleButton fanqie = ((QFanqie)surface.Children[0]).TFanqieSwitchRead();
            ToggleButton script = ((QScript)surface.Children[1]).TScriptSwitchRead();
            fanqie.IsChecked = true;
            script.IsChecked = true;

            TFoldSwitchToggle(fanqie, false);
            TFoldSwitchToggle(script, false);
            fanqieShown = fanqie.IsChecked;
            scriptShown = script.IsChecked;
        });

        Assert.True(fanqieShown);
        Assert.True(scriptShown);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void FoldSwitchClick_HeldEntry_KeepsTheSwitchAndStoresTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(water);
        bool? shown = null;

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            QScript script = (QScript)TFoldSwitchCreate(editor, atelier).Children[1];

            TFoldSwitchToggle(script.TScriptSwitchRead(), true);
            shown = script.TScriptSwitchRead().IsChecked;
        });

        Assert.True(shown);
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
    }

    [Fact]
    public void LecternBoxRefine_StoredEntryShown_ShowsBothSwitchesWithTheStoredState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        engine.TEngineBoxSpread(water, LFoldBox.LFoldBoxFanqie, true);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        (bool?, Visibility) fanqieShown = (null, Visibility.Collapsed);
        (bool?, Visibility) scriptShown = (null, Visibility.Collapsed);

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            StackPanel surface = TFoldViewCreate(wing, atelier);
            ToggleButton fanqie = ((QFanqie)surface.Children[0]).TFanqieSwitchRead();
            ToggleButton script = ((QScript)surface.Children[1]).TScriptSwitchRead();
            fanqieShown = (fanqie.IsChecked, ((FrameworkElement)fanqie.Parent).Visibility);
            scriptShown = (script.IsChecked, ((FrameworkElement)script.Parent).Visibility);
        });

        Assert.Equal((true, Visibility.Visible), fanqieShown);
        Assert.Equal((false, Visibility.Visible), scriptShown);
    }

    [Fact]
    public void LecternSwitchClick_NoEntryShown_PutsBothSwitchesBackAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        bool? fanqieShown = null;
        bool? scriptShown = null;

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            StackPanel surface = TFoldViewCreate(wing, atelier);
            ToggleButton fanqie = ((QFanqie)surface.Children[0]).TFanqieSwitchRead();
            ToggleButton script = ((QScript)surface.Children[1]).TScriptSwitchRead();

            TFoldSwitchToggle(fanqie, true);
            TFoldSwitchToggle(script, true);
            fanqieShown = fanqie.IsChecked;
            scriptShown = script.IsChecked;
        });

        Assert.False(fanqieShown);
        Assert.False(scriptShown);
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    [Fact]
    public void LecternSwitchClick_ShownEntry_KeepsTheSwitchAndStoresTheBox()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        long water = TFoldSwitchPrepare(engine);
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, []), true);
        wing.CWingEntryOpen(water);
        bool? shown = null;

        TWindow.TWindowRun(() =>
        {
            TFoldIconPrepare();
            QFanqie fanqie = (QFanqie)TFoldViewCreate(wing, atelier).Children[0];

            TFoldSwitchToggle(fanqie.TFanqieSwitchRead(), true);
            shown = fanqie.TFanqieSwitchRead().IsChecked;
        });

        Assert.True(shown);
        Assert.True(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxFanqie));
        Assert.False(engine.TEngineBoxCheck(water, LFoldBox.LFoldBoxScript));
    }

    private static StackPanel TFoldSwitchCreate(TEditorFixture editor, CAtelier atelier)
    {
        StackPanel surface = new();
        surface.Children.Add(new QFanqie { Name = "PEditorFanqie" });
        surface.Children.Add(new QScript { Name = "PEditorScript" });
        TInterfaceDeportment.TCadenceCreate(
            surface, editor.TEditorFixtureEditor, atelier.CAtelierLedger, TEnvoyFake.TEnvoyCreate(false, []));
        return surface;
    }

    private static StackPanel TFoldViewCreate(CWing wing, CAtelier atelier)
    {
        StackPanel surface = new();
        surface.Children.Add(new QFanqie { Name = "PDisplayFanqie", QFanqieFolded = true });
        surface.Children.Add(new QScript { Name = "PDisplayScript", QScriptFolded = true });
        surface.Children.Add(new TextBlock { Name = "PDisplayReading" });
        surface.Children.Add(new QParadigm { Name = "PDisplayParadigm" });
        TInterfaceDeportment.TLecternSoundCreate(
            wing.CWingDisplay, surface, atelier.CAtelierLedger, TEnvoyFake.TEnvoyCreate(false, [])).QLecternBoxRefine();
        return surface;
    }

    private static void TFoldSwitchToggle(ToggleButton hinge, bool opened)
    {
        hinge.IsChecked = opened;
        hinge.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, hinge));
    }

    private static long TFoldSwitchPrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], [])).LEntryId;
    }

    private static void TFoldIconPrepare()
    {
        System.Windows.Application application =
            System.Windows.Application.Current ?? new System.Windows.Application();
        application.Resources["PIconRoot"] = "pack://application:,,,/Llyn.Tests.Windows;component/icons/";
    }
}
