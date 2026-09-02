using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PCard : INotifyPropertyChanged
{
    internal const string PCardUnreadableMark = "(?)";

    private readonly string _pCardPrefix;
    private readonly ObservableCollection<PReference> _pCardReference;
    private int _pCardOrder;
    private string _pTitle;
    private bool _pTitleUnreadable;
    private string _pCardDefinition;
    private bool _pCardDefinitionUnreadable;
    private string _pCardExpression;
    private bool _pCardExpressionUnreadable;
    private string _pCardTag;

    internal PCard(string prefix, int order, ObservableCollection<PReference> catalog)
    {
        _pCardPrefix = prefix;
        _pCardOrder = order;
        _pCardReference = catalog;
        _pTitle = string.Empty;
        _pCardDefinition = string.Empty;
        _pCardExpression = string.Empty;
        _pCardTag = string.Empty;
        PCardExample = [];
        PCardExampleAdd(new PExample(catalog));
        PCardExampleUpdate();
        PCardSituation = [];
        PCardSituationAdd(new PSituation(catalog));
        PCardSituationUpdate();
    }

    public string PCardId { get; set; } = string.Empty;

    public int PCardOrder
    {
        get => _pCardOrder;
        set
        {
            if (_pCardOrder == value)
            {
                return;
            }

            _pCardOrder = value;
            PCardRaise(nameof(PCardOrder));
            PCardRaise(nameof(PCardTitle));
        }
    }

    public string PCardTitle => $"{_pCardPrefix} {_pCardOrder}";

    public string PTitle
    {
        get => _pTitle;
        set
        {
            if (string.Equals(_pTitle, value, StringComparison.Ordinal))
            {
                return;
            }

            _pTitle = value;
            PTitleUnreadable = false;
            PCardRaise(nameof(PTitle));
        }
    }

    public bool PTitleUnreadable
    {
        get => _pTitleUnreadable;
        private set
        {
            if (_pTitleUnreadable == value)
            {
                return;
            }

            _pTitleUnreadable = value;
            PCardRaise(nameof(PTitleUnreadable));
        }
    }

    public string PCardDefinition
    {
        get => _pCardDefinition;
        set
        {
            if (string.Equals(_pCardDefinition, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardDefinition = value;
            PCardDefinitionUnreadable = false;
            PCardRaise(nameof(PCardDefinition));
        }
    }

    public bool PCardDefinitionUnreadable
    {
        get => _pCardDefinitionUnreadable;
        private set
        {
            if (_pCardDefinitionUnreadable == value)
            {
                return;
            }

            _pCardDefinitionUnreadable = value;
            PCardRaise(nameof(PCardDefinitionUnreadable));
        }
    }

    public string PCardExpression
    {
        get => _pCardExpression;
        set
        {
            if (string.Equals(_pCardExpression, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardExpression = value;
            PCardExpressionUnreadable = false;
            PCardRaise(nameof(PCardExpression));
        }
    }

    public bool PCardExpressionUnreadable
    {
        get => _pCardExpressionUnreadable;
        private set
        {
            if (_pCardExpressionUnreadable == value)
            {
                return;
            }

            _pCardExpressionUnreadable = value;
            PCardRaise(nameof(PCardExpressionUnreadable));
        }
    }

    public ObservableCollection<PExample> PCardExample { get; }

    public ObservableCollection<PSituation> PCardSituation { get; }

    public string PCardTag
    {
        get => _pCardTag;
        set
        {
            if (string.Equals(_pCardTag, value, StringComparison.Ordinal))
            {
                return;
            }

            _pCardTag = value;
            PCardRaise(nameof(PCardTag));
        }
    }

    internal LStateValue PCardTitleRead()
    {
        return PCardValueRead(_pTitleUnreadable, _pTitle);
    }

    internal LStateValue PCardDefinitionRead()
    {
        return PCardValueRead(_pCardDefinitionUnreadable, _pCardDefinition);
    }

    internal LStateValue PCardExpressionRead()
    {
        return PCardValueRead(_pCardExpressionUnreadable, _pCardExpression);
    }

    internal void PCardTitleShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pTitle = value.LStateValueShow();
        PCardRaise(nameof(PTitle));
        PTitleUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardDefinitionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardDefinition = value.LStateValueShow();
        PCardRaise(nameof(PCardDefinition));
        PCardDefinitionUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardExpressionShow(LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _pCardExpression = value.LStateValueShow();
        PCardRaise(nameof(PCardExpression));
        PCardExpressionUnreadable = value.LStateValueState == LState.LStateUnknown;
    }

    internal void PCardExampleShow(IReadOnlyList<LExampleDraft> drafts)
    {
        foreach (PExample row in PCardExample)
        {
            row.PropertyChanged -= PCardExampleChange;
        }

        PCardExample.Clear();
        foreach (LExampleDraft draft in drafts)
        {
            PCardExampleAdd(new PExample(
                _pCardReference,
                draft.LExampleDraftText,
                draft.LExampleDraftId,
                draft.LExampleDraftReference));
        }

        if (PCardExample.Count == 0)
        {
            PCardExampleAdd(new PExample(_pCardReference));
        }

        PCardExampleUpdate();
    }

    internal IReadOnlyList<LExampleDraft> PCardExampleRead()
    {
        List<LExampleDraft> drafts = [];
        foreach (PExample row in PCardExample)
        {
            LStateValue text = row.PExampleTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LExampleDraft(text, row.PExampleId, row.PExampleReferenceRead()));
        }

        return drafts;
    }

    internal void PCardExampleInsert(PExample row)
    {
        int index = PCardExample.IndexOf(row);
        PExample opened = new(_pCardReference);
        opened.PropertyChanged += PCardExampleChange;
        PCardExample.Insert(index < 0 ? PCardExample.Count : index + 1, opened);
        PCardExampleUpdate();
    }

    internal void PCardExampleRemove(PExample row)
    {
        if (PCardExample.Count <= 1)
        {
            row.PExampleClear();
            return;
        }

        row.PropertyChanged -= PCardExampleChange;
        PCardExample.Remove(row);
        PCardExampleUpdate();
    }

    internal void PCardExampleUpdate()
    {
        bool numbered = PCardExample.Count > 1;
        for (int index = 0; index < PCardExample.Count; index++)
        {
            PCardExample[index].PExampleOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    internal void PCardSituationShow(IReadOnlyList<LSituationDraft> drafts)
    {
        foreach (PSituation row in PCardSituation)
        {
            row.PropertyChanged -= PCardSituationChange;
        }

        PCardSituation.Clear();
        foreach (LSituationDraft draft in drafts)
        {
            PCardSituationAdd(new PSituation(
                _pCardReference,
                draft.LSituationDraftText,
                draft.LSituationDraftId,
                draft.LSituationDraftReference));
        }

        if (PCardSituation.Count == 0)
        {
            PCardSituationAdd(new PSituation(_pCardReference));
        }

        PCardSituationUpdate();
    }

    internal IReadOnlyList<LSituationDraft> PCardSituationRead()
    {
        List<LSituationDraft> drafts = [];
        foreach (PSituation row in PCardSituation)
        {
            LStateValue text = row.PSituationTextRead();
            if (text.LStateValueEmpty)
            {
                continue;
            }

            drafts.Add(new LSituationDraft(text, row.PSituationId, row.PSituationReferenceRead()));
        }

        return drafts;
    }

    internal void PCardSituationInsert(PSituation row)
    {
        int index = PCardSituation.IndexOf(row);
        PSituation opened = new(_pCardReference);
        opened.PropertyChanged += PCardSituationChange;
        PCardSituation.Insert(index < 0 ? PCardSituation.Count : index + 1, opened);
        PCardSituationUpdate();
    }

    internal void PCardSituationRemove(PSituation row)
    {
        if (PCardSituation.Count <= 1)
        {
            row.PSituationClear();
            return;
        }

        row.PropertyChanged -= PCardSituationChange;
        PCardSituation.Remove(row);
        PCardSituationUpdate();
    }

    internal void PCardSituationUpdate()
    {
        bool numbered = PCardSituation.Count > 1;
        for (int index = 0; index < PCardSituation.Count; index++)
        {
            PCardSituation[index].PSituationOrderText = numbered
                ? $"({index + 1})"
                : string.Empty;
        }
    }

    private static LStateValue PCardValueRead(bool unreadable, string text)
    {
        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueRead(text);
    }

    private void PCardExampleAdd(PExample row)
    {
        row.PropertyChanged += PCardExampleChange;
        PCardExample.Add(row);
    }

    private void PCardExampleChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PExample row &&
            string.Equals(arguments.PropertyName, nameof(PExample.PExampleText), StringComparison.Ordinal))
        {
            row.PExampleIdentityApply();
        }
    }

    private void PCardSituationAdd(PSituation row)
    {
        row.PropertyChanged += PCardSituationChange;
        PCardSituation.Add(row);
    }

    private void PCardSituationChange(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is PSituation row &&
            string.Equals(arguments.PropertyName, nameof(PSituation.PSituationText), StringComparison.Ordinal))
        {
            row.PSituationIdentityApply();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void PCardRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
