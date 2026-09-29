using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardContextHint = "Card.SituationHint";

    private readonly PContextCaret _pCardContextCaret = new();

    private static readonly Func<object, long?> _pCardContextKey =
        row => row is PContext chip ? chip.PContextId : null;

    public ObservableCollection<object> PCardContext { get; } = [];

    internal event Action<PCard, string>? PCardContextNotice;

    internal string PCardContextText => _pCardContextCaret.PContextCaretText;

    internal int PCardContextPosition =>
        PCardRowResolve(PCardContext, _pCardContextKey, PCardContext.IndexOf(_pCardContextCaret));

    internal void PCardContextShow(IReadOnlyList<CSituationDraft> drafts)
    {
        PCardRowShow(
            PCardContext,
            drafts,
            _pCardContextKey,
            static draft => draft.CSituationDraftId,
            PCardContextCreate,
            static (row, draft) =>
                row is PContext chip
                    ? draft.CSituationDraftTitle == chip.PContextText ? row : PCardContextCreate(draft)
                    : PCardContextCreate(draft));

        PCardContextUpdate();
    }

    internal PContext? PCardContextFind(int step)
    {
        int index = PCardContext.IndexOf(_pCardContextCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardContext.Count)
        {
            return null;
        }

        return PCardContext[target] as PContext;
    }

    internal bool PCardContextMove(int step)
    {
        int index = PCardContext.IndexOf(_pCardContextCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardContext.Count)
        {
            return false;
        }

        PCardContext.Move(index, target);
        return true;
    }

    internal void PCardContextClear()
    {
        _pCardContextCaret.PContextCaretText = string.Empty;
        PCardContextUpdate();
    }

    private static object PCardContextCreate(CSituationDraft draft)
    {
        return new PContext(draft.CSituationDraftTitle, draft.CSituationDraftId);
    }

    private void PCardContextStart()
    {
        PCardContext.Add(_pCardContextCaret);
        PCardContextUpdate();
    }

    internal void PCardContextRefine(string rest)
    {
        if (!string.Equals(_pCardContextCaret.PContextCaretText, rest, StringComparison.Ordinal))
        {
            _pCardContextCaret.PContextCaretText = rest;
            PCardContextUpdate();
        }

        PCardContextNotice?.Invoke(this, _pCardContextCaret.PContextCaretText);
    }

    private void PCardContextUpdate()
    {
        _pCardContextCaret.PContextCaretHint = PCardContext.Count > 1
            ? string.Empty
            : QLocalizationCatalog.QLocalizationCatalogCurrent[PCardContextHint];
    }
}
