using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    private const string PCardContextHint = "Card.SituationHint";

    private readonly PContextCaret _pCardContextCaret = new();
    private bool _pCardContextBusy;

    public ObservableCollection<object> PCardContext { get; } = [];

    internal Action<string>? PCardContextNotice { get; set; }

    internal string PCardContextText => _pCardContextCaret.PContextCaretText;

    internal void PCardContextShow(IReadOnlyList<LSituationDraft> drafts)
    {
        PCardContext.Clear();
        foreach (LSituationDraft draft in drafts)
        {
            PContext chip = new(
                draft.LSituationDraftTitle,
                draft.LSituationDraftId,
                draft.LSituationDraftDescription,
                draft.LSituationDraftKind);
            if (chip.PContextTextRead().LStateValueEmpty || PCardContextCheck(chip.PContextText))
            {
                continue;
            }

            PCardContext.Add(chip);
        }

        _pCardContextCaret.PContextCaretText = string.Empty;
        PCardContext.Add(_pCardContextCaret);
        PCardContextUpdate();
    }

    internal IReadOnlyList<LSituationDraft> PCardContextRead()
    {
        List<LSituationDraft> drafts = [];
        foreach (object row in PCardContext)
        {
            if (row is PContext chip)
            {
                drafts.Add(new LSituationDraft(
                    chip.PContextTextRead(),
                    chip.PContextId,
                    chip.PContextDescription,
                    chip.PContextKind));
                continue;
            }

            string written = _pCardContextCaret.PContextCaretText.Trim();
            if (written.Length != 0 && !PCardContextCheck(written))
            {
                drafts.Add(LSituationDraft.LSituationDraftCreate(written));
            }
        }

        return drafts;
    }

    internal void PCardContextRemove(PContext chip)
    {
        PCardContext.Remove(chip);
        PCardContextUpdate();
    }

    internal void PCardContextRemove(int step)
    {
        int index = PCardContext.IndexOf(_pCardContextCaret);
        int target = index + step;
        if (index < 0 || target < 0 || target >= PCardContext.Count || PCardContext[target] is not PContext chip)
        {
            return;
        }

        PCardContextRemove(chip);
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

    internal void PCardContextCommit()
    {
        PCardContextCommit(_pCardContextCaret.PContextCaretText);
        PCardContextClear();
    }

    internal bool PCardContextCommit(long id, string title)
    {
        string written = (title ?? string.Empty).Trim();
        if (written.Length == 0 || PCardContextMatch(id) || PCardContextCheck(written))
        {
            return false;
        }

        int index = PCardContext.IndexOf(_pCardContextCaret);
        PCardContext.Insert(
            index < 0 ? PCardContext.Count : index,
            new PContext(LStateValue.LStateValueRead(written), id));
        PCardContextUpdate();
        return true;
    }

    internal void PCardContextClear()
    {
        _pCardContextBusy = true;
        _pCardContextCaret.PContextCaretText = string.Empty;
        _pCardContextBusy = false;
        PCardContextUpdate();
    }

    internal bool PCardContextMatch(long id)
    {
        if (id <= 0)
        {
            return false;
        }

        foreach (object row in PCardContext)
        {
            if (row is PContext chip &&
                chip.PContextId == id)
            {
                return true;
            }
        }

        return false;
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
                PCardContextCommit(parts[index]);
            }

            _pCardContextCaret.PContextCaretText = parts[^1].TrimStart();
            _pCardContextBusy = false;
            PCardContextUpdate();
        }

        PCardContextNotice?.Invoke(_pCardContextCaret.PContextCaretText);
    }

    private void PCardContextCommit(string text)
    {
        string written = (text ?? string.Empty).Trim();
        if (written.Length == 0 || PCardContextCheck(written))
        {
            return;
        }

        int index = PCardContext.IndexOf(_pCardContextCaret);
        PCardContext.Insert(index < 0 ? PCardContext.Count : index, new PContext(written));
    }

    private bool PCardContextCheck(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        foreach (object row in PCardContext)
        {
            if (row is PContext chip &&
                string.Equals(chip.PContextText, text, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void PCardContextUpdate()
    {
        _pCardContextCaret.PContextCaretHint = PCardContext.Count > 1
            ? string.Empty
            : PLocalizationCatalog.PLocalizationCatalogCurrent[PCardContextHint];
    }
}
