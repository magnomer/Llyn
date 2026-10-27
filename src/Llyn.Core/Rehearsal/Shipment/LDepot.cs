using System;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LDepot
{
    Task<LDeckReceipt> LDepotExport(LDepotShipment shipment, Action<Exception> fault, CancellationToken cancellation);
}
