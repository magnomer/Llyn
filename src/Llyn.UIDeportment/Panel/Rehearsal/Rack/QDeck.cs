using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDeck
{
    private readonly UserControl _qDeckSurface;

    private readonly ObservableCollection<QDeckItem> _qDeckList = [];

    private readonly QDeckName _qDeckName = new();

    private CDeck _cDeck = null!;

    private QDeckMenu _qDeckMenu = null!;

    internal QDeck(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDeckSurface = surface;
        QDeckFreshIcon.QIconSource = QIcon.QIconResolve("new", 24);
        QDeckFolderIcon.QIconSource = QIcon.QIconResolve("folder", 24);
        QDeckFresh.Click += QDeckFreshObserve;
        QDeckFolder.Click += QDeckFolderObserve;
        QDeckKind.SelectionChanged += QDeckKindObserve;
        QDeckAccession.Click += QDeckViewObserve;
        QDeckStencil.Click += QDeckViewObserve;
        QDeckCharter.Click += QDeckViewObserve;
    }

    private Button QDeckFresh => QContract.QContractFind<Button>(_qDeckSurface, "PDeckFresh");

    private Button QDeckFolder => QContract.QContractFind<Button>(_qDeckSurface, "PDeckFolder");

    private QIconImage QDeckFreshIcon => QContract.QContractFind<QIconImage>(_qDeckSurface, "PDeckFreshIcon");

    private QIconImage QDeckFolderIcon => QContract.QContractFind<QIconImage>(_qDeckSurface, "PDeckFolderIcon");

    private ComboBox QDeckKind => QContract.QContractFind<ComboBox>(_qDeckSurface, "PDeckKind");

    private ItemsControl QDeckShelf => QContract.QContractFind<ItemsControl>(_qDeckSurface, "PDeckShelf");

    private TextBlock QDeckEmpty => QContract.QContractFind<TextBlock>(_qDeckSurface, "PDeckEmpty");

    private TextBlock QDeckLine => QContract.QContractFind<TextBlock>(_qDeckSurface, "PDeckLine");

    private Button QDeckAccession => QContract.QContractFind<Button>(_qDeckSurface, "PDeckAccession");

    private Button QDeckStencil => QContract.QContractFind<Button>(_qDeckSurface, "PDeckStencil");

    private Button QDeckCharter => QContract.QContractFind<Button>(_qDeckSurface, "PDeckCharter");

    private (CDeckView QDeckView, Button QDeckTab, Grid QDeckSheet)[] QDeckViews =>
    [
        (CDeckView.CDeckViewAccession,
            QDeckAccession,
            QContract.QContractFind<Grid>(_qDeckSurface, "PDeckAccessionSheet")),
        (CDeckView.CDeckViewStencil, QDeckStencil, QContract.QContractFind<Grid>(_qDeckSurface, "PDeckStencilSheet")),
        (CDeckView.CDeckViewCharter, QDeckCharter, QContract.QContractFind<Grid>(_qDeckSurface, "PDeckCharterSheet")),
    ];

    internal void QDeckIntroduce(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cDeck = new CDeck(atelier, envoy);
        CDepot depot = new(atelier);
        _qDeckMenu = new QDeckMenu(_cDeck, depot, envoy, _qDeckName);
        _cDeck.CDeckChanged += QDeckRefine;
        _cDeck.CDeckChanged += QDeckViewRefine;
        depot.CDepotChanged += QDeckLineRefine;
        QDeckShelf.ItemsSource = _qDeckList;
        QLookItem.QLookItemAttach(QDeckShelf, QDeckItemRefine);
        QCue cue = new(_qDeckSurface);
        cue.QCueIntroduce(_cDeck.CDeckCue);
        _cDeck.CDeckChanged += cue.QCueRefine;
        _cDeck.CDeckAccession.CAccessionChanged += cue.QCueRefine;
        QJotter cell = new(
            QContract.QContractFind<UserControl>(_qDeckSurface, "PJotter"),
            QContract.QContractFind<ListBox>(_qDeckSurface, "PCue"));
        cell.QJotterIntroduce(_cDeck.CDeckCue);
        _cDeck.CDeckChanged += cell.QJotterRefine;
        QCharter charter = new(QContract.QContractFind<UserControl>(_qDeckSurface, "PCharter"));
        charter.QCharterIntroduce(_cDeck.CDeckCharter);
        _cDeck.CDeckChanged += charter.QCharterRefine;
        QStencil stencil = new(QContract.QContractFind<UserControl>(_qDeckSurface, "PStencil"));
        stencil.QStencilIntroduce(_cDeck.CDeckStencil);
        _cDeck.CDeckChanged += stencil.QStencilRefine;
        QAccession accession = new(QContract.QContractFind<UserControl>(_qDeckSurface, "PAccession"));
        accession.QAccessionIntroduce(_cDeck.CDeckAccession);
        _cDeck.CDeckChanged += accession.QAccessionRefine;
        _cDeck.CDeckChanged += accession.QAccessionTallyRefine;
        QDeckViewRefine();
    }

    private void QDeckViewRefine()
    {
        foreach ((CDeckView view, Button tab, Grid sheet) in QDeckViews)
        {
            bool shown = _cDeck.CDeckView == view;
            tab.SetValue(QLook.QLookCueProperty, shown ? QLookCue.QLookCueSelected : QLookCue.QLookCueIdle);
            sheet.Visibility = QLook.QLookVisibleRead(shown);
        }
    }

    private void QDeckRefine()
    {
        CDeckRoll roll = _cDeck.CDeckRead();
        _qDeckList.Clear();
        foreach (CDeckRow row in roll.CDeckRollDecks)
        {
            if (row.CDeckRowFolder is null)
            {
                _qDeckList.Add(new QDeckItem(row.CDeckRowId, row.CDeckRowName, false, row.CDeckRowChosen));
            }
        }

        foreach (CDeckFolder folder in roll.CDeckRollFolders)
        {
            _qDeckList.Add(new QDeckItem(folder.CDeckFolderId, folder.CDeckFolderName, true, false));
            foreach (CDeckRow row in roll.CDeckRollDecks)
            {
                if (row.CDeckRowFolder == folder.CDeckFolderId)
                {
                    _qDeckList.Add(new QDeckItem(row.CDeckRowId, row.CDeckRowName, false, row.CDeckRowChosen));
                }
            }
        }

        QDeckEmpty.Visibility = QLook.QLookVisibleRead(roll.CDeckRollEmpty);
        QDeckKindRefine(roll.CDeckRollGrains);
    }

    private void QDeckKindRefine(IReadOnlyList<CDeckGrain> grains)
    {
        QDeckKind.SelectionChanged -= QDeckKindObserve;
        QDeckKind.Items.Clear();
        foreach (CDeckGrain grain in grains)
        {
            ComboBoxItem choice = new();
            choice.SetResourceReference(ContentControl.ContentProperty, grain.CDeckGrainKey);
            QDeckKind.Items.Add(choice);
            choice.IsSelected = grain.CDeckGrainChosen;
        }

        QDeckKind.SelectionChanged += QDeckKindObserve;
    }

    private void QDeckLineRefine(CDepotState state)
    {
        QDeckLine.Text = state.CDepotStateLine;
        _qDeckMenu.QDeckMenuBusy = state.CDepotStateBusy;
    }

    private void QDeckItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QDeckItem deck)
        {
            return;
        }

        _qDeckMenu.QDeckMenuRefine(container);

        if (QLook.QLookPartFind<Button>(container, "PDeckRow") is Button row)
        {
            if (deck.QDeckItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QDeckRowObserve;
            if (!deck.QDeckItemFolder)
            {
                row.Click += QDeckRowObserve;
            }
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PDeckIcon") is QIconImage icon)
        {
            if (deck.QDeckItemFolder)
            {
                icon.QIconSource = QIcon.QIconResolve("folder", 16);
            }
            else
            {
                icon.ClearValue(QIconImage.QIconSourceProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PDeckText") is TextBlock text)
        {
            text.Text = deck.QDeckItemText;
            text.Visibility = Visibility.Visible;
        }

        _qDeckName.QDeckNameRefine(container);

        if (QLook.QLookPartFind<QIconImage>(container, "PDeckEarlierIcon") is QIconImage earlier)
        {
            earlier.QIconSource = QIcon.QIconResolve("earlier", 16);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PDeckLaterIcon") is QIconImage later)
        {
            later.QIconSource = QIcon.QIconResolve("later", 16);
        }

        QDeckHandleRefine(container, "PDeckEarlier", deck, QDeckEarlierObserve);
        QDeckHandleRefine(container, "PDeckLater", deck, QDeckLaterObserve);
    }

    private static void QDeckHandleRefine(
        FrameworkElement container, string part, QDeckItem deck, RoutedEventHandler observe)
    {
        if (QLook.QLookPartFind<Button>(container, part) is Button handle)
        {
            handle.Visibility = QLook.QLookVisibleRead(!deck.QDeckItemFolder);
            handle.Click -= observe;
            handle.Click += observe;
        }
    }

    private void QDeckRowObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem deck)
        {
            _cDeck.CDeckSelect(deck.QDeckItemId);
        }
    }

    private void QDeckEarlierObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem deck)
        {
            _cDeck.CDeckOrderMove(deck.QDeckItemId, -1);
        }
    }

    private void QDeckLaterObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem deck)
        {
            _cDeck.CDeckOrderMove(deck.QDeckItemId, 1);
        }
    }

    private void QDeckFreshObserve(object sender, RoutedEventArgs e)
    {
        _cDeck.CDeckCreate(QSender.QSenderItemRead<QDeckItem>(sender)?.QDeckItemId);
    }

    private void QDeckFolderObserve(object sender, RoutedEventArgs e)
    {
        _cDeck.CDeckFolderCreate();
    }

    private void QDeckViewObserve(object sender, RoutedEventArgs e)
    {
        foreach ((CDeckView view, Button tab, Grid _) in QDeckViews)
        {
            if (tab == sender)
            {
                _cDeck.CDeckViewSet((int)view);
            }
        }
    }

    private void QDeckKindObserve(object sender, SelectionChangedEventArgs e)
    {
        _cDeck.CDeckGrainSelect(QDeckKind.SelectedIndex);
    }
}
