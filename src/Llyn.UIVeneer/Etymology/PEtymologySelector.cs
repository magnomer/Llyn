using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIVeneer;

internal sealed class PEtymologySelector : DataTemplateSelector
{
    public DataTemplate? PEtymologySelectorChip { get; set; }

    public DataTemplate? PEtymologySelectorCaret { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PEtymologyCaret ? PEtymologySelectorCaret : PEtymologySelectorChip;
    }
}
