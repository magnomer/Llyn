using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PStateConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 1 && values[1] is string marked ? marked : string.Empty;

        if (values.Length > 0 && values[0] is LStateValue state)
        {
            return state.LStateValueState switch
            {
                LState.LStateSpecified => state.LStateValueShow(),
                LState.LStateUnknown => mark,
                _ => string.Empty,
            };
        }

        if (values.Length > 0 && values[0] is true)
        {
            return mark;
        }

        return values.Length > 2 && values[2] is string hint ? hint : string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
