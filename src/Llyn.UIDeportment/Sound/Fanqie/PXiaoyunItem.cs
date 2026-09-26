using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PXiaoyunItem : INotifyPropertyChanged
{
    private bool _pXiaoyunItemChosen;

    internal PXiaoyunItem(LVistaRow row, bool chosen)
    {
        _pXiaoyunItemChosen = chosen;
        PXiaoyunItemId = row.LVistaRowId;
        PXiaoyunItemHeadword = row.LVistaRowHeadword;
        PXiaoyunItemName = row.LVistaRowName;
        PXiaoyunItemEpithet = row.LVistaRowEpithet ?? string.Empty;
        PXiaoyunItemFlag = LEnsignImage.LEnsignFind(row.LVistaRowLanguage);
    }

    public long PXiaoyunItemId { get; }

    public string PXiaoyunItemHeadword { get; }

    public string PXiaoyunItemEpithet { get; }

    public string PXiaoyunItemName { get; }

    public ImageSource? PXiaoyunItemFlag { get; }

    internal static IReadOnlyList<PXiaoyunItem> PXiaoyunItemBuild(IReadOnlyList<LVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<PXiaoyunItem> built = new(rows.Count);
        foreach (LVistaRow row in rows)
        {
            built.Add(new PXiaoyunItem(row, row.LVistaRowChosen));
        }

        return built;
    }

    internal static bool PXiaoyunItemMatch(PXiaoyunItem held, PXiaoyunItem fresh)
    {
        return held.PXiaoyunItemId == fresh.PXiaoyunItemId
            && string.Equals(held.PXiaoyunItemHeadword, fresh.PXiaoyunItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemEpithet, fresh.PXiaoyunItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PXiaoyunItemName, fresh.PXiaoyunItemName, StringComparison.Ordinal);
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

    internal static void PXiaoyunItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PXiaoyunItem xiaoyun)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PXiaoyunRow") is Button row)
        {
            if (xiaoyun.PXiaoyunItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<Image>(container, "PXiaoyunFlag") is Image flag)
        {
            flag.Source = xiaoyun.PXiaoyunItemFlag;
        }

        if (PLook.PLookPartFind<Run>(container, "PXiaoyunName") is Run name)
        {
            name.Text = xiaoyun.PXiaoyunItemName;
        }

        if (PLook.PLookPartFind<Run>(container, "PXiaoyunEpithet") is Run epithet)
        {
            epithet.Text = " " + xiaoyun.PXiaoyunItemEpithet;
        }
    }
}
