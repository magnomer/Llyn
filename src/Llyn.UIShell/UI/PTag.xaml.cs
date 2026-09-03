using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PTag : UserControl
{
    private PWindow _pTagHost = null!;

    private LEngine _lEngine = null!;

    public PTag()
    {
        InitializeComponent();
    }

    internal void PTagAttach(PWindow host, LEngine engine)
    {
        _pTagHost = host;
        _lEngine = engine;

        PDisplay.PDisplayAttach(host, engine);

        PEditor.PEditorAttach(host, engine, null);

        PEditor.PEditorStoreDispatcher = PMembershipEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
    }

    internal void PTagReset()
    {
        PTagClear();
        PDirectoryReset();
        PDirectoryFind(PExploration.Text ?? string.Empty);
    }

    internal bool PTagChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PTagClose()
    {
        PEditor.PEditorClose();
        PDisplay.PDisplayClose();
    }
}
