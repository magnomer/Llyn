using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardContextHint = "Card.SituationHint";

    private readonly PContextCaret _pCardContextCaret = new();

    private static readonly Func<PContext, long?> _pCardContextKey =
        static chip => chip.PContextId;

    public ObservableCollection<PContext> PCardContext { get; } = [];

    internal string PCardContextText => _pCardContextCaret.PContextCaretText;

    internal PContextCaret PCardContextCaret => _pCardContextCaret;

    internal int PCardContextPosition =>
        PCardCaretFind(PCardContext, _pCardContextCaret.PContextCaretAnchor);

    internal void PCardContextShow(IReadOnlyList<CSituationDraft> drafts)
    {
        List<PContext> trail = PCardCaretRead(PCardContext, _pCardContextCaret.PContextCaretAnchor);
        PCardRowShow(
            PCardContext,
            drafts,
            _pCardContextKey,
            static draft => draft.CSituationDraftId,
            PCardContextCreate,
            static (row, draft) =>
                draft.CSituationDraftWording == row.PContextText ? row : PCardContextCreate(draft));
        _pCardContextCaret.PContextCaretAnchor = PCardCaretResolve(PCardContext, trail);

        PCardContextUpdate();
    }

    internal PContext? PCardContextFind(int step)
    {
        int target = PCardContextPosition + (step < 0 ? step : step - 1);
        if (target < 0 || target >= PCardContext.Count)
        {
            return null;
        }

        return PCardContext[target];
    }

    internal bool PCardContextMove(int step)
    {
        int target = PCardContextPosition + step;
        if (target < 0 || target > PCardContext.Count)
        {
            return false;
        }

        _pCardContextCaret.PContextCaretAnchor = target < PCardContext.Count ? PCardContext[target] : null;
        return true;
    }

    internal void PCardContextClear()
    {
        _pCardContextCaret.PContextCaretText = string.Empty;
        PCardContextUpdate();
    }

    private static PContext PCardContextCreate(CSituationDraft draft)
    {
        return new PContext(draft.CSituationDraftWording, draft.CSituationDraftId);
    }

    private void PCardContextStart()
    {
        PCardContextUpdate();
    }

    internal void PCardContextRefine(string rest)
    {
        if (!string.Equals(_pCardContextCaret.PContextCaretText, rest, StringComparison.Ordinal))
        {
            _pCardContextCaret.PContextCaretText = rest;
            PCardContextUpdate();
        }
    }

    private void PCardContextUpdate()
    {
        _pCardContextCaret.PContextCaretHint = PCardContext.Count > 0
            ? string.Empty
            : QLocalizationCatalog.QLocalizationTextRead(PCardContextHint);
    }
}
