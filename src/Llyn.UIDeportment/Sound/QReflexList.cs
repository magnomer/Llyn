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

    private readonly ToggleButton _qReflexListHinge;

    internal QReflexList(ItemsControl list, ToggleButton hinge)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(hinge);

        _qReflexListHinge = hinge;
        list.ItemsSource = _qReflexListRow;
        QLookItem.QLookItemAttach(list, QReflexItem.QReflexItemRefine);
    }

    internal event Action<QReflexItem, CReflexField, string>? QReflexListTyped;

    internal void QReflexListShow(IReadOnlyList<CReflex> rows, bool foldable)
    {
        ArgumentNullException.ThrowIfNull(rows);

        _qReflexListHinge.Visibility = QLook.QLookVisibleRead(foldable);
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
        QReflexHiddenRefine(_qReflexListRow, opened);
        _qReflexListHinge.IsChecked = opened;
    }

    private static void QReflexHiddenRefine(IEnumerable<QReflexItem> rows, bool opened)
    {
        ArgumentNullException.ThrowIfNull(rows);

        foreach (QReflexItem row in rows)
        {
            row.QReflexItemHidden = row.QReflexItemReflex.CReflexHiddenCheck(opened);
        }
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
