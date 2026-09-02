using System.Collections.ObjectModel;
using System.Windows;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PCard> _pSenseList = [];
    private readonly ObservableCollection<PCard> _pCollocationList = [];

    private void PSenseHandle(object sender, RoutedEventArgs e)
    {
        _pSenseList.Add(new PCard("Meaning", _pSenseList.Count + 1, _pSentenceReference));
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationList.Add(new PCard("Collocation", _pCollocationList.Count + 1, _pSentenceReference));
    }

    internal void PCardHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCard card })
        {
            return;
        }

        ObservableCollection<PCard>? list = PCardListFind(card);

        if (list is null || list.Count <= 1)
        {
            return;
        }

        list.Remove(card);
        PCardOrderUpdate(list);
    }

    private static void PCardOrderUpdate(ObservableCollection<PCard> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            list[index].PCardOrder = index + 1;
        }
    }

    private ObservableCollection<PCard>? PCardListFind(PCard card)
    {
        return _pSenseList.Contains(card) ? _pSenseList
            : _pCollocationList.Contains(card) ? _pCollocationList
            : null;
    }
}
