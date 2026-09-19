using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<PReflexItem> _pDisplayReflex = [];

    private IReadOnlyList<LFanqieRow> _pDisplayFanqie = [];

    private void PDisplayReflexStart(long id)
    {
        try
        {
            _lEngine.LEngineReflexStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayReflexShow(LEntryDraft draft)
    {
        _pDisplayReflex.Clear();
        HashSet<string> folded = PReflexFoldRead(_lEngine, draft.LEntryDraftLanguage);
        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            if (reflex.LReflexDraftWritten)
            {
                _pDisplayReflex.Add(PReflexItemCreate(_pDisplayHost, _lEngine, reflex, folded));
            }
        }

        PReflexLeadApply(_pDisplayReflex);
        PReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lDisplay.LDisplayFoldOpened);
        PReflexAnchorApply(_pDisplayReflex, _pDisplayFanqie, draft.LEntryDraftHeadword);
    }

    private void PReflexPendingShow(long id)
    {
        try
        {
            PDisplayReflexLoading.Visibility = _lEngine.LEngineReflexCheck(id)
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
            draft = _lEngine.LEngineEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PDisplayReflexShow(draft);
        }

        PReflexPendingShow(id);
    }

    private void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _lDisplay.LDisplayFoldSet(PLook.PLookCheckedRead(PDisplayReflexFold.IsChecked));
        PReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lDisplay.LDisplayFoldOpened);
    }

    internal static PReflexItem PReflexItemCreate(
        PWindow host, LEngine engine, LReflexDraft reflex, HashSet<string> folded)
    {
        string language = reflex.LReflexDraftLanguage.Trim();
        return PReflexItem.PReflexItemCreate(
            host,
            reflex,
            PRespelling.PRespellingRead(engine, language),
            engine.LEnginePhonemicCheck(language),
            folded.Contains(language));
    }

    internal static HashSet<string> PReflexFoldRead(LEngine engine, string language)
    {
        HashSet<string> folded = new(StringComparer.Ordinal);
        if (language.Trim().Length == 0)
        {
            return folded;
        }

        foreach (LReflexRule rule in engine.LEngineReflexRead(language))
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
        IReadOnlyList<PReflexItem> rows, IReadOnlyList<LFanqieRow> fanqie, string headword)
    {
        bool anchorable = fanqie.Count > 0 && LGlyph.LGlyphSingleCheck(headword);
        foreach (PReflexItem row in rows)
        {
            row.PReflexItemAnchorable = anchorable;
            row.PReflexItemAnchor = anchorable
                ? PAnchorItem.PAnchorTextFormat(fanqie, row.PReflexItemAnchors)
                : string.Empty;
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
