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

    private static readonly Func<object, long?> _pCardRegisterKey =
        row => row is PRegister chip ? chip.PRegisterId : null;

    public ObservableCollection<object> PCardRegister { get; } = [];

    internal Action<string>? PCardRegisterNotice { get; set; }

    internal Func<string, bool>? PCardRegisterDispatcher { get; set; }

    internal string PCardRegisterText => _pCardRegisterCaret.PRegisterCaretText;

    internal int PCardRegisterPosition =>
        PCardRowResolve(PCardRegister, _pCardRegisterKey, PCardRegister.IndexOf(_pCardRegisterCaret));

    internal void PCardRegisterShow(IReadOnlyList<LRegisterDraft> drafts)
    {
        PCardRowShow(
            PCardRegister,
            drafts,
            _pCardRegisterKey,
            static draft => draft.LRegisterDraftId,
            PCardRegisterCreate,
            static (row, draft) =>
                row is PRegister chip && chip.PRegisterTextRead().LStateWrittenMatch(draft.LRegisterDraftName)
                    ? row
                    : PCardRegisterCreate(draft));

        PCardRegisterUpdate();
    }

    internal PRegister? PCardRegisterFind(int step)
    {
        int index = PCardRegister.IndexOf(_pCardRegisterCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardRegister.Count)
        {
            return null;
        }

        return PCardRegister[target] as PRegister;
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

    internal void PCardRegisterClear()
    {
        _pCardRegisterBusy = true;
        _pCardRegisterCaret.PRegisterCaretText = string.Empty;
        _pCardRegisterBusy = false;
        PCardRegisterUpdate();
    }

    internal bool PCardRegisterMatch(long? id)
    {
        if (id is null or <= 0)
        {
            return false;
        }

        foreach (object row in PCardRegister)
        {
            if (row is PRegister chip && chip.PRegisterId == id)
            {
                return true;
            }
        }

        return false;
    }

    internal bool PCardRegisterCheck(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        foreach (object row in PCardRegister)
        {
            if (row is PRegister chip && string.Equals(chip.PRegisterText, text, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static object PCardRegisterCreate(LRegisterDraft draft)
    {
        return new PRegister(draft.LRegisterDraftName, draft.LRegisterDraftId);
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
                PCardRegisterDispatcher?.Invoke(parts[index]);
            }

            _pCardRegisterCaret.PRegisterCaretText = parts[^1].TrimStart();
            _pCardRegisterBusy = false;
            PCardRegisterUpdate();
        }

        PCardRegisterNotice?.Invoke(_pCardRegisterCaret.PRegisterCaretText);
    }

    private void PCardRegisterUpdate()
    {
        _pCardRegisterCaret.PRegisterCaretHint = PCardRegister.Count > 1
            ? string.Empty
            : PLocalizationCatalog.PLocalizationCatalogCurrent[PCardRegisterHint];
    }
}
