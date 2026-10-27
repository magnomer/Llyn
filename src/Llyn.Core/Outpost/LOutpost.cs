using System;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LOutpost
{
    Task<string> LOutpostAttach(int port, Action<Exception> fault, CancellationToken cancellation);

    Task<LReceipt> LOutpostSend(LOutpostCargo cargo, CancellationToken cancellation);
}
