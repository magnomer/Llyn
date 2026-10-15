using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QXiaoyunItem : INotifyPropertyChanged
{
    private bool _qXiaoyunItemChosen;

    internal QXiaoyunItem(CVistaRow row, bool chosen)
    {
        _qXiaoyunItemChosen = chosen;
        QXiaoyunItemId = row.CVistaRowId;
        QXiaoyunItemHeadword = row.CVistaRowHeadword;
        QXiaoyunItemName = row.CVistaRowName;
        QXiaoyunItemEpithet = row.CVistaRowEpithet;
        QXiaoyunItemFlag = QEnsignImage.QEnsignRead(row.CVistaRowLanguage);
    }

    public long QXiaoyunItemId { get; }

    public string QXiaoyunItemHeadword { get; }

    public string QXiaoyunItemEpithet { get; }

    public string QXiaoyunItemName { get; }

    public ImageSource? QXiaoyunItemFlag { get; }

    internal static IReadOnlyList<QXiaoyunItem> QXiaoyunItemBuild(IReadOnlyList<CVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<QXiaoyunItem> built = new(rows.Count);
        foreach (CVistaRow row in rows)
        {
            built.Add(new QXiaoyunItem(row, row.CVistaRowChosen));
        }

        return built;
    }

    internal static bool QXiaoyunItemMatch(QXiaoyunItem held, QXiaoyunItem fresh)
    {
        return held.QXiaoyunItemId == fresh.QXiaoyunItemId
            && string.Equals(held.QXiaoyunItemHeadword, fresh.QXiaoyunItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QXiaoyunItemEpithet, fresh.QXiaoyunItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QXiaoyunItemName, fresh.QXiaoyunItemName, StringComparison.Ordinal)
            && ReferenceEquals(held.QXiaoyunItemFlag, fresh.QXiaoyunItemFlag);
    }

    internal static void QXiaoyunItemSync(QXiaoyunItem held, QXiaoyunItem fresh)
    {
        held.QXiaoyunItemChosen = fresh.QXiaoyunItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QXiaoyunItemChosen
    {
        get => _qXiaoyunItemChosen;

        set
        {
            if (_qXiaoyunItemChosen == value)
            {
                return;
            }

            _qXiaoyunItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QXiaoyunItemChosen)));
        }
    }

    internal static void QXiaoyunItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QXiaoyunItem xiaoyun)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PXiaoyunRow") is Button row)
        {
            if (xiaoyun.QXiaoyunItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }
        }

        if (QLook.QLookPartFind<Image>(container, "PXiaoyunFlag") is Image flag)
        {
            flag.Source = xiaoyun.QXiaoyunItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PXiaoyunName") is Run name)
        {
            name.Text = xiaoyun.QXiaoyunItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PXiaoyunEpithet") is Run epithet)
        {
            epithet.Text = QLook.QLookEpithetRead(xiaoyun.QXiaoyunItemEpithet);
        }
    }
}
