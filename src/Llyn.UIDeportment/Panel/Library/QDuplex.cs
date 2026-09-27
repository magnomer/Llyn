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

    internal void QDuplexAttach(PWindow host)
    {
        _qLeftWing.QWingAttach(host);
        _qRightWing.QWingAttach(host);
    }

    internal void QDuplexRestore(CWorkspaceState state)
    {
        _qLeftWing.QWingRestore("left", state.CWorkspaceStateLeft);
        _qRightWing.QWingRestore("right", state.CWorkspaceStateRight);
    }

    internal void QDuplexClose()
    {
        _qLeftWing.QWingClose();
        _qRightWing.QWingClose();
    }
}
