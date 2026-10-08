using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDuplex
{
    private readonly QWing _qLeftWing;

    private readonly QWing _qRightWing;

    internal QDuplex(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qLeftWing = new QWing(QContract.QContractFind<UserControl>(surface, "PLeftWing"));
        _qRightWing = new QWing(QContract.QContractFind<UserControl>(surface, "PRightWing"));
    }

    internal void QDuplexIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        _qLeftWing.QWingIntroduce(atelier, envoy, volume, mentionMenu, true);
        _qRightWing.QWingIntroduce(atelier, envoy, volume, mentionMenu, false);
    }
}
