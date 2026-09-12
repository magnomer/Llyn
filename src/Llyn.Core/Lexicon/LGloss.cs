using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LGloss(
    long LGlossId,
    string LGlossLanguage,
    LStateValue LGlossText)
{
    public string LGlossLanguage { get; init; } = LGlossLanguage ?? string.Empty;

    public LStateValue LGlossText { get; init; } = LGlossText ?? LStateValue.LStateValueUnspecified;

    public LGloss LGlossNormalize()
    {
        return this with { LGlossText = LGlossText.LStateValueNormalize() };
    }

    public static IReadOnlyList<LGloss> LGlossNormalize(IReadOnlyList<LGloss> glosses)
    {
        ArgumentNullException.ThrowIfNull(glosses);

        List<LGloss> written = new(glosses.Count);
        foreach (LGloss gloss in glosses)
        {
            written.Add(gloss.LGlossNormalize());
        }

        return written;
    }
}
