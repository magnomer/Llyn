using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardLabelHint = "Add tags";

    private readonly PLabelCaret _pCardLabelCaret = new();
    private bool _pCardLabelBusy;

    private static readonly Func<object, long?> _pCardLabelKey =
        row => row is PLabelChip chip ? chip.PLabelChipId : null;

    public ObservableCollection<object> PCardLabel { get; } = [];

    internal Action<string>? PCardLabelNotice { get; set; }

    internal Func<string, bool>? PCardLabelDispatcher { get; set; }

    internal string PCardLabelText => _pCardLabelCaret.PLabelCaretText;

    internal int PCardLabelPosition =>
        PCardRowResolve(PCardLabel, _pCardLabelKey, PCardLabel.IndexOf(_pCardLabelCaret));

    internal void PCardLabelShow(IReadOnlyList<LTagDraft> drafts)
    {
        PCardRowShow(
            PCardLabel,
            drafts,
            _pCardLabelKey,
            static draft => draft.LTagDraftId,
            PCardLabelCreate,
            static (row, draft) =>
                row is PLabelChip chip
                && string.Equals(chip.PLabelChipName, draft.LTagDraftText.Trim(), StringComparison.Ordinal)
                    ? row
                    : PCardLabelCreate(draft));

        PCardLabelUpdate();
    }

    internal PLabelChip? PCardLabelFind(int step)
    {
        int index = PCardLabel.IndexOf(_pCardLabelCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLabel.Count)
        {
            return null;
        }

        return PCardLabel[target] as PLabelChip;
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

    internal void PCardLabelClear()
    {
        _pCardLabelBusy = true;
        _pCardLabelCaret.PLabelCaretText = string.Empty;
        _pCardLabelBusy = false;
        PCardLabelUpdate();
    }

    internal bool PCardLabelMatch(long? id)
    {
        if (id is null or <= 0)
        {
            return false;
        }

        foreach (object row in PCardLabel)
        {
            if (row is PLabelChip chip && chip.PLabelChipId == id)
            {
                return true;
            }
        }

        return false;
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

    private static object PCardLabelCreate(LTagDraft draft)
    {
        return new PLabelChip(draft.LTagDraftId, draft.LTagDraftText.Trim());
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
                PCardLabelDispatcher?.Invoke(parts[index]);
            }

            _pCardLabelCaret.PLabelCaretText = parts[^1].TrimStart();
            _pCardLabelBusy = false;
            PCardLabelUpdate();
        }

        PCardLabelNotice?.Invoke(_pCardLabelCaret.PLabelCaretText);
    }

    private void PCardLabelUpdate()
    {
        _pCardLabelCaret.PLabelCaretHint = PCardLabel.Count > 1 ? string.Empty : PCardLabelHint;
    }
}
