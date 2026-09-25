using Llyn.UIDeportment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PLinkConverter : IValueConverter
{
    private readonly Dictionary<long, LLinkChip> _pLinkConverterTargets = [];

    internal void PLinkConverterShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        _pLinkConverterTargets.Clear();
        foreach (LTranslationTarget target in targets)
        {
            _pLinkConverterTargets[target.LTranslationTargetId] = new LLinkChip(
                target.LTranslationTargetId,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage);
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        List<LLinkChip> chips = [];
        if (value is not IEnumerable<long> ids)
        {
            return chips;
        }

        foreach (long id in ids)
        {
            if (_pLinkConverterTargets.TryGetValue(id, out LLinkChip? chip))
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
