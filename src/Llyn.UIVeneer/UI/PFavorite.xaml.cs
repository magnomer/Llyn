using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PFavorite : UserControl
{
    private PWindow _pFavoriteHost = null!;

    private LFavorite _lFavorite = null!;

    private LEditor _lEditor = null!;

    public PFavorite()
    {
        InitializeComponent();
    }

    internal void PFavoriteAttach(PWindow host)
    {
        _pFavoriteHost = host;
        _lFavorite = host.PWindowDeportment.LWindowFavoriteCreate();

        PRoster.ItemsSource = _pRosterList;

        _lEditor = host.PWindowDeportment.LWindowEditorCreate(host.PWindowUnreadableConfirm);
        PDisplay.PDisplayAttach(host, _lEditor.LEditorDisplay);
        _lEditor.LEditorStateChanged += PFavoriteStoreUpdate;
        PEditor.PEditorAttach(host, _lEditor);
    }

    private void PFavoriteStoreUpdate()
    {
        PFavoriteStore.IsEnabled = _lEditor.LEditorStorable;
    }

    internal void PFavoriteReset()
    {
        PFavoriteClear();
        PRosterFind();
    }

    internal bool PFavoriteDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PFavoriteChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PFavoriteClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _lFavorite.LFavoriteChosen is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PFavoritePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lFavorite.LFavoriteChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pFavoriteHost.PWindowPressRun(
                    ticket => _lFavorite.LFavoritePortraitPrint(_pFavoriteHost.PWindowLabelRead(), ticket));
            }
        }
    }

    private async void PFavoritePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_lFavorite.LFavoriteChosen is not null)
        {
            if (PDisplay.Visibility == Visibility.Visible)
            {
                await _pFavoriteHost.PWindowPortraitExport(
                    _lFavorite.LFavoriteFileRead(), _lFavorite.LFavoritePortraitExport);
            }
        }
    }
}
