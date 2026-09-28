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

    public long QReflexItemId { get; }

    public bool QReflexItemRespelled { get; private set; }

    public bool QReflexItemFolded { get; private set; }

    public string QReflexItemTone { get; private set; } = string.Empty;

    public IReadOnlyList<long> QReflexItemAnchors { get; private set; } = [];

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

    public string QReflexItemLanguage
    {
        get => _qReflexItemLanguage;
        set
        {
            if (QReflexValueRefine(ref _qReflexItemLanguage, value, nameof(QReflexItemLanguage)))
            {
                QReflexChangeRefine(nameof(QReflexItemHead));
                QReflexChangeRefine(nameof(QReflexItemLabel));
            }
        }
    }

    public string QReflexItemKind
    {
        get => _qReflexItemKind;
        set
        {
            if (QReflexValueRefine(ref _qReflexItemKind, value, nameof(QReflexItemKind)))
            {
                QReflexChangeRefine(nameof(QReflexItemTag));
            }
        }
    }

    public string QReflexItemText
    {
        get => _qReflexItemText;
        set => QReflexValueRefine(ref _qReflexItemText, value, nameof(QReflexItemText));
    }

    public string QReflexItemRomanization
    {
        get => _qReflexItemRomanization;
        set => QReflexValueRefine(ref _qReflexItemRomanization, value, nameof(QReflexItemRomanization));
    }

    public string QReflexItemMeaning
    {
        get => _qReflexItemMeaning;
        set => QReflexValueRefine(ref _qReflexItemMeaning, value, nameof(QReflexItemMeaning));
    }

    public string QReflexItemNote
    {
        get => _qReflexItemNote;
        set => QReflexValueRefine(ref _qReflexItemNote, value, nameof(QReflexItemNote));
    }

    public string QReflexItemRegion
    {
        get => _qReflexItemRegion;
        set
        {
            if (QReflexValueRefine(ref _qReflexItemRegion, value, nameof(QReflexItemRegion)))
            {
                QReflexChangeRefine(nameof(QReflexItemArea));
            }
        }
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
        set => QReflexItemLanguage = value;
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
                QField.QFieldCellProperty,
                row.QReflexItemMain
                    ? new QField.QFieldCell("Theme.Reflex.Lead", "QReflexItemText", "Input.Reflex")
                    : new QField.QFieldCell("Theme.Reflex.Field", "QReflexItemText", "Input.Reflex"));
            (string, QField.QFieldCell)[] asides =
            [
                ("PReflexLabel", new("Theme.Reflex.Name", "QReflexItemHead", null)),
                ("PReflexTag", new("Theme.Reflex.Name", "QReflexItemKind", null)),
                ("PReflexRomanization", new("Theme.Reflex.Aside", "QReflexItemRomanization", null)),
                ("PReflexMeaning", new("Theme.Reflex.Aside", "QReflexItemMeaning", "Input.Meaning")),
                ("PReflexNote", new("Theme.Reflex.Aside", "QReflexItemNote", null)),
            ];
            foreach ((string name, QField.QFieldCell order) in asides)
            {
                if (QLook.QLookPartFind<TextBlock>(container, name)?.Parent is Grid aside)
                {
                    aside.SetValue(QField.QFieldCellProperty, order);
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
        QReflexItemRespelled = reflex.CReflexMark.CRespellingMarkShown;
        QReflexItemFolded = reflex.CReflexFolded;
        QReflexItemAnchors = reflex.CReflexAnchors;
        QReflexItemTone = reflex.CReflexTone;
        QReflexItemOpener = reflex.CReflexMark.CRespellingMarkOpener;
        QReflexItemCloser = reflex.CReflexMark.CRespellingMarkCloser;
        QReflexItemMain = reflex.CReflexMain;
        QReflexItemLanguage = reflex.CReflexLanguage;
        QReflexItemKind = reflex.CReflexKind;
        QReflexItemText = reflex.CReflexText;
        QReflexItemRomanization = reflex.CReflexRomanization;
        QReflexItemMeaning = reflex.CReflexMeaning;
        QReflexItemNote = reflex.CReflexNote;
        QReflexItemRegion = reflex.CReflexRegion;
        QReflexItemLead = reflex.CReflexLead;
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
        IReadOnlyList<QReflexItem> rows, bool anchorable, Func<IReadOnlyList<long>, string> format)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(format);

        Dictionary<long, string> texts = [];
        foreach (QReflexItem row in rows)
        {
            texts[row.QReflexItemId] = format(row.QReflexItemAnchors);
        }

        QReflexAnchorRefine(rows, anchorable, texts);
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

    private bool QReflexValueRefine(ref string field, string? value, string name)
    {
        string text = value ?? string.Empty;
        if (string.Equals(field, text, StringComparison.Ordinal))
        {
            return false;
        }

        field = text;
        QReflexChangeRefine(name);
        return true;
    }

    private void QReflexChangeRefine(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
