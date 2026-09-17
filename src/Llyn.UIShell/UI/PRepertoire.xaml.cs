using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PRepertoire : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pRepertoireHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pRepertoireObserver;

    public PRepertoire()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
    }

    internal void PRepertoireAttach(PWindow host, LEngine engine)
    {
        _pRepertoireHost = host;
        _lEngine = engine;

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;
        PScenarioImage.ItemsSource = _pScenarioImage;
        PScenarioVideo.ItemsSource = _pScenarioVideo;

        PDisplay.PDisplayAttach(host, engine);
        PEditor.PEditorAttach(host, engine, PScenarioOrigin, null);
        PEditor.PEditorChangeNotice = changed => PRepertoireStore.IsEnabled = changed;
        PEditor.PEditorChronicleNotice = PChronicleUpdate;

        _pRepertoireObserver = new PObserver(this, PRepertoireBulletinHandle);
        engine.LEngineObserverAttach(_pRepertoireObserver);
    }

    internal void PRepertoireReset()
    {
        PRepertoireClear();
        PAtlasFind(PInquest.Text ?? string.Empty);
    }

    internal bool PRepertoireChangeCheck()
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorChangeCheck()
            : PScenarioChangeCheck();
    }

    internal bool PRepertoireDraftFinish(bool store)
    {
        return PEditor.Visibility == Visibility.Visible
            ? PEditor.PEditorDraftFinish(store)
            : PScenarioDraftFinish(store);
    }

    internal void PRepertoireClose()
    {
        if (_pRepertoireObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pRepertoireObserver);
            _pRepertoireObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PTierDropdown.IsOpen = false;
        PMeshDropdown.IsOpen = false;
    }

    private void PRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = (_pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible)
            || (_pVignetteSituation is not null && PVignette.Visibility == Visibility.Visible);
    }

    private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pRepertoireHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pRepertoireHost.PWindowLabelRead(), ticket));
            return;
        }

        if (_pVignetteSituation is long shown && PVignette.Visibility == Visibility.Visible)
        {
            await _pRepertoireHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                shown, LOwner.LOwnerSituation, _pRepertoireHost.PWindowLegendRead("Situation"), ticket));
        }
    }

    private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pDisplayEntry is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pDisplayEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pRepertoireHost.PWindowPortraitExport(entry);
        }
    }
}
