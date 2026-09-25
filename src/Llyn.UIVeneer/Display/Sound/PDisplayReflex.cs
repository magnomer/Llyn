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
    private readonly ObservableCollection<LReflexItem> _pDisplayReflex = [];

    private void PDisplayReflexStart(long id)
    {
        try
        {
            _lLectern.LLecternReflexStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayReflexShow(LEntryDraft draft)
    {
        _pDisplayReflex.Clear();
        LWindow window = _pDisplayHost.PWindowDeportment;
        HashSet<string> folded = LReflexItem.LReflexFoldRead(window, draft.LEntryDraftLanguage);
        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            if (reflex.LReflexDraftWritten)
            {
                _pDisplayReflex.Add(LReflexItem.LReflexItemCreate(window, reflex, folded));
            }
        }

        LReflexItem.LReflexLeadApply(_pDisplayReflex);
        LReflexItem.LReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lLectern.LLecternFoldOpened);
    }

    private void PReflexPendingShow(long id)
    {
        try
        {
            PDisplayReflexLoading.Visibility = _lLectern.LLecternReflexCheck(id)
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
            draft = _lLectern.LLecternEntryLoad(id);
        }
        catch (Exception)
        {
            return;
        }

        if (draft is not null)
        {
            PDisplayReflexShow(draft);
            LWindow window = _pDisplayHost.PWindowDeportment;
            LReflexItem.LReflexAnchorApply(
                window, _pDisplayReflex, _lLectern.LLecternAnchorRead(id), PDisplayHeadword.Text);
        }

        PReflexPendingShow(id);
    }

    private void PReflexFoldHandle(object sender, RoutedEventArgs e)
    {
        _lLectern.LLecternFoldSet(PLook.PLookCheckedRead(PDisplayReflexFold.IsChecked));
        LReflexItem.LReflexFoldApply(_pDisplayReflex, PDisplayReflexFold, _lLectern.LLecternFoldOpened);
    }
}
