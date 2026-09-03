using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardLabelHint = "Add tags";

    private readonly PLabelCaret _pCardLabelCaret = new();
    private bool _pCardLabelBusy;

    public ObservableCollection<object> PCardLabel { get; } = [];

    internal void PCardLabelShow(IReadOnlyList<string> texts)
    {
        PCardLabel.Clear();
        foreach (string text in texts)
        {
            string written = (text ?? string.Empty).Trim();
            if (written.Length == 0 || PCardLabelCheck(written))
            {
                continue;
            }

            PCardLabel.Add(new PLabelChip(written));
        }

        _pCardLabelCaret.PLabelCaretText = string.Empty;
        PCardLabel.Add(_pCardLabelCaret);
        PCardLabelUpdate();
    }

    internal IReadOnlyList<string> PCardLabelRead()
    {
        List<string> texts = [];
        foreach (object row in PCardLabel)
        {
            if (row is PLabelChip chip)
            {
                texts.Add(chip.PLabelChipName);
                continue;
            }

            string written = _pCardLabelCaret.PLabelCaretText.Trim();
            if (written.Length != 0 && !PCardLabelCheck(written))
            {
                texts.Add(written);
            }
        }

        return texts;
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
        _pCardLabelCaret.PLabelCaretText = string.Empty;
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
        if (written.IndexOf(',', StringComparison.Ordinal) < 0)
        {
            return;
        }

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

    private void PCardLabelCommit(string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || PCardLabelCheck(written))
        {
            return;
        }

        int index = PCardLabel.IndexOf(_pCardLabelCaret);
        PCardLabel.Insert(index < 0 ? PCardLabel.Count : index, new PLabelChip(written));
    }

    private bool PCardLabelCheck(string text)
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
