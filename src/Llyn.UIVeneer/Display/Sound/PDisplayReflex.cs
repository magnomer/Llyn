using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<PReflexItem> _pDisplayReflex = [];

    private void PDisplayReflexStart(long id)
    {
        try
        {
            _lDisplay.LDisplayReflexStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayReflexShow(long id, LEntryDraft draft)
    {
        _pDisplayReflex.Clear();
        HashSet<string> folded = PReflexFoldRead(_pDisplayHost.PWindowDeportment, draft.LEntryDraftLanguage);
        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            if (reflex.LReflexDraftWritten)
            {
                _pDisplayReflex.Add(PReflexItemCreate(_pDisplayHost, reflex, folded));
            }
        }

        PReflexLeadApply(_pDisplayReflex);
        PReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lDisplay.LDisplayFoldOpened);
        LWindow window = _pDisplayHost.PWindowDeportment;
        PReflexAnchorApply(window, _pDisplayReflex, _lDisplay.LDisplayAnchorRead(id), draft.LEntryDraftHeadword);
    }

    private void PReflexPendingShow(long id)
    {
        try
        {
            PDisplayReflexLoading.Visibility = _lDisplay.LDisplayReflexCheck(id)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        catch (Exception)
        {
            PDisplayReflexLoading.Visibility = Visibility.Collapsed;
        }
    }

    private void PDisplayReflexLoad(long id)
    {
        LEntryDraft? draft;
        try
        {
            draft = _lDisplay.LDisplayEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PDisplayReflexShow(id, draft);
        }

        PReflexPendingShow(id);
    }

    private void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _lDisplay.LDisplayFoldSet(PLook.PLookCheckedRead(PDisplayReflexFold.IsChecked));
        PReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lDisplay.LDisplayFoldOpened);
    }

    internal static PReflexItem PReflexItemCreate(
        PWindow host, LReflexDraft reflex, HashSet<string> folded)
    {
        string language = reflex.LReflexDraftLanguage.Trim();
        return PReflexItem.PReflexItemCreate(
            host,
            reflex,
            PRespelling.PRespellingRead(host.PWindowDeportment, language),
            host.PWindowDeportment.LWindowPhonemicCheck(language),
            folded.Contains(language));
    }

    internal static HashSet<string> PReflexFoldRead(LWindow window, string language)
    {
        HashSet<string> folded = new(StringComparer.Ordinal);
        if (language.Trim().Length == 0)
        {
            return folded;
        }

        foreach (LReflexRule rule in window.LWindowReflexRead(language))
        {
            if (rule.LReflexRuleFolded)
            {
                folded.Add(rule.LReflexRuleLanguage);
            }
        }

        return folded;
    }

    internal static void PReflexLeadApply(IReadOnlyList<PReflexItem> rows)
    {
        string? held = null;
        foreach (PReflexItem row in rows)
        {
            bool lead = !string.Equals(held, row.PReflexItemLanguage, StringComparison.Ordinal);
            row.PReflexItemLead = lead;
            held = row.PReflexItemLanguage;
        }
    }

    internal static void PReflexAnchorApply(
        LWindow window, IReadOnlyList<PReflexItem> rows, IReadOnlyList<LFanqieRow> fanqie, string headword)
    {
        bool anchorable = window.LWindowAnchorCheck(fanqie, headword);
        foreach (PReflexItem row in rows)
        {
            row.PReflexItemAnchorable = anchorable;
            row.PReflexItemAnchor = window.LWindowAnchorFormat(
                fanqie, row.PReflexItemAnchors, headword, PAnchorItem.PAnchorItemSeparator);
        }
    }

    internal static void PReflexFoldApply(IReadOnlyList<PReflexItem> rows, ToggleButton fold, bool opened)
    {
        bool any = false;
        foreach (PReflexItem row in rows)
        {
            any |= row.PReflexItemFolded;
            row.PReflexItemHidden = PLook.PLookFirstRead(opened, false, row.PReflexItemFolded);
        }

        fold.IsChecked = PLook.PLookCheckedRead(opened);
        fold.Visibility = PLook.PLookVisibleRead(any);
    }
}
