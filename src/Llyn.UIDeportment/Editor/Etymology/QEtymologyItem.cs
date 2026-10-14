using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEtymologyItem : INotifyPropertyChanged
{
    private QEtymologyItem(
        CAncestor ancestor,
        bool first,
        bool last,
        IReadOnlyList<QEtymologyChoice> relations,
        IReadOnlyList<QEtymologyChoice> languages)
    {
        QEtymologyItemAncestor = ancestor;
        QEtymologyItemFirst = first;
        QEtymologyItemLast = last;
        QEtymologyItemRelations = relations;
        QEtymologyItemLanguages = languages;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CAncestor QEtymologyItemAncestor { get; private set; }

    public bool QEtymologyItemFirst { get; private set; }

    public bool QEtymologyItemLast { get; private set; }

    public IReadOnlyList<QEtymologyChoice> QEtymologyItemRelations { get; private set; }

    public IReadOnlyList<QEtymologyChoice> QEtymologyItemLanguages { get; private set; }

    public ObservableCollection<QEtymologyItem> QEtymologyItemSources { get; } = [];

    internal static void QEtymologyItemShow(ObservableCollection<QEtymologyItem> items, CEtymologyState state)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(state);

        IReadOnlyList<CAncestor> rows = state.CEtymologyStateAncestors;
        QEtymologyItemShow(
            items,
            rows,
            0,
            rows.Count,
            [.. state.CEtymologyStateRelations.Select(static choice => new QEtymologyChoice(choice))],
            [.. state.CEtymologyStateLanguages.Select(static choice => new QEtymologyChoice(choice))]);
    }

    private static void QEtymologyItemShow(
        ObservableCollection<QEtymologyItem> items,
        IReadOnlyList<CAncestor> rows,
        int start,
        int end,
        IReadOnlyList<QEtymologyChoice> relations,
        IReadOnlyList<QEtymologyChoice> languages)
    {
        List<int> heads = [];
        for (int index = start; index < end; index++)
        {
            if (rows[index].CAncestorDepth == rows[start].CAncestorDepth)
            {
                heads.Add(index);
            }
        }

        List<CAncestor> siblings = heads.ConvertAll(index => rows[index]);
        QLookItem.QLookItemShow(
            items,
            siblings,
            static item => item.QEtymologyItemAncestor.CAncestorKey,
            static row => row.CAncestorKey,
            row => new QEtymologyItem(row, row == siblings[0], row == siblings[^1], relations, languages),
            (item, row) => item.QEtymologyItemRefine(
                row, row == siblings[0], row == siblings[^1], relations, languages));
        for (int place = 0; place < heads.Count; place++)
        {
            int next = place + 1 < heads.Count ? heads[place + 1] : end;
            QEtymologyItemShow(items[place].QEtymologyItemSources, rows, heads[place] + 1, next, relations, languages);
        }
    }

    private QEtymologyItem QEtymologyItemRefine(
        CAncestor ancestor,
        bool first,
        bool last,
        IReadOnlyList<QEtymologyChoice> relations,
        IReadOnlyList<QEtymologyChoice> languages)
    {
        QEtymologyItemAncestor = ancestor;
        QEtymologyItemFirst = first;
        QEtymologyItemLast = last;
        QEtymologyItemRelations = relations;
        QEtymologyItemLanguages = languages;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QEtymologyItemAncestor)));
        return this;
    }
}
