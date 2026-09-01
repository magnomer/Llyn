using System;
using System.Globalization;
using System.Windows.Data;

namespace Llyn.UIShell;

/// <summary>Turns a card container's zero-based position into the one-based number shown in its header.</summary>
public sealed class PCardOrderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is int index ? index + 1 : 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
