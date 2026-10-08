using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternFold
{
    [Fact]
    public void FoldObserve_DisagreeingToggle_SettlesOnFlag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        bool? shown = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                ToggleButton fold = new() { Name = "PDisplayReflexFold" };
                StackPanel surface = new();
                surface.Children.Add(new ItemsControl { Name = "PDisplayReflex" });
                surface.Children.Add(new TextBlock { Name = "PDisplayReflexLoading" });
                surface.Children.Add(fold);
                _ = TInterfaceDeportment.TLecternCreate(display, surface);

                display.TDisplayFoldSet(true);
                TInterfaceDeportment.TReflexFoldRefine(fold,display.TDisplayFoldRead());
                shown = fold.IsChecked;
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
        Assert.True(display.TDisplayFoldRead());
        Assert.True(shown);
    }
}
