using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Llyn.UIDeportment;

internal sealed class QCitationConverter : IValueConverter
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
        return value is long id && _qCitationConverterLines.TryGetValue(id, out string? line) ? line : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
