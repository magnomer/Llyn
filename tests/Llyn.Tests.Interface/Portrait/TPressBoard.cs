using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPressBoard : LPressBoard
{
    private readonly SemaphoreSlim _tPressBoardTurn = new(1, 1);

    internal LPressSetting? TPressBoardSetting { get; private set; }

    internal string? TPressBoardPath { get; private set; }

    internal bool TPressBoardShown { get; private set; }

    internal int TPressBoardCount { get; private set; }

    internal bool TPressBoardDisposed { get; private set; }

    internal LPressStatus TPressBoardStatus { get; set; } = LPressStatus.LPressStatusDone;

    internal TaskCompletionSource? TPressBoardHold { get; set; }

    internal List<TPressProof> TPressBoardProof { get; } = [];

    internal Func<LPressProof, LPressProof>? TPressBoardCover { get; set; }

    public async Task<LPressProof> LPressBoardDraw(LPressSetting setting)
    {
        await _tPressBoardTurn.WaitAsync();
        try
        {
            TPressBoardSetting = setting;
            TPressBoardCount++;
            if (TPressBoardHold is { } hold)
            {
                await hold.Task;
            }

            LPressPaper paper = setting.LPressSettingPaper;
            TPressProof proof = new(
                setting.LPressSettingOrientation == LPressOrientation.LPressOrientationLandscape
                    ? new LPressPaper(paper.LPressPaperHeight, paper.LPressPaperWidth, paper.LPressPaperName)
                    : paper);
            TPressBoardProof.Add(proof);
            return TPressBoardCover?.Invoke(proof) ?? proof;
        }
        finally
        {
            _tPressBoardTurn.Release();
        }
    }

    public async Task<LPressStatus> LPressBoardPrint(LPressSetting setting)
    {
        await _tPressBoardTurn.WaitAsync();
        try
        {
            TPressBoardSetting = setting;
            if (TPressBoardHold is { } hold)
            {
                await hold.Task;
            }

            return TPressBoardStatus;
        }
        finally
        {
            _tPressBoardTurn.Release();
        }
    }

    public Task LPressBoardExport(LPressSetting setting, string path)
    {
        TPressBoardSetting = setting;
        TPressBoardPath = path;
        return Task.CompletedTask;
    }

    public Task LPressBoardShow()
    {
        TPressBoardShown = true;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        TPressBoardDisposed = true;
        return ValueTask.CompletedTask;
    }
}
