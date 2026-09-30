using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardLinkHint = "Card.TranslationHint";

    private readonly PLinkCaret _pCardLinkCaret = new();

    private static readonly Func<object, long?> _pCardLinkKey =
        row => row is QLinkChip chip ? chip.QLinkChipTarget.CTranslationTargetId : null;

    public ObservableCollection<object> PCardLink { get; } = [];

    internal int PCardLinkPosition =>
        PCardRowResolve(PCardLink, _pCardLinkKey, PCardLink.IndexOf(_pCardLinkCaret));

    internal void PCardLinkShow(IReadOnlyList<CTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        PCardRowShow(
            PCardLink,
            targets,
            _pCardLinkKey,
            static target => target.CTranslationTargetId,
            PCardLinkCreate,
            static (row, target) => row is QLinkChip chip && chip.QLinkChipTarget == target
                ? row
                : PCardLinkCreate(target));

        PCardLinkUpdate();
    }

    internal QLinkChip? PCardLinkFind(int step)
    {
        int index = PCardLink.IndexOf(_pCardLinkCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLink.Count)
        {
            return null;
        }

        return PCardLink[target] as QLinkChip;
    }

    internal bool PCardLinkMove(int step)
    {
        int index = PCardLink.IndexOf(_pCardLinkCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLink.Count)
        {
            return false;
        }

        PCardLink.Move(index, target);
        return true;
    }

    internal void PCardLinkClear()
    {
        _pCardLinkCaret.PLinkCaretText = string.Empty;
        PCardLinkUpdate();
    }

    internal void PCardFlagUpdate()
    {
        for (int index = 0; index < PCardLink.Count; index++)
        {
            if (PCardLink[index] is not QLinkChip chip ||
                chip.QLinkChipFlag is not null)
            {
                continue;
            }

            PCardLink[index] = new QLinkChip(chip.QLinkChipTarget);
        }
    }

    private static object PCardLinkCreate(CTranslationTarget target)
    {
        return new QLinkChip(target);
    }

    private void PCardLinkStart()
    {
        PCardLink.Add(_pCardLinkCaret);
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
            PCardLink.Count > 1 ? string.Empty : PCardHintRead();
    }
}
