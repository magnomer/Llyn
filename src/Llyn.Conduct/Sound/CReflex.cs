using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

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

    internal string LReflexFieldRead(CReflexField field)
    {
        return field switch
        {
            CReflexField.CReflexFieldLanguage => CReflexLanguage,
            CReflexField.CReflexFieldKind => CReflexKind,
            CReflexField.CReflexFieldText => CReflexText,
            CReflexField.CReflexFieldRomanization => CReflexRomanization,
            CReflexField.CReflexFieldMeaning => CReflexMeaning,
            CReflexField.CReflexFieldNote => CReflexNote,
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null),
        };
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

    internal static CLecternAnchor LReflexAnchorRead(
        CEnvoy envoy,
        LSettingsPort settings,
        CLedgerNoticed noticed,
        LDraftPort drafts,
        long? entry,
        string headword,
        IReadOnlyList<CReflex> rows)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(noticed);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(rows);

        if (entry is not long id)
        {
            return new CLecternAnchor(false, new Dictionary<long, string>());
        }

        try
        {
            Dictionary<long, string> texts = [];
            foreach (CReflex reflex in rows)
            {
                texts[reflex.CReflexId] = drafts.LEngineAnchorFormat(
                    id, reflex.CReflexAnchors, headword, LReflexSeparator);
            }

            return new CLecternAnchor(drafts.LEngineAnchorCheck(id, headword), texts);
        }
        catch (Exception exception)
        {
            noticed.LLedgerRepaintShow(envoy, settings, "Display.AnchorFailed", exception);
            return new CLecternAnchor(false, new Dictionary<long, string>());
        }
    }

    private static string LReflexKeyRead(string name)
    {
        return string.Concat("Reflex.", name);
    }
}
