using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PRepertoire : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pRepertoireHost = null!;

    private LEngine _lEngine = null!;

    private LEditor _lEditor = null!;

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

        PMedia.PMediaAttach(this, engine);
        PDisplay.PDisplayAttach(host, engine);
        _lEditor = new LEditor(engine, host.PWindowUnreadableConfirm);
        _lEditor.LEditorStateChanged += PRepertoireStoreUpdate;
        _lEditor.LEditorStateChanged += PChronicleUpdate;
        PEditor.PEditorAttach(host, engine, _lEditor);
    }

    private void PRepertoireStoreUpdate()
    {
        PRepertoireStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PRepertoireReset()
    {
        PRepertoireClear();
        PAtlasFind();
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
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
        PTierDropdown.IsOpen = false;
        PMeshDropdown.IsOpen = false;
    }

    private void PRepertoirePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = (_pOccurrenceVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible)
            || (_pRepertoireVista?.LVistaChosen is not null && PVignette.Visibility == Visibility.Visible);
    }

    private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pOccurrenceVista?.LVistaChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPressRun(
                    ticket => _lEngine.LEnginePortraitPrint(
                        _pOccurrenceVista, _pRepertoireHost.PWindowLabelRead(), ticket));
                return;
            }
        }

        if (_pRepertoireVista?.LVistaChosen is not null)
        {
            if (PVignette.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPressRun(ticket => _lEngine.LEnginePortraitPrint(
                    _pRepertoireVista, _pRepertoireHost.PWindowLegendRead("Situation"), ticket));
            }
        }
    }

    private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pOccurrenceVista?.LVistaChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pOccurrenceVista?.LVistaChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPortraitExport(_pOccurrenceVista);
            }
        }
    }
}
