using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDeckMenu
{
    private readonly CDeck _cDeck;

    private readonly CDepot _cDepot;

    private readonly CEnvoy _cEnvoy;

    private readonly QDeckName _qDeckName;

    internal bool QDeckMenuBusy { get; set; }

    internal QDeckMenu(CDeck deck, CDepot depot, CEnvoy envoy, QDeckName name)
    {
        ArgumentNullException.ThrowIfNull(deck);
        ArgumentNullException.ThrowIfNull(depot);
        ArgumentNullException.ThrowIfNull(name);

        _cDeck = deck;
        _cDepot = depot;
        _cEnvoy = envoy;
        _qDeckName = name;
    }

    internal void QDeckMenuRefine(FrameworkElement container)
    {
        ContextMenu menu = new();
        menu.Opened += QDeckMenuBuild;
        container.ContextMenu = menu;
    }

    private void QDeckMenuBuild(object sender, RoutedEventArgs e)
    {
        if (sender is not ContextMenu { PlacementTarget: FrameworkElement { DataContext: QDeckItem deck } } menu)
        {
            return;
        }

        menu.Items.Clear();
        if (deck.QDeckItemFolder)
        {
            menu.Items.Add(QDeckOptionBuild("Deck.New", QDeckFreshObserve));
            menu.Items.Add(QDeckOptionBuild("Deck.FolderNameSet", QDeckTitleObserve));
            menu.Items.Add(QDeckOptionBuild("Deck.FolderDelete", QDeckRemoveObserve));
            return;
        }

        menu.Items.Add(QDeckOptionBuild("Deck.NameSet", QDeckRenameObserve));
        MenuItem move = new();
        move.SetResourceReference(HeaderedItemsControl.HeaderProperty, "Deck.Move");
        foreach (CDeckDestination destination in _cDeck.CDeckDestinationRead(deck.QDeckItemId))
        {
            MenuItem target = new() { DataContext = new QDeckDestination(destination) };
            if (destination.CDeckDestinationKey is string key)
            {
                target.SetResourceReference(HeaderedItemsControl.HeaderProperty, key);
            }
            else
            {
                target.Header = destination.CDeckDestinationName;
            }

            target.Click += QDeckMoveObserve;
            move.Items.Add(target);
        }

        move.IsEnabled = move.Items.Count > 0;
        menu.Items.Add(move);
        MenuItem export = QDeckOptionBuild("Deck.Export", QDeckExportObserve);
        export.IsEnabled = !QDeckMenuBusy;
        menu.Items.Add(export);
        menu.Items.Add(QDeckOptionBuild("Deck.Delete", QDeckDeleteObserve));
    }

    private static MenuItem QDeckOptionBuild(string key, RoutedEventHandler observe)
    {
        MenuItem item = new();
        item.SetResourceReference(HeaderedItemsControl.HeaderProperty, key);
        item.Click += observe;
        return item;
    }

    private void QDeckFreshObserve(object sender, RoutedEventArgs e)
    {
        _cDeck.CDeckCreate(QSender.QSenderItemRead<QDeckItem>(sender)?.QDeckItemId);
    }

    private void QDeckTitleObserve(object sender, RoutedEventArgs e)
    {
        _qDeckName.QDeckNameOpen(sender, _cDeck.CDeckFolderUpdate);
    }

    private void QDeckRenameObserve(object sender, RoutedEventArgs e)
    {
        _qDeckName.QDeckNameOpen(sender, _cDeck.CDeckNameSet);
    }

    private void QDeckMoveObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckDestination>(sender) is QDeckDestination destination)
        {
            _cDeck.CDeckMove(destination.QDeckDestinationDeck);
        }
    }

    private async void QDeckExportObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem deck)
        {
            await _cDepot.CDepotExport(deck.QDeckItemId, _cEnvoy);
        }
    }

    private void QDeckDeleteObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem deck)
        {
            _cDeck.CDeckDelete(deck.QDeckItemId);
        }
    }

    private void QDeckRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<QDeckItem>(sender) is QDeckItem folder)
        {
            _cDeck.CDeckFolderDelete(folder.QDeckItemId);
        }
    }
}
