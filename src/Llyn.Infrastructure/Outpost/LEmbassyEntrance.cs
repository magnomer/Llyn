using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LEmbassyEntrance : LOutpost
{
    private readonly LOutpostPairing _lEmbassyEntrancePairing;
    private readonly LOutpostBatch _lEmbassyEntranceBatch;

    public LEmbassyEntrance(LKeep keep, LEngraverEntrance engraver)
        : this(keep, engraver, LOutpostWire.LOutpostWireClient)
    {
    }

    public LEmbassyEntrance(LKeep keep, LEngraverEntrance engraver, HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(keep);
        ArgumentNullException.ThrowIfNull(engraver);
        ArgumentNullException.ThrowIfNull(client);
        LOutpostHttp http = new(new LOutpostWire(client));
        _lEmbassyEntrancePairing = new LOutpostPairing(http);
        _lEmbassyEntranceBatch = new LOutpostBatch(http, engraver, new LManifestFile(keep));
    }

    Task<string> LOutpost.LOutpostAttach(int port, Action<Exception> fault, CancellationToken cancellation) =>
        _lEmbassyEntrancePairing.LOutpostPairingAttach(port, fault, cancellation);

    Task<LReceipt> LOutpost.LOutpostSend(LOutpostCargo cargo, CancellationToken cancellation) =>
        _lEmbassyEntranceBatch.LOutpostBatchSend(cargo, cancellation);
}
