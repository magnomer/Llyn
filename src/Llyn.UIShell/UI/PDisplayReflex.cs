using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private readonly ObservableCollection<PReflexItem> _pDisplayReflex = [];

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
        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            if (reflex.LReflexDraftText.Trim().Length > 0)
            {
                _pDisplayReflex.Add(PReflexItem.PReflexItemCreate(
                    _pDisplayHost, reflex, PRespelling.PRespellingRead(_lEngine, reflex.LReflexDraftLanguage)));
            }
        }

        PReflexLeadApply(_pDisplayReflex);
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
}
