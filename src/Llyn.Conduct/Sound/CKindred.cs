using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CKindred
{
    private readonly CDesk _cKindredDesk;

    private readonly LReflexPort _cKindredReflexPort;

    private readonly LDisplay _cKindredDisplay;

    private readonly LDraftPort _cKindredDraftPort;

    private readonly LSettingsPort _cKindredSettingsPort;

    private readonly CEnvoy _cKindredEnvoy;

    internal CKindred(
        CDesk desk,
        LReflexPort reflexes,
        LDisplay display,
        LDraftPort drafts,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cKindredDesk = desk;
        _cKindredReflexPort = reflexes;
        _cKindredDisplay = display;
        _cKindredDraftPort = drafts;
        _cKindredSettingsPort = settings;
        _cKindredEnvoy = envoy;
        desk.CDeskDraft.CDeskDraftPrepared += LKindredStart;
    }

    public event Action? CKindredChanged;

    public bool CKindredPending => _cKindredDisplay.LDisplaySound.LDisplayReflexCheck(LKindredEntry);

    public CTimbreReflex CKindredRead()
    {
        bool opened = _cKindredDisplay.LDisplaySound.LDisplayFoldOpened;
        if (_cKindredDesk.CDeskDraft.CDeskDraftTenure is not LTenure held || held.LTenureRead() is not { } draft)
        {
            return new CTimbreReflex(
                false, [], new CLecternAnchor(false, new Dictionary<long, string>()), opened, false);
        }

        LEntryDraft content = draft.LDraftContent;
        IReadOnlyList<CReflex> rows = LKindredRowRead(content);
        return new CTimbreReflex(
            new LQuillReflex(held, _cKindredReflexPort).LQuillReflexCheck(),
            rows,
            CReflex.LReflexAnchorRead(
                _cKindredEnvoy,
                _cKindredSettingsPort,
                _cKindredDisplay.LDisplayNoticed,
                _cKindredDraftPort,
                LKindredEntry,
                content.LEntryDraftHeadword,
                rows),
            opened,
            CKindredPending);
    }

    private void LKindredStart(LDraft _)
    {
        if (_cKindredDesk.CDeskDraft.CDeskDraftTenure is LTenure held)
        {
            new LQuillReflex(held, _cKindredReflexPort).LQuillReflexStart();
        }
    }

    public void CKindredRebuild()
    {
        _cKindredDisplay.LDisplaySound.LDisplayReflexRebuild(LKindredEntry);
    }

    public void CKindredAdd(long? reflex)
    {
        LKindredQuill?.LQuillReflexAdd(reflex ?? 0);
    }

    public void CKindredRemove(long reflex)
    {
        LKindredQuill?.LQuillReflexRemove(reflex);
    }

    public void CKindredToggle(long reflex)
    {
        LKindredQuill?.LQuillReflexToggle(reflex);
    }

    public CReflexTyped CKindredSet(long reflex, CReflexField field, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CReflex? held = LKindredFind(reflex);
        if (LKindredQuill is not LQuillReflex quill)
        {
            return new CReflexTyped(held, []);
        }

        switch (field)
        {
            case CReflexField.CReflexFieldLanguage:
                return new CReflexTyped(
                    held?.CReflexTypedApply(field, text), LKindredLeadRead(quill.LQuillLanguageSet(reflex, text)));
            case CReflexField.CReflexFieldKind:
                quill.LQuillKindSet(reflex, text);
                break;
            case CReflexField.CReflexFieldText:
                quill.LQuillTextSet(reflex, text);
                break;
            case CReflexField.CReflexFieldRomanization:
                quill.LQuillRomanizationSet(reflex, text);
                break;
            case CReflexField.CReflexFieldMeaning:
                quill.LQuillMeaningSet(reflex, text);
                break;
            case CReflexField.CReflexFieldNote:
                quill.LQuillNoteSet(reflex, text);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), field, null);
        }

        return new CReflexTyped(held?.CReflexTypedApply(field, text), []);
    }

    private CReflex? LKindredFind(long reflex)
    {
        if (_cKindredDesk.CDeskDraft.CDeskDraftTenure is not LTenure held || held.LTenureRead() is not { } draft)
        {
            return null;
        }

        return LKindredRowRead(draft.LDraftContent).FirstOrDefault(row => row.CReflexId == reflex);
    }

    private IReadOnlyList<CReflex> LKindredRowRead(LEntryDraft content)
    {
        return CRespelling.LRespellingReflexScan(
            _cKindredReflexPort,
            content.LEntryDraftLanguage,
            CReflexDraft.CReflexDraftRead(content.LEntryDraftReflexes));
    }

    private static IReadOnlyList<CReflexHead> LKindredLeadRead(IReadOnlyList<LReflexDraft> typed)
    {
        IReadOnlyList<CReflexDraft> rows = CReflexDraft.CReflexDraftRead(typed);
        IReadOnlyList<bool> leads =
            CReflex.LReflexLeadRead(rows.Select(static row => row.CReflexDraftLanguage).ToList());
        return rows.Select((row, index) => new CReflexHead(row.CReflexDraftId, leads[index])).ToList();
    }

    internal void LKindredObserverAttach(Action<Action> marshal)
    {
        _cKindredDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectReflex, _ => marshal(() => CKindredChanged?.Invoke()));
    }

    private long? LKindredEntry => _cKindredDesk.CDeskStoredRead();

    private LTenure? LKindredTenure =>
        _cKindredDesk.CDeskDraft.CDeskDraftFilling ? null : _cKindredDesk.CDeskDraft.CDeskDraftTenure;

    internal LQuillReflex? LKindredQuill =>
        LKindredTenure is LTenure held ? new LQuillReflex(held, _cKindredReflexPort) : null;
}
