using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QReflexItem : INotifyPropertyChanged
{
    private string _qReflexItemLanguage = string.Empty;
    private string _qReflexItemKind = string.Empty;
    private string _qReflexItemText = string.Empty;
    private string _qReflexItemRomanization = string.Empty;
    private string _qReflexItemMeaning = string.Empty;
    private string _qReflexItemNote = string.Empty;
    private string _qReflexItemRegion = string.Empty;
    private string _qReflexItemOpener = string.Empty;
    private string _qReflexItemCloser = string.Empty;
    private bool _qReflexItemMain;
    private bool _qReflexItemLead;
    private bool _qReflexItemHidden;
    private string _qReflexItemAnchor = string.Empty;
    private bool _qReflexItemAnchorable;
    private string _qReflexItemTitle = string.Empty;
    private string _qReflexItemRubric = string.Empty;
    private CReflex _qReflexItemReflex;

    public QReflexItem(CReflex reflex)
    {
        ArgumentNullException.ThrowIfNull(reflex);

        QReflexItemId = reflex.CReflexId;
        _qReflexItemReflex = reflex;
        QReflexStateRefine(reflex);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event Action<QReflexItem, CReflexField, string>? QReflexItemTyped;

    public long QReflexItemId { get; }

    public bool QReflexItemFolded { get; private set; }

    public string QReflexItemOpener
    {
        get => _qReflexItemOpener;
        private set => QReflexValueRefine(ref _qReflexItemOpener, value, nameof(QReflexItemOpener));
    }

    public string QReflexItemCloser
    {
        get => _qReflexItemCloser;
        private set => QReflexValueRefine(ref _qReflexItemCloser, value, nameof(QReflexItemCloser));
    }

    public string QReflexItemKind
    {
        get => _qReflexItemKind;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldKind, value);
    }

    public string QReflexItemText
    {
        get => _qReflexItemText;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldText, value);
    }

    public string QReflexItemRomanization
    {
        get => _qReflexItemRomanization;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldRomanization, value);
    }

    public string QReflexItemMeaning
    {
        get => _qReflexItemMeaning;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldMeaning, value);
    }

    public string QReflexItemNote
    {
        get => _qReflexItemNote;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldNote, value);
    }

    public bool QReflexItemMain
    {
        get => _qReflexItemMain;
        set
        {
            if (_qReflexItemMain == value)
            {
                return;
            }

            _qReflexItemMain = value;
            QReflexChangeRefine(nameof(QReflexItemMain));
        }
    }

    public bool QReflexItemLead
    {
        get => _qReflexItemLead;
        set
        {
            if (_qReflexItemLead == value)
            {
                return;
            }

            _qReflexItemLead = value;
            QReflexChangeRefine(nameof(QReflexItemLead));
            QReflexChangeRefine(nameof(QReflexItemHead));
            QReflexChangeRefine(nameof(QReflexItemLabel));
            QReflexChangeRefine(nameof(QReflexItemArea));
        }
    }

    public bool QReflexItemHidden
    {
        get => _qReflexItemHidden;
        set
        {
            if (_qReflexItemHidden == value)
            {
                return;
            }

            _qReflexItemHidden = value;
            QReflexChangeRefine(nameof(QReflexItemHidden));
        }
    }

    public string QReflexItemAnchor
    {
        get => _qReflexItemAnchor;
        set => QReflexValueRefine(ref _qReflexItemAnchor, value, nameof(QReflexItemAnchor));
    }

    public bool QReflexItemAnchorable
    {
        get => _qReflexItemAnchorable;
        set
        {
            if (_qReflexItemAnchorable == value)
            {
                return;
            }

            _qReflexItemAnchorable = value;
            QReflexChangeRefine(nameof(QReflexItemAnchorable));
        }
    }

    public string QReflexItemHead
    {
        get => _qReflexItemLead ? _qReflexItemLanguage : string.Empty;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldLanguage, value);
    }

    public string QReflexItemLabel =>
        _qReflexItemLead ? QReflexLabelRefine(_qReflexItemTitle, _qReflexItemLanguage) : string.Empty;

    public string QReflexItemArea => _qReflexItemLead ? _qReflexItemRegion : string.Empty;

    public string QReflexItemTag => QReflexLabelRefine(_qReflexItemRubric, _qReflexItemKind);

    internal static void QReflexItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QReflexItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Border>(container, "PReflexSurface") is Border surface)
        {
            surface.Visibility = row.QReflexItemHidden ? Visibility.Hidden : Visibility.Visible;
            if (row.QReflexItemHidden)
            {
                surface.Height = 0;
            }
            else
            {
                surface.ClearValue(FrameworkElement.HeightProperty);
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PReflexLabel") is TextBlock label)
        {
            label.Text = row.QReflexItemLabel;
            label.ToolTip = row.QReflexItemArea.Length > 0 ? row.QReflexItemArea : null;
        }

        QReflexTextRefine(container, "PReflexTag", row.QReflexItemTag, false);
        QReflexTextRefine(container, "PReflexOpener", row.QReflexItemOpener, row.QReflexItemMain);
        QReflexTextRefine(container, "PReflexCloser", row.QReflexItemCloser, row.QReflexItemMain);
        QReflexTextRefine(container, "PReflexRomanization", row.QReflexItemRomanization, false);
        QReflexTextRefine(container, "PReflexMeaning", row.QReflexItemMeaning, false);
        QReflexTextRefine(container, "PReflexNote", row.QReflexItemNote, false);
        QReflexTextRefine(container, "PReflexAnchor", row.QReflexItemAnchor, false);
        Grid? cell = QLook.QLookPartFind<Grid>(container, "PReflexCell");
        if (cell is not null)
        {
            cell.SetValue(
                QQuill.QQuillCellProperty,
                row.QReflexItemMain
                    ? new QQuill.QQuillCell("Theme.Reflex.Lead", "QReflexItemText", "Input.Reflex")
                    : new QQuill.QQuillCell("Theme.Reflex.Field", "QReflexItemText", "Input.Reflex"));
            (string, QQuill.QQuillCell)[] asides =
            [
                ("PReflexLabel", new("Theme.Reflex.Name", "QReflexItemHead", null)),
                ("PReflexTag", new("Theme.Reflex.Name", "QReflexItemKind", null)),
                ("PReflexRomanization", new("Theme.Reflex.Aside", "QReflexItemRomanization", null)),
                ("PReflexMeaning", new("Theme.Reflex.Aside", "QReflexItemMeaning", "Input.Meaning")),
                ("PReflexNote", new("Theme.Reflex.Aside", "QReflexItemNote", null)),
            ];
            foreach ((string name, QQuill.QQuillCell order) in asides)
            {
                if (QLook.QLookPartFind<TextBlock>(container, name)?.Parent is Grid aside)
                {
                    aside.SetValue(QQuill.QQuillCellProperty, order);
                }
            }

            if (QLook.QLookPartFind<ContentControl>(container, "PAccentShelf") is ContentControl shelf)
            {
                shelf.Tag = "Theme.Reflex.Control";
            }
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PReflexText") is TextBlock text)
        {
            if (cell is null)
            {
                QReflexTextRefine(container, "PReflexText", row.QReflexItemText, row.QReflexItemMain);
            }
            else
            {
                string? ink = QLook.QLookFirstRead<string?>(row.QReflexItemMain, "Theme.Accent", null);
                QLook.QLookPromptApply(text, row.QReflexItemText, "Input.Reflex", ink);
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PReflexAnchoring") is Button anchoring)
        {
            anchoring.Visibility = QLook.QLookVisibleRead(row.QReflexItemAnchorable);
            anchoring.Content = row.QReflexItemAnchor;
        }
    }

    private static void QReflexTextRefine(FrameworkElement container, string name, string text, bool main)
    {
        if (QLook.QLookPartFind<TextBlock>(container, name) is not TextBlock block)
        {
            return;
        }

        block.Text = text;
        if (main)
        {
            block.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Accent");
        }
        else
        {
            block.ClearValue(TextBlock.ForegroundProperty);
        }
    }

    internal void QReflexStateRefine(CReflex reflex)
    {
        ArgumentNullException.ThrowIfNull(reflex);

        _qReflexItemReflex = reflex;
        _qReflexItemTitle = reflex.CReflexLanguageKey;
        _qReflexItemRubric = reflex.CReflexKindKey;
        QReflexItemFolded = reflex.CReflexFolded;
        QReflexItemOpener = reflex.CReflexMark.CRespellingMarkOpener;
        QReflexItemCloser = reflex.CReflexMark.CRespellingMarkCloser;
        QReflexItemMain = reflex.CReflexMain;
        QReflexValueRefine(
            ref _qReflexItemLanguage, reflex.CReflexLanguage, nameof(QReflexItemHead), nameof(QReflexItemLabel));
        QReflexValueRefine(ref _qReflexItemKind, reflex.CReflexKind, nameof(QReflexItemKind), nameof(QReflexItemTag));
        QReflexValueRefine(ref _qReflexItemText, reflex.CReflexText, nameof(QReflexItemText));
        QReflexValueRefine(
            ref _qReflexItemRomanization, reflex.CReflexRomanization, nameof(QReflexItemRomanization));
        QReflexValueRefine(ref _qReflexItemMeaning, reflex.CReflexMeaning, nameof(QReflexItemMeaning));
        QReflexValueRefine(ref _qReflexItemNote, reflex.CReflexNote, nameof(QReflexItemNote));
        QReflexValueRefine(ref _qReflexItemRegion, reflex.CReflexRegion, nameof(QReflexItemArea));
        QReflexItemLead = reflex.CReflexLead;
    }

    internal void QReflexTypeRefine(CReflexTyped typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        string text = typed.CReflexTypedText;
        switch (typed.CReflexTypedField)
        {
            case CReflexField.CReflexFieldLanguage:
                QReflexValueRefine(ref _qReflexItemLanguage, text, nameof(QReflexItemHead), nameof(QReflexItemLabel));
                break;
            case CReflexField.CReflexFieldKind:
                QReflexValueRefine(ref _qReflexItemKind, text, nameof(QReflexItemKind), nameof(QReflexItemTag));
                break;
            case CReflexField.CReflexFieldText:
                QReflexValueRefine(ref _qReflexItemText, text, nameof(QReflexItemText));
                break;
            case CReflexField.CReflexFieldRomanization:
                QReflexValueRefine(ref _qReflexItemRomanization, text, nameof(QReflexItemRomanization));
                break;
            case CReflexField.CReflexFieldMeaning:
                QReflexValueRefine(ref _qReflexItemMeaning, text, nameof(QReflexItemMeaning));
                break;
            case CReflexField.CReflexFieldNote:
                QReflexValueRefine(ref _qReflexItemNote, text, nameof(QReflexItemNote));
                break;
        }
    }

    private static string QReflexLabelRefine(string key, string name)
    {
        return QLocalizationCatalog.QLocalizationTextFind(key) ?? name;
    }

    internal static void QReflexLeadRefine(IReadOnlyList<QReflexItem> rows, IReadOnlyList<CReflexHead> heads)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(heads);

        foreach (CReflexHead head in heads)
        {
            foreach (QReflexItem row in rows)
            {
                if (row.QReflexItemId == head.CReflexHeadId)
                {
                    row.QReflexItemLead = head.CReflexHeadLead;
                }
            }
        }
    }

    internal static void QReflexAnchorRefine(
        IReadOnlyList<QReflexItem> rows, bool anchorable, IReadOnlyDictionary<long, string> texts)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(texts);

        foreach (QReflexItem row in rows)
        {
            row.QReflexItemAnchorable = anchorable;
            row.QReflexItemAnchor = texts.TryGetValue(row.QReflexItemId, out string? text) ? text : string.Empty;
        }
    }

    internal static void QReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(fold);

        bool any = false;
        foreach (QReflexItem row in rows)
        {
            any |= row.QReflexItemFolded;
            row.QReflexItemHidden = row._qReflexItemReflex.CReflexHiddenCheck(opened);
        }

        fold.IsChecked = opened;
        fold.Visibility = any ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QReflexValueRefine(ref string field, string value, params string[] names)
    {
        if (string.Equals(field, value, StringComparison.Ordinal))
        {
            return;
        }

        field = value;
        foreach (string name in names)
        {
            QReflexChangeRefine(name);
        }
    }

    private void QReflexChangeRefine(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
