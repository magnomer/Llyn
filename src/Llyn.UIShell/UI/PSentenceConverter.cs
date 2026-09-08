using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

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
        string particle = PSentenceConverterFormat(values, 0, mark);
        string dependence = PSentenceConverterFormat(values, 1, mark);
        string text = PSentenceConverterFormat(values, 2, mark);

        string first = _pSentenceConverterOrder.LSentenceOrderParticle == 0 ? particle : dependence;
        string second = _pSentenceConverterOrder.LSentenceOrderParticle == 0 ? dependence : particle;
        string frame = first.Length == 0 || second.Length == 0
            ? first + second
            : first + " " + second;

        string part = parameter as string ?? string.Empty;

        if (part == PSentenceConverterText)
        {
            return text;
        }

        string head = frame.Length == 0 ? string.Empty : "(+" + frame + ")";

        if (part == PSentenceConverterHead)
        {
            return head;
        }

        return head.Length == 0 || text.Length == 0 ? head + text : head + " " + text;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string PSentenceConverterFormat(object[] values, int place, string mark)
    {
        if (values.Length <= place || values[place] is not LStateValue state)
        {
            return string.Empty;
        }

        return state.LStateValueState switch
        {
            LState.LStateSpecified => state.LStateValueShow(),
            LState.LStateUnknown => mark,
            _ => string.Empty,
        };
    }
}
