using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDrawerItem
{
    internal QDrawerItem(CProfferRow row)
    {
        QDrawerItemId = row.CProfferRowId;
        QDrawerItemLead = row.CProfferRowLead;
        QDrawerItemMark = row.CProfferRowMark;
        QDrawerItemTail = row.CProfferRowTail;
        QDrawerItemCount = row.CProfferRowCount;
    }

    public long QDrawerItemId { get; }

    public string QDrawerItemLead { get; }

    public string QDrawerItemMark { get; }

    public string QDrawerItemTail { get; }

    public string QDrawerItemCount { get; }

    internal static void QDrawerItemApply(
        FrameworkElement container, object item, MouseButtonEventHandler press)
    {
        if (item is not QDrawerItem drawer)
        {
            return;
        }

        if (QLook.QLookPartFind<Grid>(container, "PCitationRow") is Grid row)
        {
            row.PreviewMouseLeftButtonDown -= press;
            row.PreviewMouseLeftButtonDown += press;
        }

        if (QLook.QLookPartFind<Run>(container, "PCitationLead") is Run lead)
        {
            lead.Text = drawer.QDrawerItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PCitationMark") is Run mark)
        {
            mark.Text = drawer.QDrawerItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PCitationTail") is Run tail)
        {
            tail.Text = drawer.QDrawerItemTail;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PCitationCount") is TextBlock count)
        {
            count.Text = drawer.QDrawerItemCount;
        }
    }
}
