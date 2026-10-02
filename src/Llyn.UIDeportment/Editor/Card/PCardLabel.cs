using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardLabelHint = "Add tags";

    private readonly PLabelCaret _pCardLabelCaret = new();

    private static readonly Func<PLabelChip, long?> _pCardLabelKey =
        static chip => chip.PLabelChipId;

    public ObservableCollection<PLabelChip> PCardLabel { get; } = [];

    internal string PCardLabelText => _pCardLabelCaret.PLabelCaretText;

    internal PLabelCaret PCardLabelCaret => _pCardLabelCaret;

    internal int PCardLabelPosition =>
        PCardCaretFind(PCardLabel, _pCardLabelCaret.PLabelCaretAnchor);

    internal void PCardLabelShow(IReadOnlyList<CTagDraft> drafts)
    {
        List<PLabelChip> trail = PCardCaretRead(PCardLabel, _pCardLabelCaret.PLabelCaretAnchor);
        PCardRowShow(
            PCardLabel,
            drafts,
            _pCardLabelKey,
            static draft => draft.CTagDraftId,
            PCardLabelCreate,
            static (row, draft) =>
                string.Equals(row.PLabelChipName, draft.CTagDraftText, StringComparison.Ordinal)
                    ? row
                    : PCardLabelCreate(draft));
        _pCardLabelCaret.PLabelCaretAnchor = PCardCaretResolve(PCardLabel, trail);

        PCardLabelUpdate();
    }

    internal PLabelChip? PCardLabelFind(int step)
    {
        int target = PCardLabelPosition + (step < 0 ? step : step - 1);
        if (target < 0 || target >= PCardLabel.Count)
        {
            return null;
        }

        return PCardLabel[target];
    }

    internal bool PCardLabelMove(int step)
    {
        int target = PCardLabelPosition + step;
        if (target < 0 || target > PCardLabel.Count)
        {
            return false;
        }

        _pCardLabelCaret.PLabelCaretAnchor = target < PCardLabel.Count ? PCardLabel[target] : null;
        return true;
    }

    internal void PCardLabelClear()
    {
        _pCardLabelCaret.PLabelCaretText = string.Empty;
        PCardLabelUpdate();
    }

    private static PLabelChip PCardLabelCreate(CTagDraft draft)
    {
        return new PLabelChip(draft.CTagDraftId, draft.CTagDraftText);
    }

    private void PCardLabelStart()
    {
        PCardLabelUpdate();
    }

    internal void PCardLabelRefine(string rest)
    {
        if (!string.Equals(_pCardLabelCaret.PLabelCaretText, rest, StringComparison.Ordinal))
        {
            _pCardLabelCaret.PLabelCaretText = rest;
            PCardLabelUpdate();
        }
    }

    private void PCardLabelUpdate()
    {
        _pCardLabelCaret.PLabelCaretHint = PCardLabel.Count > 0 ? string.Empty : PCardLabelHint;
    }
}
