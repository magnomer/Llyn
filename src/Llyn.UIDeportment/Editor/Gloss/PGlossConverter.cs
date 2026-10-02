using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PGlossConverter : IValueConverter
{
    private static readonly ObservableCollection<PLanguageItem> PGlossConverterCatalog = [];

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return PGlossConverterCreate(value as IEnumerable<CGlossDraft>);
    }

    internal static IReadOnlyList<PGloss> PGlossConverterCreate(IEnumerable<CGlossDraft>? rows)
    {
        List<PGloss> shown = [];
        if (rows is null)
        {
            return shown;
        }

        foreach (CGlossDraft row in rows)
        {
            shown.Add(new PGloss(PGlossConverterCatalog, row));
        }

        return shown;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
