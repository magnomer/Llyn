using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSGalley : IDisposable
{
    private readonly LGalleyRelay _csGalleyEngine;

    private readonly CEnvoy _csGalleyEnvoy;

    private readonly LSettingsPort _csGalleySettings;

    private readonly CSPress _csGalleyPress;

    private CSGalleyWindow _csGalleyWindow = new(0, 0, 0, -1, null, new HashSet<CSGalleyLeaf>());

    private bool _csGalleyClosed;

    internal CSGalley(LGalleyRelay relay, CEnvoy envoy, LSettingsPort settings, CSPress press)
    {
        ArgumentNullException.ThrowIfNull(relay);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(press);

        _csGalleyEngine = relay;
        _csGalleyEnvoy = envoy;
        _csGalleySettings = settings;
        _csGalleyPress = press;
    }

    public event Action<CSPressState>? CSGalleyStateChanged;

    public event Action<CSPressPage>? CSGalleyPageChanged;

    public CSLens CSGalleyLens { get; } = new();

    internal CSGalleyDelay CSGalleyEditDelay { get; } = new(250);

    internal CSGalleyDelay CSGalleySightDelay { get; } = new(150);

    public async Task<CSPressState?> CSGalleyDraw()
    {
        CSPressState state;
        try
        {
            LGalleyProof? proof = await _csGalleyEngine.LGalleyRelayDraw();
            if (proof is null)
            {
                return null;
            }

            state = new CSPressState(
                proof.LGalleyProofCount,
                proof.LGalleyProofWidth,
                proof.LGalleyProofHeight,
                CSGalleyLens.LSLensPaperSet(
                    proof.LGalleyProofWidth, proof.LGalleyProofHeight, proof.LGalleyProofResolution),
                null,
                _csGalleyPress.CSPressSettingRead());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_csGalleyEnvoy, _csGalleySettings, "Print.RenderFailed", exception);
            state = new CSPressState(
                0,
                0.0,
                0.0,
                CSGalleyLens.LSLensPaperSet(0.0, 0.0, 0),
                "Print.RenderFailed",
                _csGalleyPress.CSPressSettingRead() with { CSPressSettingStale = false });
        }

        LSGalleyWindowSet(new CSGalleyWindow(
            _csGalleyWindow.CSGalleyWindowTurn + 1, state.CSPressStateCount, 0, -1, null, new HashSet<CSGalleyLeaf>()));
        return state;
    }

    public async Task CSGalleySightSet(int page, int first, int last, double scale)
    {
        _csGalleyPress.LSPressPageSelect(page);
        if (_csGalleyClosed)
        {
            return;
        }

        if (first >= 0)
        {
            LSGalleyWindowSet(_csGalleyWindow.LSGalleyWindowMove(
                first, last, CSGalleyLens.LSLensResolutionRead(scale)));
        }

        if (await CSGalleySightDelay.LSGalleyDelayStart() is not CancellationToken token || first < 0)
        {
            return;
        }

        CSGalleyWindow window = _csGalleyWindow;
        for (int sheet = window.CSGalleyWindowLow;
            sheet <= window.CSGalleyWindowHigh && !token.IsCancellationRequested;
            sheet++)
        {
            CSGalleyLeaf leaf = new(sheet, window.CSGalleyWindowResolution);
            if (!_csGalleyWindow.LSGalleyWindowCheck(leaf))
            {
                continue;
            }

            LPressImage? image;
            try
            {
                image = await _csGalleyEngine.LGalleyRelayDraw(sheet, leaf.CSGalleyLeafResolution);
            }
            catch (Exception exception)
            {
                CLedger.LLedgerFailureShow(_csGalleyEnvoy, _csGalleySettings, "Print.RenderFailed", exception);
                return;
            }

            CSGalleyWindow now = _csGalleyWindow;
            if (_csGalleyClosed || now.CSGalleyWindowTurn != window.CSGalleyWindowTurn)
            {
                return;
            }

            if (image is not null && now.LSGalleyWindowCheck(leaf))
            {
                LSGalleyWindowSet(now.LSGalleyWindowAdd(leaf));
                CSGalleyPageChanged?.Invoke(new CSPressPage(sheet, image.LPressImagePng));
            }
        }
    }

    public void Dispose()
    {
        _csGalleyClosed = true;
        CSGalleyEditDelay.Dispose();
        CSGalleySightDelay.Dispose();
    }

    internal CSPressSetting LSGalleyProofDefer(CSPressSetting setting)
    {
        if (setting.CSPressSettingStale && !_csGalleyClosed)
        {
            _ = LSGalleyProofDraw();
        }

        return setting;
    }

    private async Task LSGalleyProofDraw()
    {
        if (await CSGalleyEditDelay.LSGalleyDelayStart() is null)
        {
            return;
        }

        CSPressState? state = await CSGalleyDraw();
        if (state is not null && !_csGalleyClosed)
        {
            CSGalleyStateChanged?.Invoke(state);
        }
    }

    private void LSGalleyWindowSet(CSGalleyWindow window)
    {
        CSGalleyWindow before = _csGalleyWindow;
        _csGalleyWindow = window;
        if (_csGalleyClosed)
        {
            return;
        }

        foreach (CSGalleyLeaf leaf in before.CSGalleyWindowHeld.OrderBy(held => held.CSGalleyLeafPage))
        {
            if (!window.CSGalleyWindowHeld.Any(held => held.CSGalleyLeafPage == leaf.CSGalleyLeafPage))
            {
                CSGalleyPageChanged?.Invoke(new CSPressPage(leaf.CSGalleyLeafPage, null));
            }
        }
    }
}
