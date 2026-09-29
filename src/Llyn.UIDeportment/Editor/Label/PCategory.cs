using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PCategoryTemplate _pCategoryTemplate;

    private Popup PCategory => (Popup)FindName(nameof(PCategory));

    private TextBlock PCategoryNotice => (TextBlock)FindName(nameof(PCategoryNotice));

    private ItemsControl PCategoryList => (ItemsControl)FindName(nameof(PCategoryList));

    private void PCategoryAttach()
    {
        PCategoryList.ItemsSource = _pCategoryItem;
        QLookItem.QLookItemAttach(PCategoryList, PCategoryApply);
        QChoice.QChoiceDropperAttach(PMarkerSwitch, PCategory, PMarkerSurface);
    }

    private void PCategoryApply(FrameworkElement container, object item, string? _)
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
            choice.Click -= PCategoryObserve;
            choice.Click += PCategoryObserve;
        }
    }

    private readonly ObservableCollection<PCategoryItem> _pCategoryItem = [];

    private void PCategoryRefine(CCategory category)
    {
        _pCategoryItem.Clear();
        foreach (CCategoryRow row in category.CCategoryRows)
        {
            _pCategoryItem.Add(new PCategoryItem(row.CCategoryRowName, row.CCategoryRowTaken));
        }

        if (category.CCategoryHint is string hint)
        {
            PCategoryNotice.SetResourceReference(TextBlock.TextProperty, hint);
            PCategoryNotice.Visibility = Visibility.Visible;
            return;
        }

        PCategoryNotice.Visibility = Visibility.Collapsed;
    }

    private void PCategoryObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCategoryItem item })
        {
            _qEditor.QEditorArea.CEditorSpeech.CCardSpeechAdd(item.PCategoryItemName);
            PCategoryPickRefine();
        }
    }

    private void PCategoryPickRefine()
    {
        PMarkerFieldRefine();
        PMarkerSwitch.IsChecked = false;
    }
}
