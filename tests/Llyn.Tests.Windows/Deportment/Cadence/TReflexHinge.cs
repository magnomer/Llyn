using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TReflexHinge
{
    [Fact]
    public void LecternHingeClick_NoEntryShown_PutsTheHingeBackAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexHingePrepare(engine);
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        bool? shown = null;

        TReflexHingeRun(() =>
        {
            StackPanel surface = TReflexLecternCreate();
            _ = TInterfaceDeportment.TLecternCreate(display, surface);
            ToggleButton hinge = TReflexHingeFind(surface, "PDisplayReflexHinge");

            TReflexHingeToggle(hinge, true);
            shown = hinge.IsChecked;
        });

        Assert.False(shown);
        Assert.False(engine.TEngineSpreadCheck(water));
    }

    [Fact]
    public void LecternFoldRefine_StoredOpenedEntry_ChecksTheRenamedHinge()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexHingePrepare(engine);
        engine.TEngineReflexSpread(water, true);
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        display.TDisplayEntryShow(water, engine.TEngineEntryLoad(water)!);
        bool? shown = null;

        TReflexHingeRun(() =>
        {
            StackPanel surface = TReflexLecternCreate();
            QLecternReflex reflex = TInterfaceDeportment.TLecternCreate(display, surface);

            reflex.QLecternFoldRefine();
            shown = TReflexHingeFind(surface, "PDisplayReflexHinge").IsChecked;
        });

        Assert.True(shown);
        Assert.True(engine.TEngineSpreadCheck(water));
    }

    [Fact]
    public void ReflexHingeClick_NoHeldEntry_PutsTheRenamedHingeBackAndStoresNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = TReflexHingePrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        bool? shown = null;

        TReflexHingeRun(() =>
        {
            StackPanel surface = new();
            surface.Children.Add(new StackPanel { Name = "PEditorSound" });
            surface.Children.Add(new Popup { Name = "PAnchor" });
            surface.Children.Add(new ItemsControl { Name = "PReflex" });
            surface.Children.Add(new Button { Name = "PReflexRenewal" });
            surface.Children.Add(new ToggleButton { Name = "PReflexHinge" });
            TInterfaceDeportment.TReflexCreate(surface, editor.TEditorFixtureEditor);
            ToggleButton hinge = TReflexHingeFind(surface, "PReflexHinge");

            TReflexHingeToggle(hinge, true);
            shown = hinge.IsChecked;
        });

        Assert.False(shown);
        Assert.False(engine.TEngineSpreadCheck(water));
    }

    private static StackPanel TReflexLecternCreate()
    {
        StackPanel surface = new();
        surface.Children.Add(new ItemsControl { Name = "PDisplayReflex" });
        surface.Children.Add(new TextBlock { Name = "PDisplayReflexLoading" });
        surface.Children.Add(new ToggleButton { Name = "PDisplayReflexHinge" });
        return surface;
    }

    private static ToggleButton TReflexHingeFind(Panel surface, string name)
    {
        foreach (UIElement child in surface.Children)
        {
            if (child is ToggleButton { Name: var held } hinge && held == name)
            {
                return hinge;
            }
        }

        throw new InvalidOperationException(name);
    }

    private static void TReflexHingeToggle(ToggleButton hinge, bool opened)
    {
        hinge.IsChecked = opened;
        hinge.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, hinge));
    }

    private static long TReflexHingePrepare(LEngine engine)
    {
        return engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], [])).LEntryId;
    }

    private static void TReflexHingeRun(Action body)
    {
        Exception? failure = null;
        Thread thread = new(() =>
        {
            try
            {
                body();
            }
            catch (Exception caught)
            {
                failure = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        Assert.Null(failure);
    }
}
