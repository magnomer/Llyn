using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public static class CRespelling
{
    internal static IReadOnlyList<CReflex> LRespellingReflexScan(
        LReflexPort port, string language, IReadOnlyList<LReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(port);
        ArgumentNullException.ThrowIfNull(reflexes);

        IReadOnlyList<LReflexGuise> guises = port.LEngineGuiseRead(language, reflexes);
        return LRespellingReflexScan(CReflexDraft.CReflexDraftRead(reflexes), guises);
    }

    internal static IReadOnlyList<CReflex> LRespellingReflexScan(
        IReadOnlyList<CReflexDraft> reflexes, IReadOnlyList<LReflexGuise> guises)
    {
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(guises);

        IReadOnlyList<bool> leads =
            CReflex.LReflexLeadRead(reflexes.Select(static reflex => reflex.CReflexDraftLanguage).ToList());
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
            guise.LReflexGuiseShown,
            reflex.CReflexDraftRomanization,
            reflex.CReflexDraftMeaning,
            reflex.CReflexDraftNote,
            reflex.CReflexDraftMain,
            reflex.CReflexDraftRegion.Length > 0 ? reflex.CReflexDraftRegion : null,
            reflex.CReflexDraftAnchors,
            mark,
            guise.LReflexGuiseFolded,
            lead);
    }
}
