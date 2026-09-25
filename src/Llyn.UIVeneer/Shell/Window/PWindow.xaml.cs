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

    private readonly LFootprint _lFootprint;

    private LNavigation _lNavigation = null!;

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
        Deactivated += PMentionLeaveHandle;

        _lFootprint = new LFootprint(this, window);
        _lFootprint.LFootprintRestore();

        PWindowAttach();

        Closing += PWindowClosingHandle;
        Closed += PWindowExitHandle;
    }

    internal PLayout PWindowLayout => _pLayout;

    internal LWindow PWindowDeportment => _lWindow;

    internal PWindowScreen PWindowScreen { get; } = new();

    internal void PWindowWorkspaceChange(string path)
    {
        _pWindowWorkspaceOpener(path);
    }

    private void PWindowAttach()
    {
        LEnsignImage.LEnsignAttach(_lWindow);

        _lWindow.LWindowLeftoverSweep();
        _lWindow.LWindowRecordingSweep();

        LWorkspaceState state = _lWindow.LWindowStateRead();

        PInput.PInputAttach(this);
        PLibrary.PLibraryAttach(this);
        PPhonology.PPhonologyAttach(this);
        PXiesheng.PXieshengAttach(this);
        PYunjing.PYunjingAttach(this);
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
        _lNavigation = new LNavigation(this, _lWindow, PTab.PTabChosenProperty, PNavigationTabRead());

        PWindowLayoutAttach();

        PWindowViewRestore(state);
    }

    private void PWindowLayoutAttach()
    {
        _pLayout.PLayoutAttach((Grid)PLibrary.Content, "library");
        _pLayout.PLayoutAttach((Grid)PPhonology.Content, "phonology");
        _pLayout.PLayoutAttach((Grid)PXiesheng.Content, "xiesheng");
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
        _lFootprint.LFootprintClosingHandle(e, PWindowDiscardConfirm());
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PInput.PInputClose();
        PLibrary.PLibraryClose();
        PPhonology.PPhonologyClose();
        PXiesheng.PXieshengClose();
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
