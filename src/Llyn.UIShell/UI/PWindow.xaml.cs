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

        PWindowAttach(engine);

        Closing += PWindowClosingHandle;
        Closed += PWindowExitHandle;
    }

    internal int PWindowLeftover { get; private set; }

    private void PWindowAttach(LEngine engine)
    {
        PWindowLeftover = engine.LEngineLeftoverRead().Count;

        PInput.PInputAttach(this, engine);
        PLibrary.PLibraryAttach(this, engine);
        PPhonology.PPhonologyAttach(this, engine);
        PTaxonomy.PTaxonomyAttach(this, engine);
        PRepertoire.PRepertoireAttach(this, engine);
        PCorpus.PCorpusAttach(this, engine);
        PDuplex.PDuplexAttach(this, engine);
        PSettings.PSettingsAttach(this, engine);
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
        PLibrary.PLibraryClose();
        PPhonology.PPhonologyClose();
        PTaxonomy.PTaxonomyClose();
        PRepertoire.PRepertoireClose();
        PCorpus.PCorpusClose();
        PDuplex.PDuplexClose();
        _lEngine.Dispose();
    }
}
