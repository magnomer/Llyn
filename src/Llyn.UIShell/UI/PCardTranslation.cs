using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardTranslationKey = "Card.TranslationHint";

    private readonly PTranslationEntry _pCardTranslationEntry = new();
    private bool _pCardTranslationBusy;

    public ObservableCollection<object> PCardTranslation { get; } = [];

    internal Func<string, bool, bool>? PCardTranslationDispatcher { get; set; }

    internal void PCardTranslationShow(IReadOnlyList<LTranslationTarget> targets)
    {
        ArgumentNullException.ThrowIfNull(targets);

        PCardTranslation.Clear();
        foreach (LTranslationTarget target in targets)
        {
            string id = target.LTranslationTargetId.Trim();
            if (id.Length == 0 || PCardTranslationCheck(id))
            {
                continue;
            }

            PCardTranslation.Add(new PTranslationChip(
                id,
                target.LTranslationTargetHeadword,
                target.LTranslationTargetLanguage));
        }

        _pCardTranslationEntry.PTranslationEntryText = string.Empty;
        PCardTranslation.Add(_pCardTranslationEntry);
        PCardTranslationUpdate();
    }

    internal IReadOnlyList<string> PCardTranslationRead()
    {
        List<string> ids = [];
        foreach (object row in PCardTranslation)
        {
            if (row is PTranslationChip chip)
            {
                ids.Add(chip.PTranslationChipId);
            }
        }

        return ids;
    }

    internal void PCardTranslationRemove(PTranslationChip chip)
    {
        PCardTranslation.Remove(chip);
        PCardTranslationUpdate();
    }

    internal void PCardTranslationRemove(int step)
    {
        int index = PCardTranslation.IndexOf(_pCardTranslationEntry);
        int target = index + step;
        if (index < 0 ||
            target < 0 ||
            target >= PCardTranslation.Count ||
            PCardTranslation[target] is not PTranslationChip chip)
        {
            return;
        }

        PCardTranslationRemove(chip);
    }

    internal bool PCardTranslationMove(int step)
    {
        int index = PCardTranslation.IndexOf(_pCardTranslationEntry);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardTranslation.Count)
        {
            return false;
        }

        PCardTranslation.Move(index, target);
        return true;
    }

    internal void PCardTranslationCommit()
    {
        string written = _pCardTranslationEntry.PTranslationEntryText.Trim();
        if (written.Length == 0)
        {
            return;
        }

        if (PCardTranslationDispatcher?.Invoke(written, true) == true)
        {
            PCardTranslationClear();
        }
    }

    internal bool PCardTranslationCommit(string id, string headword, string language)
    {
        string written = (id ?? string.Empty).Trim();
        if (written.Length == 0)
        {
            return false;
        }

        if (PCardTranslationCheck(written))
        {
            return true;
        }

        int index = PCardTranslation.IndexOf(_pCardTranslationEntry);
        PCardTranslation.Insert(
            index < 0 ? PCardTranslation.Count : index,
            new PTranslationChip(written, headword, language));
        PCardTranslationUpdate();
        return true;
    }

    internal void PCardTranslationClear()
    {
        _pCardTranslationBusy = true;
        _pCardTranslationEntry.PTranslationEntryText = string.Empty;
        _pCardTranslationBusy = false;
        PCardTranslationUpdate();
    }

    internal void PCardFlagUpdate()
    {
        for (int index = 0; index < PCardTranslation.Count; index++)
        {
            if (PCardTranslation[index] is not PTranslationChip chip ||
                chip.PTranslationChipFlag is not null)
            {
                continue;
            }

            PCardTranslation[index] = new PTranslationChip(
                chip.PTranslationChipId,
                chip.PTranslationChipHeadword,
                chip.PTranslationChipLanguage);
        }
    }

    private void PCardTranslationStart()
    {
        _pCardTranslationEntry.PropertyChanged += PCardTranslationChange;
        PCardTranslation.Add(_pCardTranslationEntry);
        PCardTranslationUpdate();
    }

    private void PCardTranslationChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardTranslationBusy ||
            !string.Equals(
                arguments.PropertyName,
                nameof(PTranslationEntry.PTranslationEntryText),
                StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardTranslationEntry.PTranslationEntryText;
        if (written.IndexOf(',', StringComparison.Ordinal) < 0)
        {
            return;
        }

        _pCardTranslationBusy = true;
        string[] parts = written.Split(',');
        List<string> unresolved = [];
        for (int index = 0; index < parts.Length - 1; index++)
        {
            string part = parts[index].Trim();
            if (part.Length == 0)
            {
                continue;
            }

            if (PCardTranslationDispatcher?.Invoke(part, false) != true)
            {
                unresolved.Add(part);
            }
        }

        unresolved.Add(parts[^1].TrimStart());
        _pCardTranslationEntry.PTranslationEntryText = string.Join(", ", unresolved);
        _pCardTranslationBusy = false;
        PCardTranslationUpdate();
    }

    private static string PCardHintRead()
    {
        return System.Windows.Application.Current?.TryFindResource(PCardTranslationKey) as string
            ?? "Add translations";
    }

    private bool PCardTranslationCheck(string id)
    {
        foreach (object row in PCardTranslation)
        {
            if (row is PTranslationChip chip &&
                string.Equals(chip.PTranslationChipId, id, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardTranslationUpdate()
    {
        _pCardTranslationEntry.PTranslationEntryHint =
            PCardTranslation.Count > 1 ? string.Empty : PCardHintRead();
    }
}
