using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PVideoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<PVideo> shown = [];
        if (value is not IEnumerable<LVideoDraft> rows)
        {
            return shown;
        }

        foreach (LVideoDraft row in rows)
        {
            if (row.LVideoDraftEmpty)
            {
                continue;
            }

            shown.Add(new PVideo(row));
        }

        return shown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
