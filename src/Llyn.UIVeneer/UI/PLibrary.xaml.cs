using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PLibrary : UserControl
{
    private PWindow _pLibraryHost = null!;

    private LEngine _lEngine = null!;

    public PLibrary()
    {
        InitializeComponent();
    }

    internal void PLibraryAttach(PWindow host, LEngine engine)
    {
        _pLibraryHost = host;
        _lEngine = engine;

        PIndex.ItemsSource = _pIndexList;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Library", null);
        PEditor.PEditorChangeNotice = changed => PLibraryStore.IsEnabled = changed;

        IsVisibleChanged += (_, _) => PLibraryCommandApply();
        PLibraryCommandApply();
    }

    internal void PLibraryReset()
    {
        PLibraryClear();
        PIndexFind();
    }

    internal bool PLibraryDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PLibraryChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PLibraryClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PLibraryPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pLibraryVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PLibraryPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pLibraryVista?.LVistaChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pLibraryHost.PWindowPressRun(
                    ticket => _lEngine.LEnginePortraitPrint(_pLibraryVista, _pLibraryHost.PWindowLabelRead(), ticket));
            }
        }
    }

    private async void PLibraryPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pLibraryVista?.LVistaChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pLibraryHost.PWindowPortraitExport(_pLibraryVista);
            }
        }
    }
}
