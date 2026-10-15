using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGlossConverter : IValueConverter
{
    private static readonly ObservableCollection<QLanguageItem> QGlossConverterCatalog = [];

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return QGlossConverterCreate(value as IEnumerable<CGlossDraft>);
    }

    internal static IReadOnlyList<QRendition> QGlossConverterCreate(IEnumerable<CGlossDraft>? rows)
    {
        List<QRendition> shown = [];
        if (rows is null)
        {
            return shown;
        }

        foreach (CGlossDraft row in rows)
        {
            shown.Add(new QRendition(QGlossConverterCatalog, row));
        }

        return shown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
