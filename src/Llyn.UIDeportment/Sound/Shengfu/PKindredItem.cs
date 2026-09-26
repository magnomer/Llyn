using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PKindredItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs PKindredItemMark = new(nameof(PKindredItemChosen));

    private PKindredItem(LVistaRow row, bool chosen)
    {
        PKindredItemRow = row;
        PKindredItemId = row.LVistaRowId;
        PKindredItemName = row.LVistaRowName;
        PKindredItemEpithet = row.LVistaRowEpithet;
        PKindredItemFlag = LEnsignImage.LEnsignFind(row.LVistaRowLanguage);
        PKindredItemChosen = chosen;
    }

    public long PKindredItemId { get; }

    public string? PKindredItemEpithet { get; }

    public string PKindredItemName { get; }

    public ImageSource? PKindredItemFlag { get; }

    public bool PKindredItemChosen { get; private set; }

    internal LVistaRow PKindredItemRow { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal static IReadOnlyList<PKindredItem> PKindredItemBuild(IReadOnlyList<LVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return LSplice.LSpliceBuild(rows, row => new PKindredItem(row, row.LVistaRowChosen));
    }

    internal static bool PKindredItemMatch(PKindredItem held, PKindredItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        return held.PKindredItemRow.LVistaRowMatch(fresh.PKindredItemRow);
    }

    internal static void PKindredItemSync(PKindredItem held, PKindredItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        held.PKindredItemChosen = fresh.PKindredItemChosen;
        held.PropertyChanged?.Invoke(held, PKindredItemMark);
    }

    internal static void PKindredItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PKindredItem kindred)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PKindredRow") is Button row)
        {
            if (kindred.PKindredItemChosen)
            {
                row.Tag = "Chosen";
            }
            else
            {
                row.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<Image>(container, "PKindredFlag") is Image flag)
        {
            flag.Source = kindred.PKindredItemFlag;
        }

        if (PLook.PLookPartFind<Run>(container, "PKindredName") is Run name)
        {
            name.Text = kindred.PKindredItemName;
        }

        if (PLook.PLookPartFind<Run>(container, "PKindredEpithet") is Run epithet)
        {
            epithet.Text = " " + kindred.PKindredItemEpithet;
        }
    }
}
