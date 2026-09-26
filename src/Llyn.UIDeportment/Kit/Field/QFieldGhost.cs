using System;
using System.Globalization;
using System.Windows.Data;

namespace Llyn.UIDeportment;

internal sealed class QFieldGhost : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values is [string { Length: > 0 } text, _] ? text : values is [_, string hint] ? hint : string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
