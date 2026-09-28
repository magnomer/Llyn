using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed class CRespelling
{
    private readonly CAtelier _cRespellingAtelier;

    internal CRespelling(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cRespellingAtelier = atelier;
    }

    public CRespellingMark CRespellingMarkRead(string language)
    {
        return CRespellingMarkRead(language, false);
    }

    public CRespellingMark CRespellingMarkRead(string language, bool schemed)
    {
        if (schemed)
        {
            return new CRespellingMark(false, string.Empty, string.Empty);
        }

        (bool shown, string opener, string closer) =
            _cRespellingAtelier.CAtelierPhonologyPort.LEngineMarkRead(language);
        return new CRespellingMark(shown, opener, closer);
    }

    public IReadOnlyList<CReflex> CRespellingReflexScan(string language, IReadOnlyList<CReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        IReadOnlyList<string> languages = reflexes.Select(static reflex => reflex.CReflexDraftLanguage).ToList();
        IReadOnlyList<LReflexGuise> guises =
            _cRespellingAtelier.CAtelierPhonologyPort.LEngineGuiseRead(language, languages);
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
            CRespellingResolve(mark, reflex.CReflexDraftText, reflex.CReflexDraftRespelling),
            reflex.CReflexDraftRomanization,
            reflex.CReflexDraftMeaning,
            reflex.CReflexDraftNote,
            reflex.CReflexDraftMain,
            reflex.CReflexDraftRegion,
            reflex.CReflexDraftAnchors,
            reflex.CReflexDraftTone,
            mark,
            guise.LReflexGuiseFolded,
            lead);
    }

    public static CAccent CRespellingAccentRead(CRespellingMark mark, string language, CPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return new CAccent(
            spoken.CPronunciationDraftId,
            CSounding.CSoundingVarietyRead(language, spoken.CPronunciationDraftVariety),
            CRespellingResolve(mark, spoken),
            spoken.CPronunciationDraftAudio);
    }

    public static string CRespellingResolve(CRespellingMark mark, CPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return CRespellingResolve(mark, spoken.CPronunciationDraftIpa, spoken.CPronunciationDraftRespelling);
    }

    public static string CRespellingResolve(CRespellingMark mark, string phonetic, string? respelling)
    {
        ArgumentNullException.ThrowIfNull(mark);

        return mark.CRespellingMarkShown && !string.IsNullOrEmpty(respelling) ? respelling : phonetic;
    }
}
