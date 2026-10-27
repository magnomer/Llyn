using Llyn.Core;

namespace Llyn.Tests;

public sealed class TDepotFake : LDepot
{
    public List<LDepotShipment> TDepotFakeShipments { get; } = [];

    public LDeckReceipt TDepotFakeReceipt { get; set; } = new(0, 0, 0, []);

    public Task<LDeckReceipt> LDepotExport(
        LDepotShipment shipment, Action<Exception> fault, CancellationToken cancellation)
    {
        TDepotFakeShipments.Add(shipment);
        return Task.FromResult(TDepotFakeReceipt);
    }
}
