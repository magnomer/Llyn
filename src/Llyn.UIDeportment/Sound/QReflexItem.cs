using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QReflexItem : INotifyPropertyChanged
{
    private bool _qReflexItemLead;
    private bool _qReflexItemHidden;
    private string _qReflexItemAnchor = string.Empty;
    private bool _qReflexItemAnchorable;
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

    public bool QReflexItemFolded => _qReflexItemReflex.CReflexFolded;

    public string QReflexItemOpener => _qReflexItemReflex.CReflexMark.CRespellingMarkOpener;

    public string QReflexItemCloser => _qReflexItemReflex.CReflexMark.CRespellingMarkCloser;

    public string QReflexItemKind
    {
        get => _qReflexItemReflex.CReflexKind;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldKind, value);
    }

    public string QReflexItemText
    {
        get => _qReflexItemReflex.CReflexText;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldText, value);
    }

    public string QReflexItemRomanization
    {
        get => _qReflexItemReflex.CReflexRomanization;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldRomanization, value);
    }

    public string QReflexItemMeaning
    {
        get => _qReflexItemReflex.CReflexMeaning;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldMeaning, value);
    }

    public string QReflexItemNote
    {
        get => _qReflexItemReflex.CReflexNote;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldNote, value);
    }

    public bool QReflexItemMain => _qReflexItemReflex.CReflexMain;

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
        set
        {
            if (string.Equals(_qReflexItemAnchor, value, StringComparison.Ordinal))
            {
                return;
            }

            _qReflexItemAnchor = value;
            QReflexChangeRefine(nameof(QReflexItemAnchor));
        }
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
        get => _qReflexItemLead ? _qReflexItemReflex.CReflexLanguage : string.Empty;
        set => QReflexItemTyped?.Invoke(this, CReflexField.CReflexFieldLanguage, value);
    }

    public string QReflexItemLabel =>
        _qReflexItemLead
            ? QReflexLabelRefine(_qReflexItemReflex.CReflexLanguageKey, _qReflexItemReflex.CReflexLanguage)
            : string.Empty;

    public string? QReflexItemArea => _qReflexItemLead ? _qReflexItemReflex.CReflexRegion : null;

    public string QReflexItemTag =>
        QReflexLabelRefine(_qReflexItemReflex.CReflexKindKey, _qReflexItemReflex.CReflexKind);

    internal CReflex QReflexItemReflex => _qReflexItemReflex;

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
            label.ToolTip = row.QReflexItemArea;
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

        QReflexValueRefine(reflex);
        QReflexItemLead = reflex.CReflexLead;
    }

    internal void QReflexTypeRefine(CReflex? reflex)
    {
        if (reflex is not null)
        {
            QReflexValueRefine(reflex);
        }
    }

    private static string QReflexLabelRefine(string key, string name)
    {
        return QLocalizationCatalog.QLocalizationTextFind(key) ?? name;
    }

    private void QReflexValueRefine(CReflex reflex)
    {
        CReflex held = _qReflexItemReflex;
        _qReflexItemReflex = reflex;

        CRespellingMark mark = reflex.CReflexMark;
        if (!string.Equals(
                held.CReflexMark.CRespellingMarkOpener, mark.CRespellingMarkOpener, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemOpener));
        }

        if (!string.Equals(
                held.CReflexMark.CRespellingMarkCloser, mark.CRespellingMarkCloser, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemCloser));
        }

        if (held.CReflexMain != reflex.CReflexMain)
        {
            QReflexChangeRefine(nameof(QReflexItemMain));
        }

        if (!string.Equals(held.CReflexLanguage, reflex.CReflexLanguage, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemHead));
            QReflexChangeRefine(nameof(QReflexItemLabel));
        }

        if (!string.Equals(held.CReflexKind, reflex.CReflexKind, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemKind));
            QReflexChangeRefine(nameof(QReflexItemTag));
        }

        if (!string.Equals(held.CReflexText, reflex.CReflexText, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemText));
        }

        if (!string.Equals(held.CReflexRomanization, reflex.CReflexRomanization, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemRomanization));
        }

        if (!string.Equals(held.CReflexMeaning, reflex.CReflexMeaning, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemMeaning));
        }

        if (!string.Equals(held.CReflexNote, reflex.CReflexNote, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemNote));
        }

        if (!string.Equals(held.CReflexRegion, reflex.CReflexRegion, StringComparison.Ordinal))
        {
            QReflexChangeRefine(nameof(QReflexItemArea));
        }
    }

    private void QReflexChangeRefine(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
