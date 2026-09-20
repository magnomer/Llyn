using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? [] : LEngineLanguageLoad(language).LLanguageAnatomyTones;
    }

    private static IReadOnlyList<LReflex> LEngineAnatomyClear(IReadOnlyList<LReflex> rows)
    {
        List<LReflex> bare = new(rows.Count);
        foreach (LReflex row in rows)
        {
            bare.Add(row with { LReflexAnatomy = LAnatomy.LAnatomyEmpty });
        }

        return bare;
    }
}
