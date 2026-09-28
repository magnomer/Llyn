using System;
using System.Globalization;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PSentenceConverter : IMultiValueConverter
{
    private const string PSentenceConverterHead = "Head";

    private const string PSentenceConverterText = "Text";

    private CSentenceOrder? _pSentenceConverterOrder;

    internal void PSentenceConverterApply(CSentenceOrder order)
    {
        _pSentenceConverterOrder = order;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        string mark = values.Length > 3 && values[3] is string marked ? marked : string.Empty;
        string part = parameter as string ?? string.Empty;
        return CCard.CCardOrderRead(
            _pSentenceConverterOrder,
            part == PSentenceConverterText ? CStateValue.CStateValueEmpty : PSentenceConverterRead(values, 0),
            part == PSentenceConverterText ? CStateValue.CStateValueEmpty : PSentenceConverterRead(values, 1),
            part == PSentenceConverterHead ? CStateValue.CStateValueEmpty : PSentenceConverterRead(values, 2),
            mark);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static CStateValue PSentenceConverterRead(object[] values, int place)
    {
        return values.Length > place && values[place] is CStateValue state ? state : CStateValue.CStateValueEmpty;
    }
}
