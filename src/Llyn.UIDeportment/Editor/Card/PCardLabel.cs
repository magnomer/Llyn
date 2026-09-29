using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardLabelHint = "Add tags";

    private readonly PLabelCaret _pCardLabelCaret = new();

    private static readonly Func<object, long?> _pCardLabelKey =
        row => row is PLabelChip chip ? chip.PLabelChipId : null;

    public ObservableCollection<object> PCardLabel { get; } = [];

    internal event Action<PCard, string>? PCardLabelNotice;

    internal string PCardLabelText => _pCardLabelCaret.PLabelCaretText;

    internal int PCardLabelPosition =>
        PCardRowResolve(PCardLabel, _pCardLabelKey, PCardLabel.IndexOf(_pCardLabelCaret));

    internal void PCardLabelShow(IReadOnlyList<CTagDraft> drafts)
    {
        PCardRowShow(
            PCardLabel,
            drafts,
            _pCardLabelKey,
            static draft => draft.CTagDraftId,
            PCardLabelCreate,
            static (row, draft) =>
                row is PLabelChip chip
                && string.Equals(chip.PLabelChipName, draft.CTagDraftText, StringComparison.Ordinal)
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
        _pCardLabelCaret.PLabelCaretText = string.Empty;
        PCardLabelUpdate();
    }

    private static object PCardLabelCreate(CTagDraft draft)
    {
        return new PLabelChip(draft.CTagDraftId, draft.CTagDraftText);
    }

    private void PCardLabelStart()
    {
        PCardLabel.Add(_pCardLabelCaret);
        PCardLabelUpdate();
    }

    internal void PCardLabelRefine(string rest)
    {
        if (!string.Equals(_pCardLabelCaret.PLabelCaretText, rest, StringComparison.Ordinal))
        {
            _pCardLabelCaret.PLabelCaretText = rest;
            PCardLabelUpdate();
        }

        PCardLabelNotice?.Invoke(this, _pCardLabelCaret.PLabelCaretText);
    }

    private void PCardLabelUpdate()
    {
        _pCardLabelCaret.PLabelCaretHint = PCardLabel.Count > 1 ? string.Empty : PCardLabelHint;
    }
}
