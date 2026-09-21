using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PRepertoire : UserControl, PImageHost, PVideoHost, PChronicleHost
{
    private PWindow _pRepertoireHost = null!;

    private LRepertoire _lRepertoire = null!;

    private LEditor _lEditor = null!;

    public PRepertoire()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PImageTemplate(this));
        Resources.MergedDictionaries.Add(new PVideoTemplate(this));
    }

    internal void PRepertoireAttach(PWindow host)
    {
        _pRepertoireHost = host;
        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        _lRepertoire = host.PWindowDeportment.LWindowRepertoireCreate(_lEditor, host.PWindowUnreadableConfirm);
        PScenarioDeskAttach();

        PAtlas.ItemsSource = _pAtlasList;
        POccurrence.ItemsSource = _pOccurrenceList;
        PScenarioImage.ItemsSource = _pScenarioImage;
        PScenarioVideo.ItemsSource = _pScenarioVideo;

        PMedia.PMediaAttach(this, host.PWindowDeportment);
        PDisplay.PDisplayAttach(host, _lEditor.LEditorDisplay);
        _lEditor.LEditorStateChanged += PRepertoireStoreUpdate;
        _lEditor.LEditorStateChanged += PChronicleUpdate;
        PEditor.PEditorAttach(host, _lEditor);
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
        e.CanExecute = (_lRepertoire?.LRepertoireOccurrenceChosen is not null
                        && PDisplay.Visibility == Visibility.Visible)
            || (_lRepertoire?.LRepertoireChosen is not null && PVignette.Visibility == Visibility.Visible);
    }

    private async void PRepertoirePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lRepertoire.LRepertoireOccurrenceChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPressRun(_lRepertoire.LRepertoirePortraitPrint);
                return;
            }
        }

        if (_lRepertoire.LRepertoireChosen is not null)
        {
            if (PVignette.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPressRun(
                    ticket => _lRepertoire.LRepertoirePortraitPrint(
                        _pRepertoireHost.PWindowLegendRead("Situation"), ticket));
            }
        }
    }

    private void PRepertoirePortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lRepertoire?.LRepertoireOccurrenceChosen is not null
                       && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PRepertoirePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lRepertoire.LRepertoireOccurrenceChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pRepertoireHost.PWindowPortraitExport(
                    _lRepertoire.LRepertoireFileRead(), _lRepertoire.LRepertoirePortraitExport);
            }
        }
    }
}
