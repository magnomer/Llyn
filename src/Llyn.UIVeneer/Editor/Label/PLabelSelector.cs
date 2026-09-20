using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIVeneer;

internal sealed class PLabelSelector : DataTemplateSelector
{
    public DataTemplate? PLabelSelectorChip { get; set; }

    public DataTemplate? PLabelSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PLabelCaret ? PLabelSelectorCaret : PLabelSelectorChip;
    }
}
