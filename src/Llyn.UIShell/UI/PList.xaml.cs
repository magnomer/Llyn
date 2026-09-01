using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The list panel as a control: what it is made of, and when it starts and stops. Browsing itself —
/// the search, the ordering, the index, the read-only display and the editor beside it — lives in the
/// file beside this one.
/// </summary>
public partial class PList : UserControl
{
    // The window this panel sits in, which is who reports a load that failed and asks the question put
    // before unsaved work would be lost.
    private PWindow _pListHost = null!;

    private LEngine _lEngine = null!;

    public PList()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Puts the panel to work on <paramref name="engine"/>, the workspace the window opened. Nothing
    /// is read yet: the panel fills itself the first time it is shown, so a session that never opens
    /// the List tab never queries the database.
    /// </summary>
    internal void PListAttach(PWindow host, LEngine engine)
    {
        _pListHost = host;
        _lEngine = engine;

        // The editor opens on no entry: this panel puts it on one when the reader asks to write.
        PEditor.PEditorAttach(host, engine, null);

        // A store may have changed the headword the index lists and the text the display shows, so both
        // are read again from what was written rather than left as they were.
        PEditor.PEditorStoreDispatcher = PIndexEntryUpdate;

        // Discarding here is not emptying a form: the entry stays selected and comes back as it is
        // stored, which is what there is to fall back to.
        PEditor.PEditorDiscardDispatcher = PEditorEntryRestore;
    }

    /// <summary>
    /// Puts the panel back on the workspace open now: nothing is selected, the editor is closed, and
    /// the index is re-read. A different workspace has its own database, so what the panel was showing
    /// came from one that is no longer open.
    /// </summary>
    internal void PListReset()
    {
        PDisplayClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    /// <summary>
    /// Whether the editor holds modifications that have not been stored: what the window asks before
    /// the workspace changes or the program closes.
    /// </summary>
    internal bool PListChangeCheck()
    {
        return PEditor.Visibility == System.Windows.Visibility.Visible && PEditor.PEditorChangeCheck();
    }

    /// <summary>
    /// Stops the panel: the editor is shut down and this panel's own playback is released.
    /// </summary>
    internal void PListClose()
    {
        PEditor.PEditorClose();
        _pDisplayPlayer.Close();
    }
}
