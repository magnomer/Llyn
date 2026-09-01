using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// The session the workspace remembers: which entry the input panel was on when the window last
/// closed. The workspace row is the store, the engine is the seam, and this file is the only place
/// the shell reads or writes it — on start, on exit, and after the workspace folder changes.
/// </summary>
public partial class PWindow
{
    // Id of the entry the input panel stands on, or null when the form is empty. It is what the
    // exit save records, so it survives a reset of the form: saving an entry leaves the form blank
    // but the session still stands on the entry that was written.
    private string? _pStateEntry;

    // Opens the form on the entry the workspace row names. A row naming nothing, an entry deleted
    // since it was recorded, and a load that fails all mean the same thing here — an empty form —
    // because a stale id is the normal cost of remembering one across sessions, not an error.
    private void PStateRestore()
    {
        _pStateEntry = null;

        LEntryDraft? draft = null;
        try
        {
            string? id = _lEngine.LEngineStateRead().LWorkspaceStateLeft;
            if (id is not null)
            {
                draft = _lEngine.LEngineEntryLoad(id);
                if (draft is not null)
                {
                    _pStateEntry = id;
                }
            }
        }
        catch (Exception)
        {
            // A workspace whose database cannot be read still opens: the form comes up empty rather
            // than the window failing to appear.
            _pStateEntry = null;
            draft = null;
        }

        if (draft is null)
        {
            PInputReset();
            return;
        }

        PInputDraftShow(draft);
    }

    // Records the entry the form stands on. Reading the row first keeps every other column — mode,
    // split, revision — as the engine last wrote it; only the pane this shell owns is changed.
    private void PStateSave()
    {
        try
        {
            LWorkspaceState state = _lEngine.LEngineStateRead();
            _lEngine.LEngineStateSave(state with { LWorkspaceStateLeft = _pStateEntry });
        }
        catch (Exception)
        {
            // The window is already closing; a session that cannot be written is worth losing, but
            // never worth an unhandled exception on the way out.
        }
    }
}
