using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QLabelSelector : DataTemplateSelector
{
    public DataTemplate? QLabelSelectorChip { get; set; }

    public DataTemplate? QLabelSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is QInlet<QLabelChip> ? QLabelSelectorCaret : QLabelSelectorChip;
    }
}
