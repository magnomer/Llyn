using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTenor
{
    private readonly UserControl _qTenorSurface;

    private readonly QEditor _qTenorEditor;

    private readonly QDisplay _qTenorDisplay;

    private readonly QPanelRail _qTenorRail;

    private readonly QChoiceOrder _qTenorOrder;

    private readonly QChoiceFilter _qTenorFilter;

    private readonly QCohort _qCohort;

    private readonly ObservableCollection<QGamutItem> _qGamutList = [];

    private CAtelier _cAtelier = null!;

    private CEnvoy _cEnvoy = null!;

    private CTenor _cTenor = null!;

    internal QTenor(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qTenorSurface = surface;
        _qTenorEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qTenorDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QTenorPressObserve, QTenorPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QTenorPortraitObserve, QTenorPressRefine));
        _qTenorRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PTenorRail"), QTenorBin, QTenorBinIcon, true, true);
        _qTenorOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PTenorOrder"), QDegree);
        _qTenorFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PTenorFilter"));
        _qCohort = new QCohort(surface);

        QSounding.SetResourceReference(QField.QFieldHintProperty, "Register.Search");

        QSounding.TextChanged += QSoundingObserve;
        _qTenorRail.QPanelRailCreated += QTenorFreshObserve;
        _qTenorRail.QPanelRailStored += QTenorStoreObserve;
        _qTenorRail.QPanelRailToggled += QTenorScribeObserve;
        _qTenorRail.QPanelRailDeleted += QTenorBinObserve;
    }

    private Border QDegree => QContract.QContractFind<Border>(_qTenorSurface, "PDegree");

    private TextBox QSounding => QContract.QContractFind<TextBox>(_qTenorSurface, "PSounding");

    private ItemsControl QGamut => QContract.QContractFind<ItemsControl>(_qTenorSurface, "PGamut");

    private TextBlock QGamutEmpty => QContract.QContractFind<TextBlock>(_qTenorSurface, "PGamutEmpty");

    private Button QTenorBin => QContract.QContractFind<Button>(_qTenorSurface, "PTenorBin");

    private QIconImage QTenorBinIcon => QContract.QContractFind<QIconImage>(_qTenorSurface, "PTenorBinIcon");

    internal void QTenorIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cAtelier = atelier;
        _cEnvoy = envoy;
        _cTenor = CTenor.CTenorCreate(
            atelier,
            QTenorShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cTenor.CTenorRegisterOpened += QGamutRegisterRefine;
        _cTenor.CTenorAperture.CApertureRowsChanged += QGamutRefine;
        _cTenor.CTenorWorkspaceChanged += QTenorWorkspaceRefine;
        _qTenorOrder.QChoiceOrderIntroduce(
            _cTenor.CTenorAperture,
            "Degree",
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
                CCatalogOrder.CCatalogOrderUsage,
            ]);
        _qTenorFilter.QChoiceFilterIntroduce(_cTenor.CTenorAperture, "Grille");
        CPanel panel = _cTenor.CTenorCohort.CCohortPanel;
        panel.CPanelChanged += QTenorModeRefine;
        _qCohort.QCohortIntroduce(_cTenor);

        QGamut.ItemsSource = _qGamutList;
        QLookItem.QLookItemAttach(QGamut, QGamutItemRefine);

        _qTenorDisplay.QDisplayIntroduce(atelier, envoy, volume, mentionMenu, _cTenor.CTenorEditor.CEditorDisplay);
        _cTenor.CTenorEditor.CEditorDesk.CDeskStateChanged += QTenorStoreRefine;
        _qTenorEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cTenor.CTenorEditor);
        _qTenorEditor.QEditorChronicleChanged += QTenorChronicleRefine;
        _qTenorRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qTenorEditor);
    }

    private void QTenorStoreRefine()
    {
        _qTenorRail.QEntryStorableRefine(_cTenor.CTenorEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal void QTenorExitRefine()
    {
        _qTenorEditor.QEditorPlayerRefine();
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

    private void QTenorChronicleRefine()
    {
        (bool undo, bool redo) = _cTenor.CTenorEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qTenorRail.QChronicleRefine(undo, redo);
    }

    private bool QTenorShownCheck()
    {
        return _qTenorSurface.IsVisible;
    }

    private void QTenorModeRefine()
    {
        CPanel panel = _cTenor.CTenorCohort.CCohortPanel;
        _qTenorEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qTenorDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        _qTenorRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private async void QTenorWorkspaceRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(_cEnvoy, QEnsignImage.QEnsignDraw);
    }

    private void QSoundingObserve(object sender, TextChangedEventArgs e)
    {
        _cTenor.CTenorAperture.CApertureQuerySet(QSounding.Text ?? string.Empty);
    }

    internal async void QTenorVistaRefine()
    {
        _qTenorOrder.QChoiceOrderRefine();
        _qTenorFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CCatalogRegister>> sheet =
            await _cTenor.CTenorRowsLoad(QEnsignImage.QEnsignDraw);
        _qTenorFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        QGamutRefine(sheet.CEnsignSheetRows);
    }

    private void QGamutRefine()
    {
        QGamutRefine(_cTenor.CTenorRowsRead());
    }

    private void QGamutRefine(IReadOnlyList<CCatalogRegister> rows)
    {
        _qGamutList.Clear();
        foreach (CCatalogRegister row in rows)
        {
            _qGamutList.Add(new QGamutItem(row));
        }

        QGamutEmpty.Visibility = _qGamutList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QGamutObserve(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is QGamutItem item)
        {
            _cTenor.CTenorRegisterToggle(item.QGamutItemId);
        }
    }

    private void QGamutItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QGamutItem gamut)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PGamutRow") is Button row)
        {
            if (gamut.QGamutItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QGamutObserve;
            row.Click += QGamutObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PGamutIcon") is QIconImage icon)
        {
            icon.QIconSource = gamut.QGamutItemIcon;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutName") is TextBlock name)
        {
            name.Text = gamut.QGamutItemName;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PGamutCount") is TextBlock count)
        {
            count.Text = gamut.QGamutItemCount;
        }
    }

    private void QGamutRegisterRefine()
    {
        QSounding.Text = string.Empty;
    }

    private void QTenorFreshObserve()
    {
        _cTenor.CTenorEntryCreate();
    }

    private void QTenorScribeObserve(bool scribe)
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelScribeToggle(scribe);
    }

    private void QTenorStoreObserve()
    {
        _cTenor.CTenorEditor.CEditorEntrySave();
    }

    private void QTenorBinObserve()
    {
        _cTenor.CTenorCohort.CCohortPanel.CPanelBin.CPanelBinDelete();
    }
}
