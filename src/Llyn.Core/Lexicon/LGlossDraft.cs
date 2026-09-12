using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LGlossDraft(
    long LGlossDraftId,
    string LGlossDraftLanguage,
    LStateValue LGlossDraftText)
{
    public string LGlossDraftLanguage { get; init; } = LGlossDraftLanguage ?? string.Empty;

    public LStateValue LGlossDraftText { get; init; } = LGlossDraftText ?? LStateValue.LStateValueUnspecified;

    public static LGlossDraft LGlossDraftCreate(LGloss gloss)
    {
        ArgumentNullException.ThrowIfNull(gloss);
        return new LGlossDraft(gloss.LGlossId, gloss.LGlossLanguage, gloss.LGlossText);
    }

    public LGloss LGlossDraftResolve()
    {
        return new LGloss(LGlossDraftId, LGlossDraftLanguage, LGlossDraftText);
    }

    public LGlossDraft LGlossDraftNormalize()
    {
        return this with { LGlossDraftText = LGlossDraftText.LStateValueNormalize() };
    }

    public static IReadOnlyList<LGlossDraft> LGlossDraftNormalize(IReadOnlyList<LGlossDraft> glosses)
    {
        ArgumentNullException.ThrowIfNull(glosses);

        List<LGlossDraft> written = new(glosses.Count);
        foreach (LGlossDraft gloss in glosses)
        {
            written.Add(gloss.LGlossDraftNormalize());
        }

        return written;
    }
}
