using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PFieldConverter : IMultiValueConverter
{
    private const double PFieldConverterHeight = 31;
    private const double PFieldConverterSide = 9;

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [FontFamily pFamily, double pSize, Thickness pPadding])
        {
            return new Thickness(0);
        }

        var pLine = pFamily.LineSpacing * pSize;
        var pRoom = (PFieldConverterHeight - pLine - pPadding.Top - pPadding.Bottom) / 2;
        var pVertical = Math.Max(0, pRoom);
        var pLeft = Math.Max(0, PFieldConverterSide - pPadding.Left);
        var pRight = Math.Max(0, PFieldConverterSide - pPadding.Right);
        return new Thickness(-pLeft, -pVertical, -pRight, -pVertical);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
