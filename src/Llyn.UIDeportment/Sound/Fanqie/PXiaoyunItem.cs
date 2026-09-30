using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class PXiaoyunItem : INotifyPropertyChanged
{
    private bool _pXiaoyunItemChosen;

    internal PXiaoyunItem(CVistaRow row, bool chosen)
    {
        _pXiaoyunItemChosen = chosen;
        PXiaoyunItemId = row.CVistaRowId;
        PXiaoyunItemHeadword = row.CVistaRowHeadword;
        PXiaoyunItemName = row.CVistaRowName;
        PXiaoyunItemEpithet = row.CVistaRowEpithet;
        PXiaoyunItemFlag = QEnsignImage.QEnsignRead(row.CVistaRowLanguage);
    }

    public long PXiaoyunItemId { get; }

    public string PXiaoyunItemHeadword { get; }

    public string PXiaoyunItemEpithet { get; }

    public string PXiaoyunItemName { get; }

    public ImageSource? PXiaoyunItemFlag { get; }

    internal static IReadOnlyList<PXiaoyunItem> PXiaoyunItemBuild(IReadOnlyList<CVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PXiaoyunItem> built = new(rows.Count);
        foreach (CVistaRow row in rows)
        {
            built.Add(new PXiaoyunItem(row, row.CVistaRowChosen));
        }

        return built;
    }

    internal static bool PXiaoyunItemMatch(PXiaoyunItem held, PXiaoyunItem fresh)
    {
        return held.PXiaoyunItemId == fresh.PXiaoyunItemId
            && string.Equals(held.PXiaoyunItemHeadword, fresh.PXiaoyunItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemEpithet, fresh.PXiaoyunItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemName, fresh.PXiaoyunItemName, StringComparison.Ordinal)
            && ReferenceEquals(held.PXiaoyunItemFlag, fresh.PXiaoyunItemFlag);
    }

    internal static void PXiaoyunItemSync(PXiaoyunItem held, PXiaoyunItem fresh)
    {
        held.PXiaoyunItemChosen = fresh.PXiaoyunItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PXiaoyunItemChosen
    {
        get => _pXiaoyunItemChosen;

        set
        {
            if (_pXiaoyunItemChosen == value)
            {
                return;
            }

            _pXiaoyunItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PXiaoyunItemChosen)));
        }
    }

    internal static void PXiaoyunItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not PXiaoyunItem xiaoyun)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PXiaoyunRow") is Button row)
        {
            if (xiaoyun.PXiaoyunItemChosen)
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
            flag.Source = xiaoyun.PXiaoyunItemFlag;
        }

        if (QLook.QLookPartFind<Run>(container, "PXiaoyunName") is Run name)
        {
            name.Text = xiaoyun.PXiaoyunItemName;
        }

        if (QLook.QLookPartFind<Run>(container, "PXiaoyunEpithet") is Run epithet)
        {
            epithet.Text = " " + xiaoyun.PXiaoyunItemEpithet;
        }
    }
}
