using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QFieldConverter : IMultiValueConverter
{
    private const double QFieldConverterHeight = 31;
    private const double QFieldConverterSide = 9;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [FontFamily pFamily, double pSize, Thickness pPadding])
        {
            return new Thickness(0);
        }

        var pLine = pFamily.LineSpacing * pSize;
        var pRoom = (QFieldConverterHeight - pLine - pPadding.Top - pPadding.Bottom) / 2;
        var pVertical = Math.Max(0, pRoom);
        var pLeft = Math.Max(0, QFieldConverterSide - pPadding.Left);
        var pRight = Math.Max(0, QFieldConverterSide - pPadding.Right);
        return new Thickness(-pLeft, -pVertical, -pRight, -pVertical);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
