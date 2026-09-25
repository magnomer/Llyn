using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PLinkConverter : IValueConverter
{
    private readonly Dictionary<long, PLinkChip> _pLinkConverterTargets = [];

    internal void PLinkConverterShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        _pLinkConverterTargets.Clear();
        foreach (LTranslationTarget target in targets)
        {
            _pLinkConverterTargets[target.LTranslationTargetId] = new PLinkChip(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage);
        }
    }

    internal void PLinkConverterClear()
    {
        _pLinkConverterTargets.Clear();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<PLinkChip> chips = [];
        if (value is not IEnumerable<long> ids)
        {
            return chips;
        }

        foreach (long id in ids)
        {
            if (_pLinkConverterTargets.TryGetValue(id, out PLinkChip? chip))
            {
                chips.Add(chip);
            }
        }

        return chips;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
