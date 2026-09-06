using System;
using System.ComponentModel;
using System.Windows;
using Llyn.Core;
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
        PEnsign.PEnsignAttach(engine);

        engine.LEngineLeftoverSweep();
        PWindowLeftover = engine.LEngineLeftoverRead().Count;

        LWorkspaceState state = engine.LEngineStateRead();

        PInput.PInputAttach(this, engine);
        PLibrary.PLibraryAttach(this, engine);
        PPhonology.PPhonologyAttach(this, engine);
        PTaxonomy.PTaxonomyAttach(this, engine);
        PRepertoire.PRepertoireAttach(this, engine);
        PCorpus.PCorpusAttach(this, engine);
        PReference.PReferenceAttach(this, engine);
        PFavorite.PFavoriteAttach(this, engine);
        PDuplex.PDuplexAttach(this, engine);
        PSettings.PSettingsAttach(this, engine);

        PWindowViewRestore(state);
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
        PReference.PReferenceClose();
        PFavorite.PFavoriteClose();
        PDuplex.PDuplexClose();
        _lEngine.LEngineLeftoverSweep();
        _lEngine.Dispose();
    }
}
