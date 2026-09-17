using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PYunjing : UserControl
{
    private PWindow _pYunjingHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pYunjingObserver;

    public PYunjing()
    {
        InitializeComponent();
    }

    internal void PYunjingAttach(PWindow host, LEngine engine)
    {
        _pYunjingHost = host;
        _lEngine = engine;

        PShengmu.ItemsSource = _pShengmuList;
        PYunmu.ItemsSource = _pYunmuList;
        PXiaoyun.ItemsSource = _pXiaoyunList;

        _pYunjingObserver = new PObserver(this, PYunjingBulletinHandle);
        engine.LEngineObserverAttach(_pYunjingObserver);

        PDisplay.PDisplayAttach(host, engine);
        PDiweiAttach();

        PEditor.PEditorAttach(host, engine, "Yunjing", null);
        PEditor.PEditorChangeNotice = changed => PYunjingStore.IsEnabled = changed;

        PYunjingReset();
    }

    internal bool PYunjingCheck()
    {
        return PYunjingLanguageFind() is not null;
    }

    internal void PYunjingReset()
    {
        _pYunjingLanguage = PYunjingLanguageFind();
        _pShengmuVista?.LVistaSelect(null);
        _pYunmuVista?.LVistaSelect(null);
        PYunjingClear();
        PYunjingLoad();
    }

    internal bool PYunjingDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PYunjingChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PYunjingClose()
    {
        if (_pYunjingObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pYunjingObserver);
            _pYunjingObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private string? PYunjingLanguageFind()
    {
        foreach (string language in _lEngine.LEngineLanguageRead())
        {
            if (_lEngine.LEngineBookRead(language).Count > 0)
            {
                return language;
            }
        }

        return null;
    }

    private void PYunjingPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PYunjingPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pYunjingHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pYunjingHost.PWindowLabelRead(), ticket));
        }
    }

    private async void PYunjingPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pYunjingHost.PWindowPortraitExport(entry);
        }
    }
}
