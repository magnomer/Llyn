using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QScoreboard
{
    private readonly UserControl _qScoreboardSurface;

    private readonly ObservableCollection<QScoreboardItem> _qScoreboardList = [];

    private readonly ObservableCollection<QScoreboardBar> _qScoreboardChart = [];

    private CScoreboard _cScoreboard = null!;

    internal QScoreboard(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qScoreboardSurface = surface;
    }

    private ItemsControl QScoreboardShelf =>
        QContract.QContractFind<ItemsControl>(_qScoreboardSurface, "PScoreboardShelf");

    private TextBlock QScoreboardEmpty => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PScoreboardEmpty");

    private Border QScoreboardCensus => QContract.QContractFind<Border>(_qScoreboardSurface, "PCensus");

    private TextBlock QScoreboardName => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusName");

    private TextBlock QScoreboardTotal => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusTotal");

    private TextBlock QScoreboardActive => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusActive");

    private TextBlock QScoreboardExcluded => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusExcluded");

    private TextBlock QScoreboardOrphaned => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusOrphaned");

    private TextBlock QScoreboardFlagged => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusFlagged");

    private TextBlock QScoreboardSynced => QContract.QContractFind<TextBlock>(_qScoreboardSurface, "PCensusSynced");

    private ItemsControl QScoreboardWeeks => QContract.QContractFind<ItemsControl>(_qScoreboardSurface, "PCensusChart");

    internal void QScoreboardIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        _cScoreboard = new CScoreboard(atelier, envoy);
        _cScoreboard.CScoreboardChanged += QScoreboardRefine;
        _cScoreboard.CScoreboardChanged += QScoreboardCensusRefine;
        QScoreboardShelf.ItemsSource = _qScoreboardList;
        QLookItem.QLookItemAttach(QScoreboardShelf, QScoreboardItemRefine);
        QScoreboardWeeks.ItemsSource = _qScoreboardChart;
        QLookItem.QLookItemAttach(QScoreboardWeeks, QScoreboardBarRefine);
        QScoreboardEmpty.SetResourceReference(TextBlock.TextProperty, _cScoreboard.CScoreboardEmptyKey);
    }

    private void QScoreboardRefine()
    {
        CScoreboardRoll roll = _cScoreboard.CScoreboardRead();
        _qScoreboardList.Clear();
        foreach (CScoreboardRow row in roll.CScoreboardRollDecks)
        {
            if (row.CScoreboardRowFolder is null)
            {
                _qScoreboardList.Add(QScoreboardItemRead(row));
            }
        }

        foreach (CDeckFolder folder in roll.CScoreboardRollFolders)
        {
            _qScoreboardList.Add(new QScoreboardItem(folder.CDeckFolderId, folder.CDeckFolderName, true, false));
            foreach (CScoreboardRow row in roll.CScoreboardRollDecks)
            {
                if (row.CScoreboardRowFolder == folder.CDeckFolderId)
                {
                    _qScoreboardList.Add(QScoreboardItemRead(row));
                }
            }
        }

        QScoreboardNameRefine(roll.CScoreboardRollDecks.FirstOrDefault(static row => row.CScoreboardRowChosen));
    }

    private static QScoreboardItem QScoreboardItemRead(CScoreboardRow row)
    {
        return new QScoreboardItem(row.CScoreboardRowId, row.CScoreboardRowName, false, row.CScoreboardRowChosen);
    }

    private void QScoreboardNameRefine(CScoreboardRow? chosen)
    {
        if (chosen is { CScoreboardRowId: null })
        {
            QScoreboardName.SetResourceReference(TextBlock.TextProperty, "Scoreboard.All");
        }
        else
        {
            QScoreboardName.Text = chosen?.CScoreboardRowName ?? string.Empty;
        }
    }

    private void QScoreboardCensusRefine()
    {
        CScoreboardCensus? census = _cScoreboard.CScoreboardCensusRead();
        string? empty = census?.CScoreboardCensusEmpty;
        if (empty is not null)
        {
            QScoreboardEmpty.SetResourceReference(TextBlock.TextProperty, empty);
        }

        QScoreboardEmpty.Visibility = QLook.QLookVisibleRead(empty is not null);
        QScoreboardCensus.Visibility = QLook.QLookVisibleRead(census is { CScoreboardCensusEmpty: null });
        _qScoreboardChart.Clear();
        if (census is not { CScoreboardCensusEmpty: null } ready)
        {
            return;
        }

        QScoreboardTotal.Text = ready.CScoreboardCensusTotal;
        QScoreboardActive.Text = ready.CScoreboardCensusActive;
        QScoreboardExcluded.Text = ready.CScoreboardCensusExcluded;
        QScoreboardOrphaned.Text = ready.CScoreboardCensusOrphaned;
        QScoreboardFlagged.Text = ready.CScoreboardCensusFlagged;
        QScoreboardSynced.Text = ready.CScoreboardCensusSynced;
        IReadOnlyList<CScoreboardWeek> weeks = ready.CScoreboardCensusWeeks;
        int peak = weeks.Select(static week => week.CScoreboardWeekAdded).DefaultIfEmpty().Max();
        for (int index = 0; index < weeks.Count; index++)
        {
            CScoreboardWeek week = weeks[index];
            _qScoreboardChart.Add(new QScoreboardBar(
                index % 3 == 2 ? week.CScoreboardWeekMonday : string.Empty,
                week.CScoreboardWeekTally,
                week.CScoreboardWeekAdded,
                peak));
        }
    }

    private void QScoreboardItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QScoreboardItem deck)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PScoreboardRow") is Button row)
        {
            if (deck.QScoreboardItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QScoreboardRowObserve;
            if (!deck.QScoreboardItemFolder)
            {
                row.Click += QScoreboardRowObserve;
            }
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PScoreboardIcon") is QIconImage icon)
        {
            if (deck.QScoreboardItemFolder)
            {
                icon.QIconSource = QIcon.QIconResolve("folder", 16);
            }
            else
            {
                icon.ClearValue(QIconImage.QIconSourceProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PScoreboardText") is TextBlock text)
        {
            if (deck.QScoreboardItemId is null)
            {
                text.SetResourceReference(TextBlock.TextProperty, "Scoreboard.All");
            }
            else
            {
                text.Text = deck.QScoreboardItemText;
            }
        }
    }

    private void QScoreboardBarRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QScoreboardBar bar)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PWeek") is Grid week)
        {
            int added = bar.QScoreboardBarAdded;
            week.RowDefinitions[0].Height = new GridLength(bar.QScoreboardBarPeak - added, GridUnitType.Star);
            week.RowDefinitions[1].Height = new GridLength(added, GridUnitType.Star);
            week.ToolTip = bar.QScoreboardBarTally;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PWeekMonday") is TextBlock monday)
        {
            monday.Text = bar.QScoreboardBarMonday;
        }
    }

    private void QScoreboardRowObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QScoreboardItem>(sender) is QScoreboardItem deck)
        {
            _cScoreboard.CScoreboardSelect(deck.QScoreboardItemId);
        }
    }
}
