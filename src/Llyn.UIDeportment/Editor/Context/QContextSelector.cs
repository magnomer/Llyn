using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QContextSelector : DataTemplateSelector
{
    public DataTemplate? QContextSelectorChip { get; set; }

    public DataTemplate? QContextSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is QInlet<QContextChip> ? QContextSelectorCaret : QContextSelectorChip;
    }
}
