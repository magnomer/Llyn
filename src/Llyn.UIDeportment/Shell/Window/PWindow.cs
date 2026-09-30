using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private readonly Window _pWindowSurface;

    private readonly PLayout _pLayout;

    private readonly LFootprint _lFootprint;

    private readonly QEstablishment _qEstablishment;

    private readonly QTaxonomy _qTaxonomy;

    private readonly QTenor _qTenor;

    private readonly QRepertoire _qRepertoire;

    private readonly QCorpus _qCorpus;

    private readonly QReference _qReference;

    private readonly QLibrary _qLibrary;

    private readonly QPhonology _qPhonology;

    private readonly QXiesheng _qXiesheng;

    private readonly QFavorite _qFavorite;

    private readonly QDuplex _qDuplex;

    private readonly QGuild _qGuild;

    private CNavigation _cNavigation = null!;

    private QTab[] _pNavigationTabs = [];

    public PWindow(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        PWindowAtelier = atelier;
        PWindowForge = new QForge(atelier);
        PWindowPosture = new QPosture(atelier.CAtelierPathRead);
        _pLayout = new PLayout(PWindowPosture);

        _pWindowSurface = QContract.QContractSheetFind<Window>("PWindow");
        PWindowEnvoy = new QEnvoy(_pWindowSurface, this);
        _qEstablishment = new QEstablishment(
            QContract.QContractFind<FrameworkElement>(_pWindowSurface, "PEstablishment"));
        _qTaxonomy = new QTaxonomy(PTaxonomy);
        _qTenor = new QTenor(PTenor);
        _qRepertoire = new QRepertoire(PRepertoire);
        _qCorpus = new QCorpus(PCorpus);
        _qReference = new QReference(PReference);
        _qLibrary = new QLibrary(PLibrary);
        _qPhonology = new QPhonology(PPhonology);
        _qXiesheng = new QXiesheng(PXiesheng);
        _qFavorite = new QFavorite(PFavorite);
        _qDuplex = new QDuplex(PDuplex);
        _qGuild = new QGuild(PGuild);
        _pMentionMenuTemplate = new PMentionMenuTemplate(this);
        _lFootprint = new LFootprint(PWindowPosture);

        PWindowIconRefine();
        PFootprintRefine();
        PWindowIntroduce();

        PWindowAtelier.CAtelierOpen();
    }

    internal Window PWindowSurface => _pWindowSurface;

    internal CAtelier PWindowAtelier { get; }

    internal QForge PWindowForge { get; }

    internal QPosture PWindowPosture { get; }

    internal PWindowScreen PWindowScreen { get; } = new();

    private Grid PRoof => (Grid)_pWindowSurface.FindName(nameof(PRoof));

    private Grid PLogo => (Grid)_pWindowSurface.FindName(nameof(PLogo));

    private TextBlock PHeadquarterAbout => (TextBlock)_pWindowSurface.FindName(nameof(PHeadquarterAbout));

    private TextBlock PHeadquarterExit => (TextBlock)_pWindowSurface.FindName(nameof(PHeadquarterExit));

    private Button PCaptionMinimize => (Button)_pWindowSurface.FindName(nameof(PCaptionMinimize));

    private Button PCaptionMaximize => (Button)_pWindowSurface.FindName(nameof(PCaptionMaximize));

    private Button PCaptionExit => (Button)_pWindowSurface.FindName(nameof(PCaptionExit));

    private PInput PInput => (PInput)_pWindowSurface.FindName(nameof(PInput));

    private UserControl PLibrary => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PLibrary));

    private UserControl PPhonology => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PPhonology));

    private UserControl PXiesheng => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PXiesheng));

    private PYunjing PYunjing => (PYunjing)_pWindowSurface.FindName(nameof(PYunjing));

    private UserControl PTaxonomy => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PTaxonomy));

    private UserControl PTenor => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PTenor));

    private UserControl PRepertoire => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PRepertoire));

    private UserControl PCorpus => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PCorpus));

    private UserControl PReference => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PReference));

    private UserControl PGuild => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PGuild));

    private UserControl PFavorite => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PFavorite));

    private UserControl PDuplex => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PDuplex));

    private PSettings PSettings => (PSettings)_pWindowSurface.FindName(nameof(PSettings));

    internal CEnvoy PWindowEnvoy { get; }

    private void PWindowIntroduce()
    {
        _pWindowSurface.Tag = this;
        _pWindowSurface.SetValue(PMention.PMentionHostProperty, this);

        PRoof.MouseLeftButtonDown += PRoofRefine;
        PLogo.MouseLeftButtonDown += PLogoRefine;
        PHeadquarterAbout.MouseLeftButtonDown += PHeadquarterAboutRefine;
        PHeadquarterExit.MouseLeftButtonDown += PHeadquarterExitRefine;
        PCaptionMinimize.Click += PCaptionMinimizeRefine;
        PCaptionMaximize.Click += PCaptionMaximizeRefine;
        PCaptionExit.Click += PCaptionExitRefine;

        PNavigationInput.Click += PNavigationObserve;
        PNavigationLibrary.Click += PNavigationObserve;
        PNavigationFavorite.Click += PNavigationObserve;
        PNavigationPhonology.Click += PNavigationObserve;
        PNavigationRepertoire.Click += PNavigationObserve;
        PNavigationTenor.Click += PNavigationObserve;
        PNavigationTaxonomy.Click += PNavigationObserve;
        PNavigationCorpus.Click += PNavigationObserve;
        PNavigationSource.Click += PNavigationObserve;
        PNavigationGuild.Click += PNavigationObserve;
        PNavigationXiesheng.Click += PNavigationObserve;
        PNavigationYunjing.Click += PNavigationObserve;
        PNavigationDuplex.Click += PNavigationObserve;
        PNavigationSettings.Click += PNavigationObserve;

        _pWindowSurface.Resources.MergedDictionaries.Add(_pMentionMenuTemplate);
        QLook.QLookStyleAttach(_pMentionMenuTemplate);
        PMentionList.ItemsSource = _pMentionItem;
        QLookItem.QLookItemAttach(PMentionList, PMentionRowApply);
        _pWindowSurface.PreviewKeyDown += PMentionKeyHandle;
        _pWindowSurface.PreviewKeyDown += PChronicleKeyObserve;
        _pWindowSurface.Deactivated += PMentionLeaveRefine;

        _pWindowSurface.Loaded += (_, _) => PFootprintAttach();
        _pWindowSurface.Closing += PWindowClosingObserve;
        _pWindowSurface.Closed += PWindowExitRefine;
        _pWindowSurface.Closed += PWindowExitObserve;

        LEnsignImage.LEnsignIntroduce(PWindowAtelier);

        PInput.PInputIntroduce(this);
        _qLibrary.QLibraryIntroduce(this);
        _qPhonology.QPhonologyAttach(this);
        _qXiesheng.QXieshengAttach(this);
        PYunjing.PYunjingAttach(this);
        _qTaxonomy.QTaxonomyIntroduce(this);
        _qTenor.QTenorIntroduce(this);
        _qRepertoire.QRepertoireIntroduce(this);
        _qCorpus.QCorpusIntroduce(this);
        _qReference.QReferenceIntroduce(this);
        _qGuild.QGuildIntroduce(this);
        _qFavorite.QFavoriteIntroduce(this);
        _qDuplex.QDuplexIntroduce(this);
        PSettings.PSettingsIntroduce(this);
        _qEstablishment.QEstablishmentAttach(this);
        PNavigationIntroduce();

        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qLibrary.QLibraryVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qPhonology.QPhonologyVistaRestore;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qFavorite.QFavoriteVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qTaxonomy.QTaxonomyVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qTenor.QTenorVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qRepertoire.QRepertoireVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qRepertoire.QOccurrenceVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qReference.QReferenceVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qReference.QFootnoteVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qCorpus.QCorpusVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qCorpus.QQuotationVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QGuildVistaRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QOeuvreRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QTallyRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qXiesheng.QXieshengVistaRestore;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += PYunjing.PYunjingVistaRestore;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _pLayout.PLayoutResetRefine;
        PWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _pLayout.PLayoutRefine;

        PWindowLayoutAttach();
    }

    private void PWindowIconRefine()
    {
        ((Shape)PCaptionMinimize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionMinimize });
        ((Shape)PCaptionMaximize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionMaximize });
        ((Shape)PCaptionExit.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = PCaptionExit });

        PNavigationInput.PTabIcon = QIcon.QIconResolve("input", 24);
        PNavigationLibrary.PTabIcon = QIcon.QIconResolve("library", 24);
        PNavigationFavorite.PTabIcon = QIcon.QIconResolve("favorite", 24);
        PNavigationPhonology.PTabIcon = QIcon.QIconResolve("phonology", 24);
        PNavigationRepertoire.PTabIcon = QIcon.QIconResolve("repertoire", 24);
        PNavigationTenor.PTabIcon = QIcon.QIconResolve("tenor", 24);
        PNavigationTaxonomy.PTabIcon = QIcon.QIconResolve("taxonomy", 24);
        PNavigationCorpus.PTabIcon = QIcon.QIconResolve("corpus", 24);
        PNavigationSource.PTabIcon = QIcon.QIconResolve("reference", 24);
        PNavigationGuild.PTabIcon = QIcon.QIconResolve("guild", 24);
        PNavigationXiesheng.PTabIcon = QIcon.QIconResolve("xiesheng", 24);
        PNavigationYunjing.PTabIcon = QIcon.QIconResolve("yunjing", 24);
        PNavigationDuplex.PTabIcon = QIcon.QIconResolve("duplex", 24);
        PNavigationSettings.PTabIcon = QIcon.QIconResolve("settings", 24);
    }

    private void PWindowLayoutAttach()
    {
        _pLayout.PLayoutAttach((Grid)PLibrary.Content, "library");
        _pLayout.PLayoutAttach((Grid)PPhonology.Content, "phonology");
        _pLayout.PLayoutAttach((Grid)PXiesheng.Content, "xiesheng");
        _pLayout.PLayoutAttach((Grid)((UserControl)PYunjing.Content).Content, "yunjing");
        _pLayout.PLayoutAttach((Grid)PTaxonomy.Content, "taxonomy");
        _pLayout.PLayoutAttach((Grid)PTenor.Content, "tenor");
        _pLayout.PLayoutAttach((Grid)PRepertoire.Content, "repertoire");
        _pLayout.PLayoutAttach((Grid)PCorpus.Content, "corpus");
        _pLayout.PLayoutAttach((Grid)PReference.Content, "reference");
        _pLayout.PLayoutAttach((Grid)PGuild.Content, "guild");
        _pLayout.PLayoutAttach((Grid)PFavorite.Content, "favorite");
    }

    private void PWindowClosingObserve(object? sender, CancelEventArgs e)
    {
        PWindowClosureRefine(e, PWindowAtelier.CAtelierQuitConfirm(PWindowEnvoy));
    }

    private void PWindowClosureRefine(CancelEventArgs e, bool confirmed)
    {
        e.Cancel = !confirmed;

        if (confirmed)
        {
            _lFootprint.LFootprintDefer(PFootprintRead(), _pWindowSurface.WindowState == WindowState.Minimized, true);
        }
    }

    private void PFootprintRefine()
    {
        Window window = _pWindowSurface;
        if (_lFootprint.LFootprintRead(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight,
                window.MinWidth,
                window.MinHeight) is not LCapsuleWindow state)
        {
            Rect area = SystemParameters.WorkArea;
            (window.Width, window.Height) =
                _lFootprint.LFootprintSizeRead(area.Width, area.Height, window.MinWidth, window.MinHeight);
            return;
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = state.LCapsuleWindowLeft;
        window.Top = state.LCapsuleWindowTop;
        window.Width = state.LCapsuleWindowWidth;
        window.Height = state.LCapsuleWindowHeight;

        if (state.LCapsuleWindowMaximized)
        {
            window.WindowState = WindowState.Maximized;
        }
    }

    private void PFootprintAttach()
    {
        _pWindowSurface.LocationChanged += (_, _) => PFootprintSave();
        _pWindowSurface.SizeChanged += (_, _) => PFootprintSave();
        _pWindowSurface.StateChanged += (_, _) => PFootprintSave();
    }

    private void PFootprintSave()
    {
        _lFootprint.LFootprintDefer(PFootprintRead(), _pWindowSurface.WindowState == WindowState.Minimized, false);
    }

    private LCapsuleWindow PFootprintRead()
    {
        Window window = _pWindowSurface;
        Rect bounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        return new LCapsuleWindow(
            bounds.Left, bounds.Top, bounds.Width, bounds.Height, window.WindowState == WindowState.Maximized);
    }

    private void PWindowExitRefine(object? sender, EventArgs e)
    {
        PInput.PEditor.PEditorClose();
        _qLibrary.QLibraryExitRefine();
        _qPhonology.QPhonologyClose();
        _qXiesheng.QXieshengClose();
        PYunjing.PYunjingClose();
        _qTaxonomy.QTaxonomyExitRefine();
        _qTenor.QTenorExitRefine();
        _qRepertoire.QRepertoireExitRefine();
        _qCorpus.QCorpusExitRefine();
        _qReference.QReferenceExitRefine();
        _qGuild.QGuildClose();
        _qFavorite.QFavoriteExitRefine();
        _qEstablishment.QEstablishmentClose();
        PWindowPosture.Dispose();
    }

    private void PWindowExitObserve(object? sender, EventArgs e)
    {
        PWindowAtelier.CAtelierClose();
    }
}
