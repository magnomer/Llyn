using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PFavorite : UserControl
{
    private PWindow _pFavoriteHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pFavoriteObserver;

    public PFavorite()
    {
        InitializeComponent();
    }

    internal void PFavoriteAttach(PWindow host, LEngine engine)
    {
        _pFavoriteHost = host;
        _lEngine = engine;

        PRoster.ItemsSource = _pRosterList;

        _pFavoriteObserver = new PObserver(this, PFavoriteBulletinHandle);
        engine.LEngineObserverAttach(_pFavoriteObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Favorite", null);
        PEditor.PEditorChangeNotice = changed => PFavoriteStore.IsEnabled = changed;
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
        if (_pFavoriteObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pFavoriteObserver);
            _pFavoriteObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }

    private void PFavoritePressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _pRosterEntry is not null && PDisplay.Visibility == Visibility.Visible;
    }

    private async void PFavoritePressHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pRosterEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pFavoriteHost.PWindowPressRun(
                ticket => _lEngine.LEnginePortraitPrint(entry, _pFavoriteHost.PWindowLabelRead(), ticket));
        }
    }

    private async void PFavoritePortraitHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (_pRosterEntry is long entry && PDisplay.Visibility == Visibility.Visible)
        {
            await _pFavoriteHost.PWindowPortraitExport(entry);
        }
    }
}
