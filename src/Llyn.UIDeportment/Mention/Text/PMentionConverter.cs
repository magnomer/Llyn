using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PMentionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyList<LMentionDraft> drafts)
        {
            return Array.Empty<CMention>();
        }

        List<LMention> mentions = new(drafts.Count);
        foreach (LMentionDraft draft in drafts)
        {
            mentions.Add(draft.LMentionDraftResolve());
        }

        return LWindow.LWindowMentionRead(mentions);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
