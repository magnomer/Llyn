using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PStateConverter : IMultiValueConverter, IValueConverter
{
    public static bool PStateConverterCheck(LState state)
    {
        return state == LState.LStateUnknown;
    }

    public static bool PStateConverterCheck(LStateValue? value)
    {
        return value is not null && PStateConverterCheck(value.LStateValueState);
    }

    public static bool PStateConverterCheck(LStateMark? mark)
    {
        return mark is not null && PStateConverterCheck(mark.LStateMarkState);
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 1 && values[1] is string marked ? marked : string.Empty;
        bool placeholder = values.Length > 2;
        string hint = placeholder && values[2] is string hinted ? hinted : string.Empty;

        if (values.Length > 0 && values[0] is LStateValue state)
        {
            return state.LStateValueState switch
            {
                _ when state.LStateValueUnreadable => placeholder ? hint : state.LStateValueShow(),
                LState.LStateSpecified => placeholder ? hint : state.LStateValueShow(),
                LState.LStateUnknown => mark,
                _ => hint,
            };
        }

        return hint;
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
