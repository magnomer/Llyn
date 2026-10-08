using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTaxonomy
{
    private readonly UserControl _qTaxonomySurface;

    private readonly QEditor _qTaxonomyEditor;

    private readonly QDisplay _qTaxonomyDisplay;

    private readonly QPanelRail _qTaxonomyRail;

    private readonly QChoiceOrder _qTaxonomyOrder;

    private readonly QChoiceFilter _qTaxonomyFilter;

    private readonly QMembership _qMembership;

    private readonly ObservableCollection<QDirectoryItem> _qDirectoryList = [];

    private CAtelier _cAtelier = null!;

    private CEnvoy _cEnvoy = null!;

    private CTaxonomy _cTaxonomy = null!;

    internal QTaxonomy(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qTaxonomySurface = surface;
        _qTaxonomyEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qTaxonomyDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QTaxonomyPressObserve, QTaxonomyPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QTaxonomyPortraitObserve, QTaxonomyPressRefine));
        _qTaxonomyRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PTaxonomyRail"),
            QTaxonomyBin,
            QTaxonomyBinIcon,
            true,
            true);
        _qTaxonomyOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PTaxonomyOrder"), QFunnel);
        _qTaxonomyFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PTaxonomyFilter"));
        _qMembership = new QMembership(surface);

        QExploration.SetResourceReference(QField.QFieldHintProperty, "Tag.Search");

        QExploration.TextChanged += QExplorationObserve;
        _qTaxonomyRail.QPanelRailCreated += QTaxonomyFreshObserve;
        _qTaxonomyRail.QPanelRailStored += QTaxonomyStoreObserve;
        _qTaxonomyRail.QPanelRailToggled += QTaxonomyScribeObserve;
        _qTaxonomyRail.QPanelRailDeleted += QTaxonomyBinObserve;
    }

    private Border QFunnel => QContract.QContractFind<Border>(_qTaxonomySurface, "PFunnel");

    private TextBox QExploration => QContract.QContractFind<TextBox>(_qTaxonomySurface, "PExploration");

    private ItemsControl QDirectory => QContract.QContractFind<ItemsControl>(_qTaxonomySurface, "PDirectory");

    private TextBlock QDirectoryEmpty => QContract.QContractFind<TextBlock>(_qTaxonomySurface, "PDirectoryEmpty");

    private Button QTaxonomyBin => QContract.QContractFind<Button>(_qTaxonomySurface, "PTaxonomyBin");

    private QIconImage QTaxonomyBinIcon => QContract.QContractFind<QIconImage>(_qTaxonomySurface, "PTaxonomyBinIcon");

    internal void QTaxonomyIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cAtelier = atelier;
        _cEnvoy = envoy;
        _cTaxonomy = CTaxonomy.CTaxonomyCreate(
            atelier,
            QTaxonomyShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cTaxonomy.CTaxonomyTagOpened += QDirectoryTagRefine;
        _cTaxonomy.CTaxonomyAperture.CApertureRowsChanged += QDirectoryRefine;
        _cTaxonomy.CTaxonomyWorkspaceChanged += QTaxonomyWorkspaceRefine;
        _qTaxonomyOrder.QChoiceOrderIntroduce(
            _cTaxonomy.CTaxonomyAperture,
            "Funnel",
            [
                CCatalogOrder.CCatalogOrderName,
                CCatalogOrder.CCatalogOrderReverse,
            ]);
        _qTaxonomyFilter.QChoiceFilterIntroduce(_cTaxonomy.CTaxonomyAperture, "Lattice");
        CPanel panel = _cTaxonomy.CTaxonomyMembership.CMembershipPanel;
        panel.CPanelChanged += QTaxonomyModeRefine;
        _qMembership.QMembershipIntroduce(_cTaxonomy);

        QDirectory.ItemsSource = _qDirectoryList;
        QLookItem.QLookItemAttach(QDirectory, QDirectoryItemRefine);

        _qTaxonomyDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cTaxonomy.CTaxonomyEditor.CEditorDisplay);
        _cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskStateChanged += QTaxonomyStoreRefine;
        _qTaxonomyEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cTaxonomy.CTaxonomyEditor);
        _qTaxonomyEditor.QEditorChronicleChanged += QTaxonomyChronicleRefine;
        _qTaxonomyRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qTaxonomyEditor);
    }

    private void QTaxonomyStoreRefine()
    {
        _qTaxonomyRail.QEntryStorableRefine(_cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal void QTaxonomyExitRefine()
    {
        _qTaxonomyEditor.QEditorPlayerRefine();
    }

    private void QTaxonomyPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cTaxonomy?.CTaxonomyMembership.CMembershipPanel.CPanelPressAllowed ?? false;
    }

    private async void QTaxonomyPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTaxonomy.CTaxonomyMembership.CMembershipPortraitPrint();
    }

    private async void QTaxonomyPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cTaxonomy.CTaxonomyMembership.CMembershipPortraitExport();
    }

    private void QTaxonomyChronicleRefine()
    {
        (bool undo, bool redo) = _cTaxonomy.CTaxonomyEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qTaxonomyRail.QChronicleRefine(undo, redo);
    }

    private bool QTaxonomyShownCheck()
    {
        return _qTaxonomySurface.IsVisible;
    }

    private void QTaxonomyModeRefine()
    {
        CPanel panel = _cTaxonomy.CTaxonomyMembership.CMembershipPanel;
        _qTaxonomyEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(panel.CPanelEditing));
        _qTaxonomyDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(panel.CPanelViewerChecked));
        _qTaxonomyRail.QPanelRailRefine(
            panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private async void QTaxonomyWorkspaceRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(_cEnvoy, QEnsignImage.QEnsignDraw);
    }

    private void QExplorationObserve(object sender, TextChangedEventArgs e)
    {
        _cTaxonomy.CTaxonomyAperture.CApertureQuerySet(QExploration.Text ?? string.Empty);
    }

    internal async void QTaxonomyVistaRefine()
    {
        _qTaxonomyOrder.QChoiceOrderRefine();
        _qTaxonomyFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CCatalogTag>> sheet =
            await _cTaxonomy.CTaxonomyRowsLoad(QEnsignImage.QEnsignDraw);
        _qTaxonomyFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        QDirectoryRefine(sheet.CEnsignSheetRows);
    }

    private void QDirectoryRefine()
    {
        QDirectoryRefine(_cTaxonomy.CTaxonomyRowsRead());
    }

    private void QDirectoryRefine(IReadOnlyList<CCatalogTag> rows)
    {
        _qDirectoryList.Clear();
        foreach (CCatalogTag row in rows)
        {
            _qDirectoryList.Add(new QDirectoryItem(
                row.CCatalogTagStored.CTagId, row.CCatalogTagStored.CTagText, row.CCatalogTagChosen));
        }

        QDirectoryEmpty.Visibility = _qDirectoryList.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QDirectoryObserve(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is QDirectoryItem item)
        {
            _cTaxonomy.CTaxonomyTagToggle(item.QDirectoryItemId);
        }
    }

    private void QDirectoryItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QDirectoryItem directory)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PDirectoryRow") is Button row)
        {
            if (directory.QDirectoryItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QDirectoryObserve;
            row.Click += QDirectoryObserve;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PDirectoryIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("tag", 16);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDirectoryText") is TextBlock text)
        {
            text.Text = directory.QDirectoryItemText;
        }
    }

    private void QDirectoryTagRefine()
    {
        QExploration.Text = string.Empty;
    }

    private void QTaxonomyFreshObserve()
    {
        _cTaxonomy.CTaxonomyEntryCreate();
    }

    private void QTaxonomyScribeObserve(bool scribe)
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelScribeToggle(scribe);
    }

    private void QTaxonomyStoreObserve()
    {
        _cTaxonomy.CTaxonomyEditor.CEditorEntrySave();
    }

    private void QTaxonomyBinObserve()
    {
        _cTaxonomy.CTaxonomyMembership.CMembershipPanel.CPanelBin.CPanelBinDelete();
    }
}
