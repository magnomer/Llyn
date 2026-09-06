using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PFavorite : UserControl
{
    private PWindow _pFavoriteHost = null!;

    private LEngine _lEngine = null!;

    public PFavorite()
    {
        InitializeComponent();
    }

    internal void PFavoriteAttach(PWindow host, LEngine engine)
    {
        _pFavoriteHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(host, engine);

        PDisplay.PDisplayFavoriteDispatcher = PRosterUpdate;

        PEditor.PEditorAttach(host, engine, "Favorite", null);

        PEditor.PEditorStoreDispatcher = PRosterEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
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
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
