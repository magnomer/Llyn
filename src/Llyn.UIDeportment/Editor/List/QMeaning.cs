using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QMeaning
{
    private CEditor _cEditor = null!;

    internal QMeaning(FrameworkElement surface, ObservableCollection<PCard> cards, QCard card)
    {
        ItemsControl list = QContract.QContractFind<ItemsControl>(surface, "PMeaningList");
        list.ItemsSource = cards;
        QLookItem.QLookItemAttach(list, card.QCardApply);
        QContract.QContractFind<Button>(surface, "PMeaningAddition").Click += QMeaningAddObserve;
    }

    internal void QMeaningIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    private void QMeaningAddObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorList.CCardMeaningAdd();
    }
}
