using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEntry
{
    private readonly CDesk _cEntryDesk;

    private readonly LDraftPort _cEntryDraftPort;

    private readonly LMediaPort _cEntryMediaPort;

    internal CEntry(CDesk desk, LDraftPort drafts, LMediaPort media)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(media);

        _cEntryDesk = desk;
        _cEntryDraftPort = drafts;
        _cEntryMediaPort = media;
        _cEntryDesk.CDeskDraft.CDeskDraftPrepared += draft =>
        {
            if (_cEntryDesk.CDeskDraft.CDeskDraftTenure is LTenure held)
            {
                CEntryDraftChanged?.Invoke(CFolio.CFolioEntryRead(
                    draft.LDraftContent,
                    new LQuillChip(held, _cEntryDraftPort).LQuillTranslationRead(draft.LDraftContent),
                    _cEntryMediaPort));
            }
        };
    }

    public event Action<CEntryDraft>? CEntryDraftChanged;

    public string CEntryLanguage => _cEntryDesk.CDeskDraft.CDeskDraftTenure?.LTenureLanguageRead() ?? string.Empty;

    private LTenure? CEntryTenure =>
        _cEntryDesk.CDeskDraft.CDeskDraftFilling ? null : _cEntryDesk.CDeskDraft.CDeskDraftTenure;

    public string CEntryPronunciationRead()
    {
        return _cEntryDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillPronunciation(held).LQuillPronunciationRead()
            : string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> CEntryEtymonRead()
    {
        return _cEntryDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? CFolio.CFolioTargetRead(new LQuillEtymology(held).LQuillEtymonRead())
            : [];
    }

    public void CEntryHeadwordSet(string text)
    {
        if (CEntryTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillHeadwordSet(text);
        }
    }

    public void CEntryPronunciationSet(string text)
    {
        if (CEntryTenure is LTenure held)
        {
            new LQuillPronunciation(held).LQuillPronunciationSet(text);
        }
    }

    public void CEntryNoteSet(string text)
    {
        if (CEntryTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillNoteSet(text);
        }
    }

    public static bool CEntryNoteCheck(string text, string note) => LQuillEntry.LQuillNoteCheck(text, note);

    public void CEntryLanguageSet(string language)
    {
        if (CEntryTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillLanguageSet(language);
        }
    }

    public void CEntryUnitSet(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (CEntryTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillUnitSet(CCardSpeech.LUnitRowParse(key));
        }
    }
}
