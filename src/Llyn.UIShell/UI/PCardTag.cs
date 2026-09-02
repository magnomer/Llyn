using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardTagHint = "Add tags";

    private readonly PTagEntry _pCardTagEntry = new();
    private bool _pCardTagBusy;

    public ObservableCollection<object> PCardTag { get; } = [];

    internal void PCardTagShow(IReadOnlyList<string> texts)
    {
        PCardTag.Clear();
        foreach (string text in texts)
        {
            string written = (text ?? string.Empty).Trim();
            if (written.Length == 0 || PCardTagCheck(written))
            {
                continue;
            }

            PCardTag.Add(new PTagChip(written));
        }

        _pCardTagEntry.PTagEntryText = string.Empty;
        PCardTag.Add(_pCardTagEntry);
        PCardTagUpdate();
    }

    internal IReadOnlyList<string> PCardTagRead()
    {
        List<string> texts = [];
        foreach (object row in PCardTag)
        {
            if (row is PTagChip chip)
            {
                texts.Add(chip.PTagChipName);
                continue;
            }

            string written = _pCardTagEntry.PTagEntryText.Trim();
            if (written.Length != 0 && !PCardTagCheck(written))
            {
                texts.Add(written);
            }
        }

        return texts;
    }

    internal void PCardTagRemove(PTagChip chip)
    {
        PCardTag.Remove(chip);
        PCardTagUpdate();
    }

    internal void PCardTagRemove(int step)
    {
        int index = PCardTag.IndexOf(_pCardTagEntry);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardTag.Count || PCardTag[target] is not PTagChip chip)
        {
            return;
        }

        PCardTagRemove(chip);
    }

    internal bool PCardTagMove(int step)
    {
        int index = PCardTag.IndexOf(_pCardTagEntry);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardTag.Count)
        {
            return false;
        }

        PCardTag.Move(index, target);
        return true;
    }

    internal void PCardTagCommit()
    {
        PCardTagCommit(_pCardTagEntry.PTagEntryText);
        _pCardTagEntry.PTagEntryText = string.Empty;
        PCardTagUpdate();
    }

    private void PCardTagStart()
    {
        _pCardTagEntry.PropertyChanged += PCardTagChange;
        PCardTag.Add(_pCardTagEntry);
        PCardTagUpdate();
    }

    private void PCardTagChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardTagBusy ||
            !string.Equals(arguments.PropertyName, nameof(PTagEntry.PTagEntryText), StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardTagEntry.PTagEntryText;
        if (written.IndexOf(',', StringComparison.Ordinal) < 0)
        {
            return;
        }

        _pCardTagBusy = true;
        string[] parts = written.Split(',');
        for (int index = 0; index < parts.Length - 1; index++)
        {
            PCardTagCommit(parts[index]);
        }

        _pCardTagEntry.PTagEntryText = parts[^1].TrimStart();
        _pCardTagBusy = false;
        PCardTagUpdate();
    }

    private void PCardTagCommit(string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || PCardTagCheck(written))
        {
            return;
        }

        int index = PCardTag.IndexOf(_pCardTagEntry);
        PCardTag.Insert(index < 0 ? PCardTag.Count : index, new PTagChip(written));
    }

    private bool PCardTagCheck(string text)
    {
        foreach (object row in PCardTag)
        {
            if (row is PTagChip chip &&
                string.Equals(chip.PTagChipName, text, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardTagUpdate()
    {
        _pCardTagEntry.PTagEntryHint = PCardTag.Count > 1 ? string.Empty : PCardTagHint;
    }
}
