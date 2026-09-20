using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWindow : Window
{
    private readonly LWindow _lWindow;

    private readonly PLayout _pLayout;

    private readonly Action<string> _pWindowWorkspaceOpener;

    public PWindow(LWindow window, Action<string> workspaceOpener)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(workspaceOpener);

        _lWindow = window;
        _pWindowWorkspaceOpener = workspaceOpener;
        _pLayout = new PLayout(window);
        SetValue(PMention.PMentionWindowProperty, _lWindow);

        InitializeComponent();

        Resources.MergedDictionaries.Add(new PMentionMenuTemplate(this));
        PMentionList.ItemsSource = _pMentionItem;
        PreviewKeyDown += PMentionKeyHandle;
        PreviewKeyDown += PChronicleKeyHandle;
        PreviewKeyDown += PVoyageKeyHandle;
        PreviewMouseDown += PVoyageMouseHandle;
        Deactivated += PMentionLeaveHandle;

        PWindowStateRestore();
        PWindowStateAttach();

        PWindowAttach();

        Closing += PWindowClosingHandle;
        Closed += PWindowExitHandle;
    }

    internal int PWindowLeftover { get; private set; }

    internal PLayout PWindowLayout => _pLayout;

    internal LWindow PWindowDeportment => _lWindow;

    internal PWindowScreen PWindowScreen { get; } = new();

    internal void PWindowWorkspaceChange(string path)
    {
        _pWindowWorkspaceOpener(path);
    }

    private void PWindowAttach()
    {
        PEnsign.PEnsignAttach(_lWindow);

        _lWindow.LWindowLeftoverSweep();
        _lWindow.LWindowRecordingSweep();
        PWindowLeftover = _lWindow.LWindowLeftoverRead().Count;

        LWorkspaceState state = _lWindow.LWindowStateRead();

        PInput.PInputAttach(this);
        PLibrary.PLibraryAttach(this);
        PPhonology.PPhonologyAttach(this);
        PYunjing.PYunjingAttach(this);
        PNavigationYunjing.Visibility = PYunjing.PYunjingCheck() ? Visibility.Visible : Visibility.Collapsed;
        PTaxonomy.PTaxonomyAttach(this);
        PTenor.PTenorAttach(this);
        PRepertoire.PRepertoireAttach(this);
        PCorpus.PCorpusAttach(this);
        PReference.PReferenceAttach(this);
        PGuild.PGuildAttach(this);
        PFavorite.PFavoriteAttach(this);
        PDuplex.PDuplexAttach(this);
        PSettings.PSettingsAttach(this);
        PEstablishment.PEstablishmentAttach(this);

        PWindowLayoutAttach();

        PWindowViewRestore(state);
    }

    private void PWindowLayoutAttach()
    {
        _pLayout.PLayoutAttach((Grid)PLibrary.Content, "library");
        _pLayout.PLayoutAttach((Grid)PPhonology.Content, "phonology");
        _pLayout.PLayoutAttach((Grid)PYunjing.Content, "yunjing");
        _pLayout.PLayoutAttach((Grid)PTaxonomy.Content, "taxonomy");
        _pLayout.PLayoutAttach((Grid)PTenor.Content, "tenor");
        _pLayout.PLayoutAttach((Grid)PRepertoire.Content, "repertoire");
        _pLayout.PLayoutAttach((Grid)PCorpus.Content, "corpus");
        _pLayout.PLayoutAttach((Grid)PReference.Content, "reference");
        _pLayout.PLayoutAttach((Grid)PGuild.Content, "guild");
        _pLayout.PLayoutAttach((Grid)PFavorite.Content, "favorite");

        _pLayout.PLayoutRestore();
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
        PYunjing.PYunjingClose();
        PTaxonomy.PTaxonomyClose();
        PTenor.PTenorClose();
        PRepertoire.PRepertoireClose();
        PCorpus.PCorpusClose();
        PReference.PReferenceClose();
        PGuild.PGuildClose();
        PFavorite.PFavoriteClose();
        PDuplex.PDuplexClose();
        PEstablishment.PEstablishmentClose();
        _lWindow.LWindowLeftoverSweep();
        _lWindow.Dispose();
    }
}
