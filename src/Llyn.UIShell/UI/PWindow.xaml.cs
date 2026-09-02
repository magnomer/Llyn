using System;
using System.ComponentModel;
using System.Windows;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PWindow : Window
{
    private readonly LEngine _lEngine;

    public PWindow(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;

        InitializeComponent();

        PWindowStateRestore();

        PInput.PInputAttach(this, engine);
        PList.PListAttach(this, engine);
        PSettings.PSettingsAttach(this, engine);

        Closing += PWindowClosingHandle;
        Closed += PWindowExitHandle;
    }

    private void PWindowClosingHandle(object? sender, CancelEventArgs e)
    {
        e.Cancel = !PWindowDiscardConfirm();

        if (!e.Cancel)
        {
            PWindowStateSave();
        }
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PInput.PInputClose();
        PList.PListClose();
        _lEngine.Dispose();
    }
}
