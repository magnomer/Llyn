using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardRegisterHint = "Card.RegisterHint";

    private readonly PRegisterCaret _pCardRegisterCaret = new();

    private static readonly Func<object, long?> _pCardRegisterKey =
        row => row is PRegister chip ? chip.PRegisterId : null;

    public ObservableCollection<object> PCardRegister { get; } = [];

    internal string PCardRegisterText => _pCardRegisterCaret.PRegisterCaretText;

    internal int PCardRegisterPosition =>
        PCardRowResolve(PCardRegister, _pCardRegisterKey, PCardRegister.IndexOf(_pCardRegisterCaret));

    internal void PCardRegisterShow(IReadOnlyList<CRegisterDraft> drafts)
    {
        PCardRowShow(
            PCardRegister,
            drafts,
            _pCardRegisterKey,
            static draft => draft.CRegisterDraftId,
            PCardRegisterCreate,
            static (row, draft) =>
                row is PRegister chip
                    ? draft.CRegisterDraftName == chip.PRegisterText ? row : PCardRegisterCreate(draft)
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
        _pCardRegisterCaret.PRegisterCaretText = string.Empty;
        PCardRegisterUpdate();
    }

    private static object PCardRegisterCreate(CRegisterDraft draft)
    {
        return new PRegister(draft.CRegisterDraftName, draft.CRegisterDraftId);
    }

    private void PCardRegisterStart()
    {
        PCardRegister.Add(_pCardRegisterCaret);
        PCardRegisterUpdate();
    }

    internal void PCardRegisterRefine(string rest)
    {
        if (!string.Equals(_pCardRegisterCaret.PRegisterCaretText, rest, StringComparison.Ordinal))
        {
            _pCardRegisterCaret.PRegisterCaretText = rest;
            PCardRegisterUpdate();
        }
    }

    private void PCardRegisterUpdate()
    {
        _pCardRegisterCaret.PRegisterCaretHint = PCardRegister.Count > 1
            ? string.Empty
            : QLocalizationCatalog.QLocalizationCatalogCurrent[PCardRegisterHint];
    }
}
