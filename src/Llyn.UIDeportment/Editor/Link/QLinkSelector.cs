using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QLinkSelector : DataTemplateSelector
{
    public DataTemplate? QLinkSelectorChip { get; set; }

    public DataTemplate? QLinkSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is QInlet<QLinkChip> ? QLinkSelectorCaret : QLinkSelectorChip;
    }
}
