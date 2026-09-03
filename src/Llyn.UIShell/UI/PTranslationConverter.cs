using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PTranslationConverter : IValueConverter
{
    private readonly Dictionary<string, PTranslationChip> _pTranslationConverterTargets =
        new(StringComparer.Ordinal);

    internal void PTranslationConverterShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        _pTranslationConverterTargets.Clear();
        foreach (LTranslationTarget target in targets)
        {
            _pTranslationConverterTargets[target.LTranslationTargetId] = new PTranslationChip(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage);
        }
    }

    internal void PTranslationConverterClear()
    {
        _pTranslationConverterTargets.Clear();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<PTranslationChip> chips = [];
        if (value is not IEnumerable<string> ids)
        {
            return chips;
        }

        foreach (string id in ids)
        {
            if (_pTranslationConverterTargets.TryGetValue(id, out PTranslationChip? chip))
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
