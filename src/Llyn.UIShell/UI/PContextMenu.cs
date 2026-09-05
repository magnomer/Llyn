using System.Windows;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PContextAddHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext row })
        {
            PCardContextFind(row)?.PCardContextInsert(row);
        }
    }

    internal void PContextRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext row })
        {
            PCardContextFind(row)?.PCardContextRemove(row);
        }
    }

    private PCard? PCardContextFind(PContext row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardContext.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
