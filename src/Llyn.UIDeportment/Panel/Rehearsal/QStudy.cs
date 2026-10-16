using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStudy
{
    private readonly UserControl _qStudySurface;

    private readonly ObservableCollection<QStudyItem> _qStudyList = [];

    private readonly QSitting _qSitting;

    private CStudy _cStudy = null!;

    internal QStudy(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStudySurface = surface;
        _qSitting = new QSitting(QContract.QContractFind<Border>(surface, "PSitting"));
        QStudyLaunch.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("play", 24));
        QStudyShuffle.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("sort", 24));
        QStudyLaunch.Click += QStudyLaunchObserve;
        QStudyShuffle.Click += QStudyShuffleObserve;
    }

    private Button QStudyLaunch => QContract.QContractFind<Button>(_qStudySurface, "PStudyLaunch");

    private Button QStudyShuffle => QContract.QContractFind<Button>(_qStudySurface, "PStudyShuffle");

    private ItemsControl QStudyShelf => QContract.QContractFind<ItemsControl>(_qStudySurface, "PStudyShelf");

    private TextBlock QStudyEmpty => QContract.QContractFind<TextBlock>(_qStudySurface, "PStudyEmpty");

    internal void QStudyIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        _cStudy = new CStudy(atelier, envoy, Random.Shared.Next);
        _cStudy.CStudyChanged += QStudyRefine;
        _qSitting.QSittingIntroduce(_cStudy);
        QStudyShelf.ItemsSource = _qStudyList;
        QLookItem.QLookItemAttach(QStudyShelf, QStudyItemRefine);
        QStudyEmpty.SetResourceReference(TextBlock.TextProperty, _cStudy.CStudyEmptyKey);
    }

    private void QStudyRefine()
    {
        CStudyRoll roll = _cStudy.CStudyRead();
        _qStudyList.Clear();
        foreach (CStudyRow row in roll.CStudyRollDecks)
        {
            if (row.CStudyRowFolder is null)
            {
                _qStudyList.Add(QStudyItemRead(row));
            }
        }

        foreach (CDeckFolder folder in roll.CStudyRollFolders)
        {
            _qStudyList.Add(new QStudyItem(folder.CDeckFolderId, folder.CDeckFolderName, string.Empty, true, false));
            foreach (CStudyRow row in roll.CStudyRollDecks)
            {
                if (row.CStudyRowFolder == folder.CDeckFolderId)
                {
                    _qStudyList.Add(QStudyItemRead(row));
                }
            }
        }

        if (roll.CStudyRollEmpty is string key)
        {
            QStudyEmpty.SetResourceReference(TextBlock.TextProperty, key);
        }
        else
        {
            QStudyEmpty.ClearValue(TextBlock.TextProperty);
        }

        QStudyEmpty.Visibility = QLook.QLookVisibleRead(roll.CStudyRollEmpty is not null);
        QStudyLaunch.IsEnabled = roll.CStudyRollEmpty is null;
        QStudyShuffleRefine(roll.CStudyRollSitting);
        _qSitting.QSittingRefine(roll.CStudyRollSitting);
    }

    private static QStudyItem QStudyItemRead(CStudyRow row)
    {
        return new QStudyItem(
            row.CStudyRowId,
            row.CStudyRowName,
            row.CStudyRowActive.ToString(CultureInfo.CurrentCulture),
            false,
            row.CStudyRowChosen);
    }

    private void QStudyShuffleRefine(CSittingState? state)
    {
        QStudyShuffle.IsEnabled = state is not null;
        if (state is null)
        {
            QStudyShuffle.ClearValue(QLook.QLookCueProperty);
            return;
        }

        QStudyShuffle.SetValue(
            QLook.QLookCueProperty,
            state.CSittingStateShuffled ? QLookCue.QLookCueSelected : QLookCue.QLookCueIdle);
    }

    private void QStudyItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QStudyItem deck)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PStudyRow") is Button row)
        {
            if (deck.QStudyItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QStudyRowObserve;
            if (!deck.QStudyItemFolder)
            {
                row.Click += QStudyRowObserve;
            }
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PStudyIcon") is QIconImage icon)
        {
            if (deck.QStudyItemFolder)
            {
                icon.QIconSource = QIcon.QIconResolve("folder", 16);
            }
            else
            {
                icon.ClearValue(QIconImage.QIconSourceProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PStudyText") is TextBlock text)
        {
            text.Text = deck.QStudyItemText;
        }

        if (QLook.QLookPartFind<Border>(container, "PStudyPellet") is Border pellet)
        {
            pellet.Visibility = QLook.QLookVisibleRead(!deck.QStudyItemFolder);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PStudyTally") is TextBlock tally)
        {
            tally.Text = deck.QStudyItemTally;
        }
    }

    private void QStudyRowObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QStudyItem>(sender) is QStudyItem deck)
        {
            _cStudy.CStudySelect(deck.QStudyItemId);
        }
    }

    private void QStudyLaunchObserve(object sender, RoutedEventArgs e)
    {
        _cStudy.CStudyStart();
        _qSitting.QSittingFocusRefine();
    }

    private void QStudyShuffleObserve(object sender, RoutedEventArgs e)
    {
        if (_cStudy.CStudySitting is CSitting sitting)
        {
            CSittingState state = sitting.CSittingShuffleToggle();
            QStudyShuffleRefine(state);
            _qSitting.QSittingRefine(state);
        }

        _qSitting.QSittingFocusRefine();
    }
}
