using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public static class CRespelling
{
    internal static IReadOnlyList<CReflex> LRespellingReflexScan(
        LPhonologyPort phonology, string language, IReadOnlyList<CReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(reflexes);

        IReadOnlyList<string> languages = reflexes.Select(static reflex => reflex.CReflexDraftLanguage).ToList();
        IReadOnlyList<LReflexGuise> guises = phonology.LEngineGuiseRead(language, languages);
        IReadOnlyList<bool> leads = CReflex.LReflexLeadRead(languages);
        return reflexes.Select((reflex, index) => LRespellingReflexRead(reflex, guises[index], leads[index])).ToList();
    }

    private static CReflex LRespellingReflexRead(CReflexDraft reflex, LReflexGuise guise, bool lead)
    {
        string slash = guise.LReflexGuisePhonemic ? "/" : string.Empty;
        CRespellingMark mark = new(guise.LReflexGuiseRespelled, slash, slash);
        return new CReflex(
            reflex.CReflexDraftId,
            reflex.CReflexDraftLanguage,
            reflex.CReflexDraftKind,
            LRespellingResolve(mark, reflex.CReflexDraftText, reflex.CReflexDraftRespelling),
            reflex.CReflexDraftRomanization,
            reflex.CReflexDraftMeaning,
            reflex.CReflexDraftNote,
            reflex.CReflexDraftMain,
            reflex.CReflexDraftRegion,
            reflex.CReflexDraftAnchors,
            mark,
            guise.LReflexGuiseFolded,
            lead);
    }

    internal static string LRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)
    {
        ArgumentNullException.ThrowIfNull(mark);

        return mark.CRespellingMarkShown && !string.IsNullOrEmpty(respelling) ? respelling : phonetic;
    }
}
