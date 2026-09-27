using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QStateConverter : IMultiValueConverter, IValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 1 && values[1] is string marked ? marked : string.Empty;
        bool placeholder = values.Length > 2;
        string hint = placeholder && values[2] is string hinted ? hinted : string.Empty;

        return values.FirstOrDefault() is CStateValue state ? QStateFormat(state, mark, hint, placeholder) : hint;
    }

    private static string QStateFormat(CStateValue state, string mark, string hint, bool placeholder)
    {
        if (state.CStateValueLegible)
        {
            return placeholder ? hint : state.CStateValueText;
        }

        return state.CStateValueUncertain ? mark : hint;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is CStateValue state ? state.CStateValueText : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
