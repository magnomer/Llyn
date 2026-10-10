using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEntry
{
    private readonly CDesk _cEntryDesk;

    private readonly LDraftPort _cEntryDraftPort;

    private readonly LMediaPort _cEntryMediaPort;

    private readonly LCardPort _cEntryCardPort;

    private readonly LSettingsPort _cEntrySettings;

    private readonly CEnvoy _cEntryEnvoy;

    private readonly CLedgerNoticed _cEntryNoticed;

    internal CEntry(
        CDesk desk,
        LDraftPort drafts,
        LCardPort cards,
        LMediaPort media,
        LSettingsPort settings,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cEntryDesk = desk;
        _cEntryDraftPort = drafts;
        _cEntryCardPort = cards;
        _cEntryMediaPort = media;
        _cEntrySettings = settings;
        _cEntryEnvoy = envoy;
        _cEntryNoticed = noticed;
        _cEntryDesk.CDeskDraft.CDeskDraftPrepared += draft =>
        {
            if (_cEntryDesk.CDeskDraft.CDeskDraftTenure is LTenure held)
            {
                CEntryDraftChanged?.Invoke(LEntryDraftRead(held, draft));
            }
        };
    }

    public event Action<CEntryDraft>? CEntryDraftChanged;

    public string CEntryLanguage => _cEntryDesk.CDeskDraft.CDeskDraftTenure?.LTenureLanguageRead() ?? string.Empty;

    private LTenure? CEntryTenure =>
        _cEntryDesk.CDeskDraft.CDeskDraftFilling ? null : _cEntryDesk.CDeskDraft.CDeskDraftTenure;

    internal void LEntryObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        _cEntryDesk.CDeskVigil.LVigilEntryAttach(CSubject.CSubjectFold, _ => marshal(LEntryDraftShow));
    }

    internal CEntryDraft LEntryDraftRead(LTenure held, LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(draft);

        return CFolio.CFolioEntryRead(
            draft.LDraftContent,
            new LQuillChip(held, _cEntryDraftPort).LQuillTranslationRead(draft.LDraftContent),
            LEntryFoldRead(),
            _cEntryMediaPort);
    }

    private void LEntryDraftShow()
    {
        if (_cEntryDesk.CDeskDraft.CDeskDraftTenure is LTenure held && held.LTenureRead() is LDraft draft)
        {
            CEntryDraftChanged?.Invoke(LEntryDraftRead(held, draft));
        }
    }

    private IReadOnlySet<long> LEntryFoldRead()
    {
        if (_cEntryDesk.CDeskStoredRead() is not long id)
        {
            return new HashSet<long>();
        }

        try
        {
            return _cEntryCardPort.LEngineFoldRead(id);
        }
        catch (Exception exception)
        {
            _cEntryNoticed.LLedgerRepaintShow(_cEntryEnvoy, _cEntrySettings, "Fold.ReadFailed", exception);
            return new HashSet<long>();
        }
    }

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
