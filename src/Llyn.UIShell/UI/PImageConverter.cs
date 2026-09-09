using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<PImage> shown = [];
        if (value is not IEnumerable<LImageDraft> rows)
        {
            return shown;
        }

        foreach (LImageDraft row in rows)
        {
            if (row.LImageDraftEmpty)
            {
                continue;
            }

            shown.Add(new PImage(row));
        }

        return shown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
