using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PCitationConverter : IValueConverter, IMultiValueConverter
{
    private readonly Dictionary<long, string> _pCitationConverterLines = [];

    internal void PCitationConverterShow(IReadOnlyDictionary<long, string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        _pCitationConverterLines.Clear();
        foreach ((long id, string line) in lines)
        {
            _pCitationConverterLines[id] = line;
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LStateAnchor anchor)
        {
            return string.Empty;
        }

        if (anchor.LStateAnchorShown is not long id)
        {
            return string.Empty;
        }

        return _pCitationConverterLines.GetValueOrDefault(id) ?? id.ToString(CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        return values.Length > 1
            && values[0] is LStateAnchor anchor
            && values[1] is ObservableCollection<PCitationItem> catalog
            ? PSentence.PSentenceCitationFind(catalog, anchor)
            : string.Empty;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
