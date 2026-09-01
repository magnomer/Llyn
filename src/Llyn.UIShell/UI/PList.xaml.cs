using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PList : UserControl
{
    private PWindow _pListHost = null!;

    private LEngine _lEngine = null!;

    public PList()
    {
        InitializeComponent();
    }

    internal void PListAttach(PWindow host, LEngine engine)
    {
        _pListHost = host;
        _lEngine = engine;

        PEditor.PEditorAttach(host, engine, null);

        PEditor.PEditorStoreDispatcher = PIndexEntryUpdate;

        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
    }

    internal void PListReset()
    {
        PDisplayClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    internal bool PListChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    internal void PListClose()
    {
        PEditor.PEditorClose();
        _pDisplayPlayer.Close();
    }
}
