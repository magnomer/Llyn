using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LImprint
{
    private const int LBylineLimit = 8;

    private readonly LDraftPort _lDraftPort;

    private readonly LEntryPort _lEntryPort;

    private int _lImprintBlankAt = -1;

    private int _lImprintCount;

    private int _lBylineIndex = -1;

    private int _lBylineCount;

    private string _lBylineWord = string.Empty;

    public LImprint(LDraftPort drafts, LEntryPort entries, Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);

        _lDraftPort = drafts;
        _lEntryPort = entries;
        LImprintDesk = new LDesk(drafts, "Source", unreadableSeam);
        LImprintDesk.LDeskDraftChanged += LImprintDraftUpdate;
    }

    public event Action? LImprintChanged;

    public event Action? LImprintFocused;

    public event Action? LImprintReverted;

    public event Action? LBylineChanged;

    public LDesk LImprintDesk { get; }

    public bool LImprintHeld => LImprintDesk.LDeskHeld;

    public int LImprintBlankAt => _lImprintBlankAt;

    public bool LBylineShown => _lBylineWord.Length > 0 && _lBylineCount > 0;

    public int LBylineIndex => _lBylineIndex;

    public string LBylineWord => _lBylineWord;

    public void LImprintVistaRestore(LVista vista)
    {
        LImprintDesk.LDeskVistaRestore(vista);
    }

    public void LImprintOpen(long? id)
    {
        _lImprintBlankAt = -1;
        LBylineReset();
        LImprintDesk.LDeskStart(id);
    }

    public void LImprintCancel()
    {
        LBylineReset();
        LImprintDesk.LDeskCancel();
        LImprintChanged?.Invoke();
        LBylineChanged?.Invoke();
    }

    public void LImprintSave()
    {
        if (!LImprintDesk.LDeskChangeCheck())
        {
            return;
        }

        LImprintDesk.LDeskFinish(true);
    }

    private void LImprintDraftUpdate(LDraft draft)
    {
        LImprintChanged?.Invoke();
    }

    public LReference LImprintReferenceRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return draft.LDraftReference
            ?? throw new InvalidOperationException("The imprint draft holds no reference.");
    }

    public string LImprintTallyRead()
    {
        return LReference.LReferenceUsageFormat(LImprintUsageRead(), LLocalization.LLocalizationTextRead);
    }

    private int LImprintUsageRead()
    {
        return LImprintDesk.LDeskRead()?.LDraftStored is long stored
            ? _lEntryPort.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored)
            : 0;
    }

    public void LImprintTitleSet(string text)
    {
        LImprintDesk.LDeskDefer(new LRequestReferenceTitle(LImprintDesk.LDeskId, new LStateWritten(text)));
    }

    public void LImprintYearSet(string text)
    {
        LImprintDesk.LDeskDefer(new LRequestReferenceYear(LImprintDesk.LDeskId, new LStateWritten(text)));
    }

    public void LImprintUrlSet(string text)
    {
        LImprintDesk.LDeskDefer(new LRequestReferenceUrl(LImprintDesk.LDeskId, new LStateWritten(text)));
    }

    public void LImprintNoteSet(string text)
    {
        LImprintDesk.LDeskDefer(new LRequestReferenceNote(LImprintDesk.LDeskId, new LStateWritten(text)));
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

        LImprintDesk.LDeskSend(new LRequestReferenceKind(LImprintDesk.LDeskId, LReference.LReferenceKindParse(tag)));
    }

    private bool LImprintKindCheck(string tag)
    {
        return LImprintDesk.LDeskRead()?.LDraftReference?.LReferenceKindMatch(LReference.LReferenceKindParse(tag))
            ?? false;
    }

    public IReadOnlyList<LAuthorRow> LImprintCreditRead()
    {
        IReadOnlyList<LAuthorRow> rows = LImprintDesk.LDeskRead()?.LDraftCreditRead() ?? [];
        _lImprintCount = rows.Count;
        if (LImprintHeld)
        {
            LImprintBlankSet();
        }

        return rows;
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

    public void LImprintCreditApply(string? action, int? position, long? id)
    {
        if (!LImprintHeld)
        {
            return;
        }

        if (action is null || position is not int at || id is not long author)
        {
            return;
        }

        switch (action)
        {
            case "Add":
                LImprintCreditAdd(at, author);
                return;
            case "Remove":
                LImprintCreditRemove(author);
                return;
            case "Earlier":
                LImprintCreditMove(author, at - 1);
                return;
            case "Later":
                LImprintCreditMove(author, at + 1);
                return;
            default:
                return;
        }
    }

    private void LImprintCreditAdd(int at, long author)
    {
        if (author != 0)
        {
            _lImprintBlankAt = at + 1;
            LImprintChanged?.Invoke();
        }

        LImprintFocused?.Invoke();
    }

    private void LImprintCreditRemove(long author)
    {
        LBylineHide();
        if (author == 0)
        {
            _lImprintBlankAt = -1;
            LImprintChanged?.Invoke();
            return;
        }

        LImprintDesk.LDeskSend(new LRequestAuthorRemoval(LImprintDesk.LDeskId, author));
    }

    private void LImprintCreditMove(long author, int to)
    {
        if (author == 0)
        {
            return;
        }

        LImprintDesk.LDeskSend(new LRequestAuthorShift(LImprintDesk.LDeskId, author, to));
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
        LImprintDesk.LDeskSend(new LRequestAuthorAddition(LImprintDesk.LDeskId, name, at, author));
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

    public IReadOnlyList<LAuthor> LBylineRowsRead()
    {
        IReadOnlyList<LAuthor> rows = LBylineFind();
        _lBylineCount = rows.Count;
        return rows;
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
            return _lDraftPort.LEngineBylineFind(LImprintDesk.LDeskId, LBylineWord, LBylineLimit);
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
        LImprintDesk.LDeskSend(new LRequestAuthorPick(LImprintDesk.LDeskId, picked, at, author));
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
