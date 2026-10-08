using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QReflexList
{
    private readonly ObservableCollection<QReflexItem> _qReflexListRow = [];

    private readonly ToggleButton _qReflexListFold;

    internal QReflexList(ItemsControl list, ToggleButton fold)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(fold);

        _qReflexListFold = fold;
        list.ItemsSource = _qReflexListRow;
        QLookItem.QLookItemAttach(list, QReflexItem.QReflexItemRefine);
    }

    internal event Action<QReflexItem, CReflexField, string>? QReflexListTyped;

    internal void QReflexListShow(IReadOnlyList<CReflex> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        QLookItem.QLookItemShow(
            _qReflexListRow,
            rows,
            static row => row.QReflexItemId,
            static row => row.CReflexId,
            QReflexRowRefine,
            QReflexStateRefine);
    }

    internal void QReflexLeadRefine(IReadOnlyList<CReflexHead> heads)
    {
        ArgumentNullException.ThrowIfNull(heads);

        foreach (CReflexHead head in heads)
        {
            foreach (QReflexItem row in _qReflexListRow)
            {
                if (row.QReflexItemId == head.CReflexHeadId)
                {
                    row.QReflexItemLead = head.CReflexHeadLead;
                }
            }
        }
    }

    internal void QReflexAnchorRefine(CLecternAnchor anchor)
    {
        ArgumentNullException.ThrowIfNull(anchor);

        IReadOnlyDictionary<long, string> texts = anchor.CLecternAnchorTexts;
        foreach (QReflexItem row in _qReflexListRow)
        {
            row.QReflexItemAnchorable = anchor.CLecternAnchorOffered;
            row.QReflexItemAnchor = texts.TryGetValue(row.QReflexItemId, out string? text) ? text : string.Empty;
        }
    }

    internal void QReflexFoldRefine(bool opened)
    {
        bool any = false;
        foreach (QReflexItem row in _qReflexListRow)
        {
            any |= row.QReflexItemFolded;
            row.QReflexItemHidden = row.QReflexItemReflex.CReflexHiddenCheck(opened);
        }

        _qReflexListFold.IsChecked = opened;
        _qReflexListFold.Visibility = QLook.QLookVisibleRead(any);
    }

    private QReflexItem QReflexRowRefine(CReflex reflex)
    {
        QReflexItem row = new(reflex);
        row.QReflexItemTyped += (typed, field, text) => QReflexListTyped?.Invoke(typed, field, text);
        return row;
    }

    private static QReflexItem QReflexStateRefine(QReflexItem row, CReflex reflex)
    {
        row.QReflexStateRefine(reflex);
        return row;
    }
}
