using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PTenor : UserControl
{
    private PWindow _pTenorHost = null!;

    private LTenor _lTenor = null!;

    private LEditor _lEditor = null!;

    public PTenor()
    {
        InitializeComponent();
    }

    internal void PTenorAttach(PWindow host, LEngine engine)
    {
        _pTenorHost = host;
        _lTenor = new LTenor(engine, engine, engine);

        PGamut.ItemsSource = _pGamutList;
        PCohort.ItemsSource = _pCohortList;

        _lEditor = new LEditor(engine, engine, engine, engine, host.PWindowUnreadableConfirm);
        PDisplay.PDisplayAttach(host, _lEditor.LEditorDisplay);
        _lEditor.LEditorStateChanged += PTenorStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
    }

    private void PTenorStoreUpdate()
    {
        PTenorStore.IsEnabled = _lEditor.LEditorStorable;
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
        e.CanExecute = _lTenor.LTenorCohortChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PTenorPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lTenor.LTenorCohortChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pTenorHost.PWindowPressRun(
                    ticket => _lTenor.LTenorPortraitPrint(_pTenorHost.PWindowLabelRead(), ticket));
            }
        }
    }

    private async void PTenorPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lTenor.LTenorCohortChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pTenorHost.PWindowPortraitExport(_lTenor.LTenorFileRead(), _lTenor.LTenorPortraitExport);
            }
        }
    }
}
