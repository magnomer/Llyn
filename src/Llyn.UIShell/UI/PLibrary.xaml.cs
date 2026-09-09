using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PLibrary : UserControl
{
    private PWindow _pLibraryHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pLibraryObserver;

    public PLibrary()
    {
        InitializeComponent();
    }

    internal void PLibraryAttach(PWindow host, LEngine engine)
    {
        _pLibraryHost = host;
        _lEngine = engine;

        PIndex.ItemsSource = _pIndexList;

        _pLibraryObserver = new PObserver(this, PLibraryBulletinHandle);
        engine.LEngineObserverAttach(_pLibraryObserver);

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Library", null);
        PEditor.PEditorChangeNotice = changed => PLibraryStore.IsEnabled = changed;

        IsVisibleChanged += (_, _) => PLibraryCommandApply();
        PLibraryCommandApply();
    }

    internal void PLibraryReset()
    {
        PLibraryClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal bool PLibraryDraftFinish(bool store)
    {
        return PEditor.PEditorDraftFinish(store);
    }

    internal bool PLibraryChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PLibraryClose()
    {
        if (_pLibraryObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pLibraryObserver);
            _pLibraryObserver = null;
        }

        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
