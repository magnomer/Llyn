using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    private readonly Window _qWindowSurface;

    private readonly QLayout _qLayout;

    private readonly QFootprint _qFootprint;

    private readonly QEstablishment _qEstablishment;

    private readonly QInput _qInput;

    private readonly QTaxonomy _qTaxonomy;

    private readonly QTenor _qTenor;

    private readonly QRepertoire _qRepertoire;

    private readonly QCorpus _qCorpus;

    private readonly QReference _qReference;

    private readonly QLibrary _qLibrary;

    private readonly QPhonology _qPhonology;

    private readonly QXiesheng _qXiesheng;

    private readonly QYunjing _qYunjing;

    private readonly QFavorite _qFavorite;

    private readonly QDuplex _qDuplex;

    private readonly QGuild _qGuild;

    private readonly QSettings _qSettings;

    private CNavigation _cNavigation = null!;

    private QTab[] _qNavigationTabs = [];

    public QWindow(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        QWindowAtelier = atelier;
        QWindowPosture = new QPosture();
        QWindowVolume = new QVolume(atelier);
        _qLayout = new QLayout(QWindowPosture);

        _qWindowSurface = QContract.QContractSheetFind<Window>("PWindow");
        QWindowEnvoy = new QEnvoy(_qWindowSurface, this);
        _qEstablishment = new QEstablishment(
            QContract.QContractFind<FrameworkElement>(_qWindowSurface, "PEstablishment"));
        _qInput = new QInput(QInputSurface);
        _qTaxonomy = new QTaxonomy(QTaxonomySurface);
        _qTenor = new QTenor(QTenorSurface);
        _qRepertoire = new QRepertoire(QRepertoireSurface);
        _qCorpus = new QCorpus(QCorpusSurface);
        _qReference = new QReference(QReferenceSurface);
        _qLibrary = new QLibrary(QLibrarySurface);
        _qPhonology = new QPhonology(QPhonologySurface);
        _qXiesheng = new QXiesheng(QXieshengSurface);
        _qYunjing = new QYunjing(QYunjingSurface);
        _qFavorite = new QFavorite(QFavoriteSurface);
        _qDuplex = new QDuplex(QDuplexSurface);
        _qGuild = new QGuild(QGuildSurface);
        _qSettings = new QSettings(QSettingsSurface, _qLayout);
        _qFootprint = new QFootprint(QWindowPosture);

        QWindowIconRefine();
        QWindowIntroduce();

        QWindowAtelier.CAtelierOpen(QWindowEnvoy, QObserver.QObserverCreate<Action>(static run => run()));
        QFootprintRefine();
    }

    internal Window QWindowSurface => _qWindowSurface;

    internal CAtelier QWindowAtelier { get; }

    internal QPosture QWindowPosture { get; }

    internal QVolume QWindowVolume { get; }

    internal QWindowScreen QWindowScreen { get; } = new();

    private Grid QRoof => (Grid)_qWindowSurface.FindName("PRoof");

    private Grid QLogo => (Grid)_qWindowSurface.FindName("PLogo");

    private TextBlock QHeadquarterAbout => (TextBlock)_qWindowSurface.FindName("PHeadquarterAbout");

    private TextBlock QHeadquarterExit => (TextBlock)_qWindowSurface.FindName("PHeadquarterExit");

    private Button QCaptionMinimize => (Button)_qWindowSurface.FindName("PCaptionMinimize");

    private Button QCaptionMaximize => (Button)_qWindowSurface.FindName("PCaptionMaximize");

    private Button QCaptionExit => (Button)_qWindowSurface.FindName("PCaptionExit");

    private UserControl QInputSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PInput");

    private UserControl QLibrarySurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PLibrary");

    private UserControl QPhonologySurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PPhonology");

    private UserControl QXieshengSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PXiesheng");

    private UserControl QYunjingSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PYunjing");

    private UserControl QTaxonomySurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PTaxonomy");

    private UserControl QTenorSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PTenor");

    private UserControl QRepertoireSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PRepertoire");

    private UserControl QCorpusSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PCorpus");

    private UserControl QReferenceSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PReference");

    private UserControl QGuildSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PGuild");

    private UserControl QFavoriteSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PFavorite");

    private UserControl QDuplexSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PDuplex");

    private UserControl QSettingsSurface => QContract.QContractFind<UserControl>(_qWindowSurface, "PSettings");

    internal CEnvoy QWindowEnvoy { get; }

    private void QWindowIntroduce()
    {
        _qWindowSurface.Tag = this;
        _qWindowSurface.SetValue(PMention.PMentionHostProperty, this);

        QRoof.MouseLeftButtonDown += QRoofRefine;
        QLogo.MouseLeftButtonDown += QLogoRefine;
        QHeadquarterAbout.MouseLeftButtonDown += QHeadquarterAboutRefine;
        QHeadquarterExit.MouseLeftButtonDown += QHeadquarterExitRefine;
        QCaptionMinimize.Click += QCaptionMinimizeRefine;
        QCaptionMaximize.Click += QCaptionMaximizeRefine;
        QCaptionExit.Click += QCaptionExitRefine;

        QNavigationInput.Click += QNavigationObserve;
        QNavigationLibrary.Click += QNavigationObserve;
        QNavigationFavorite.Click += QNavigationObserve;
        QNavigationPhonology.Click += QNavigationObserve;
        QNavigationRepertoire.Click += QNavigationObserve;
        QNavigationTenor.Click += QNavigationObserve;
        QNavigationTaxonomy.Click += QNavigationObserve;
        QNavigationCorpus.Click += QNavigationObserve;
        QNavigationSource.Click += QNavigationObserve;
        QNavigationGuild.Click += QNavigationObserve;
        QNavigationXiesheng.Click += QNavigationObserve;
        QNavigationYunjing.Click += QNavigationObserve;
        QNavigationDuplex.Click += QNavigationObserve;
        QNavigationSettings.Click += QNavigationObserve;

        QLook.QLookStyleAttach(_qWindowSurface);
        QMentionList.ItemsSource = _qMentionItem;
        QLookItem.QLookItemAttach(QMentionList, QMentionRowRefine);
        QMentionMenu.Closed += (_, _) => QMentionMenuHide();
        _qWindowSurface.PreviewKeyDown += QMentionKeyObserve;
        _qWindowSurface.PreviewKeyDown += QChronicleKeyObserve;
        _qWindowSurface.Deactivated += QMentionLeaveRefine;

        _qWindowSurface.Loaded += (_, _) => QFootprintAttach();
        _qWindowSurface.Closing += QWindowClosingObserve;
        _qWindowSurface.Closed += QWindowExitRefine;
        _qWindowSurface.Closed += QWindowExitObserve;

        QEnsignImage.QEnsignIntroduce(QWindowAtelier);

        _qInput.QInputIntroduce(this);
        _qLibrary.QLibraryIntroduce(this);
        _qPhonology.QPhonologyIntroduce(this);
        _qXiesheng.QXieshengIntroduce(this);
        _qYunjing.QYunjingIntroduce(this);
        _qTaxonomy.QTaxonomyIntroduce(this);
        _qTenor.QTenorIntroduce(this);
        _qRepertoire.QRepertoireIntroduce(this);
        _qCorpus.QCorpusIntroduce(this);
        _qReference.QReferenceIntroduce(this);
        _qGuild.QGuildIntroduce(this);
        _qFavorite.QFavoriteIntroduce(this);
        _qDuplex.QDuplexIntroduce(this);
        _qSettings.QSettingsIntroduce(this);
        _qEstablishment.QEstablishmentAttach(this);
        QNavigationIntroduce();

        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += QWindowPostureRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qLibrary.QLibraryVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qPhonology.QPhonologyVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qFavorite.QFavoriteVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qTaxonomy.QTaxonomyVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qTenor.QTenorVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qRepertoire.QRepertoireVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qRepertoire.QOccurrenceVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qReference.QReferenceVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qReference.QFootnoteVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qCorpus.QCorpusVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qCorpus.QQuotationVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QGuildVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QOeuvreRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qGuild.QTallyRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qXiesheng.QXieshengVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qXiesheng.QGroveRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qXiesheng.QXieshengStemRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qYunjing.QYunjingVistaRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qYunjing.QShengmuRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qYunjing.QYunmuRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qYunjing.QYunjingDiweiRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qLayout.QLayoutResetRefine;
        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += _qLayout.QLayoutRefine;

        QWindowLayoutAttach();
    }

    private void QWindowIconRefine()
    {
        ((Shape)QCaptionMinimize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionMinimize });
        ((Shape)QCaptionMaximize.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionMaximize });
        ((Shape)QCaptionExit.Content).SetBinding(
            Shape.StrokeProperty, new Binding(nameof(Control.Foreground)) { Source = QCaptionExit });

        QNavigationInput.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("input", 24));
        QNavigationLibrary.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("library", 24));
        QNavigationFavorite.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("favorite", 24));
        QNavigationPhonology.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("phonology", 24));
        QNavigationRepertoire.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("repertoire", 24));
        QNavigationTenor.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("tenor", 24));
        QNavigationTaxonomy.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("taxonomy", 24));
        QNavigationCorpus.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("corpus", 24));
        QNavigationSource.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("reference", 24));
        QNavigationGuild.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("guild", 24));
        QNavigationXiesheng.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("xiesheng", 24));
        QNavigationYunjing.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("yunjing", 24));
        QNavigationDuplex.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("duplex", 24));
        QNavigationSettings.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("settings", 24));
    }

    private void QWindowLayoutAttach()
    {
        _qLayout.QLayoutAttach((Grid)QLibrarySurface.Content, "library");
        _qLayout.QLayoutAttach((Grid)QPhonologySurface.Content, "phonology");
        _qLayout.QLayoutAttach((Grid)QXieshengSurface.Content, "xiesheng");
        _qLayout.QLayoutAttach((Grid)QYunjingSurface.Content, "yunjing");
        _qLayout.QLayoutAttach((Grid)QTaxonomySurface.Content, "taxonomy");
        _qLayout.QLayoutAttach((Grid)QTenorSurface.Content, "tenor");
        _qLayout.QLayoutAttach((Grid)QRepertoireSurface.Content, "repertoire");
        _qLayout.QLayoutAttach((Grid)QCorpusSurface.Content, "corpus");
        _qLayout.QLayoutAttach((Grid)QReferenceSurface.Content, "reference");
        _qLayout.QLayoutAttach((Grid)QGuildSurface.Content, "guild");
        _qLayout.QLayoutAttach((Grid)QFavoriteSurface.Content, "favorite");
    }

    private void QWindowClosingObserve(object? sender, CancelEventArgs e)
    {
        QWindowClosureRefine(e, QWindowAtelier.CAtelierQuitConfirm(QWindowEnvoy));
    }

    private void QWindowClosureRefine(CancelEventArgs e, bool confirmed)
    {
        e.Cancel = !confirmed;

        if (confirmed)
        {
            _qFootprint.QFootprintDefer(QFootprintRead(), _qWindowSurface.WindowState == WindowState.Minimized, true);
        }
    }

    private void QWindowPostureRefine()
    {
        QWindowPosture.QPostureRootRefine(QWindowAtelier.CAtelierPathRead());
    }

    private void QFootprintRefine()
    {
        Window window = _qWindowSurface;
        if (_qFootprint.QFootprintRead(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight,
                window.MinWidth,
                window.MinHeight) is not LCapsuleWindow state)
        {
            Rect area = SystemParameters.WorkArea;
            (window.Width, window.Height) =
                _qFootprint.QFootprintSizeRead(area.Width, area.Height, window.MinWidth, window.MinHeight);
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

    private void QFootprintAttach()
    {
        _qWindowSurface.LocationChanged += (_, _) => QFootprintSave();
        _qWindowSurface.SizeChanged += (_, _) => QFootprintSave();
        _qWindowSurface.StateChanged += (_, _) => QFootprintSave();
    }

    private void QFootprintSave()
    {
        _qFootprint.QFootprintDefer(QFootprintRead(), _qWindowSurface.WindowState == WindowState.Minimized, false);
    }

    private LCapsuleWindow QFootprintRead()
    {
        Window window = _qWindowSurface;
        Rect bounds = window.WindowState == WindowState.Normal
            ? new Rect(window.Left, window.Top, window.Width, window.Height)
            : window.RestoreBounds;

        return new LCapsuleWindow(
            bounds.Left, bounds.Top, bounds.Width, bounds.Height, window.WindowState == WindowState.Maximized);
    }

    private void QWindowExitRefine(object? sender, EventArgs e)
    {
        _qInput.QInputExitRefine();
        _qLibrary.QLibraryExitRefine();
        _qPhonology.QPhonologyExitRefine();
        _qXiesheng.QXieshengExitRefine();
        _qYunjing.QYunjingExitRefine();
        _qTaxonomy.QTaxonomyExitRefine();
        _qTenor.QTenorExitRefine();
        _qRepertoire.QRepertoireExitRefine();
        _qCorpus.QCorpusExitRefine();
        _qReference.QReferenceExitRefine();
        _qGuild.QGuildClose();
        _qFavorite.QFavoriteExitRefine();
        _qEstablishment.QEstablishmentClose();
        QWindowPosture.Dispose();
    }

    private void QWindowExitObserve(object? sender, EventArgs e)
    {
        QWindowAtelier.CAtelierClose();
    }
}
