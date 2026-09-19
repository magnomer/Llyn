using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIVeneer;

internal sealed class PLinkSelector : DataTemplateSelector
{
    public DataTemplate? PLinkSelectorChip { get; set; }

    public DataTemplate? PLinkSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PLinkCaret ? PLinkSelectorCaret : PLinkSelectorChip;
    }
}
