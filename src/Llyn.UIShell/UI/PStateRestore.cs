using System;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// The session the workspace remembers: which entry the input panel was on when the window last
/// closed. The workspace row is the store, the engine is the seam, and this file is the only place
/// the shell reads or writes it — on start, on exit, and after the workspace folder changes.
/// </summary>
public partial class PInput
{
    // Id of the entry the input panel stands on, or null when the form is empty. It is what the
    // exit save records, so it survives a reset of the form: saving an entry leaves the form blank
    // but the session still stands on the entry that was written.
    private string? _pStateEntry;

    // Opens the form on the entry the workspace row names. A row naming nothing, an entry deleted
    // since it was recorded, and a load that fails all mean the same thing here — an empty form —
    // because a stale id is the normal cost of remembering one across sessions, not an error.
    internal void PStateRestore()
    {
        string? id;
        try
        {
            id = _lEngine.LEngineStateRead().LWorkspaceStateLeft;
        }
        catch (Exception)
        {
            // A workspace whose database cannot be read still opens: the form comes up empty rather
            // than the window failing to appear.
            id = null;
        }

        if (id is null)
        {
            _pStateEntry = null;
            PInputReset();
            return;
        }

        PStateEntryShow(id);
    }

    // Opens the form on one entry and leaves the session standing on it, which is what makes the next
    // save an edit of that entry rather than a copy of it. It is the one way the form is put on an
    // entry: the start-up restore rides on it, and so does a save that keeps the user where they
    // were, which needs the entry read back so its cards carry the ids they were just written under.
    private void PStateEntryShow(string id)
    {
        _pStateEntry = null;

        LEntryDraft? draft = null;
        try
        {
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            draft = null;
        }

        if (draft is null)
        {
            PInputReset();
            return;
        }

        PInputDraftShow(draft);
        _pStateEntry = id;
    }

    // Records the entry the form stands on. Reading the row first keeps every other column — mode,
    // split, revision — as the engine last wrote it; only the pane this shell owns is changed.
    internal void PStateSave()
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
