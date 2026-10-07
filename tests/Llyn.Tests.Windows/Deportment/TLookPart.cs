using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLookPart
{
    [Fact]
    public void LookPartFind_TemplateSelector_FindsThePartOfTheChosenTemplate()
    {
        TextBlock? part = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                FrameworkElementFactory chip = new(typeof(Border));
                chip.AppendChild(new FrameworkElementFactory(typeof(TextBlock)) { Name = "PRegisterName" });
                ItemsControl list = new()
                {
                    Template = new ControlTemplate(typeof(ItemsControl))
                    {
                        VisualTree = new FrameworkElementFactory(typeof(ItemsPresenter)),
                    },
                    ItemTemplateSelector = new PRegisterSelector
                    {
                        PRegisterSelectorChip = new DataTemplate { VisualTree = chip },
                    },
                    ItemsSource = new[] { "situation" },
                };
                list.Measure(new Size(400, 400));
                list.Arrange(new Rect(0, 0, 400, 400));
                list.UpdateLayout();

                FrameworkElement row = (FrameworkElement)list.ItemContainerGenerator.ContainerFromIndex(0);
                part = QLook.QLookPartFind<TextBlock>(row, "PRegisterName");
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
        Assert.NotNull(part);
    }
}
