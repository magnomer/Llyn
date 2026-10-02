using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardRegisterHint = "Card.RegisterHint";

    private readonly PRegisterCaret _pCardRegisterCaret = new();

    private static readonly Func<PRegister, long?> _pCardRegisterKey =
        static chip => chip.PRegisterId;

    public ObservableCollection<PRegister> PCardRegister { get; } = [];

    internal string PCardRegisterText => _pCardRegisterCaret.PRegisterCaretText;

    internal PRegisterCaret PCardRegisterCaret => _pCardRegisterCaret;

    internal int PCardRegisterPosition =>
        PCardCaretFind(PCardRegister, _pCardRegisterCaret.PRegisterCaretAnchor);

    internal void PCardRegisterShow(IReadOnlyList<CRegisterDraft> drafts)
    {
        List<PRegister> trail = PCardCaretRead(PCardRegister, _pCardRegisterCaret.PRegisterCaretAnchor);
        PCardRowShow(
            PCardRegister,
            drafts,
            _pCardRegisterKey,
            static draft => draft.CRegisterDraftId,
            PCardRegisterCreate,
            static (row, draft) =>
                draft.CRegisterDraftName == row.PRegisterText ? row : PCardRegisterCreate(draft));
        _pCardRegisterCaret.PRegisterCaretAnchor = PCardCaretResolve(PCardRegister, trail);

        PCardRegisterUpdate();
    }

    internal PRegister? PCardRegisterFind(int step)
    {
        int target = PCardRegisterPosition + (step < 0 ? step : step - 1);
        if (target < 0 || target >= PCardRegister.Count)
        {
            return null;
        }

        return PCardRegister[target];
    }

    internal bool PCardRegisterMove(int step)
    {
        int target = PCardRegisterPosition + step;
        if (target < 0 || target > PCardRegister.Count)
        {
            return false;
        }

        _pCardRegisterCaret.PRegisterCaretAnchor = target < PCardRegister.Count ? PCardRegister[target] : null;
        return true;
    }

    internal void PCardRegisterClear()
    {
        _pCardRegisterCaret.PRegisterCaretText = string.Empty;
        PCardRegisterUpdate();
    }

    private static PRegister PCardRegisterCreate(CRegisterDraft draft)
    {
        return new PRegister(draft.CRegisterDraftName, draft.CRegisterDraftId);
    }

    private void PCardRegisterStart()
    {
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
        _pCardRegisterCaret.PRegisterCaretHint = PCardRegister.Count > 0
            ? string.Empty
            : QLocalizationCatalog.QLocalizationTextRead(PCardRegisterHint);
    }
}
