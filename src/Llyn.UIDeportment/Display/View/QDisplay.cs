using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDisplay
{
    private readonly FrameworkElement _qDisplaySurface;

    internal QDisplay(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDisplaySurface = surface;
    }

    private ScrollViewer QDisplayContents =>
        QContract.QContractFind<ScrollViewer>(_qDisplaySurface, "PDisplayContents");

    private QSwath QDisplaySwath => QContract.QContractFind<QSwath>(_qDisplaySurface, "PDisplaySwath");

    internal void QDisplayVisibleRefine(Visibility visible)
    {
        _qDisplaySurface.Visibility = visible;
    }

    internal void QDisplayIntroduce(
        CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu, CDisplay display)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(volume);
        ArgumentNullException.ThrowIfNull(mentionMenu);
        ArgumentNullException.ThrowIfNull(display);

        QLook.QLookStyleAttach(_qDisplaySurface);

        QDisplaySwath.QSwathAttach(QDisplayContents);
        display.CDisplayOpened += QDisplaySwath.QSwathClear;
        display.CDisplayClosed += QDisplaySwath.QSwathClear;
        QLectern lectern = new(_qDisplaySurface, display, atelier, envoy);
        lectern.QLecternCard.QLecternMentionNotice += mentionMenu.QMentionOfferRefine;
        volume.QVolumeSliderAttach(_qDisplaySurface);
    }
}
