using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class QWindow
{
    internal void QWindowFailureRefine(string key, Exception exception)
    {
        QWindowEnvoy.CEnvoyFailureShow(key, QWindowAtelier.CAtelierLedger.CLedgerNoticeRead(exception));
    }
}
