using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LImprint
{
    private const int LBylineLimit = 8;

    private readonly LDraftPort _lDraftPort;

    private readonly LEntryPort _lEntryPort;

    private readonly LSettingsPort _lSettingsPort;

    private int _lImprintBlankAt = -1;

    private int _lImprintCount;

    private int _lBylineIndex = -1;

    private int _lBylineCount;

    private string _lBylineWord = string.Empty;

    internal LImprint(LDraftPort drafts, LEntryPort entries, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);

        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lSettingsPort = settings;
        LImprintDesk = new CDesk(drafts, "Source", envoy);
        LImprintDesk.CDeskDraftPrepared += LImprintDraftUpdate;
    }

    public event Action? LImprintChanged;

    public event Action<CImprint>? LImprintReferenceChanged;

    public event Action? LImprintFocused;

    public event Action? LImprintReverted;

    public event Action? LBylineChanged;

    public CDesk LImprintDesk { get; }

    public static CImprint LImprintEmpty { get; } = LImprintReferenceRead(new LReference(
        0,
        LStateValue.LStateValueUnspecified,
        LStateValue.LStateValueUnspecified,
        LReferenceKind.LReferenceKindUnspecified,
        LStateValue.LStateValueUnspecified,
        LStateValue.LStateValueUnspecified,
        LStateMark.LStateMarkUnspecified));

    public bool LImprintHeld => LImprintDesk.CDeskHeld;

    public int LImprintBlankAt => _lImprintBlankAt;

    public bool LBylineShown => _lBylineWord.Length > 0 && _lBylineCount > 0;

    public int LBylineIndex => _lBylineIndex;

    public string LBylineWord => _lBylineWord;

    internal void LImprintVistaRestore(LVista vista)
    {
        LImprintDesk.CDeskVistaRestore(vista);
    }

    public void LImprintOpen(long? id)
    {
        _lImprintBlankAt = -1;
        LBylineReset();
        LImprintDesk.CDeskStart(id);
    }

    public void LImprintCancel()
    {
        LBylineReset();
        LImprintDesk.CDeskCancel();
        LImprintChanged?.Invoke();
        LBylineChanged?.Invoke();
    }

    public void LImprintSave()
    {
        if (!LImprintDesk.CDeskChangeCheck())
        {
            return;
        }

        LImprintDesk.CDeskFinish(true);
    }

    private void LImprintDraftUpdate(LDraft draft)
    {
        LImprintChanged?.Invoke();
        LImprintReferenceChanged?.Invoke(LImprintReferenceRead(draft.LDraftReference
            ?? throw new InvalidOperationException("The imprint draft holds no reference.")));
    }

    internal static CImprint LImprintReferenceRead(LReference reference)
    {
        return new CImprint(
            reference.LReferenceTitle.LStateValueShow(),
            reference.LReferenceTitleHint,
            reference.LReferenceYear.LStateValueShow(),
            reference.LReferenceYearHint,
            reference.LReferenceUrl.LStateValueShow(),
            reference.LReferenceUrlHint,
            reference.LReferenceNote.LStateValueShow(),
            reference.LReferenceNoteHint,
            reference.LReferenceKindKey,
            reference.LReferenceKindTag);
    }

    public string LImprintTallyRead()
    {
        return LReference.LReferenceUsageFormat(LImprintUsageRead(), _lSettingsPort.LEngineTextRead);
    }

    private int LImprintUsageRead()
    {
        return LImprintDesk.CDeskStoredRead() is long stored
            ? _lEntryPort.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored)
            : 0;
    }

    public void LImprintTitleSet(string text)
    {
        LImprintDesk.CDeskDefer(new LRequestReferenceTitle(LImprintDesk.CDeskId, new LStateWritten(text)));
    }

    public void LImprintYearSet(string text)
    {
        LImprintDesk.CDeskDefer(new LRequestReferenceYear(LImprintDesk.CDeskId, new LStateWritten(text)));
    }

    public void LImprintUrlSet(string text)
    {
        LImprintDesk.CDeskDefer(new LRequestReferenceUrl(LImprintDesk.CDeskId, new LStateWritten(text)));
    }

    public void LImprintNoteSet(string text)
    {
        LImprintDesk.CDeskDefer(new LRequestReferenceNote(LImprintDesk.CDeskId, new LStateWritten(text)));
    }

    public void LImprintKindSet(string? tag)
    {
        if (tag is null)
        {
            return;
        }

        if (!LImprintHeld)
        {
            return;
        }

        if (LImprintKindCheck(tag))
        {
            return;
        }

        LImprintDesk.CDeskSend(new LRequestReferenceKind(LImprintDesk.CDeskId, LReference.LReferenceKindParse(tag)));
    }

    private bool LImprintKindCheck(string tag)
    {
        return LImprintDesk.CDeskRead()?.LDraftReference?.LReferenceKindMatch(LReference.LReferenceKindParse(tag))
            ?? false;
    }

    public IReadOnlyList<CAuthorRow> LImprintCreditRead()
    {
        IReadOnlyList<LAuthorRow> rows = LImprintDesk.CDeskRead()?.LDraftCreditRead() ?? [];
        _lImprintCount = rows.Count;
        if (LImprintHeld)
        {
            LImprintBlankSet();
        }

        return LSplice.LSpliceBuild(
            rows,
            static row => new CAuthorRow(
                row.LAuthorRowId,
                row.LAuthorRowName,
                row.LAuthorRowPosition,
                row.LAuthorRowEarlier,
                row.LAuthorRowLater));
    }

    private void LImprintBlankSet()
    {
        if (_lImprintCount == 0 && _lImprintBlankAt < 0)
        {
            _lImprintBlankAt = 0;
        }

        if (_lImprintBlankAt > _lImprintCount)
        {
            _lImprintBlankAt = _lImprintCount;
        }
    }

    public void LImprintAuthorAdd(int? position, long? id)
    {
        if (!LImprintHeld)
        {
            return;
        }

        if (position is not int at || id is not long author)
        {
            return;
        }

        if (author != 0)
        {
            _lImprintBlankAt = at + 1;
            LImprintChanged?.Invoke();
        }

        LImprintFocused?.Invoke();
    }

    public void LImprintAuthorRemove(long? id)
    {
        if (!LImprintHeld)
        {
            return;
        }

        if (id is not long author)
        {
            return;
        }

        LBylineHide();
        if (author == 0)
        {
            _lImprintBlankAt = -1;
            LImprintChanged?.Invoke();
            return;
        }

        LImprintDesk.CDeskSend(new LRequestAuthorRemoval(LImprintDesk.CDeskId, author));
    }

    public void LImprintAuthorRetreat(int? position, long? id)
    {
        if (position is int at)
        {
            LImprintCreditMove(id, at - 1);
        }
    }

    public void LImprintAuthorAdvance(int? position, long? id)
    {
        if (position is int at)
        {
            LImprintCreditMove(id, at + 1);
        }
    }

    private void LImprintCreditMove(long? id, int to)
    {
        if (!LImprintHeld)
        {
            return;
        }

        if (id is not long author || author == 0)
        {
            return;
        }

        LImprintDesk.CDeskSend(new LRequestAuthorShift(LImprintDesk.CDeskId, author, to));
    }

    private void LImprintCreditCommit(int at, long author, string? text)
    {
        LBylineHide();
        string name = (text ?? string.Empty).Trim();
        if (name.Length == 0)
        {
            LImprintReverted?.Invoke();
            return;
        }

        _lImprintBlankAt = -1;
        LImprintDesk.CDeskSend(new LRequestAuthorAddition(LImprintDesk.CDeskId, name, at, author));
    }

    public bool LImprintKeyApply(string key, int? position, long? id, string? text, long? chosen)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (!LImprintHeld)
        {
            return false;
        }

        if (position is not int at || id is not long author)
        {
            return false;
        }

        if (LBylineShown)
        {
            if (LBylineKeyApply(key, at, author, chosen))
            {
                return true;
            }
        }

        switch (key)
        {
            case "Enter":
                LImprintCreditCommit(at, author, text);
                return true;
            case "Escape":
                LBylineHide();
                LImprintReverted?.Invoke();
                return true;
            default:
                return false;
        }
    }

    private bool LBylineKeyApply(string key, int at, long author, long? chosen)
    {
        switch (key)
        {
            case "Escape":
                LBylineHide();
                return true;
            case "Down":
                LBylineAdjust(1);
                return true;
            case "Up":
                LBylineAdjust(-1);
                return true;
            case "Enter" when chosen is long picked:
                LBylineCommit(at, author, picked);
                return true;
            default:
                return false;
        }
    }

    public void LBylineWordSet(string? text, bool? focused)
    {
        if (!LImprintHeld)
        {
            return;
        }

        if (focused != true)
        {
            return;
        }

        _lBylineIndex = -1;
        _lBylineWord = (text ?? string.Empty).Trim();
        LBylineChanged?.Invoke();
    }

    public IReadOnlyList<CAuthor> LBylineRowsRead()
    {
        IReadOnlyList<LAuthor> rows = LBylineFind();
        _lBylineCount = rows.Count;
        return LSplice.LSpliceBuild(rows, static row => new CAuthor(row.LAuthorId, row.LAuthorName));
    }

    private IReadOnlyList<LAuthor> LBylineFind()
    {
        if (!LImprintHeld)
        {
            return [];
        }

        if (_lBylineWord.Length == 0)
        {
            return [];
        }

        try
        {
            return _lDraftPort.LEngineBylineFind(LImprintDesk.CDeskId, LBylineWord, LBylineLimit);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private void LBylineAdjust(int delta)
    {
        if (_lBylineCount == 0)
        {
            return;
        }

        int chosen = _lBylineIndex < 0 && delta < 0 ? 0 : _lBylineIndex;
        _lBylineIndex = ((chosen + delta) % _lBylineCount + _lBylineCount) % _lBylineCount;
        LBylineChanged?.Invoke();
    }

    public void LBylineSelect(long? id, int? position, long? held)
    {
        if (id is not long picked || position is not int at || held is not long author)
        {
            LBylineHide();
            return;
        }

        LBylineCommit(at, author, picked);
    }

    private void LBylineCommit(int at, long author, long picked)
    {
        LBylineHide();
        if (picked == author)
        {
            LImprintReverted?.Invoke();
            return;
        }

        _lImprintBlankAt = -1;
        LImprintDesk.CDeskSend(new LRequestAuthorPick(LImprintDesk.CDeskId, picked, at, author));
    }

    public void LBylineHide()
    {
        LBylineReset();
        LBylineChanged?.Invoke();
    }

    private void LBylineReset()
    {
        _lBylineIndex = -1;
        _lBylineWord = string.Empty;
    }
}
