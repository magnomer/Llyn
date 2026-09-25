using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternFold
{
    [Fact]
    public void FoldHandle_DisagreeingToggle_SettlesOnFlag()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LLectern lectern = TInterfaceDeportment.TEditorCreate(engine).LEditorLectern;
        bool? shown = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                ToggleButton fold = new();
                lectern.TLecternReflexAttach(new ItemsControl(), new TextBlock(), fold);
                fold.Checked += (_, _) => lectern.TLecternFoldHandle(fold.IsChecked == true);
                fold.Unchecked += (_, _) => lectern.TLecternFoldHandle(fold.IsChecked == true);

                lectern.TLecternFoldSet(true);
                TInterfaceDeportment.TReflexFoldApply([], fold, lectern.LLecternFoldOpened);
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
        Assert.True(lectern.LLecternFoldOpened);
        Assert.True(shown);
    }
}
