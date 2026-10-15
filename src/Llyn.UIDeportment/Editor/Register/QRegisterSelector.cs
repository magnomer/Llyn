using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QRegisterSelector : DataTemplateSelector
{
    public DataTemplate? QRegisterSelectorChip { get; set; }

    public DataTemplate? QRegisterSelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is QInlet<QRegisterChip> ? QRegisterSelectorCaret : QRegisterSelectorChip;
    }
}
