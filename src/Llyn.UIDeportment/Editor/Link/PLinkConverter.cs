using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PLinkConverter : IValueConverter
{
    private readonly Dictionary<long, LLinkChip> _pLinkConverterTargets = [];

    internal void PLinkConverterShow(IReadOnlyList<CTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        _pLinkConverterTargets.Clear();
        foreach (CTranslationTarget target in targets)
        {
            _pLinkConverterTargets[target.CTranslationTargetId] = new LLinkChip(
                target.CTranslationTargetId,
                target.CTranslationTargetHeadword,
                target.CTranslationTargetLanguage);
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
