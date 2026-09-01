using System.Collections.ObjectModel;
using System.Windows;

namespace Llyn.UIShell;

/// <summary>
/// Which cards the input editor holds: the sense list and the collocation list, the buttons that add
/// and remove a card, and the numbering that keeps each list reading 1, 2, 3 after every change. The
/// order the cards are in is the order they are saved in, so it is kept here rather than derived.
/// </summary>
public partial class PInput
{
    private readonly ObservableCollection<PInputCard> _pSenseList = [];
    private readonly ObservableCollection<PInputCard> _pCollocationList = [];

    private void PSenseHandle(object sender, RoutedEventArgs e)
    {
        _pSenseList.Add(new PInputCard("Sense", _pSenseList.Count + 1));
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationList.Add(new PInputCard("Collocation", _pCollocationList.Count + 1));
    }

    // Removing the last card would leave the tab with nothing to type into, so a list of one keeps it.
    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PInputCard card })
        {
            return;
        }

        ObservableCollection<PInputCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        list.Remove(card);
        PInputOrderUpdate(list);
    }

    private static void PInputOrderUpdate(ObservableCollection<PInputCard> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            list[index].PInputCardOrder = index + 1;
        }
    }

    // Which of the two lists a card belongs to, since both are drawn from the same template.
    private ObservableCollection<PInputCard>? PCardListFind(PInputCard card)
    {
        return _pSenseList.Contains(card) ? _pSenseList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
