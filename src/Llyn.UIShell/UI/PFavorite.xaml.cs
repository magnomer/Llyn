using System.Windows.Controls;
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
    }

    internal void PFavoriteReset()
    {
        PFavoriteClear();
        PRosterFind(PRecall.Text ?? string.Empty);
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
}
