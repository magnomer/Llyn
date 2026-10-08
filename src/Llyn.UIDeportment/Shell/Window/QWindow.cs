using System;
using System.ComponentModel;
using System.Windows;
using Llyn.Conduct;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class QWindow
{
    private readonly Window _qWindowSurface;

    private readonly QFootprint _qFootprint;

    private readonly QCaption _qCaption;

    private readonly QHeadquarter _qHeadquarter;

    private readonly QEstablishment _qEstablishment;

    private readonly QPanelRoster _qPanelRoster;

    public QWindow(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        QWindowAtelier = atelier;
        QWindowPosture = new QPosture();
        QWindowVolume = new QVolume(atelier);

        _qWindowSurface = QContract.QContractSheetFind<Window>("PWindow");
        QWindowEnvoy = new QEnvoy(_qWindowSurface);
        _qEstablishment = new QEstablishment(
            QContract.QContractFind<FrameworkElement>(_qWindowSurface, "PEstablishment"));
        _qPanelRoster = new QPanelRoster(_qWindowSurface, QWindowPosture);
        _qFootprint = new QFootprint(_qWindowSurface, QWindowPosture);
        _qCaption = new QCaption(_qWindowSurface);
        _qHeadquarter = new QHeadquarter(_qWindowSurface);

        QWindowMention = new QMentionMenu(_qWindowSurface, atelier.CAtelierNavigation);
        QWindowIntroduce();
        _ = new QNavigation(_qWindowSurface, atelier.CAtelierNavigation);

        QWindowAtelier.CAtelierOpen(QWindowEnvoy, QObserver.QObserverCreate<Action>(static run => run()));
        _qFootprint.QFootprintRefine();
    }

    internal Window QWindowSurface => _qWindowSurface;

    internal CAtelier QWindowAtelier { get; }

    internal QPosture QWindowPosture { get; }

    internal QVolume QWindowVolume { get; }

    internal QWindowScreen QWindowScreen { get; } = new();

    internal QMentionMenu QWindowMention { get; }

    internal CEnvoy QWindowEnvoy { get; }

    private void QWindowIntroduce()
    {
        _qWindowSurface.Tag = QWindowScreen;

        _qCaption.QCaptionIntroduce();
        _qHeadquarter.QHeadquarterIntroduce();

        QLook.QLookStyleAttach(_qWindowSurface);
        _qWindowSurface.PreviewKeyDown += QChronicle.QChronicleKeyObserve;

        _qWindowSurface.Loaded += (_, _) => _qFootprint.QFootprintAttach();
        _qWindowSurface.Closing += QWindowClosingObserve;
        _qWindowSurface.Closed += QWindowExitRefine;
        _qWindowSurface.Closed += QWindowExitObserve;

        QEnsignImage.QEnsignIntroduce(QWindowAtelier);

        QWindowAtelier.CAtelierWorkspace.CWorkspaceOpened += QWindowPostureRefine;
        _qPanelRoster.QPanelRosterIntroduce(QWindowAtelier, QWindowEnvoy, QWindowVolume, QWindowMention);
        _qPanelRoster.QPanelRosterAttach();
        _qEstablishment.QEstablishmentAttach(_qWindowSurface, QWindowAtelier, QWindowEnvoy);
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
            _qFootprint.QFootprintSave(true);
        }
    }

    private void QWindowPostureRefine()
    {
        QWindowPosture.QPostureRootRefine(QWindowAtelier.CAtelierPathRead());
    }

    private void QWindowExitRefine(object? sender, EventArgs e)
    {
        _qPanelRoster.QPanelRosterClose();
        _qEstablishment.QEstablishmentClose();
        QWindowPosture.Dispose();
    }

    private void QWindowExitObserve(object? sender, EventArgs e)
    {
        QWindowAtelier.CAtelierClose();
    }
}
