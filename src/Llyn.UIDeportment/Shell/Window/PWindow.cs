using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private readonly Window _pWindowSurface;

    private readonly LWindow _lWindow;

    private readonly PLayout _pLayout;

    private readonly Action<string> _pWindowWorkspaceOpener;

    private readonly LFootprint _lFootprint;

    private readonly QEstablishment _qEstablishment;

    private readonly QTaxonomy _qTaxonomy;

    private readonly QTenor _qTenor;

    private readonly QRepertoire _qRepertoire;

    private readonly QCorpus _qCorpus;

    private readonly QReference _qReference;

    private readonly QLibrary _qLibrary;

    private readonly QFavorite _qFavorite;

    private readonly QDuplex _qDuplex;

    private readonly QGuild _qGuild;

    private LNavigation _lNavigation = null!;

    private LTab[] _pNavigationTabs = [];

    public PWindow(LWindow window, Action<string> workspaceOpener)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(workspaceOpener);

        _lWindow = window;
        _pWindowWorkspaceOpener = workspaceOpener;
        _pLayout = new PLayout(window);

        _pWindowSurface = QContract.QContractSheetFind<Window>("PWindow");
        _qEstablishment = new QEstablishment(
            QContract.QContractFind<FrameworkElement>(_pWindowSurface, "PEstablishment"));
        _qTaxonomy = new QTaxonomy(PTaxonomy);
        _qTenor = new QTenor(PTenor);
        _qRepertoire = new QRepertoire(PRepertoire);
        _qCorpus = new QCorpus(PCorpus);
        _qReference = new QReference(PReference);
        _qLibrary = new QLibrary(PLibrary);
        _qFavorite = new QFavorite(PFavorite);
        _qDuplex = new QDuplex(PDuplex);
        _qGuild = new QGuild(PGuild);
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
        QLook.QLookStyleAttach(_pMentionMenuTemplate);
        PMentionList.ItemsSource = _pMentionItem;
        QLookItem.QLookItemAttach(PMentionList, PMentionRowApply);
        _pWindowSurface.PreviewKeyDown += PMentionKeyHandle;
        _pWindowSurface.PreviewKeyDown += PChronicleKeyHandle;
        _pWindowSurface.Deactivated += PMentionLeaveHandle;

        _lFootprint = new LFootprint(window);
        PFootprintRestore();
        _pWindowSurface.Loaded += (_, _) => PFootprintAttach();

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

    private UserControl PLibrary => QContract.QContractFind<UserControl>(_pWindowSurface, nameof(PLibrary));

    private PPhonology PPhonology => (PPhonology)_pWindowSurface.FindName(nameof(PPhonology));

    private PXiesheng PXiesheng => (PXiesheng)_pWindowSurface.FindName(nameof(PXiesheng));

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

    internal LWing PWindowWingCreate()
    {
        return new LWing(_lWindow);
    }

    internal void PWindowWorkspaceChange(string path)
    {
        _pWindowWorkspaceOpener(path);
    }

    private void PWindowAttach()
    {
        LEnsignImage.LEnsignAttach(_lWindow);

        _lWindow.LWindowLeftoverSweep();
        _lWindow.LWindowWorkspace.QWorkspaceRecordingSweep();

        CWorkspaceState state = _lWindow.LWindowWorkspace.QWorkspaceStateRead();

        PInput.PInputAttach(this);
        _qLibrary.QLibraryAttach(this);
        PPhonology.PPhonologyAttach(this);
        PXiesheng.PXieshengAttach(this);
        PYunjing.PYunjingAttach(this);
        _qTaxonomy.QTaxonomyAttach(this);
        _qTenor.QTenorAttach(this);
        _qRepertoire.QRepertoireAttach(this);
        _qCorpus.QCorpusAttach(this);
        _qReference.QReferenceAttach(this);
        _qGuild.QGuildAttach(this);
        _qFavorite.QFavoriteAttach(this);
        _qDuplex.QDuplexAttach(this);
        PSettings.PSettingsAttach(this);
        _qEstablishment.QEstablishmentAttach(this);
        PNavigationAttach();

        PWindowLayoutAttach();

        PWindowViewRestore(state);
    }

    private void PWindowLayoutAttach()
    {
        _pLayout.PLayoutAttach((Grid)PLibrary.Content, "library");
        _pLayout.PLayoutAttach((Grid)((UserControl)PPhonology.Content).Content, "phonology");
        _pLayout.PLayoutAttach((Grid)((UserControl)PXiesheng.Content).Content, "xiesheng");
        _pLayout.PLayoutAttach((Grid)((UserControl)PYunjing.Content).Content, "yunjing");
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
        bool confirmed = PWindowDiscardConfirm();
        e.Cancel = !confirmed;

        if (confirmed)
        {
            _lFootprint.LFootprintClose(PFootprintRead(), _pWindowSurface.WindowState == WindowState.Minimized);
        }
    }

    private void PFootprintRestore()
    {
        Window window = _pWindowSurface;
        if (_lFootprint.LFootprintRead(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight,
                window.MinWidth,
                window.MinHeight) is not CWindowState state)
        {
            Rect area = SystemParameters.WorkArea;
            (window.Width, window.Height) =
                _lFootprint.LFootprintSizeRead(area.Width, area.Height, window.MinWidth, window.MinHeight);
            return;
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = state.CWindowStateLeft;
        window.Top = state.CWindowStateTop;
        window.Width = state.CWindowStateWidth;
        window.Height = state.CWindowStateHeight;

        if (state.CWindowStateMaximized)
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
        _lFootprint.LFootprintSave(PFootprintRead(), _pWindowSurface.WindowState == WindowState.Minimized);
    }

    private CWindowState PFootprintRead()
    {
        Window window = _pWindowSurface;
        Rect bounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        return new CWindowState(
            bounds.Left, bounds.Top, bounds.Width, bounds.Height, window.WindowState == WindowState.Maximized);
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PInput.PInputClose();
        _qLibrary.QLibraryClose();
        PPhonology.PPhonologyClose();
        PXiesheng.PXieshengClose();
        PYunjing.PYunjingClose();
        _qTaxonomy.QTaxonomyClose();
        _qTenor.QTenorClose();
        _qRepertoire.QRepertoireClose();
        _qCorpus.QCorpusClose();
        _qReference.QReferenceClose();
        _qGuild.QGuildClose();
        _qFavorite.QFavoriteClose();
        _qDuplex.QDuplexClose();
        _qEstablishment.QEstablishmentClose();
        _lWindow.LWindowLeftoverSweep();
        _lWindow.Dispose();
    }
}
