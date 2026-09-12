using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PGlossConverter : IValueConverter
{
    private static readonly ObservableCollection<PLanguageItem> PGlossConverterCatalog = [];

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<PGloss> shown = [];
        if (value is not IEnumerable<LGlossDraft> rows)
        {
            return shown;
        }

        foreach (LGlossDraft row in rows)
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
