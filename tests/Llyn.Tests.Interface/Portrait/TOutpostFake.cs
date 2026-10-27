using Llyn.Core;

namespace Llyn.Tests;

public sealed class TOutpostFake : LOutpost
{
    public List<LOutpostCargo> TOutpostFakeCargo { get; } = [];

    public List<int> TOutpostFakePort { get; } = [];

    public string TOutpostFakeToken { get; set; } = "raw:token";

    public LReceipt TOutpostFakeReceipt { get; set; } = new(1, 0, 0, []);

    public Task<string> LOutpostAttach(int port, Action<Exception> fault, CancellationToken cancellation)
    {
        TOutpostFakePort.Add(port);
        return Task.FromResult(TOutpostFakeToken);
    }

    public Task<LReceipt> LOutpostSend(LOutpostCargo cargo, CancellationToken cancellation)
    {
        TOutpostFakeCargo.Add(cargo);
        return Task.FromResult(TOutpostFakeReceipt);
    }
}
