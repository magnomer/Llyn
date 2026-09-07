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
        if (value is not IEnumerable<LStateValue> locations)
        {
            return shown;
        }

        foreach (LStateValue location in locations)
        {
            if (location.LStateValueEmpty)
            {
                continue;
            }

            shown.Add(new PImage(location));
        }

        return shown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
