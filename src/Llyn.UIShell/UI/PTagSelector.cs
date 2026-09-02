using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

internal sealed class PTagSelector : DataTemplateSelector
{
    public DataTemplate? PTagSelectorChip { get; set; }

    public DataTemplate? PTagSelectorEntry { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PTagEntry ? PTagSelectorEntry : PTagSelectorChip;
    }
}
