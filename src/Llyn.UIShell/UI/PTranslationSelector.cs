using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

internal sealed class PTranslationSelector : DataTemplateSelector
{
    public DataTemplate? PTranslationSelectorChip { get; set; }

    public DataTemplate? PTranslationSelectorEntry { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is PTranslationEntry ? PTranslationSelectorEntry : PTranslationSelectorChip;
    }
}
