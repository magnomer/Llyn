using System;
using System.Threading;
using System.Windows;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TEtymologyFace
{
    [Theory]
    [InlineData(true, false, true, Visibility.Visible, Visibility.Collapsed)]
    [InlineData(false, true, false, Visibility.Collapsed, Visibility.Visible)]
    [InlineData(false, false, true, Visibility.Visible, Visibility.Collapsed)]
    public void EtymologySourceShow_HandedVerdicts_PaintTheRowAndTheReadFace(
        bool editable, bool narrated, bool linked, Visibility row, Visibility prose)
    {
        (Visibility, Visibility)? faces = null;
        Exception? failure = null;

        Thread thread = new(() =>
        {
            try
            {
                QEtymology etymology = new()
                {
                    QEtymologyEditable = editable,
                    QEtymologyText = "a tale",
                    QEtymologyNarrated = narrated,
                };
                etymology.TEtymologySourceShow(linked);
                faces = etymology.TEtymologyFaceRead();
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
        Assert.Equal((row, prose), faces);
    }
}
