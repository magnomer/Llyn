using System;

namespace Llyn.Conduct;

public sealed class CRespelling
{
    private readonly CAtelier _cRespellingAtelier;

    internal CRespelling(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cRespellingAtelier = atelier;
    }

    public bool CRespellingCheck(string language)
    {
        return _cRespellingAtelier.CAtelierPhonologyPort.LEngineRespellingCheck(language);
    }

    public bool CRespellingPhonemicCheck(string language)
    {
        return _cRespellingAtelier.CAtelierPhonologyPort.LEnginePhonemicCheck(language);
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

        bool shown = CRespellingCheck(language);
        bool slashed = shown && CRespellingPhonemicCheck(language);
        return new CRespellingMark(shown, slashed ? "/" : "[", slashed ? "/" : "]");
    }

    public CRespellingMark CRespellingReflexRead(string language)
    {
        bool phonemic = CRespellingPhonemicCheck(language);
        string slash = phonemic ? "/" : string.Empty;
        return new CRespellingMark(CRespellingCheck(language), slash, slash);
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
