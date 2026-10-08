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

    internal QLectern QDisplayLectern { get; private set; } = null!;

    private ScrollViewer QDisplayContents =>
        QContract.QContractFind<ScrollViewer>(_qDisplaySurface, "PDisplayContents");

    private PSwath QDisplaySwath => QContract.QContractFind<PSwath>(_qDisplaySurface, "PDisplaySwath");

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

        QDisplaySwath.PSwathAttach(QDisplayContents);
        display.CDisplayOpened += QDisplaySwath.PSwathClear;
        display.CDisplayClosed += QDisplaySwath.PSwathClear;
        QLectern lectern = new(_qDisplaySurface, display, atelier, envoy);
        QDisplayLectern = lectern;
        lectern.QLecternEtymology.QLecternEtymologyNotice += mentionMenu.QMentionOfferRefine;
        lectern.QLecternCard.QLecternMentionNotice += mentionMenu.QMentionOfferRefine;

        volume.QVolumeSliderAttach(_qDisplaySurface);
    }
}
