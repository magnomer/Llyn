using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowFailureRefine(string key, Exception exception)
    {
        PWindowEnvoy.CEnvoyFailureShow(key, PWindowAtelier.CAtelierLedger.CLedgerNoticeRead(exception));
    }
}
