using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PCitationConverter : IValueConverter
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

    internal void PCitationConverterClear()
    {
        _pCitationConverterLines.Clear();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not LStateAnchor anchor)
        {
            return string.Empty;
        }

        long id = anchor.LStateAnchorShow();
        if (id == 0)
        {
            return string.Empty;
        }

        return _pCitationConverterLines.TryGetValue(id, out string? line)
            ? line
            : id.ToString(CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
