using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private readonly Window _pWindowSurface;

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

        _pWindowSurface = (Window)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Shell/Window/PWindow.xaml", UriKind.Relative));
        _pWindowSurface.Tag = this;
        _pWindowSurface.SetValue(PMention.PMentionWindowProperty, _lWindow);

        PRoof.MouseLeftButtonDown += PRoofHandle;
        PLogo.MouseLeftButtonDown += PLogoHandle;
        PHeadquarterAbout.MouseLeftButtonDown += PHeadquarterAboutHandle;
        PHeadquarterExit.MouseLeftButtonDown += PHeadquarterExitHandle;
        PCaptionMinimize.Click += PCaptionMinimizeHandle;
        PCaptionMaximize.Click += PCaptionMaximizeHandle;
        PCaptionExit.Click += PCaptionExitHandle;
        ((Shape)PCaptionMinimize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionMinimize });
        ((Shape)PCaptionMaximize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionMaximize });
        ((Shape)PCaptionExit.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionExit });

        PNavigationInput.PTabIcon = PIcon.PIconResolve("input", 24);
        PNavigationLibrary.PTabIcon = PIcon.PIconResolve("library", 24);
        PNavigationFavorite.PTabIcon = PIcon.PIconResolve("favorite", 24);
        PNavigationPhonology.PTabIcon = PIcon.PIconResolve("phonology", 24);
        PNavigationRepertoire.PTabIcon = PIcon.PIconResolve("repertoire", 24);
        PNavigationTenor.PTabIcon = PIcon.PIconResolve("tenor", 24);
        PNavigationTaxonomy.PTabIcon = PIcon.PIconResolve("taxonomy", 24);
        PNavigationCorpus.PTabIcon = PIcon.PIconResolve("corpus", 24);
        PNavigationSource.PTabIcon = PIcon.PIconResolve("reference", 24);
        PNavigationGuild.PTabIcon = PIcon.PIconResolve("guild", 24);
        PNavigationXiesheng.PTabIcon = PIcon.PIconResolve("xiesheng", 24);
        PNavigationYunjing.PTabIcon = PIcon.PIconResolve("yunjing", 24);
        PNavigationDuplex.PTabIcon = PIcon.PIconResolve("duplex", 24);
        PNavigationSettings.PTabIcon = PIcon.PIconResolve("settings", 24);

        PNavigationInput.Click += PNavigationHandle;
        PNavigationLibrary.Click += PNavigationHandle;
        PNavigationFavorite.Click += PNavigationHandle;
        PNavigationPhonology.Click += PNavigationHandle;
        PNavigationRepertoire.Click += PNavigationHandle;
        PNavigationTenor.Click += PNavigationHandle;
        PNavigationTaxonomy.Click += PNavigationHandle;
        PNavigationCorpus.Click += PNavigationHandle;
        PNavigationSource.Click += PNavigationHandle;
        PNavigationGuild.Click += PNavigationHandle;
        PNavigationXiesheng.Click += PNavigationHandle;
        PNavigationYunjing.Click += PNavigationHandle;
        PNavigationDuplex.Click += PNavigationHandle;
        PNavigationSettings.Click += PNavigationHandle;

        _pMentionMenuTemplate = new PMentionMenuTemplate(this);
        _pWindowSurface.Resources.MergedDictionaries.Add(_pMentionMenuTemplate);
        PLook.PLookStyleAttach(_pMentionMenuTemplate);
        PMentionList.ItemsSource = _pMentionItem;
        PLookItem.PLookItemAttach(PMentionList, PMentionRowApply);
        _pWindowSurface.PreviewKeyDown += PMentionKeyHandle;
        _pWindowSurface.PreviewKeyDown += PChronicleKeyHandle;
        _pWindowSurface.Deactivated += PMentionLeaveHandle;

        _lFootprint = new LFootprint(_pWindowSurface, window);
        _lFootprint.LFootprintRestore();

        PWindowAttach();

        _pWindowSurface.Closing += PWindowClosingHandle;
        _pWindowSurface.Closed += PWindowExitHandle;
    }

    internal Window PWindowSurface => _pWindowSurface;

    internal PLayout PWindowLayout => _pLayout;

    internal LWindow PWindowDeportment => _lWindow;

    internal PWindowScreen PWindowScreen { get; } = new();

    private Grid PRoof => (Grid)_pWindowSurface.FindName(nameof(PRoof));

    private Grid PLogo => (Grid)_pWindowSurface.FindName(nameof(PLogo));

    private TextBlock PHeadquarterAbout => (TextBlock)_pWindowSurface.FindName(nameof(PHeadquarterAbout));

    private TextBlock PHeadquarterExit => (TextBlock)_pWindowSurface.FindName(nameof(PHeadquarterExit));

    private Button PCaptionMinimize => (Button)_pWindowSurface.FindName(nameof(PCaptionMinimize));

    private Button PCaptionMaximize => (Button)_pWindowSurface.FindName(nameof(PCaptionMaximize));

    private Button PCaptionExit => (Button)_pWindowSurface.FindName(nameof(PCaptionExit));

    private PInput PInput => (PInput)_pWindowSurface.FindName(nameof(PInput));

    private PLibrary PLibrary => (PLibrary)_pWindowSurface.FindName(nameof(PLibrary));

    private PPhonology PPhonology => (PPhonology)_pWindowSurface.FindName(nameof(PPhonology));

    private PXiesheng PXiesheng => (PXiesheng)_pWindowSurface.FindName(nameof(PXiesheng));

    private PYunjing PYunjing => (PYunjing)_pWindowSurface.FindName(nameof(PYunjing));

    private PTaxonomy PTaxonomy => (PTaxonomy)_pWindowSurface.FindName(nameof(PTaxonomy));

    private PTenor PTenor => (PTenor)_pWindowSurface.FindName(nameof(PTenor));

    private PRepertoire PRepertoire => (PRepertoire)_pWindowSurface.FindName(nameof(PRepertoire));

    private PCorpus PCorpus => (PCorpus)_pWindowSurface.FindName(nameof(PCorpus));

    private PReference PReference => (PReference)_pWindowSurface.FindName(nameof(PReference));

    private PGuild PGuild => (PGuild)_pWindowSurface.FindName(nameof(PGuild));

    private PFavorite PFavorite => (PFavorite)_pWindowSurface.FindName(nameof(PFavorite));

    private PDuplex PDuplex => (PDuplex)_pWindowSurface.FindName(nameof(PDuplex));

    private PSettings PSettings => (PSettings)_pWindowSurface.FindName(nameof(PSettings));

    private PEstablishment PEstablishment => (PEstablishment)_pWindowSurface.FindName(nameof(PEstablishment));

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
        _lNavigation = new LNavigation(_pWindowSurface, _lWindow, PTab.PTabChosenProperty, PNavigationTabRead());

        PWindowLayoutAttach();

        PWindowViewRestore(state);
    }

    private void PWindowLayoutAttach()
    {
        _pLayout.PLayoutAttach((Grid)((UserControl)PLibrary.Content).Content, "library");
        _pLayout.PLayoutAttach((Grid)((UserControl)PPhonology.Content).Content, "phonology");
        _pLayout.PLayoutAttach((Grid)((UserControl)PXiesheng.Content).Content, "xiesheng");
        _pLayout.PLayoutAttach((Grid)((UserControl)PYunjing.Content).Content, "yunjing");
        _pLayout.PLayoutAttach((Grid)((UserControl)PTaxonomy.Content).Content, "taxonomy");
        _pLayout.PLayoutAttach((Grid)((UserControl)PTenor.Content).Content, "tenor");
        _pLayout.PLayoutAttach((Grid)((UserControl)PRepertoire.Content).Content, "repertoire");
        _pLayout.PLayoutAttach((Grid)((UserControl)PCorpus.Content).Content, "corpus");
        _pLayout.PLayoutAttach((Grid)((UserControl)PReference.Content).Content, "reference");
        _pLayout.PLayoutAttach((Grid)((UserControl)PGuild.Content).Content, "guild");
        _pLayout.PLayoutAttach((Grid)((UserControl)PFavorite.Content).Content, "favorite");

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
