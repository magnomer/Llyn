using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PPhonology : UserControl
{
    private PWindow _pPhonologyHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pPhonologyObserver;

    public PPhonology()
    {
        InitializeComponent();
    }

    internal void PPhonologyAttach(PWindow host, LEngine engine)
    {
        _pPhonologyHost = host;
        _lEngine = engine;

        PInventory.ItemsSource = _pInventoryList;

        _pPhonologyObserver = new PObserver(this, PPhonologyBulletinHandle);
        engine.LEngineObserverAttach(_pPhonologyObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Phonology", null);
        PEditor.PEditorChangeNotice = changed => PPhonologyStore.IsEnabled = changed;

        PArticulation.PArticulationAttach(PProbe, PEditor.PPronunciationField);
    }

    internal void PPhonologyReset()
    {
        PPhonologyClear();
        PInventoryFind();
    }

    internal bool PPhonologyDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PPhonologyChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PPhonologyClose()
    {
        if (_pPhonologyObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pPhonologyObserver);
            _pPhonologyObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PPhonologyPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PPhonologyPressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pPhonologyHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pPhonologyHost.PWindowLabelRead(), ticket));
        }
    }

    private async void PPhonologyPortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pPhonologyHost.PWindowPortraitExport(entry);
        }
    }
}
