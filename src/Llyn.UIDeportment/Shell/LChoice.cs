using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

public static class LChoice
{
    public static LCatalogFilter LChoiceFilterRead(Panel list)
    {
        ArgumentNullException.ThrowIfNull(list);

        List<string> hidden = [];
        foreach (object child in list.Children)
        {
            if (child is CheckBox { IsChecked: not true, Tag: string language })
            {
                hidden.Add(language);
            }
        }

        return hidden.Count == 0 ? LCatalogFilter.LCatalogFilterEmpty : new LCatalogFilter(hidden);
    }

    public static LCatalogOrder? LChoiceOrderRead(object sender)
    {
        return (sender as FrameworkElement)?.Tag switch
        {
            LCatalogOrder order => order,
            _ => null,
        };
    }
}
