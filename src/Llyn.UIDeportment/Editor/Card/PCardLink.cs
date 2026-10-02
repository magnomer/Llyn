using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardLinkHint = "Card.TranslationHint";

    private readonly PLinkCaret _pCardLinkCaret = new();

    private static readonly Func<QLinkChip, long?> _pCardLinkKey =
        static chip => chip.QLinkChipTarget.CTranslationTargetId;

    public ObservableCollection<QLinkChip> PCardLink { get; } = [];

    internal PLinkCaret PCardLinkCaret => _pCardLinkCaret;

    internal int PCardLinkPosition =>
        PCardCaretFind(PCardLink, _pCardLinkCaret.PLinkCaretAnchor);

    internal void PCardLinkShow(IReadOnlyList<CTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        List<QLinkChip> trail = PCardCaretRead(PCardLink, _pCardLinkCaret.PLinkCaretAnchor);
        PCardRowShow(
            PCardLink,
            targets,
            _pCardLinkKey,
            static target => target.CTranslationTargetId,
            PCardLinkCreate,
            static (row, target) => row.QLinkChipTarget == target ? row : PCardLinkCreate(target));
        _pCardLinkCaret.PLinkCaretAnchor = PCardCaretResolve(PCardLink, trail);

        PCardLinkUpdate();
    }

    internal QLinkChip? PCardLinkFind(int step)
    {
        int target = PCardLinkPosition + (step < 0 ? step : step - 1);
        if (target < 0 || target >= PCardLink.Count)
        {
            return null;
        }

        return PCardLink[target];
    }

    internal bool PCardLinkMove(int step)
    {
        int target = PCardLinkPosition + step;
        if (target < 0 || target > PCardLink.Count)
        {
            return false;
        }

        _pCardLinkCaret.PLinkCaretAnchor = target < PCardLink.Count ? PCardLink[target] : null;
        return true;
    }

    internal void PCardLinkClear()
    {
        _pCardLinkCaret.PLinkCaretText = string.Empty;
        PCardLinkUpdate();
    }

    internal void PCardFlagUpdate()
    {
        foreach (object row in PCardLink)
        {
            if (row is QLinkChip chip)
            {
                chip.QLinkChipRefine();
            }
        }
    }

    private static QLinkChip PCardLinkCreate(CTranslationTarget target)
    {
        return new QLinkChip(target);
    }

    private void PCardLinkStart()
    {
        PCardLinkUpdate();
    }

    internal void PCardLinkRefine(string rest)
    {
        if (!string.Equals(_pCardLinkCaret.PLinkCaretText, rest, StringComparison.Ordinal))
        {
            _pCardLinkCaret.PLinkCaretText = rest;
            PCardLinkUpdate();
        }
    }

    private static string PCardHintRead()
    {
        return System.Windows.Application.Current?.TryFindResource(PCardLinkHint) as string
            ?? "Add translations";
    }

    private void PCardLinkUpdate()
    {
        _pCardLinkCaret.PLinkCaretHint =
            PCardLink.Count > 0 ? string.Empty : PCardHintRead();
    }
}
