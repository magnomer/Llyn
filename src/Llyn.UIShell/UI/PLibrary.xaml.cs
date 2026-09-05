using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PLibrary : UserControl
{
    private PWindow _pLibraryHost = null!;

    private LEngine _lEngine = null!;

    public PLibrary()
    {
        InitializeComponent();
    }

    internal void PLibraryAttach(PWindow host, LEngine engine)
    {
        _pLibraryHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, "Library", null);

        PEditor.PEditorStoreDispatcher = PIndexEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
    }

    internal void PLibraryReset()
    {
        PLibraryClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal void PLibraryDraftFinish(bool store)
    {
        PEditor.PEditorDraftFinish(store);
    }

    internal bool PLibraryChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PLibraryClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
