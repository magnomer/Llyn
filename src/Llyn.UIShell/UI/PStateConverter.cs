using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PStateConverter : IValueConverter, IMultiValueConverter
{
    private const string PStateConverterKey = "Display.Unreadable";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LStateValue state)
        {
            return string.Empty;
        }

        return state.LStateValueState switch
        {
            LState.LStateSpecified => state.LStateValueShow(),
            LState.LStateUnknown => PStateTextRead(),
            _ => string.Empty,
        };
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length > 0 && values[0] is true)
        {
            return PStateTextRead();
        }

        return values.Length > 1 && values[1] is string hint ? hint : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string PStateTextRead()
    {
        return System.Windows.Application.Current?.TryFindResource(PStateConverterKey) as string ?? "Unreadable";
    }
}
