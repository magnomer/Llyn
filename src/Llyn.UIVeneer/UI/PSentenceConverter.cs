using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PSentenceConverter : IMultiValueConverter
{
    private const string PSentenceConverterHead = "Head";

    private const string PSentenceConverterText = "Text";

    private LSentenceOrder _pSentenceConverterOrder = LSentenceOrder.LSentenceOrderDefault;

    internal void PSentenceConverterApply(LSentenceOrder order)
    {
        _pSentenceConverterOrder = order ?? LSentenceOrder.LSentenceOrderDefault;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 3 && values[3] is string marked ? marked : string.Empty;
        string part = parameter as string ?? string.Empty;
        if (part == PSentenceConverterText)
        {
            return PSentenceConverterFormat(values, 2, mark);
        }

        if (part == PSentenceConverterHead)
        {
            return _pSentenceConverterOrder.LSentenceOrderFormat(
                PSentenceConverterFormat(values, 0, mark), PSentenceConverterFormat(values, 1, mark));
        }

        return _pSentenceConverterOrder.LSentenceOrderFormat(
            PSentenceConverterFormat(values, 0, mark),
            PSentenceConverterFormat(values, 1, mark),
            PSentenceConverterFormat(values, 2, mark));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string PSentenceConverterFormat(object[] values, int place, string mark)
    {
        if (values.Length <= place)
        {
            return string.Empty;
        }

        if (values[place] is not LStateValue state)
        {
            return string.Empty;
        }

        return state.LStateValueLegible ? state.LStateValueShow() : state.LStateValueUncertain ? mark : string.Empty;
    }
}
