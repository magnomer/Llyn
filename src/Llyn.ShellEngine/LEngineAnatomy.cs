using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LReflexDraft LEngineAnatomyResolve(string language, LReflexDraft row)
    {
        IReadOnlyList<LAnatomyRule> rules = language.Trim().Length == 0
            ? []
            : LEngineLanguageLoad(language).LLanguageAnatomies;
        LAnatomy anatomy = rules.Count == 0
            ? LAnatomy.LAnatomyEmpty
            : LAnatomy.LAnatomyScan(
                rules, row.LReflexDraftLanguage, row.LReflexDraftText, row.LReflexDraftRespelling);
        return row with { LReflexDraftAnatomy = anatomy };
    }

    private IReadOnlyList<LReflexDraft> LEngineAnatomyScan(string language, IReadOnlyList<LReflexDraft> rows)
    {
        List<LReflexDraft> filled = new(rows.Count);
        foreach (LReflexDraft row in rows)
        {
            filled.Add(LEngineAnatomyResolve(language, row));
        }

        return filled;
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

    private LEntryDraft LEngineAnatomyRebuild(LEntryDraft content)
    {
        return content with
        {
            LEntryDraftReflexes = LEngineAnatomyScan(content.LEntryDraftLanguage, content.LEntryDraftReflexes),
        };
    }
}
