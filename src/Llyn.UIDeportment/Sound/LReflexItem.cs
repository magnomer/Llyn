using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LReflexItem : INotifyPropertyChanged
{
    private const string LReflexAnchorSeparator = " · ";

    private readonly LWindow _lReflexItemWindow;
    private readonly CRespellingMark _lReflexItemRespelling;
    private string _lReflexItemLanguage;
    private string _lReflexItemKind;
    private string _lReflexItemText;
    private string _lReflexItemRomanization;
    private string _lReflexItemMeaning;
    private string _lReflexItemNote;
    private string _lReflexItemRegion;
    private bool _lReflexItemMain;
    private bool _lReflexItemLead;
    private bool _lReflexItemHidden;
    private string _lReflexItemAnchor = string.Empty;
    private bool _lReflexItemAnchorable;

    public LReflexItem(
        LWindow window,
        long id,
        string language,
        string kind,
        string text,
        string romanization,
        string meaning,
        string note,
        bool main,
        CRespellingMark respelling,
        string region,
        bool folded,
        IReadOnlyList<long> anchors)
    {
        ArgumentNullException.ThrowIfNull(respelling);

        _lReflexItemWindow = window;
        _lReflexItemRespelling = respelling;
        LReflexItemRespelled = respelling.CRespellingMarkShown;
        LReflexItemOpener = respelling.CRespellingMarkOpener;
        LReflexItemCloser = respelling.CRespellingMarkCloser;
        LReflexItemAnchors = anchors;
        LReflexItemId = id;
        _lReflexItemLanguage = language;
        _lReflexItemKind = kind;
        _lReflexItemText = text;
        _lReflexItemRomanization = romanization;
        _lReflexItemMeaning = meaning;
        _lReflexItemNote = note;
        _lReflexItemRegion = region;
        _lReflexItemMain = main;
        LReflexItemFolded = folded;
        _lReflexItemHidden = folded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long LReflexItemId { get; }

    public bool LReflexItemRespelled { get; }

    public bool LReflexItemMatch(CRespellingMark respelling, bool folded, IReadOnlyList<long> anchors)
    {
        return _lReflexItemRespelling == respelling
            && LReflexItemFolded == folded
            && _lReflexItemWindow.LWindowAnchorMatch(LReflexItemAnchors, anchors);
    }

    public bool LReflexItemFolded { get; }

    public string LReflexItemTone { get; set; } = string.Empty;

    public string LReflexItemOpener { get; }

    public string LReflexItemCloser { get; }

    public string LReflexItemLanguage
    {
        get => _lReflexItemLanguage;
        set
        {
            if (LReflexItemSet(ref _lReflexItemLanguage, value, nameof(LReflexItemLanguage)))
            {
                LReflexItemRaise(nameof(LReflexItemHead));
                LReflexItemRaise(nameof(LReflexItemLabel));
            }
        }
    }

    public string LReflexItemKind
    {
        get => _lReflexItemKind;
        set
        {
            if (LReflexItemSet(ref _lReflexItemKind, value, nameof(LReflexItemKind)))
            {
                LReflexItemRaise(nameof(LReflexItemTag));
            }
        }
    }

    public string LReflexItemText
    {
        get => _lReflexItemText;
        set
        {
            LReflexItemSet(ref _lReflexItemText, value, nameof(LReflexItemText));
        }
    }

    public string LReflexItemRomanization
    {
        get => _lReflexItemRomanization;
        set => LReflexItemSet(ref _lReflexItemRomanization, value, nameof(LReflexItemRomanization));
    }

    public string LReflexItemMeaning
    {
        get => _lReflexItemMeaning;
        set => LReflexItemSet(ref _lReflexItemMeaning, value, nameof(LReflexItemMeaning));
    }

    public string LReflexItemNote
    {
        get => _lReflexItemNote;
        set => LReflexItemSet(ref _lReflexItemNote, value, nameof(LReflexItemNote));
    }

    public string LReflexItemRegion
    {
        get => _lReflexItemRegion;
        set
        {
            if (LReflexItemSet(ref _lReflexItemRegion, value, nameof(LReflexItemRegion)))
            {
                LReflexItemRaise(nameof(LReflexItemArea));
            }
        }
    }

    public bool LReflexItemMain
    {
        get => _lReflexItemMain;
        set
        {
            if (_lReflexItemMain == value)
            {
                return;
            }

            _lReflexItemMain = value;
            LReflexItemRaise(nameof(LReflexItemMain));
        }
    }

    public bool LReflexItemLead
    {
        get => _lReflexItemLead;
        set
        {
            if (_lReflexItemLead == value)
            {
                return;
            }

            _lReflexItemLead = value;
            LReflexItemRaise(nameof(LReflexItemLead));
            LReflexItemRaise(nameof(LReflexItemHead));
            LReflexItemRaise(nameof(LReflexItemLabel));
            LReflexItemRaise(nameof(LReflexItemArea));
        }
    }

    public bool LReflexItemHidden
    {
        get => _lReflexItemHidden;
        set
        {
            if (_lReflexItemHidden == value)
            {
                return;
            }

            _lReflexItemHidden = value;
            LReflexItemRaise(nameof(LReflexItemHidden));
        }
    }

    public IReadOnlyList<long> LReflexItemAnchors { get; }


    public string LReflexItemAnchor
    {
        get => _lReflexItemAnchor;
        set => LReflexItemSet(ref _lReflexItemAnchor, value, nameof(LReflexItemAnchor));
    }

    public bool LReflexItemAnchorable
    {
        get => _lReflexItemAnchorable;
        set
        {
            if (_lReflexItemAnchorable == value)
            {
                return;
            }

            _lReflexItemAnchorable = value;
            LReflexItemRaise(nameof(LReflexItemAnchorable));
        }
    }

    public string LReflexItemHead
    {
        get => _lReflexItemLead ? _lReflexItemLanguage : string.Empty;
        set => LReflexItemLanguage = value;
    }

    public string LReflexItemLabel => _lReflexItemLead ? LReflexLabelFormat(_lReflexItemLanguage) : string.Empty;

    public string LReflexItemArea => _lReflexItemLead ? _lReflexItemRegion : string.Empty;

    public string LReflexItemTag => LReflexLabelFormat(_lReflexItemKind);

    internal static void LReflexItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LReflexItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Border>(container, "PReflexSurface") is Border surface)
        {
            surface.Visibility = row.LReflexItemHidden ? Visibility.Hidden : Visibility.Visible;
            if (row.LReflexItemHidden)
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
            label.Text = row.LReflexItemLabel;
            label.ToolTip = row.LReflexItemArea.Length > 0 ? row.LReflexItemArea : null;
        }

        LReflexTextApply(container, "PReflexTag", row.LReflexItemTag, false);
        LReflexTextApply(container, "PReflexOpener", row.LReflexItemOpener, row.LReflexItemMain);
        LReflexTextApply(container, "PReflexCloser", row.LReflexItemCloser, row.LReflexItemMain);
        LReflexTextApply(container, "PReflexRomanization", row.LReflexItemRomanization, false);
        LReflexTextApply(container, "PReflexMeaning", row.LReflexItemMeaning, false);
        LReflexTextApply(container, "PReflexNote", row.LReflexItemNote, false);
        LReflexTextApply(container, "PReflexAnchor", row.LReflexItemAnchor, false);
        Grid? cell = QLook.QLookPartFind<Grid>(container, "PReflexCell");
        if (cell is not null)
        {
            cell.SetValue(
                QField.QFieldCellProperty,
                row.LReflexItemMain
                    ? new QField.QFieldCell("Theme.Reflex.Lead", "LReflexItemText", "Input.Reflex")
                    : new QField.QFieldCell("Theme.Reflex.Field", "LReflexItemText", "Input.Reflex"));
            (string, QField.QFieldCell)[] asides =
            [
                ("PReflexLabel", new("Theme.Reflex.Name", "LReflexItemHead", null)),
                ("PReflexTag", new("Theme.Reflex.Name", "LReflexItemKind", null)),
                ("PReflexRomanization", new("Theme.Reflex.Aside", "LReflexItemRomanization", null)),
                ("PReflexMeaning", new("Theme.Reflex.Aside", "LReflexItemMeaning", "Input.Meaning")),
                ("PReflexNote", new("Theme.Reflex.Aside", "LReflexItemNote", null)),
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
                LReflexTextApply(container, "PReflexText", row.LReflexItemText, row.LReflexItemMain);
            }
            else
            {
                string? ink = QLook.QLookFirstRead<string?>(row.LReflexItemMain, "Theme.Accent", null);
                QLook.QLookPromptApply(text, row.LReflexItemText, "Input.Reflex", ink);
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PReflexAnchoring") is Button anchoring)
        {
            anchoring.Visibility = QLook.QLookVisibleRead(row.LReflexItemAnchorable);
            anchoring.Content = row.LReflexItemAnchor;
        }
    }

    private static void LReflexTextApply(FrameworkElement container, string name, string text, bool main)
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

    public static LReflexItem LReflexItemCreate(
        LWindow window, CReflexDraft draft, CRespellingMark respelling, bool folded)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return new LReflexItem(
            window,
            draft.CReflexDraftId,
            draft.CReflexDraftLanguage,
            draft.CReflexDraftKind,
            LReflexTextRead(draft, respelling),
            draft.CReflexDraftRomanization,
            draft.CReflexDraftMeaning,
            draft.CReflexDraftNote,
            draft.CReflexDraftMain,
            respelling,
            draft.CReflexDraftRegion,
            folded,
            draft.CReflexDraftAnchors)
        {
            LReflexItemTone = draft.CReflexDraftTone,
        };
    }

    public static List<LReflexItem> LReflexItemScan(
        LWindow window, IReadOnlyList<CReflexDraft> reflexes, HashSet<string> folded)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        List<LReflexItem> rows = new(reflexes.Count);
        foreach (CReflexDraft reflex in reflexes)
        {
            rows.Add(LReflexItemCreate(window, reflex, folded));
        }

        return rows;
    }

    public static LReflexItem LReflexItemCreate(LWindow window, CReflexDraft reflex, HashSet<string> folded)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(folded);

        return LReflexItemCreate(window, reflex, folded, reflex.CReflexDraftLanguage.Trim());
    }

    private static LReflexItem LReflexItemCreate(
        LWindow window, CReflexDraft reflex, HashSet<string> folded, string language)
    {
        return LReflexItemCreate(
            window,
            reflex,
            window.LWindowAtelier.CAtelierRespelling.CRespellingReflexRead(language),
            folded.Contains(language));
    }

    public static string LReflexTextRead(CReflexDraft draft, CRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return CRespelling.CRespellingResolve(respelling, draft.CReflexDraftText, draft.CReflexDraftRespelling);
    }

    public static string LReflexLabelFormat(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return name.Length == 0
            ? string.Empty
            : QLocalizationCatalog.QLocalizationTextFind(string.Concat("Reflex.", name)) ?? name;
    }

    public static HashSet<string> LReflexFoldRead(LWindow window, string language)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(language);

        return language.Trim().Length == 0
            ? new HashSet<string>(StringComparer.Ordinal)
            : LDisplaySound.LDisplayFoldScan(window.LWindowReflexRead(language));
    }

    public static void LReflexLeadApply(IReadOnlyList<LReflexItem> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        string? held = null;
        foreach (LReflexItem row in rows)
        {
            bool lead = !string.Equals(held, row.LReflexItemLanguage, StringComparison.Ordinal);
            row.LReflexItemLead = lead;
            held = row.LReflexItemLanguage;
        }
    }

    public static void LReflexAnchorApply(
        IReadOnlyList<LReflexItem> rows, bool anchorable, Func<IReadOnlyList<long>, string, string> format)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(format);

        foreach (LReflexItem row in rows)
        {
            row.LReflexItemAnchorable = anchorable;
            row.LReflexItemAnchor = format(row.LReflexItemAnchors, LReflexAnchorSeparator);
        }
    }

    public static void LReflexFoldApply(IReadOnlyList<LReflexItem> rows, ToggleButton fold, bool opened)
    {
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(fold);

        bool any = false;
        foreach (LReflexItem row in rows)
        {
            any |= row.LReflexItemFolded;
            row.LReflexItemHidden = !opened && row.LReflexItemFolded;
        }

        fold.IsChecked = opened;
        fold.Visibility = any ? Visibility.Visible : Visibility.Collapsed;
    }

    private bool LReflexItemSet(ref string field, string? value, string name)
    {
        string text = value ?? string.Empty;
        if (string.Equals(field, text, StringComparison.Ordinal))
        {
            return false;
        }

        field = text;
        LReflexItemRaise(name);
        return true;
    }

    private void LReflexItemRaise(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
