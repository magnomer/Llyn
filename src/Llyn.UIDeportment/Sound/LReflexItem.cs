using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LReflexItem : INotifyPropertyChanged
{
    private const string LReflexAnchorSeparator = " · ";

    private readonly LWindow _lReflexItemWindow;
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
        bool respelled,
        string region,
        bool phonemic,
        bool folded,
        IReadOnlyList<long> anchors)
    {
        _lReflexItemWindow = window;
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
        LReflexItemRespelled = respelled;
        LReflexItemPhonemic = phonemic;
        LReflexItemFolded = folded;
        _lReflexItemHidden = folded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long LReflexItemId { get; }

    public bool LReflexItemRespelled { get; }

    public bool LReflexItemMatch(bool respelled, bool phonemic, bool folded, IReadOnlyList<long> anchors)
    {
        return LReflexItemRespelled == respelled
            && LReflexItemPhonemic == phonemic
            && LReflexItemFolded == folded
            && _lReflexItemWindow.LWindowAnchorMatch(LReflexItemAnchors, anchors);
    }

    public bool LReflexItemPhonemic { get; }

    public bool LReflexItemFolded { get; }

    public string LReflexItemTone { get; set; } = string.Empty;

    public string LReflexItemOpener => LReflexItemPhonemic ? "/" : string.Empty;

    public string LReflexItemCloser => LReflexItemPhonemic ? "/" : string.Empty;

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

        if (PLook.PLookPartFind<Border>(container, "PReflexSurface") is Border surface)
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

        if (PLook.PLookPartFind<TextBlock>(container, "PReflexLabel") is TextBlock label)
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
        Grid? cell = PLook.PLookPartFind<Grid>(container, "PReflexCell");
        if (cell is not null)
        {
            cell.Tag = row.LReflexItemMain
                ? "Theme.Reflex.Lead LReflexItemText Input.Reflex"
                : "Theme.Reflex.Field LReflexItemText Input.Reflex";
            (string, string)[] asides =
            [
                ("PReflexLabel", "Theme.Reflex.Name LReflexItemHead"),
                ("PReflexTag", "Theme.Reflex.Name LReflexItemKind"),
                ("PReflexRomanization", "Theme.Reflex.Aside LReflexItemRomanization"),
                ("PReflexMeaning", "Theme.Reflex.Aside LReflexItemMeaning Input.Meaning"),
                ("PReflexNote", "Theme.Reflex.Aside LReflexItemNote"),
            ];
            foreach ((string name, string order) in asides)
            {
                if (PLook.PLookPartFind<TextBlock>(container, name)?.Parent is Grid aside)
                {
                    aside.Tag = order;
                }
            }

            if (PLook.PLookPartFind<ContentControl>(container, "PAccentShelf") is ContentControl shelf)
            {
                shelf.Tag = "Theme.Reflex.Control";
            }
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PReflexText") is TextBlock text)
        {
            if (cell is null)
            {
                LReflexTextApply(container, "PReflexText", row.LReflexItemText, row.LReflexItemMain);
            }
            else
            {
                string? ink = PLook.PLookFirstRead<string?>(row.LReflexItemMain, "Theme.Accent", null);
                PLook.PLookPromptApply(text, row.LReflexItemText, "Input.Reflex", ink);
            }
        }

        if (PLook.PLookPartFind<Button>(container, "PReflexAnchoring") is Button anchoring)
        {
            anchoring.Visibility = PLook.PLookVisibleRead(row.LReflexItemAnchorable);
            anchoring.Content = row.LReflexItemAnchor;
        }
    }

    private static void LReflexTextApply(FrameworkElement container, string name, string text, bool main)
    {
        if (PLook.PLookPartFind<TextBlock>(container, name) is not TextBlock block)
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
        LWindow window, LReflexDraft draft, LRespellingMark respelling, bool phonemic, bool folded)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return new LReflexItem(
            window,
            draft.LReflexDraftId,
            draft.LReflexDraftLanguage,
            draft.LReflexDraftKind,
            LReflexTextRead(draft, respelling),
            draft.LReflexDraftRomanization,
            draft.LReflexDraftMeaning,
            draft.LReflexDraftNote,
            draft.LReflexDraftMain,
            respelling.LRespellingMarkShown,
            draft.LReflexDraftRegion,
            phonemic,
            folded,
            draft.LReflexDraftAnchors)
        {
            LReflexItemTone = draft.LReflexDraftAnatomy.LAnatomyToneIpa,
        };
    }

    public static List<LReflexItem> LReflexItemScan(
        LWindow window, IReadOnlyList<LReflexDraft> reflexes, HashSet<string> folded)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        List<LReflexItem> rows = new(reflexes.Count);
        foreach (LReflexDraft reflex in reflexes)
        {
            rows.Add(LReflexItemCreate(window, reflex, folded));
        }

        return rows;
    }

    public static LReflexItem LReflexItemCreate(LWindow window, LReflexDraft reflex, HashSet<string> folded)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(reflex);
        ArgumentNullException.ThrowIfNull(folded);

        return LReflexItemCreate(window, reflex, folded, reflex.LReflexDraftLanguage.Trim());
    }

    private static LReflexItem LReflexItemCreate(
        LWindow window, LReflexDraft reflex, HashSet<string> folded, string language)
    {
        return LReflexItemCreate(
            window,
            reflex,
            LRespellingMark.LRespellingMarkRead(window, language),
            window.LWindowPhonemicCheck(language),
            folded.Contains(language));
    }

    public static string LReflexTextRead(LReflexDraft draft, LRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(respelling);

        return respelling.LRespellingMarkResolve(draft.LReflexDraftText, draft.LReflexDraftRespelling);
    }

    public static string LReflexLabelFormat(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return name.Length == 0
            ? string.Empty
            : LLocalizationCatalog.LLocalizationTextFind(string.Concat("Reflex.", name)) ?? name;
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
        LWindow window, IReadOnlyList<LReflexItem> rows, IReadOnlyList<LFanqieRow> fanqie, string headword)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(rows);

        bool anchorable = window.LWindowAnchorCheck(fanqie, headword);
        foreach (LReflexItem row in rows)
        {
            row.LReflexItemAnchorable = anchorable;
            row.LReflexItemAnchor = window.LWindowAnchorFormat(
                fanqie, row.LReflexItemAnchors, headword, LReflexAnchorSeparator);
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
