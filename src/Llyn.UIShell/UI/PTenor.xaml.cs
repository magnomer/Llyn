using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTenor : UserControl
{
    private PWindow _pTenorHost = null!;

    private LEngine _lEngine = null!;

    public PTenor()
    {
        InitializeComponent();
    }

    internal void PTenorAttach(PWindow host, LEngine engine)
    {
        _pTenorHost = host;
        _lEngine = engine;

        PGamut.ItemsSource = _pGamutList;
        PCohort.ItemsSource = _pCohortList;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Tenor", null);
        PEditor.PEditorChangeNotice = changed => PTenorStore.IsEnabled = changed;
    }

    internal void PTenorReset()
    {
        PTenorClear();
        PGamutReset();
        PGamutFind();
    }

    internal bool PTenorDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PTenorChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PTenorClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PTenorPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pCohortVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PTenorPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pCohortVista?.LVistaChosen is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pTenorHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pTenorHost.PWindowLabelRead(), ticket));
        }
    }

    private async void PTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pCohortVista?.LVistaChosen is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pTenorHost.PWindowPortraitExport(entry);
        }
    }
}
