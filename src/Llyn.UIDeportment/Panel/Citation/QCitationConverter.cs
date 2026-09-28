using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Llyn.UIDeportment;

internal sealed class QCitationConverter : IValueConverter, IMultiValueConverter
{
    private readonly Dictionary<long, string> _qCitationConverterLines = [];

    internal void QCitationConverterShow(IReadOnlyDictionary<long, string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        _qCitationConverterLines.Clear();
        foreach ((long id, string line) in lines)
        {
            _qCitationConverterLines[id] = line;
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not long id)
        {
            return string.Empty;
        }

        return _qCitationConverterLines.GetValueOrDefault(id) ?? id.ToString(CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.ElementAtOrDefault(1) is not ObservableCollection<QCitationItem> catalog)
        {
            return string.Empty;
        }

        return PSentence.PSentenceCitationFind(catalog, values.ElementAtOrDefault(0) as long?);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
