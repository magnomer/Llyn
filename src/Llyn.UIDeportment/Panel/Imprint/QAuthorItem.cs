using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAuthorItem : INotifyPropertyChanged
{
    private string _qAuthorItemName;

    private bool _qAuthorItemEarlier;

    private bool _qAuthorItemLater;

    internal QAuthorItem(long id, string name, int position, bool earlier, bool later)
    {
        QAuthorItemId = id;
        QAuthorItemPosition = position;
        _qAuthorItemName = name;
        _qAuthorItemEarlier = earlier;
        _qAuthorItemLater = later;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long QAuthorItemId { get; }

    public int QAuthorItemPosition { get; }

    public bool QAuthorItemBlank => QAuthorItemId == 0;

    public string QAuthorItemName
    {
        get => _qAuthorItemName;

        private set
        {
            if (string.Equals(_qAuthorItemName, value, StringComparison.Ordinal))
            {
                return;
            }

            _qAuthorItemName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAuthorItemName)));
        }
    }

    public bool QAuthorItemEarlier
    {
        get => _qAuthorItemEarlier;

        private set
        {
            if (_qAuthorItemEarlier == value)
            {
                return;
            }

            _qAuthorItemEarlier = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAuthorItemEarlier)));
        }
    }

    public bool QAuthorItemLater
    {
        get => _qAuthorItemLater;

        private set
        {
            if (_qAuthorItemLater == value)
            {
                return;
            }

            _qAuthorItemLater = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAuthorItemLater)));
        }
    }

    internal static IReadOnlyList<QAuthorItem> QAuthorItemBuild(IReadOnlyList<CAuthorRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QAuthorItem> built = new(rows.Count);
        foreach (CAuthorRow row in rows)
        {
            built.Add(new QAuthorItem(
                row.CAuthorRowId,
                row.CAuthorRowName,
                row.CAuthorRowPosition,
                row.CAuthorRowEarlier,
                row.CAuthorRowLater));
        }

        return built;
    }

    internal static bool QAuthorItemMatch(QAuthorItem held, QAuthorItem fresh)
    {
        return held.QAuthorItemId == fresh.QAuthorItemId && held.QAuthorItemPosition == fresh.QAuthorItemPosition;
    }

    internal static void QAuthorItemSync(QAuthorItem held, QAuthorItem fresh)
    {
        held.QAuthorItemName = fresh.QAuthorItemName;
        held.QAuthorItemEarlier = fresh.QAuthorItemEarlier;
        held.QAuthorItemLater = fresh.QAuthorItemLater;
    }

    internal static void QAuthorItemRefine(FrameworkElement container, object item, string? change)
    {
        if (item is not QAuthorItem author)
        {
            return;
        }

        if (change is null or nameof(QAuthorItemName)
            && QLook.QLookPartFind<TextBox>(container, "PAuthorName") is TextBox name)
        {
            name.Text = author.QAuthorItemName;
            name.SetResourceReference(QField.QFieldHintProperty, "Source.AuthorName");
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorEarlier") is Button earlier)
        {
            earlier.IsEnabled = author.QAuthorItemEarlier;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorLater") is Button later)
        {
            later.IsEnabled = author.QAuthorItemLater;
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAuthorAdditionIcon") is QIconImage addition)
        {
            addition.QIconSource = QIcon.QIconResolve("add", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAuthorRemovalIcon") is QIconImage removal)
        {
            removal.QIconSource = QIcon.QIconResolve("remove", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAuthorEarlierIcon") is QIconImage earlierIcon)
        {
            earlierIcon.QIconSource = QIcon.QIconResolve("earlier", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PAuthorLaterIcon") is QIconImage laterIcon)
        {
            laterIcon.QIconSource = QIcon.QIconResolve("later", 12);
        }
    }

    internal static QAuthorItem? QAuthorItemFind(IEnumerable<QAuthorItem> rows)
    {
        foreach (QAuthorItem row in rows)
        {
            if (row.QAuthorItemBlank)
            {
                return row;
            }
        }

        return null;
    }

    internal static void QAuthorItemRefine(object? source)
    {
        if (source is TextBox { DataContext: QAuthorItem row } box)
        {
            box.Text = row.QAuthorItemName;
        }
    }
}
