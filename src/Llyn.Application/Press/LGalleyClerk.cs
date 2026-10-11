using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LGalleyClerk : IAsyncDisposable
{
    public const int LGalleyClerkResolution = 300;

    private readonly Dictionary<LPressProof, int> _lGalleyClerkReader = new(ReferenceEqualityComparer.Instance);

    private LPressBoard? _lGalleyClerkBoard;

    private LPressProof? _lGalleyClerkProof;

    private LPressSetting? _lGalleyClerkSetting;

    private int _lGalleyClerkGeneration;

    internal LGalleyClerk(LPressBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        _lGalleyClerkBoard = board;
    }

    public int? LGalleyClerkCount
    {
        get
        {
            lock (_lGalleyClerkReader)
            {
                return _lGalleyClerkProof?.LPressProofCount;
            }
        }
    }

    public bool LGalleyClerkCheck(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        LPressSetting? drawn = Volatile.Read(ref _lGalleyClerkSetting);
        return drawn is not null && setting.LPressSettingMatch(drawn);
    }

    public async Task<bool> LGalleyClerkDraw(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        int generation = Interlocked.Increment(ref _lGalleyClerkGeneration);
        if (LGalleyClerkCheck(setting))
        {
            return true;
        }

        LPressBoard board = _lGalleyClerkBoard ?? throw new ObjectDisposedException(nameof(LGalleyClerk));
        LPressProof proof = await board.LPressBoardDraw(setting);
        if (generation != Volatile.Read(ref _lGalleyClerkGeneration))
        {
            proof.Dispose();
            return false;
        }

        LGalleyClerkSet(proof, setting);
        return true;
    }

    public async Task<LPressImage?> LGalleyClerkDraw(int page, int? resolution = null)
    {
        int dots = resolution ?? LGalleyClerkResolution;
        LPressProof? proof;
        lock (_lGalleyClerkReader)
        {
            proof = _lGalleyClerkProof;
            if (proof is null)
            {
                return null;
            }

            ArgumentOutOfRangeException.ThrowIfNegative(page);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(page, proof.LPressProofCount);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dots);
            _lGalleyClerkReader[proof] = _lGalleyClerkReader.GetValueOrDefault(proof) + 1;
        }

        bool retired = false;
        try
        {
            LPressImage image = await proof.LPressProofDraw(page, dots);
            lock (_lGalleyClerkReader)
            {
                return ReferenceEquals(proof, _lGalleyClerkProof) ? image : null;
            }
        }
        finally
        {
            lock (_lGalleyClerkReader)
            {
                int left = _lGalleyClerkReader[proof] - 1;
                if (left > 0)
                {
                    _lGalleyClerkReader[proof] = left;
                }
                else
                {
                    _lGalleyClerkReader.Remove(proof);
                    retired = !ReferenceEquals(proof, _lGalleyClerkProof);
                }
            }

            if (retired)
            {
                proof.Dispose();
            }
        }
    }

    public LPressPaper? LGalleyClerkRead(int page)
    {
        lock (_lGalleyClerkReader)
        {
            LPressProof? proof = _lGalleyClerkProof;
            return proof is not null && page >= 0 && page < proof.LPressProofCount
                ? proof.LPressProofRead(page)
                : null;
        }
    }

    public async Task<LPressStatus> LGalleyClerkPrint(LPressSetting setting, string? file)
    {
        ArgumentNullException.ThrowIfNull(setting);

        LPressBoard board = _lGalleyClerkBoard ?? throw new ObjectDisposedException(nameof(LGalleyClerk));
        if (file is null)
        {
            return await board.LPressBoardPrint(setting);
        }

        await board.LPressBoardExport(setting, file);
        return LPressStatus.LPressStatusDone;
    }

    public async Task LGalleyClerkShow()
    {
        await (_lGalleyClerkBoard ?? throw new ObjectDisposedException(nameof(LGalleyClerk))).LPressBoardShow();
    }

    public async ValueTask DisposeAsync()
    {
        Interlocked.Increment(ref _lGalleyClerkGeneration);

        LPressBoard? board = _lGalleyClerkBoard;
        _lGalleyClerkBoard = null;
        LGalleyClerkSet(null, null);
        if (board is not null)
        {
            await board.DisposeAsync();
        }
    }

    private void LGalleyClerkSet(LPressProof? proof, LPressSetting? setting)
    {
        LPressProof? replaced;
        lock (_lGalleyClerkReader)
        {
            replaced = _lGalleyClerkProof;
            _lGalleyClerkProof = proof;
            Volatile.Write(ref _lGalleyClerkSetting, setting);
            if (replaced is null || _lGalleyClerkReader.ContainsKey(replaced))
            {
                return;
            }
        }

        replaced.Dispose();
    }
}
