using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Shapes;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TLecternEtymology
{
    private const string TLecternEtymologyFace = """
        <DataTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
            <StackPanel>
                <TextBlock x:Name="PEtymologyTongue" />
                <Border x:Name="PEtymologyFrame" />
                <Rectangle x:Name="PEtymologyTrace" Visibility="Collapsed" />
                <TextBlock x:Name="PEtymologyText" />
                <TextBlock x:Name="PEtymologyMark" Visibility="Collapsed" />
                <TextBlock x:Name="PEtymologyGloss" />
                <TextBlock x:Name="PEtymologyRelation" />
            </StackPanel>
        </DataTemplate>
        """;

    [Fact]
    public void LecternEtymologyApply_LinkedBorrowedBox_PaintsTextLinkAndDashedTrace()
    {
        CAncestor box = new(4, "4", 0, "aqua", "Latin", 9, true, "water", 2, false);
        CAncestorChoice relation = new(2, string.Empty, "Kin", true, false);
        string[] texts = [];
        Hyperlink? link = null;
        Visibility[]? shown = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                ContentPresenter container = new()
                {
                    Content = box,
                    ContentTemplate = (DataTemplate)XamlReader.Parse(TLecternEtymologyFace),
                };
                TInterfaceDeportment.TLecternEtymologyApply(container, box, relation);
                DataTemplate face = container.ContentTemplate;
                TextBlock text = (TextBlock)face.FindName("PEtymologyText", container);
                link = text.Inlines.OfType<Hyperlink>().SingleOrDefault();
                texts =
                [
                    ((TextBlock)face.FindName("PEtymologyTongue", container)).Text,
                    new TextRange(text.ContentStart, text.ContentEnd).Text,
                    ((TextBlock)face.FindName("PEtymologyGloss", container)).Text,
                    ((TextBlock)face.FindName("PEtymologyRelation", container)).Text,
                ];
                shown =
                [
                    ((Border)face.FindName("PEtymologyFrame", container)).Visibility,
                    ((Rectangle)face.FindName("PEtymologyTrace", container)).Visibility,
                    ((TextBlock)face.FindName("PEtymologyMark", container)).Visibility,
                ];
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
        Assert.NotNull(link);
        Assert.Equal(["Latin", "aqua", "water", "Kin"], texts);
        Assert.Equal([Visibility.Collapsed, Visibility.Visible, Visibility.Visible], shown);
    }
}
