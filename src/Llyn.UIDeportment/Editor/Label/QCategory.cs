using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCategory
{
    private readonly FrameworkElement _qCategorySurface;

    private readonly QMarker _qCategoryMarker;

    private readonly ObservableCollection<PCategoryItem> _qCategoryItem = [];

    private CEditor _cEditor = null!;

    internal QCategory(FrameworkElement surface, QMarker marker)
    {
        _qCategorySurface = surface;
        _qCategoryMarker = marker;
        QCategoryList.ItemsSource = _qCategoryItem;
        QLookItem.QLookItemAttach(QCategoryList, QCategoryApply);
        QChoice.QChoiceDropperAttach(
            QContract.QContractFind<ToggleButton>(_qCategorySurface, "PMarkerSwitch"),
            QCategoryPopup,
            QContract.QContractFind<Border>(_qCategorySurface, "PMarkerSurface"));
    }

    private Popup QCategoryPopup => QContract.QContractFind<Popup>(_qCategorySurface, "PCategory");

    private TextBlock QCategoryNotice => QContract.QContractFind<TextBlock>(_qCategorySurface, "PCategoryNotice");

    private ItemsControl QCategoryList => QContract.QContractFind<ItemsControl>(_qCategorySurface, "PCategoryList");

    internal void QCategoryIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    internal void QCategoryRefine(CCategory category)
    {
        _qCategoryItem.Clear();
        foreach (CCategoryRow row in category.CCategoryRows)
        {
            _qCategoryItem.Add(new PCategoryItem(row.CCategoryRowName, row.CCategoryRowTaken));
        }

        if (category.CCategoryHint is string hint)
        {
            QCategoryNotice.SetResourceReference(TextBlock.TextProperty, hint);
            QCategoryNotice.Visibility = Visibility.Visible;
            return;
        }

        QCategoryNotice.Visibility = Visibility.Collapsed;
    }

    private void QCategoryApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PCategoryItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCategoryName") is TextBlock name)
        {
            name.Text = row.PCategoryItemName;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PCategoryIcon") is QIconImage icon)
        {
            icon.QIconSource = QIcon.QIconResolve("check", 12);
            icon.Visibility = QLook.QLookVisibleRead(row.PCategoryItemTaken);
        }

        if (QLook.QLookPartFind<Button>(container, "PCategoryChoice") is Button choice)
        {
            choice.Click -= QCategoryObserve;
            choice.Click += QCategoryObserve;
        }
    }

    private void QCategoryObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCategoryItem item })
        {
            _cEditor.CEditorSpeech.CCardSpeechAdd(item.PCategoryItemName);
            QCategoryPickRefine();
        }
    }

    private void QCategoryPickRefine()
    {
        _qCategoryMarker.QMarkerFieldRefine();
        QContract.QContractFind<ToggleButton>(_qCategorySurface, "PMarkerSwitch").IsChecked = false;
    }
}
