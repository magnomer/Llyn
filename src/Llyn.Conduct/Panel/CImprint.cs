using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CImprint
{
    private readonly LEntryPort _cImprintEntryPort;

    private int _cImprintBlankAt = -1;

    private int _cImprintCount;

    internal CImprint(
        LDraftPort drafts,
        LEntryPort entries,
        LSettingsPort settings,
        CEnvoy envoy,
        CLedgerNoticed noticed,
        Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cImprintEntryPort = entries;
        CImprintDesk = new CDesk(drafts, settings, "Source", envoy);
        CImprintByline = new CByline(this, drafts, noticed);
        CImprintDesk.CDeskDraftPrepared += LImprintDraftShow;
        CImprintDesk.CDeskObserverAttach(marshal);
    }

    public event Action? CImprintChanged;

    public event Action<CReference>? CImprintReferenceChanged;

    public event Action? CImprintFocused;

    public event Action? CImprintReverted;

    public CDesk CImprintDesk { get; }

    public CByline CImprintByline { get; }

    public bool CImprintHeld => CImprintDesk.CDeskHeld;

    internal void LImprintOpen(long? id)
    {
        _cImprintBlankAt = -1;
        CImprintByline.LBylineReset();
        CImprintDesk.CDeskStart(id);
    }

    internal void LImprintCancel()
    {
        CImprintDesk.CDeskCancel();
        CImprintChanged?.Invoke();
        CImprintByline.CBylineClose();
    }

    internal void LImprintSave()
    {
        if (!CImprintDesk.LDeskChangeCheck())
        {
            return;
        }

        CImprintDesk.CDeskFinish(true);
    }

    public CReference CImprintEmptyRead()
    {
        return LImprintReferenceRead(_cImprintEntryPort.LEngineImprintRead(null));
    }

    public string CImprintTallyRead()
    {
        return _cImprintEntryPort.LEngineTallyRead(CImprintDesk.CDeskStoredRead());
    }

    public void CImprintTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CImprintDesk.CDeskQuill?.LQuillTitleSet(text);
    }

    public void CImprintYearSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CImprintDesk.CDeskQuill?.LQuillYearSet(text);
    }

    public void CImprintUrlSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CImprintDesk.CDeskQuill?.LQuillUrlSet(text);
    }

    public void CImprintNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CImprintDesk.CDeskQuill?.LQuillNoteSet(text);
    }

    public void CImprintKindSet(string? tag)
    {
        CImprintDesk.CDeskQuill?.LQuillKindSet(tag);
    }

    public static IReadOnlyList<CReferenceKind> CImprintKindRead()
    {
        return LEntryPort.LEngineKindRead()
            .Select(static kind => new CReferenceKind(kind.LReferenceKindTag, kind.LReferenceKindKey))
            .DistinctBy(static kind => kind.CReferenceKindTag, StringComparer.Ordinal)
            .ToList();
    }

    public IReadOnlyList<CAuthorRow> CImprintCreditRead()
    {
        List<CAuthorRow> rows = _cImprintEntryPort.LEngineCreditRead(CImprintDesk.CDeskTenure)
            .Select(static row => new CAuthorRow(
                row.LAuthorRowId,
                row.LAuthorRowName,
                row.LAuthorRowPosition,
                row.LAuthorRowEarlier,
                row.LAuthorRowLater,
                false))
            .ToList();
        _cImprintCount = rows.Count;
        if (CImprintHeld)
        {
            LImprintBlankSet();
        }

        if (_cImprintBlankAt >= 0 && _cImprintBlankAt <= rows.Count)
        {
            rows.Insert(_cImprintBlankAt, new CAuthorRow(0, string.Empty, _cImprintBlankAt, false, false, true));
        }

        return rows;
    }

    public void CImprintAuthorAdd(int? position, long? id)
    {
        if (!CImprintHeld)
        {
            return;
        }

        if (position is not int at || id is not long author)
        {
            return;
        }

        if (author == 0 && at != _cImprintBlankAt)
        {
            return;
        }

        if (author != 0)
        {
            _cImprintBlankAt = at + 1;
            CImprintChanged?.Invoke();
        }

        CImprintFocused?.Invoke();
    }

    public void CImprintAuthorRemove(long? id)
    {
        if (!CImprintHeld)
        {
            return;
        }

        if (id is not long author)
        {
            return;
        }

        CImprintByline.CBylineClose();
        if (author == 0)
        {
            if (_cImprintBlankAt >= 0)
            {
                _cImprintBlankAt = -1;
                CImprintChanged?.Invoke();
            }

            return;
        }

        CImprintDesk.CDeskQuill?.LQuillAuthorRemove(author);
    }

    public void CImprintAuthorMove(int? position, long? id, int step)
    {
        if (position is not int at)
        {
            return;
        }

        if (!CImprintHeld)
        {
            return;
        }

        if (id is not long author || author == 0)
        {
            return;
        }

        CImprintDesk.CDeskQuill?.LQuillAuthorMove(author, at + step);
    }

    public bool CImprintAuthorFinish(int? position, long? id, string? text, long? chosen)
    {
        if (!CImprintHeld)
        {
            return false;
        }

        if (position is not int at || id is not long author)
        {
            return false;
        }

        if (CImprintByline.CBylineShown && chosen is long picked)
        {
            CImprintByline.LBylineCommit(at, author, picked);
            return true;
        }

        LImprintCreditCommit(at, author, text);
        return true;
    }

    public bool CImprintAuthorCancel(int? position, long? id)
    {
        if (!CImprintHeld)
        {
            return false;
        }

        if (position is null || id is null)
        {
            return false;
        }

        bool offered = CImprintByline.CBylineShown;
        CImprintByline.CBylineClose();
        if (!offered)
        {
            CImprintReverted?.Invoke();
        }

        return true;
    }

    internal void LImprintAuthorInsert(int at, long author, long picked)
    {
        int blank = _cImprintBlankAt;
        _cImprintBlankAt = -1;
        if (CImprintDesk.CDeskQuill?.LQuillAuthorInsert(picked, at, author) == true)
        {
            return;
        }

        _cImprintBlankAt = blank;
        CImprintReverted?.Invoke();
    }

    private void LImprintCreditCommit(int at, long author, string? text)
    {
        CImprintByline.CBylineClose();
        int blank = _cImprintBlankAt;
        _cImprintBlankAt = -1;
        if (CImprintDesk.CDeskQuill?.LQuillAuthorAdd(text, at, author) == true)
        {
            return;
        }

        _cImprintBlankAt = blank;
        CImprintReverted?.Invoke();
    }

    private void LImprintBlankSet()
    {
        if (_cImprintCount == 0 && _cImprintBlankAt < 0)
        {
            _cImprintBlankAt = 0;
        }

        if (_cImprintBlankAt > _cImprintCount)
        {
            _cImprintBlankAt = _cImprintCount;
        }
    }

    private void LImprintDraftShow(LDraft draft)
    {
        CImprintChanged?.Invoke();
        CImprintReferenceChanged?.Invoke(LImprintReferenceRead(_cImprintEntryPort.LEngineImprintRead(draft)));
    }

    private static CReference LImprintReferenceRead(LImprint sheet)
    {
        return new CReference(
            sheet.LImprintTitle,
            sheet.LImprintTitleHint,
            sheet.LImprintYear,
            sheet.LImprintYearHint,
            sheet.LImprintUrl,
            sheet.LImprintUrlHint,
            sheet.LImprintNote,
            sheet.LImprintNoteHint,
            sheet.LImprintKindKey,
            sheet.LImprintKindTag);
    }
}
