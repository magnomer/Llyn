using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Llyn.UIVeneer;

internal sealed class PFontConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [FontFamily pLead, double pLeadSize, FontFamily pText, double pTextSize])
        {
            return new Thickness(0);
        }

        var pDrop = (pLead.Baseline * pLeadSize) - (pText.Baseline * pTextSize);
        return new Thickness(0, Math.Max(0, pDrop), 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
