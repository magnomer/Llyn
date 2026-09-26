using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class PRegisterSelector : DataTemplateSelector
{
    public DataTemplate? PRegisterSelectorChip { get; set; }

    public DataTemplate? PRegisterSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PRegisterCaret ? PRegisterSelectorCaret : PRegisterSelectorChip;
    }
}
