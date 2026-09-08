using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

internal sealed class PContextSelector : DataTemplateSelector
{
    public DataTemplate? PContextSelectorChip { get; set; }

    public DataTemplate? PContextSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PContextCaret ? PContextSelectorCaret : PContextSelectorChip;
    }
}
