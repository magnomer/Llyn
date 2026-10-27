using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal sealed class LOutpostPairing
{
    private const int LOutpostPairingPatience = 120;

    private static readonly TimeSpan LOutpostPairingInterval = TimeSpan.FromSeconds(1);

    private readonly LOutpostHttp _lOutpostPairingHttp;

    public LOutpostPairing(LOutpostHttp http)
    {
        ArgumentNullException.ThrowIfNull(http);
        _lOutpostPairingHttp = http;
    }

    public async Task<string> LOutpostPairingAttach(
        int stored, Action<Exception> fault, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(fault);

        int port = await _lOutpostPairingHttp.LOutpostFind(stored, cancellation)
            .ConfigureAwait(false) ?? throw new LRefusal(LRefusal.LRefusalOutpost);
        string ticket;
        try
        {
            ticket = await _lOutpostPairingHttp.LOutpostWarrantStart(port, cancellation)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
        {
            fault(exception);
            throw new LRefusal(LRefusal.LRefusalOutpost);
        }

        int stalled = 0;
        for (int poll = 0; poll < LOutpostPairingPatience; poll++)
        {
            await Task.Delay(LOutpostPairingInterval, cancellation).ConfigureAwait(false);
            LWarrantAnswer answer;
            try
            {
                answer = await _lOutpostPairingHttp
                    .LOutpostWarrantCheck(port, ticket, cancellation).ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                fault(exception);
                stalled++;
                if (stalled >= LOutpostBatch.LOutpostBatchStall)
                {
                    throw new LRefusal(LRefusal.LRefusalOutpost);
                }

                continue;
            }

            stalled = 0;
            if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateAccepted)
            {
                return answer.LWarrantAnswerToken ?? throw new LRefusal(LRefusal.LRefusalWarrant);
            }

            if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateRejected)
            {
                throw new LRefusal(LRefusal.LRefusalWarrant);
            }
        }

        throw new LRefusal(LRefusal.LRefusalPending);
    }
}
