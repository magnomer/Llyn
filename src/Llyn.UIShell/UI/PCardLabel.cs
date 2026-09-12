using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardLabelHint = "Add tags";

    private readonly PLabelCaret _pCardLabelCaret = new();
    private bool _pCardLabelBusy;

    public ObservableCollection<object> PCardLabel { get; } = [];

    internal Action<string>? PCardLabelNotice { get; set; }

    internal void PCardLabelShow(IReadOnlyList<LTagDraft> drafts)
    {
        PCardLabel.Clear();
        foreach (LTagDraft draft in drafts)
        {
            string written = draft.LTagDraftText.Trim();
            if (written.Length == 0 || PCardLabelCheck(written))
            {
                continue;
            }

            PCardLabel.Add(new PLabelChip(draft.LTagDraftId, written));
        }

        _pCardLabelCaret.PLabelCaretText = string.Empty;
        PCardLabel.Add(_pCardLabelCaret);
        PCardLabelUpdate();
    }

    internal IReadOnlyList<LTagDraft> PCardLabelRead()
    {
        List<LTagDraft> drafts = [];
        foreach (object row in PCardLabel)
        {
            if (row is PLabelChip chip)
            {
                drafts.Add(new LTagDraft(chip.PLabelChipId, chip.PLabelChipName));
                continue;
            }

            string written = _pCardLabelCaret.PLabelCaretText.Trim();
            if (written.Length != 0 && !PCardLabelCheck(written))
            {
                drafts.Add(LTagDraft.LTagDraftCreate(written) with
                {
                    LTagDraftId = _pCardLabelCaret.PLabelCaretId,
                });
            }
        }

        return drafts;
    }

    internal void PCardLabelApply(IReadOnlyList<LTagDraft> stored)
    {
        int index = 0;
        foreach (object row in PCardLabel)
        {
            if (row is PLabelChip chip)
            {
                if (index < stored.Count)
                {
                    chip.PLabelChipId = stored[index].LTagDraftId;
                }

                index++;
                continue;
            }

            if (_pCardLabelCaret.PLabelCaretText.Trim().Length != 0)
            {
                if (index < stored.Count)
                {
                    _pCardLabelCaret.PLabelCaretId = stored[index].LTagDraftId;
                }

                index++;
            }
        }
    }

    internal void PCardLabelRemove(PLabelChip chip)
    {
        PCardLabel.Remove(chip);
        PCardLabelUpdate();
    }

    internal void PCardLabelRemove(int step)
    {
        int index = PCardLabel.IndexOf(_pCardLabelCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLabel.Count || PCardLabel[target] is not PLabelChip chip)
        {
            return;
        }

        PCardLabelRemove(chip);
    }

    internal bool PCardLabelMove(int step)
    {
        int index = PCardLabel.IndexOf(_pCardLabelCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLabel.Count)
        {
            return false;
        }

        PCardLabel.Move(index, target);
        return true;
    }

    internal void PCardLabelCommit()
    {
        PCardLabelCommit(_pCardLabelCaret.PLabelCaretText);
        PCardLabelClear();
    }

    internal void PCardLabelClear()
    {
        _pCardLabelBusy = true;
        _pCardLabelCaret.PLabelCaretText = string.Empty;
        _pCardLabelCaret.PLabelCaretId = 0;
        _pCardLabelBusy = false;
        PCardLabelUpdate();
    }

    private void PCardLabelStart()
    {
        _pCardLabelCaret.PropertyChanged += PCardLabelChange;
        PCardLabel.Add(_pCardLabelCaret);
        PCardLabelUpdate();
    }

    private void PCardLabelChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardLabelBusy ||
            !string.Equals(arguments.PropertyName, nameof(PLabelCaret.PLabelCaretText), StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardLabelCaret.PLabelCaretText;
        if (written.IndexOf(',', StringComparison.Ordinal) >= 0)
        {
            _pCardLabelBusy = true;
            string[] parts = written.Split(',');
            for (int index = 0; index < parts.Length - 1; index++)
            {
                PCardLabelCommit(parts[index]);
            }

            _pCardLabelCaret.PLabelCaretText = parts[^1].TrimStart();
            _pCardLabelBusy = false;
            PCardLabelUpdate();
        }

        PCardLabelNotice?.Invoke(_pCardLabelCaret.PLabelCaretText);
    }

    internal void PCardLabelCommit(string text)
    {
        PCardLabelCommit(_pCardLabelCaret.PLabelCaretId, text);
        _pCardLabelCaret.PLabelCaretId = 0;
    }

    internal void PCardLabelCommit(long id, string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || PCardLabelCheck(written))
        {
            return;
        }

        int index = PCardLabel.IndexOf(_pCardLabelCaret);
        PCardLabel.Insert(index < 0 ? PCardLabel.Count : index, new PLabelChip(id, written));
    }

    internal bool PCardLabelCheck(string text)
    {
        foreach (object row in PCardLabel)
        {
            if (row is PLabelChip chip &&
                string.Equals(chip.PLabelChipName, text, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardLabelUpdate()
    {
        _pCardLabelCaret.PLabelCaretHint = PCardLabel.Count > 1 ? string.Empty : PCardLabelHint;
    }
}
