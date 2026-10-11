using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSPress
{
    private readonly LGalley _csPressEngine;

    private readonly LGalleyRelay _csPressRelay;

    private readonly CEnvoy _csPressEnvoy;

    private readonly LSettingsPort _csPressSettings;

    private int _csPressPage;

    private bool _csPressRunning;

    private CSPressSetting? _csPressAnswer;

    internal CSPress(LGalleyRelay relay, CEnvoy envoy, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(relay);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);

        _csPressEngine = relay.LGalleyRelayGalley;
        _csPressRelay = relay;
        _csPressEnvoy = envoy;
        _csPressSettings = settings;
        CSPressGalley = new CSGalley(relay, envoy, settings, this);
    }

    public event Action<CSPressSetting>? CSPressSettingChanged;

    public CSGalley CSPressGalley { get; }

    public CSPressSetting CSPressSettingRead()
    {
        _csPressAnswer = CSPressWording.LSPressWordingRead(
            LGalleyView.LGalleyViewRead(_csPressEngine), _csPressPage, _csPressRunning);
        return _csPressAnswer;
    }

    public async Task<CSPressSetting> CSPressRestore()
    {
        try
        {
            LGalleyView view = await _csPressEngine.LGalleyRestore();
            _csPressAnswer = CSPressWording.LSPressWordingRead(view, _csPressPage, _csPressRunning);
            return LSPressSettingRaise(_csPressAnswer);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_csPressEnvoy, _csPressSettings, "Print.Failed", exception);
            return LSPressSettingRaise(CSPressSettingRead());
        }
    }

    private CSPressSetting LSPressSettingRaise(CSPressSetting setting)
    {
        CSPressGalley.LSGalleyProofDefer(setting);
        CSPressSettingChanged?.Invoke(setting);
        return setting;
    }

    public CSPressSetting CSPressPrinterSelect(int index)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        if (index >= 0 && index < answer.CSPressSettingPrinter.Count)
        {
            _csPressEngine.LGalleyPrinterSelect(answer.CSPressSettingPrinter[index].CSPressPrinterName);
        }

        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressCopiesSet(string text)
    {
        _csPressEngine.LGalleyCopiesSet(text);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressCollationSet(bool collation)
    {
        _csPressEngine.LGalleyCollationSet(collation);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressRangeSet(CSPressRange range, string text)
    {
        _csPressEngine.LGalleyRangeSet(
            range switch
            {
                CSPressRange.CSPressRangeAll => LPressScope.LPressScopeAll,
                CSPressRange.CSPressRangeCurrent => LPressScope.LPressScopeCurrent,
                CSPressRange.CSPressRangeCustom => LPressScope.LPressScopeCustom,
                _ => throw new ArgumentOutOfRangeException(nameof(range), range, null),
            },
            text,
            _csPressPage);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressRangeSelect(int index)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return index >= 0 && index < answer.CSPressSettingRange.Count
            ? CSPressRangeSet(
                answer.CSPressSettingRange[index].CSPressChoiceValue, answer.CSPressSettingText ?? string.Empty)
            : LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressRangeSet(string text)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return CSPressRangeSet(
            answer.CSPressSettingRange.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue,
            text);
    }

    internal void LSPressPageSelect(int page)
    {
        if (page >= 0 && page != _csPressPage)
        {
            _csPressEngine.LGalleyPageSelect(page);
            _csPressPage = page;
            LSPressSettingRaise(CSPressSettingRead());
        }
    }

    public CSPressSetting CSPressPageSelect(string text)
    {
        _csPressPage = _csPressEngine.LGalleyPageSelect(text, _csPressPage);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressOrientationSet(CSPressOrientation orientation)
    {
        _csPressEngine.LGalleyOrientationSet(orientation switch
        {
            CSPressOrientation.CSPressOrientationPortrait => LPressOrientation.LPressOrientationPortrait,
            CSPressOrientation.CSPressOrientationLandscape => LPressOrientation.LPressOrientationLandscape,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null),
        });
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressPaperSelect(int index)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        if (index >= 0 && index < answer.CSPressSettingPaper.Count)
        {
            _csPressEngine.LGalleyPaperSelect(index);
        }

        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressMarginSelect(int index)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return index >= 0 && index < answer.CSPressSettingMargin.Count
            ? CSPressMarginSet(answer.CSPressSettingMargin[index].CSPressChoiceValue)
            : LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressMarginSet(CSPressMargin margin)
    {
        _csPressEngine.LGalleyMarginSet(margin switch
        {
            CSPressMargin.CSPressMarginDefault => "Default",
            CSPressMargin.CSPressMarginNone => "None",
            CSPressMargin.CSPressMarginNarrow => "Narrow",
            CSPressMargin.CSPressMarginWide => "Wide",
            CSPressMargin.CSPressMarginCustom => (string?)null,
            _ => throw new ArgumentOutOfRangeException(nameof(margin), margin, null),
        });
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressMarginSet(string top, string bottom, string left, string right)
    {
        _csPressEngine.LGalleyMarginSet(top, bottom, left, right);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressTopSet(string text)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return CSPressMarginSet(
            text,
            LSPressInsetRead(answer.CSPressSettingBottom),
            LSPressInsetRead(answer.CSPressSettingLeft),
            LSPressInsetRead(answer.CSPressSettingRight));
    }

    public CSPressSetting CSPressBottomSet(string text)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return CSPressMarginSet(
            LSPressInsetRead(answer.CSPressSettingTop),
            text,
            LSPressInsetRead(answer.CSPressSettingLeft),
            LSPressInsetRead(answer.CSPressSettingRight));
    }

    public CSPressSetting CSPressLeftSet(string text)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return CSPressMarginSet(
            LSPressInsetRead(answer.CSPressSettingTop),
            LSPressInsetRead(answer.CSPressSettingBottom),
            text,
            LSPressInsetRead(answer.CSPressSettingRight));
    }

    public CSPressSetting CSPressRightSet(string text)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return CSPressMarginSet(
            LSPressInsetRead(answer.CSPressSettingTop),
            LSPressInsetRead(answer.CSPressSettingBottom),
            LSPressInsetRead(answer.CSPressSettingLeft),
            text);
    }

    private static string LSPressInsetRead(double? inset)
    {
        return inset?.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
    }

    public CSPressSetting CSPressScaleSet(string text)
    {
        _csPressEngine.LGalleyScaleSet(text);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressColorSet(CSPressColor color)
    {
        _csPressEngine.LGalleyColorSet(color switch
        {
            CSPressColor.CSPressColorFull => LPressColor.LPressColorFull,
            CSPressColor.CSPressColorGray => LPressColor.LPressColorGray,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
        });
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressSideSet(CSPressSide side)
    {
        _csPressEngine.LGalleySideSet(side switch
        {
            CSPressSide.CSPressSideSingle => LPressSide.LPressSideSingle,
            CSPressSide.CSPressSideLong => LPressSide.LPressSideLong,
            CSPressSide.CSPressSideShort => LPressSide.LPressSideShort,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
        });
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressSideSelect(int index)
    {
        CSPressSetting answer = _csPressAnswer ?? CSPressSettingRead();
        return answer.CSPressSettingSide is { } sides && index >= 0 && index < sides.Count
            ? CSPressSideSet(sides[index].CSPressChoiceValue)
            : LSPressSettingRaise(CSPressSettingRead());
    }

    public CSPressSetting CSPressBackgroundSet(bool background)
    {
        _csPressEngine.LGalleyBackgroundSet(background);
        return LSPressSettingRaise(CSPressSettingRead());
    }

    public async Task<string?> CSPressSystemShow()
    {
        try
        {
            await _csPressRelay.LGalleyRelayShow();
            return null;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_csPressEnvoy, _csPressSettings, "Print.Failed", exception);
            return "Print.Failed";
        }
    }

    public async Task<string?> CSPressPrint()
    {
        if (_csPressRunning)
        {
            return "Print.Printing";
        }

        bool asked = false;
        try
        {
            Task<LPressStatus> printing = _csPressRelay.LGalleyRelayPrint(file =>
            {
                asked = true;
                return _csPressEnvoy.CEnvoyFileRead(
                    file,
                    [CPortrait.LPortraitChoiceCreate(LPortraitMedium.LPortraitMediumPdf, ".pdf", true)]).CEnvoyFile;
            });
            _csPressRunning = !printing.IsCompleted;
            CSPressSettingChanged?.Invoke(CSPressSettingRead());
            return CSPressWording.LSPressWordingRead(await printing);
        }
        catch (Exception exception)
        {
            string key = asked ? "Print.SaveFailed" : "Print.Failed";
            CLedger.LLedgerFailureShow(_csPressEnvoy, _csPressSettings, key, exception);
            return key;
        }
        finally
        {
            _csPressRunning = false;
            CSPressSettingChanged?.Invoke(CSPressSettingRead());
        }
    }

    public bool CSPressClose(bool cancel)
    {
        if (cancel || _csPressRunning)
        {
            return false;
        }

        try
        {
            _csPressEngine.LGalleySave();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_csPressEnvoy, _csPressSettings, "Print.Failed", exception);
        }

        return true;
    }
}
