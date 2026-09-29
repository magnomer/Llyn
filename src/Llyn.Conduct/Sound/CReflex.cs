using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CReflex(
    long CReflexId,
    string CReflexLanguage,
    string CReflexKind,
    string CReflexText,
    string CReflexRomanization,
    string CReflexMeaning,
    string CReflexNote,
    bool CReflexMain,
    string CReflexRegion,
    IReadOnlyList<long> CReflexAnchors,
    CRespellingMark CReflexMark,
    bool CReflexFolded,
    bool CReflexLead)
{
    internal const string LReflexSeparator = " · ";

    public string CReflexLanguageKey => LReflexKeyRead(CReflexLanguage);

    public string CReflexKindKey => LReflexKeyRead(CReflexKind);

    public bool CReflexHiddenCheck(bool opened)
    {
        return CReflexFolded && !opened;
    }

    internal static IReadOnlyList<bool> LReflexLeadRead(IReadOnlyList<string> languages)
    {
        ArgumentNullException.ThrowIfNull(languages);

        List<bool> leads = new(languages.Count);
        string? held = null;
        foreach (string language in languages)
        {
            leads.Add(!string.Equals(held, language, StringComparison.Ordinal));
            held = language;
        }

        return leads;
    }

    private static string LReflexKeyRead(string name)
    {
        return string.Concat("Reflex.", name);
    }
}
