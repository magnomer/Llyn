using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QTenor
{
    private readonly UserControl _qTenorSurface;

    private PWindow _qTenorHost = null!;

    private CTenor _cTenor = null!;

    internal QTenor(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qTenorSurface = surface;

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QTenorPressObserve, QTenorPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QTenorPortraitObserve, QTenorPressRefine));
        QTenorPortrait.Command = PDisplayCommand.PDisplayCommandPortrait;
        QTenorPress.Command = ApplicationCommands.Print;

        QChoice.QChoiceDropperAttach(QDegreeDropper, QDegreeDropdown, QDegree);
        QChoice.QChoiceDropperAttach(QGrilleDropper, QGrilleDropdown, QGrilleDropper);

        QDegreeIcon.QIconSource = QIcon.QIconResolve("sort", 24);
        QGrilleIcon.QIconSource = QIcon.QIconResolve("filter", 24);
        QTenorBinIcon.QIconSource = QIcon.QIconResolve("delete", 24);
        QSounding.SetResourceReference(QField.QFieldHintProperty, "Tag.Search");
        QQuest.SetResourceReference(QField.QFieldHintProperty, "Quest.Search");
        QTenorFresh.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("new", 24));
        QTenorStore.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("save", 24));
        QTenorEarlier.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("retreat", 24));
        QTenorLater.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("advance", 24));
        QTenorBackward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("undo", 24));
        QTenorForward.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("redo", 24));
        QTenorPortrait.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("export", 24));
        QTenorPress.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("print", 24));
        QTenorViewer.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("view", 24));
        QTenorScribe.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("edit", 24));

        QSounding.TextChanged += QSoundingObserve;
        QQuest.TextChanged += QQuestObserve;
        QTenorFresh.Click += QTenorFreshObserve;
        QTenorStore.Click += QTenorStoreObserve;
        QTenorEarlier.Click += QTenorRetreatObserve;
        QTenorLater.Click += QTenorAdvanceObserve;
        QTenorBackward.Click += QTenorUndoObserve;
        QTenorForward.Click += QTenorRedoObserve;
        QTenorViewer.Click += QTenorViewerObserve;
        QTenorScribe.Click += QTenorScribeObserve;
        QTenorBin.Click += QTenorBinObserve;
    }

    private Border QDegree => QContract.QContractFind<Border>(_qTenorSurface, "PDegree");

    private ToggleButton QDegreeDropper => QContract.QContractFind<ToggleButton>(_qTenorSurface, "PDegreeDropper");

    private QIconImage QDegreeIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PDegreeIcon");

    private Popup QDegreeDropdown => QContract.QContractFind<Popup>(_qTenorSurface, "PDegreeDropdown");

    private StackPanel QDegreeList => QContract.QContractFind<StackPanel>(_qTenorSurface, "PDegreeList");

    private TextBox QSounding => QContract.QContractFind<TextBox>(_qTenorSurface, "PSounding");

    private ItemsControl QGamut => QContract.QContractFind<ItemsControl>(_qTenorSurface, "PGamut");

    private TextBlock QGamutEmpty => QContract.QContractFind<TextBlock>(_qTenorSurface, "PGamutEmpty");

    private ItemsControl QCohort => QContract.QContractFind<ItemsControl>(_qTenorSurface, "PCohort");

    private TextBlock QCohortEmpty => QContract.QContractFind<TextBlock>(_qTenorSurface, "PCohortEmpty");

    private TextBox QQuest => QContract.QContractFind<TextBox>(_qTenorSurface, "PQuest");

    private ToggleButton QGrilleDropper =>
        QContract.QContractFind<ToggleButton>(_qTenorSurface, "PGrilleDropper");

    private QIconImage QGrilleIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PGrilleIcon");

    private FrameworkElement QGrilleMark =>
        QContract.QContractFind<FrameworkElement>(_qTenorSurface, "PGrilleMark");

    private Popup QGrilleDropdown => QContract.QContractFind<Popup>(_qTenorSurface, "PGrilleDropdown");

    private StackPanel QGrilleList => QContract.QContractFind<StackPanel>(_qTenorSurface, "PGrilleList");

    private Button QTenorFresh => QContract.QContractFind<Button>(_qTenorSurface, "PTenorFresh");

    private Button QTenorStore => QContract.QContractFind<Button>(_qTenorSurface, "PTenorStore");

    private StackPanel QTenorVoyage => QContract.QContractFind<StackPanel>(_qTenorSurface, "PTenorVoyage");

    private Button QTenorEarlier => QContract.QContractFind<Button>(_qTenorSurface, "PTenorEarlier");

    private Button QTenorLater => QContract.QContractFind<Button>(_qTenorSurface, "PTenorLater");

    private StackPanel QTenorChronicle =>
        QContract.QContractFind<StackPanel>(_qTenorSurface, "PTenorChronicle");

    private Button QTenorBackward => QContract.QContractFind<Button>(_qTenorSurface, "PTenorBackward");

    private Button QTenorForward => QContract.QContractFind<Button>(_qTenorSurface, "PTenorForward");

    private Button QTenorPortrait => QContract.QContractFind<Button>(_qTenorSurface, "PTenorPortrait");

    private Button QTenorPress => QContract.QContractFind<Button>(_qTenorSurface, "PTenorPress");

    private Border QTenorMode => QContract.QContractFind<Border>(_qTenorSurface, "PTenorMode");

    private RadioButton QTenorViewer => QContract.QContractFind<RadioButton>(_qTenorSurface, "PTenorViewer");

    private RadioButton QTenorScribe => QContract.QContractFind<RadioButton>(_qTenorSurface, "PTenorScribe");

    private PDisplay QTenorDisplay => QContract.QContractFind<PDisplay>(_qTenorSurface, "PDisplay");

    private PEditor QTenorEditor => QContract.QContractFind<PEditor>(_qTenorSurface, "PEditor");

    private Button QTenorBin => QContract.QContractFind<Button>(_qTenorSurface, "PTenorBin");

    private QIconImage QTenorBinIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PTenorBinIcon");

    internal void QTenorIntroduce(PWindow host)
    {
        _qTenorHost = host;
        _cTenor = CTenor.CTenorCreate(
            host.PWindowAtelier,
            QTenorShownCheck,
            host.PWindowEnvoy,
            LObserver.LObserverCreate<Action>(static run => run()));
        _cTenor.CTenorRegisterOpened += QGamutRegisterRefine;
        _cTenor.CTenorRowsChanged += QGamutRefine;
        _cTenor.CTenorWorkspaceChanged += QTenorWorkspaceRefine;
        CPanel panel = _cTenor.CTenorCohort.CCohortPanel;
        QLectern lectern = new(_cTenor.CTenorEditor.CEditorDisplay, panel);
        panel.CPanelChanged += QTenorModeRefine;
        panel.CPanelRowsChanged += QCohortRefine;

        QGamut.ItemsSource = _qGamutList;
        QCohort.ItemsSource = _qCohortList;
        QLookItem.QLookItemAttach(QGamut, QGamutItemRefine);
        QLookItem.QLookItemAttach(QCohort, QCohortItemRefine);

        QTenorDisplay.PDisplayAttach(host, lectern);
        _cTenor.CTenorEditor.CEditorDesk.CDeskStateChanged += QTenorStoreRefine;
        QTenorEditor.PEditorIntroduce(host, new QEditor(_cTenor.CTenorEditor));
        QTenorEditor.PEditorChronicleChanged += QTenorChronicleRefine;
    }

    private void QTenorStoreRefine()
    {
        QTenorStore.IsEnabled = _cTenor.CTenorEditor.CEditorDesk.CDeskStorable;
    }

    internal void QTenorExitRefine()
    {
        QTenorEditor.PEditorPlayerRefine();
    }

    private void QTenorPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cTenor?.CTenorCohort.CCohortPanel.CPanelPressAllowed ?? false;
    }

    private async void QTenorPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTenor.CTenorCohort.CCohortPortraitPrint();
    }

    private async void QTenorPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTenor.CTenorCohort.CCohortPortraitExport();
    }

    internal void QTenorVoyageRefine(bool past, bool future)
    {
        QTenorEarlier.IsEnabled = past;
        QTenorLater.IsEnabled = future;
    }

    private void QTenorRetreatObserve(object sender, RoutedEventArgs e)
    {
        _qTenorHost.PWindowAtelier.CAtelierNavigation.CNavigationStationUndo();
    }

    private void QTenorAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _qTenorHost.PWindowAtelier.CAtelierNavigation.CNavigationStationRedo();
    }

    private void QTenorUndoObserve(object sender, RoutedEventArgs e)
    {
        QTenorEditor.QChronicleUndoObserve();
    }

    private void QTenorRedoObserve(object sender, RoutedEventArgs e)
    {
        QTenorEditor.QChronicleRedoObserve();
    }

    private void QTenorChronicleRefine()
    {
        (bool undo, bool redo) = _cTenor.CTenorEditor.CEditorDesk.CDeskChronicleRead();
        QTenorBackward.IsEnabled = undo;
        QTenorForward.IsEnabled = redo;
    }

    private bool QTenorShownCheck()
    {
        return _qTenorSurface.IsVisible;
    }

    private void QTenorModeRefine()
    {
        CPanel panel = _cTenor.CTenorCohort.CCohortPanel;
        QTenorEditor.Visibility = QLook.QLookVisibleRead(panel.CPanelEditing);
        QTenorDisplay.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QTenorViewer.IsChecked = panel.CPanelViewerChecked;
        QTenorScribe.IsChecked = panel.CPanelScribeChecked;
        QTenorVoyage.Visibility = QLook.QLookVisibleRead(panel.CPanelViewerChecked);
        QTenorChronicle.Visibility = QLook.QLookVisibleRead(panel.CPanelScribeChecked);
        QTenorMode.IsEnabled = panel.CPanelModeEnabled;
        QTenorBin.IsEnabled = panel.CPanelBinEnabled;
    }
}
