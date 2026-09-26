using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LIndexItem : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs LIndexItemMark = new(nameof(LIndexItemChosen));

    private LIndexItem(LVistaRow row, ImageSource? flag)
    {
        LIndexItemRow = row;
        LIndexItemId = row.LVistaRowId;
        LIndexItemName = row.LVistaRowName;
        LIndexItemEpithet = row.LVistaRowEpithet ?? string.Empty;
        LIndexItemLanguage = row.LVistaRowLanguage;
        LIndexItemFlag = flag;
        LIndexItemChosen = row.LVistaRowChosen;
    }

    public long LIndexItemId { get; }

    public string LIndexItemName { get; }

    public string LIndexItemEpithet { get; }

    public string LIndexItemLanguage { get; }

    public ImageSource? LIndexItemFlag { get; }

    public bool LIndexItemChosen { get; private set; }

    private LVistaRow LIndexItemRow { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static IReadOnlyList<LIndexItem> LIndexItemBuild(IReadOnlyList<LVistaRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return LSplice.LSpliceBuild(
            rows, row => new LIndexItem(row, LEnsignImage.LEnsignFind(row.LVistaRowLanguage)));
    }

    public static bool LIndexItemMatch(LIndexItem held, LIndexItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        return held.LIndexItemRow.LVistaRowMatch(fresh.LIndexItemRow);
    }

    public static void LIndexItemSync(LIndexItem held, LIndexItem fresh)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(fresh);

        if (held.LIndexItemChosen == fresh.LIndexItemChosen)
        {
            return;
        }

        held.LIndexItemChosen = fresh.LIndexItemRow.LVistaRowChosen;
        held.PropertyChanged?.Invoke(held, LIndexItemMark);
    }

    internal static void LIndexItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LIndexItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<Button>(container, "PIndexRow") is Button button)
        {
            if (row.LIndexItemChosen)
            {
                button.Tag = "Chosen";
            }
            else
            {
                button.ClearValue(FrameworkElement.TagProperty);
            }
        }

        if (PLook.PLookPartFind<Image>(container, "PIndexFlag") is Image flag)
        {
            flag.Source = row.LIndexItemFlag;
        }

        if (PLook.PLookPartFind<Run>(container, "PIndexName") is Run name)
        {
            name.Text = row.LIndexItemName;
        }

        if (PLook.PLookPartFind<Run>(container, "PIndexEpithet") is Run epithet)
        {
            epithet.Text = " " + row.LIndexItemEpithet;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PIndexLanguage") is TextBlock language)
        {
            language.Text = row.LIndexItemLanguage;
        }
    }
}
