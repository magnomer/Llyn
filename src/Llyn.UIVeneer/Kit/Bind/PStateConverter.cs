using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIVeneer;

public sealed class PStateConverter : IMultiValueConverter, IValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 1 && values[1] is string marked ? marked : string.Empty;
        bool placeholder = values.Length > 2;
        string hint = placeholder && values[2] is string hinted ? hinted : string.Empty;

        if (values.Length == 0)
        {
            return hint;
        }

        if (values[0] is not LStateValue state)
        {
            return hint;
        }

        return state.LStateValueLegible
            ? placeholder ? hint : state.LStateValueShow()
            : state.LStateValueUncertain ? mark : hint;
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
