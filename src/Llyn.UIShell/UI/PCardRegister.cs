using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardRegisterHint = "Card.RegisterHint";

    private readonly PRegisterCaret _pCardRegisterCaret = new();
    private bool _pCardRegisterBusy;

    public ObservableCollection<object> PCardRegister { get; } = [];

    internal Action<string>? PCardRegisterNotice { get; set; }

    internal string PCardRegisterText => _pCardRegisterCaret.PRegisterCaretText;

    internal void PCardRegisterShow(IReadOnlyList<LRegisterDraft> drafts)
    {
        PCardRegister.Clear();
        foreach (LRegisterDraft draft in drafts)
        {
            PRegister chip = new(
                draft.LRegisterDraftName,
                draft.LRegisterDraftId,
                draft.LRegisterDraftLanguage,
                draft.LRegisterDraftBuiltin);
            if (chip.PRegisterTextRead().LStateValueEmpty || PCardRegisterCheck(chip.PRegisterText))
            {
                continue;
            }

            PCardRegister.Add(chip);
        }

        _pCardRegisterCaret.PRegisterCaretText = string.Empty;
        PCardRegister.Add(_pCardRegisterCaret);
        PCardRegisterUpdate();
    }

    internal IReadOnlyList<LRegisterDraft> PCardRegisterRead()
    {
        List<LRegisterDraft> drafts = [];
        foreach (object row in PCardRegister)
        {
            if (row is PRegister chip)
            {
                drafts.Add(new LRegisterDraft(
                    chip.PRegisterTextRead(),
                    chip.PRegisterId,
                    chip.PRegisterLanguage,
                    chip.PRegisterBuiltin));
                continue;
            }

            string written = _pCardRegisterCaret.PRegisterCaretText.Trim();
            if (written.Length != 0 && !PCardRegisterCheck(written))
            {
                drafts.Add(LRegisterDraft.LRegisterDraftCreate(written));
            }
        }

        return drafts;
    }

    internal void PCardRegisterRemove(PRegister chip)
    {
        PCardRegister.Remove(chip);
        PCardRegisterUpdate();
    }

    internal void PCardRegisterRemove(int step)
    {
        int index = PCardRegister.IndexOf(_pCardRegisterCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardRegister.Count
            || PCardRegister[target] is not PRegister chip)
        {
            return;
        }

        PCardRegisterRemove(chip);
    }

    internal bool PCardRegisterMove(int step)
    {
        int index = PCardRegister.IndexOf(_pCardRegisterCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardRegister.Count)
        {
            return false;
        }

        PCardRegister.Move(index, target);
        return true;
    }

    internal void PCardRegisterCommit()
    {
        PCardRegisterCommit(_pCardRegisterCaret.PRegisterCaretText);
        PCardRegisterClear();
    }

    internal bool PCardRegisterCommit(long id, string name)
    {
        string written = (name ?? string.Empty).Trim();
        if (written.Length == 0 || PCardRegisterMatch(id) || PCardRegisterCheck(written))
        {
            return false;
        }

        int index = PCardRegister.IndexOf(_pCardRegisterCaret);
        PCardRegister.Insert(
            index < 0 ? PCardRegister.Count : index,
            new PRegister(LStateValue.LStateValueRead(written), id));
        PCardRegisterUpdate();
        return true;
    }

    internal void PCardRegisterClear()
    {
        _pCardRegisterBusy = true;
        _pCardRegisterCaret.PRegisterCaretText = string.Empty;
        _pCardRegisterBusy = false;
        PCardRegisterUpdate();
    }

    internal bool PCardRegisterMatch(long id)
    {
        if (id <= 0)
        {
            return false;
        }

        foreach (object row in PCardRegister)
        {
            if (row is PRegister chip &&
                chip.PRegisterId == id)
            {
                return true;
            }
        }

        return false;
    }

    private void PCardRegisterStart()
    {
        _pCardRegisterCaret.PropertyChanged += PCardRegisterChange;
        PCardRegister.Add(_pCardRegisterCaret);
        PCardRegisterUpdate();
    }

    private void PCardRegisterChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardRegisterBusy ||
            !string.Equals(
                arguments.PropertyName, nameof(PRegisterCaret.PRegisterCaretText), StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardRegisterCaret.PRegisterCaretText;
        if (written.IndexOf(',', StringComparison.Ordinal) >= 0)
        {
            _pCardRegisterBusy = true;
            string[] parts = written.Split(',');
            for (int index = 0; index < parts.Length - 1; index++)
            {
                PCardRegisterCommit(parts[index]);
            }

            _pCardRegisterCaret.PRegisterCaretText = parts[^1].TrimStart();
            _pCardRegisterBusy = false;
            PCardRegisterUpdate();
        }

        PCardRegisterNotice?.Invoke(_pCardRegisterCaret.PRegisterCaretText);
    }

    private void PCardRegisterCommit(string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || PCardRegisterCheck(written))
        {
            return;
        }

        int index = PCardRegister.IndexOf(_pCardRegisterCaret);
        PCardRegister.Insert(index < 0 ? PCardRegister.Count : index, new PRegister(written));
    }

    private bool PCardRegisterCheck(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        foreach (object row in PCardRegister)
        {
            if (row is PRegister chip &&
                string.Equals(chip.PRegisterText, text, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardRegisterUpdate()
    {
        _pCardRegisterCaret.PRegisterCaretHint = PCardRegister.Count > 1
            ? string.Empty
            : PLocalizationCatalog.PLocalizationCatalogCurrent[PCardRegisterHint];
    }
}
