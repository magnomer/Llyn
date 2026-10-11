using System;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LGalleyRelay : IAsyncDisposable
{
    private readonly LGalleyClerk _lGalleyRelayClerk;

    internal LGalleyRelay(LGalleyClerk clerk, LGalley galley)
    {
        ArgumentNullException.ThrowIfNull(clerk);
        ArgumentNullException.ThrowIfNull(galley);

        _lGalleyRelayClerk = clerk;
        LGalleyRelayGalley = galley;
    }

    public LGalley LGalleyRelayGalley { get; }

    public async Task<LGalleyProof?> LGalleyRelayDraw()
    {
        if (!await _lGalleyRelayClerk.LGalleyClerkDraw(LGalleyRelayGalley.LGalleySetting))
        {
            return null;
        }

        LGalleyRelayGalley.LGalleyRangeSet();
        if (_lGalleyRelayClerk.LGalleyClerkCount is not int count)
        {
            return null;
        }

        const double unit = 96.0;
        LPressPaper? paper = _lGalleyRelayClerk.LGalleyClerkRead(0);
        return new LGalleyProof(
            count,
            (paper?.LPressPaperWidth ?? 0.0) * unit,
            (paper?.LPressPaperHeight ?? 0.0) * unit,
            LGalleyClerk.LGalleyClerkResolution);
    }

    public Task<LPressImage?> LGalleyRelayDraw(int page, int? resolution)
    {
        return _lGalleyRelayClerk.LGalleyClerkDraw(page, resolution);
    }

    public async Task<LPressStatus> LGalleyRelayPrint(Func<string, string?> path)
    {
        ArgumentNullException.ThrowIfNull(path);

        LPressSetting setting = LGalleyRelayGalley.LGalleySetting;
        string? file = setting.LPressSettingPrinter is null ? path(LGalleyRelayGalley.LGalleyFile) : null;
        if (setting.LPressSettingPrinter is null && file is null)
        {
            return LPressStatus.LPressStatusDone;
        }

        return await _lGalleyRelayClerk.LGalleyClerkPrint(setting, file);
    }

    public Task LGalleyRelayShow()
    {
        return _lGalleyRelayClerk.LGalleyClerkShow();
    }

    public ValueTask DisposeAsync()
    {
        return _lGalleyRelayClerk.DisposeAsync();
    }
}
