using System;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourierWarrant
{
    private const int LCourierWarrantPatience = 120;

    private static readonly TimeSpan LCourierWarrantInterval = TimeSpan.FromSeconds(1);

    private readonly LOutpost _lCourierWarrantOutpost;
    private readonly LWarrant _lCourierWarrantKeeper;
    private readonly Action<Exception> _lCourierWarrantFault;

    public LCourierWarrant(LOutpost outpost, LWarrant keeper, Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(outpost);
        ArgumentNullException.ThrowIfNull(keeper);
        ArgumentNullException.ThrowIfNull(fault);
        _lCourierWarrantOutpost = outpost;
        _lCourierWarrantKeeper = keeper;
        _lCourierWarrantFault = fault;
    }

    public async Task<string> LCourierWarrantAttach(int stored, CancellationToken cancellation)
    {
        int port = await _lCourierWarrantOutpost.LOutpostFind(stored, cancellation)
            .ConfigureAwait(false) ?? throw new LRefusal(LRefusal.LRefusalOutpost);
        string ticket;
        try
        {
            ticket = await _lCourierWarrantOutpost.LOutpostWarrantStart(port, cancellation)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
        {
            _lCourierWarrantFault(exception);
            throw new LRefusal(LRefusal.LRefusalOutpost);
        }

        int stalled = 0;
        for (int poll = 0; poll < LCourierWarrantPatience; poll++)
        {
            await Task.Delay(LCourierWarrantInterval, cancellation).ConfigureAwait(false);
            LWarrantAnswer answer;
            try
            {
                answer = await _lCourierWarrantOutpost
                    .LOutpostWarrantCheck(port, ticket, cancellation).ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                _lCourierWarrantFault(exception);
                stalled++;
                if (stalled >= LCourierClerk.LCourierStall)
                {
                    throw new LRefusal(LRefusal.LRefusalOutpost);
                }

                continue;
            }

            stalled = 0;
            if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateAccepted)
            {
                string token = answer.LWarrantAnswerToken ?? throw new LRefusal(LRefusal.LRefusalWarrant);
                return _lCourierWarrantKeeper.LWarrantHide(token);
            }

            if (answer.LWarrantAnswerState == LWarrantState.LWarrantStateRejected)
            {
                throw new LRefusal(LRefusal.LRefusalWarrant);
            }
        }

        throw new LRefusal(LRefusal.LRefusalPending);
    }

    public static bool LCourierWarrantCheck(Exception exception)
    {
        return exception is LRefusal { LRefusalReason: LRefusal.LRefusalWarrant };
    }
}
