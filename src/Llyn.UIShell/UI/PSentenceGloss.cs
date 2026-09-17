using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PSentence
{
    public ObservableCollection<PGloss> PSentenceGloss { get; } = [];

    internal static string PSentenceGlossFormat(PGloss gloss, string field)
    {
        return string.Concat(field, ":", gloss.PGlossId.ToString(CultureInfo.InvariantCulture));
    }

    internal PGloss? PSentenceGlossFind(string field, out string name)
    {
        int split = field.IndexOf(':', StringComparison.Ordinal);
        name = split < 0 ? field : field[..split];
        if (split < 0
            || !long.TryParse(field[(split + 1)..], NumberStyles.Integer, CultureInfo.InvariantCulture, out long id))
        {
            return null;
        }

        foreach (PGloss gloss in PSentenceGloss)
        {
            if (gloss.PGlossId == id)
            {
                return gloss;
            }
        }

        return null;
    }

    private void PSentenceGlossShow(IReadOnlyList<LGlossDraft> drafts)
    {
        PCard.PCardRowShow(
            PSentenceGloss,
            drafts,
            static row => row.PGlossId,
            static draft => draft.LGlossDraftId,
            PSentenceGlossCreate,
            (row, draft) =>
            {
                row.PGlossShow(draft);
                return row;
            });
    }

    private PGloss PSentenceGlossCreate(LGlossDraft draft)
    {
        PGloss row = new(PSentenceLanguageCatalog, draft);
        row.PropertyChanged += PSentenceGlossChange;
        return row;
    }

    private void PSentenceGlossChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PGloss gloss && arguments.PropertyName == nameof(PGloss.PGlossLanguage))
        {
            PSentenceRaise(PSentenceGlossFormat(gloss, arguments.PropertyName));
        }
    }
}
