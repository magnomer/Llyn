using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class QStateConverter : IMultiValueConverter, IValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 1 && values[1] is string marked ? marked : string.Empty;
        bool placeholder = values.Length > 2;
        string hint = placeholder && values[2] is string hinted ? hinted : string.Empty;

        return values.FirstOrDefault() is LStateValue state ? state.LStateValueFormat(mark, hint, placeholder) : hint;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is LStateValue state ? state.LStateValueShow() : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
