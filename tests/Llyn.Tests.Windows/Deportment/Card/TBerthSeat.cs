using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TBerthSeat
{
    [Theory]
    [InlineData("a", 0)]
    [InlineData("b", 1)]
    [InlineData(null, 3)]
    [InlineData("gone", 3)]
    public void BerthAnchor_SeatsTheEntryBeforeItsChipWithoutTouchingTheChips(string? anchor, int seat)
    {
        List<object>? order = null;
        TextBox? entry = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                List<string> chips = ["a", "b", "c"];
                ItemsControl list = new()
                {
                    Template = new ControlTemplate(typeof(ItemsControl))
                    {
                        VisualTree = new FrameworkElementFactory(typeof(ItemsPresenter)),
                    },
                    ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(QBerth))),
                    ItemsSource = chips,
                };
                entry = new TextBox();
                list.SetValue(QBerth.QBerthEntryProperty, entry);
                list.SetValue(QBerth.QBerthAnchorProperty, chips.Find(chip => chip == anchor) ?? anchor);
                list.Measure(new Size(1000, 1000));
                list.Arrange(new Rect(0, 0, 1000, 1000));

                QBerth berth = TBerthFind(list) ?? throw new InvalidOperationException("No berth.");
                order = [];
                for (int child = 0; child < VisualTreeHelper.GetChildrenCount(berth); child++)
                {
                    DependencyObject shown = VisualTreeHelper.GetChild(berth, child);
                    order.Add(shown is ContentPresenter { Content: string chip } ? chip : shown);
                }
            }
            catch (Exception caught)
            {
                failure = caught;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        Assert.Null(failure);
        Assert.NotNull(order);
        Assert.Equal(4, order.Count);
        Assert.Same(entry, order[seat]);
        order.RemoveAt(seat);
        Assert.Equal(["a", "b", "c"], order);
    }

    private static QBerth? TBerthFind(DependencyObject root)
    {
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, index);
            if (child is QBerth berth)
            {
                return berth;
            }

            if (TBerthFind(child) is QBerth found)
            {
                return found;
            }
        }

        return null;
    }
}
