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
        LEditor editor = TInterfaceDeportment.TEditorCreate(engine);
        QLectern lectern = TInterfaceDeportment.TLecternCreate(editor);
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

                editor.TDisplayFoldSet(true);
                TInterfaceDeportment.TReflexFoldRefine([], fold, editor.TDisplayFoldRead());
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
        Assert.True(editor.TDisplayFoldRead());
        Assert.True(shown);
    }
}
