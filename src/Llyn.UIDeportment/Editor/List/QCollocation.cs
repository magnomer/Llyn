using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCollocation
{
    private CEditor _cEditor = null!;

    internal QCollocation(FrameworkElement surface, ObservableCollection<PCard> cards, QCard card)
    {
        ItemsControl list = QContract.QContractFind<ItemsControl>(surface, "PCollocationList");
        list.ItemsSource = cards;
        QLookItem.QLookItemAttach(list, card.QCardApply);
        QContract.QContractFind<Button>(surface, "PCollocationAddition").Click += QCollocationAddObserve;
    }

    internal void QCollocationIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    private void QCollocationAddObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorList.CCardCollocationAdd();
    }
}
