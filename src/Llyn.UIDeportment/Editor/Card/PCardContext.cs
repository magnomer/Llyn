using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    private const string PCardContextHint = "Card.SituationHint";

    private readonly PContextCaret _pCardContextCaret = new();
    private bool _pCardContextBusy;

    private static readonly Func<object, long?> _pCardContextKey =
        row => row is PContext chip ? chip.PContextId : null;

    public ObservableCollection<object> PCardContext { get; } = [];

    internal Action<string>? PCardContextNotice { get; set; }

    internal Func<string, bool>? PCardContextDispatcher { get; set; }

    internal string PCardContextText => _pCardContextCaret.PContextCaretText;

    internal int PCardContextPosition =>
        PCardRowResolve(PCardContext, _pCardContextKey, PCardContext.IndexOf(_pCardContextCaret));

    internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)
    {
        PCardRowShow(
            PCardContext,
            drafts,
            _pCardContextKey,
            static draft => draft.LSituationDraftId,
            PCardContextCreate,
            static (row, draft) =>
                row is PContext chip
                    ? draft.LSituationDraftTitle.LStateValueMatch(chip.PContextText) ? row : PCardContextCreate(draft)
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
        _pCardContextBusy = true;
        _pCardContextCaret.PContextCaretText = string.Empty;
        _pCardContextBusy = false;
        PCardContextUpdate();
    }

    internal bool PCardContextMatch(long? id)
    {
        if (id is null or <= 0)
        {
            return false;
        }

        foreach (object row in PCardContext)
        {
            if (row is PContext chip && chip.PContextId == id)
            {
                return true;
            }
        }

        return false;
    }

    internal bool PCardContextCheck(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        foreach (object row in PCardContext)
        {
            if (row is not PContext chip)
            {
                continue;
            }

            if (chip.PContextText.LStateValueMatch(text))
            {
                return true;
            }
        }

        return false;
    }

    private static object PCardContextCreate(LSituationDraft draft)
    {
        return new PContext(draft.LSituationDraftTitle, draft.LSituationDraftId);
    }

    private void PCardContextStart()
    {
        _pCardContextCaret.PropertyChanged += PCardContextChange;
        PCardContext.Add(_pCardContextCaret);
        PCardContextUpdate();
    }

    private void PCardContextChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (_pCardContextBusy ||
            !string.Equals(arguments.PropertyName, nameof(PContextCaret.PContextCaretText), StringComparison.Ordinal))
        {
            return;
        }

        string written = _pCardContextCaret.PContextCaretText;
        if (written.IndexOf(',', StringComparison.Ordinal) >= 0)
        {
            _pCardContextBusy = true;
            string[] parts = written.Split(',');
            for (int index = 0; index < parts.Length - 1; index++)
            {
                PCardContextDispatcher?.Invoke(parts[index]);
            }

            _pCardContextCaret.PContextCaretText = parts[^1].TrimStart();
            _pCardContextBusy = false;
            PCardContextUpdate();
        }

        PCardContextNotice?.Invoke(_pCardContextCaret.PContextCaretText);
    }

    private void PCardContextUpdate()
    {
        _pCardContextCaret.PContextCaretHint = PCardContext.Count > 1
            ? string.Empty
            : PLocalizationCatalog.PLocalizationCatalogCurrent[PCardContextHint];
    }
}
