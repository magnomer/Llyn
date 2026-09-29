using System;
using System.Windows.Controls;

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

    internal void QDuplexIntroduce(PWindow host)
    {
        _qLeftWing.QWingIntroduce(host, true);
        _qRightWing.QWingIntroduce(host, false);
    }
}
