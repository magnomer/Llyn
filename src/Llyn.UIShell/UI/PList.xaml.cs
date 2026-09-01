using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The list panel as a control: what it is made of, and when it starts and stops. Browsing itself —
/// the search, the index, the read-only display — lives in the file beside this one.
/// </summary>
public partial class PList : UserControl
{
    // The window this panel sits in, which is who reports a load that failed.
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
    }

    /// <summary>
    /// Puts the panel back on the workspace open now: nothing is selected, and the index is re-read.
    /// A different workspace has its own database, so what the panel was showing came from one that
    /// is no longer open.
    /// </summary>
    internal void PListReset()
    {
        PDisplayClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
    }

    /// <summary>
    /// Stops the panel: its playback is released.
    /// </summary>
    internal void PListClose()
    {
        _pDisplayPlayer.Close();
    }
}
