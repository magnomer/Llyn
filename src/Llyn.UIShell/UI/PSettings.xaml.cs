using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

/// <summary>
/// The settings panel as a control: what it is made of, and the stored choices it opens on. What
/// each choice costs — a catalog swap, a whole different workspace — lives in the files beside this
/// one.
/// </summary>
public partial class PSettings : UserControl
{
    // The window this panel sits in, which is who asks before a workspace change throws typed work
    // away, and who owns the other panels that change moves onto the new workspace.
    private PWindow _pSettingsHost = null!;

    private LEngine _lEngine = null!;

    // Whether the stored choices have finished being applied. Applying them raises the same change
    // events a user action does, and those must not be written back as choices.
    private bool _pSettingsReady;

    public PSettings()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Puts the panel to work on <paramref name="engine"/> and shows the choices already stored in
    /// it, which is the last thing done before user changes start being saved.
    /// </summary>
    internal void PSettingsAttach(PWindow host, LEngine engine)
    {
        _pSettingsHost = host;
        _lEngine = engine;

        PWorkspacePath.Text = engine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = engine.LEngineSettingsRead().LSettingsLocalization;
        _pSettingsReady = true;
    }
}
