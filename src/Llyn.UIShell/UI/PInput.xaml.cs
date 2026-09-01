using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The input panel as a control: the shared editor standing on no entry at all. The lexical editing
/// structure itself is <see cref="PEditor"/>, which every panel that edits an entry mounts; this
/// panel is what that editor means here — the form an entry is created through, and only created.
/// It never opens on a stored entry, so a store here always writes a new one and leaves the form
/// empty for the next; correcting an entry that exists is the browse-style panels' work.
/// </summary>
public partial class PInput : UserControl
{
    public PInput()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Puts the panel to work on <paramref name="engine"/>, the workspace the window opened, and opens
    /// the form empty.
    /// </summary>
    internal void PInputAttach(PWindow host, LEngine engine)
    {
        // No entry: this panel creates them. A form that stood on one would turn the next store into
        // an update of it, which is how a session's second entry used to overwrite its first.
        PEditor.PEditorAttach(host, engine, null);

        // Discarding empties the form: this panel is where an entry is begun, not where a stored one
        // is corrected, so the typing is what is thrown away rather than reverted.
        PEditor.PEditorDiscardDispatcher = PEditor.PEditorReset;
    }

    /// <summary>
    /// Empties the form. The workspace folder can change while the window is up, and a different
    /// folder is a different database: whatever was typed against the old one is begun again here.
    /// </summary>
    internal void PInputReset()
    {
        PEditor.PEditorReset();
    }

    /// <summary>
    /// Whether the form differs from the one the user was given: what the window asks before typed
    /// work would be thrown away.
    /// </summary>
    internal bool PInputChangeCheck()
    {
        return PEditor.PEditorChangeCheck();
    }

    /// <summary>Stops the panel: the editor is shut down.</summary>
    internal void PInputClose()
    {
        PEditor.PEditorClose();
    }
}
