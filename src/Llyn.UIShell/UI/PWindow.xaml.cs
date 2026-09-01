using System;
using System.ComponentModel;
using System.Windows;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The window itself: what happens when it opens and when it closes. Each panel is a control with its
/// own markup and its own behaviour; this file only puts them to work on the workspace the engine
/// opened, and stops them at the end.
/// </summary>
public partial class PWindow : Window
{
    private readonly LEngine _lEngine;

    /// <summary>
    /// Opens the window on <paramref name="engine"/>, already built and bound to a workspace that
    /// opened. The engine is not constructed here: opening the workspace can fail, and a failure in a
    /// window constructor has nowhere to be shown. <see cref="LBootstrap"/> builds it and hands it
    /// over; the window owns it from here and disposes it when it closes.
    /// </summary>
    public PWindow(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;

        InitializeComponent();

        PInput.PInputAttach(this, engine);
        PList.PListAttach(this, engine);
        PSettings.PSettingsAttach(this, engine);

        // Closing runs while the window is still up and can be called off; Closed cannot. Unsaved text
        // is caught in the first, and the session is recorded in the second.
        Closing += PWindowClosingHandle;
        Closed += PWindowExitHandle;
    }

    private void PWindowClosingHandle(object? sender, CancelEventArgs e)
    {
        // Declining leaves the window open on the form exactly as typed, which is the only place the
        // work still exists.
        e.Cancel = !PWindowDiscardConfirm();
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        // Every panel is stopped before the engine goes: the session save runs through it.
        PInput.PInputClose();
        PList.PListClose();
        _lEngine.Dispose();
    }
}
