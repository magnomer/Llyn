using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardLinkHint = "Card.TranslationHint";

    private readonly PLinkCaret _pCardLinkCaret = new();
    private bool _pCardLinkBusy;

    private static readonly Func<object, long?> _pCardLinkKey =
        row => row is PLinkChip chip ? chip.PLinkChipId : null;

    public ObservableCollection<object> PCardLink { get; } = [];

    internal Func<string, bool, bool>? PCardLinkDispatcher { get; set; }

    internal Action<string>? PCardLinkNotice { get; set; }

    internal string PCardLinkText => _pCardLinkCaret.PLinkCaretText;

    internal int PCardLinkPosition =>
        PCardRowResolve(PCardLink, _pCardLinkKey, PCardLink.IndexOf(_pCardLinkCaret));

    internal void PCardLinkShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        PCardRowShow(
            PCardLink,
            targets,
            _pCardLinkKey,
            static target => target.LTranslationTargetId,
            PCardLinkCreate,
            static (row, target) =>
                row is PLinkChip chip
                && string.Equals(chip.PLinkChipHeadword, target.LTranslationTargetHeadword, StringComparison.Ordinal)
                && string.Equals(chip.PLinkChipLanguage, target.LTranslationTargetLanguage, StringComparison.Ordinal)
                    ? row
                    : PCardLinkCreate(target));

        PCardLinkUpdate();
    }

    internal PLinkChip? PCardLinkFind(int step)
    {
        int index = PCardLink.IndexOf(_pCardLinkCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardLink.Count)
        {
            return null;
        }

        return PCardLink[target] as PLinkChip;
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

    internal void PCardLinkCommit()
    {
        string written = _pCardLinkCaret.PLinkCaretText.Trim();
        if (written.Length == 0)
        {
            return;
        }

        if (PCardLinkDispatcher?.Invoke(written, true) == true)
        {
            PCardLinkClear();
        }
    }

    internal void PCardLinkClear()
    {
        _pCardLinkBusy = true;
        _pCardLinkCaret.PLinkCaretText = string.Empty;
        _pCardLinkBusy = false;
        PCardLinkUpdate();
    }

    internal bool PCardLinkCheck(long id)
    {
        foreach (object row in PCardLink)
        {
            if (row is PLinkChip chip && chip.PLinkChipId == id)
            {
                return true;
            }
        }

        return false;
    }

    internal void PCardFlagUpdate()
    {
        for (int index = 0; index < PCardLink.Count; index++)
        {
            if (PCardLink[index] is not PLinkChip chip ||
                chip.PLinkChipFlag is not null)
            {
                continue;
            }

            PCardLink[index] = new PLinkChip(
                chip.PLinkChipId,
                chip.PLinkChipHeadword,
                chip.PLinkChipLanguage);
        }
    }

    private static object PCardLinkCreate(LTranslationTarget target)
    {
        return new PLinkChip(
            target.LTranslationTargetId, target.LTranslationTargetHeadword, target.LTranslationTargetLanguage);
    }

    private void PCardLinkStart()
    {
        _pCardLinkCaret.PropertyChanged += PCardLinkChange;
        PCardLink.Add(_pCardLinkCaret);
        PCardLinkUpdate();
    }

    private void PCardLinkChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardLinkBusy ||
            !string.Equals(
                arguments.PropertyName,
                nameof(PLinkCaret.PLinkCaretText),
                StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardLinkCaret.PLinkCaretText;
        if (written.IndexOf(',', StringComparison.Ordinal) >= 0)
        {
            _pCardLinkBusy = true;
            string[] parts = written.Split(',');
            List<string> unresolved = [];
            for (int index = 0; index < parts.Length - 1; index++)
            {
                string part = parts[index].Trim();
                if (part.Length == 0)
                {
                    continue;
                }

                if (PCardLinkDispatcher?.Invoke(part, false) != true)
                {
                    unresolved.Add(part);
                }
            }

            unresolved.Add(parts[^1].TrimStart());
            _pCardLinkCaret.PLinkCaretText = string.Join(", ", unresolved);
            _pCardLinkBusy = false;
            PCardLinkUpdate();
        }

        PCardLinkNotice?.Invoke(_pCardLinkCaret.PLinkCaretText);
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
