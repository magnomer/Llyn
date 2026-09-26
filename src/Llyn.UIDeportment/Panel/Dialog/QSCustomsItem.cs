using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.UIDeportment;

internal sealed class QSCustomsItem
{
    internal QSCustomsItem(int index, string headword, string language, IReadOnlyList<long> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        QSCustomsItemIndex = index;
        QSCustomsItemHeadword = headword;
        QSCustomsItemLanguage = language;
        QSCustomsItemCandidate = candidates;
    }

    public int QSCustomsItemIndex { get; }

    public string QSCustomsItemNumber => (QSCustomsItemIndex + 1).ToString(CultureInfo.CurrentCulture);

    public string QSCustomsItemHeadword { get; }

    public string QSCustomsItemLanguage { get; }

    public IReadOnlyList<long> QSCustomsItemCandidate { get; }
}
