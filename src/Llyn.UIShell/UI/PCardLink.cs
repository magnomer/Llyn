using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardLinkKey = "Card.TranslationHint";

    private readonly PLinkCaret _pCardLinkCaret = new();
    private bool _pCardLinkBusy;

    public ObservableCollection<object> PCardLink { get; } = [];

    internal Func<string, bool, bool>? PCardLinkDispatcher { get; set; }

    internal Action<string>? PCardLinkNotice { get; set; }

    internal void PCardLinkShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        PCardLink.Clear();
        foreach (LTranslationTarget target in targets)
        {
            string id = target.LTranslationTargetId.Trim();
            if (id.Length == 0 || PCardLinkCheck(id))
            {
                continue;
            }

            PCardLink.Add(new PLinkChip(
                id,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage));
        }

        _pCardLinkCaret.PLinkCaretText = string.Empty;
        PCardLink.Add(_pCardLinkCaret);
        PCardLinkUpdate();
    }

    internal IReadOnlyList<string> PCardLinkRead()
    {
        List<string> ids = [];
        foreach (object row in PCardLink)
        {
            if (row is PLinkChip chip)
            {
                ids.Add(chip.PLinkChipId);
            }
        }

        return ids;
    }

    internal void PCardLinkRemove(PLinkChip chip)
    {
        PCardLink.Remove(chip);
        PCardLinkUpdate();
    }

    internal void PCardLinkRemove(int step)
    {
        int index = PCardLink.IndexOf(_pCardLinkCaret);
        int target = index + step;
        if (index < 0 ||
            target < 0 ||
            target >= PCardLink.Count ||
            PCardLink[target] is not PLinkChip chip)
        {
            return;
        }

        PCardLinkRemove(chip);
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

    internal bool PCardLinkCommit(string id, string headword, string language)
    {
        string written = (id ?? string.Empty).Trim();
        if (written.Length == 0)
        {
            return false;
        }

        if (PCardLinkCheck(written))
        {
            return true;
        }

        int index = PCardLink.IndexOf(_pCardLinkCaret);
        PCardLink.Insert(
            index < 0 ? PCardLink.Count : index,
            new PLinkChip(written, headword, language));
        PCardLinkUpdate();
        return true;
    }

    internal void PCardLinkClear()
    {
        _pCardLinkBusy = true;
        _pCardLinkCaret.PLinkCaretText = string.Empty;
        _pCardLinkBusy = false;
        PCardLinkUpdate();
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
        return System.Windows.Application.Current?.TryFindResource(PCardLinkKey) as string
            ?? "Add translations";
    }

    private bool PCardLinkCheck(string id)
    {
        foreach (object row in PCardLink)
        {
            if (row is PLinkChip chip &&
                string.Equals(chip.PLinkChipId, id, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardLinkUpdate()
    {
        _pCardLinkCaret.PLinkCaretHint =
            PCardLink.Count > 1 ? string.Empty : PCardHintRead();
    }
}
