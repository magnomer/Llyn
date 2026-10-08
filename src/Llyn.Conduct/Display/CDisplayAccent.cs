using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayAccent
{
    private static readonly CLecternAccent _cDisplayAccentMute = new(
        new CRespellingMark(false, string.Empty, string.Empty),
        [],
        string.Empty,
        false,
        CVariety.CVarietyRead(string.Empty, string.Empty),
        [],
        false);

    private readonly LDisplaySound _cDisplayAccentVoice;

    private readonly CLedgerNoticed _cDisplayAccentNoticed;

    private readonly LLanguagePort _cDisplayAccentLanguage;

    private readonly CEnvoy _cDisplayAccentEnvoy;

    private readonly LSettingsPort _cDisplayAccentSettings;

    internal CDisplayAccent(LDisplay display, LLanguagePort languages, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayAccentVoice = display.LDisplaySound;
        _cDisplayAccentNoticed = display.LDisplayNoticed;
        _cDisplayAccentLanguage = languages;
        _cDisplayAccentEnvoy = envoy;
        _cDisplayAccentSettings = settings;
    }

    public IReadOnlyList<int> CDisplayAccentScale => _cDisplayAccentLanguage.LEngineContourScale;

    private LEntryDraft? LDisplayAccentShown => _cDisplayAccentVoice.LDisplayShown;

    public CLecternAccent CDisplayAccentRead()
    {
        if (LDisplayAccentShown is not LEntryDraft shown)
        {
            return _cDisplayAccentMute;
        }

        try
        {
            return LDisplayAccentRead(_cDisplayAccentLanguage.LEngineAccentRead(shown));
        }
        catch (Exception exception)
        {
            _cDisplayAccentNoticed.LLedgerRepaintShow(
                _cDisplayAccentEnvoy, _cDisplayAccentSettings, "Sound.LoadFailed", exception);
            return _cDisplayAccentMute;
        }
    }

    public async Task<CLecternAccent?> CDisplayAccentLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        if (LDisplayAccentShown is not LEntryDraft shown)
        {
            return null;
        }

        LAccentSheet sheet;
        try
        {
            sheet = await _cDisplayAccentLanguage.LEngineAccentLoad(
                shown, (rows, delete) => store(CCatalog.CCatalogEnsignRead(rows), delete));
        }
        catch (Exception exception)
        {
            _cDisplayAccentNoticed.LLedgerRepaintShow(
                _cDisplayAccentEnvoy, _cDisplayAccentSettings, "Sound.LoadFailed", exception);
            return null;
        }

        return ReferenceEquals(shown, LDisplayAccentShown) ? LDisplayAccentRead(sheet) : null;
    }

    private CLecternAccent LDisplayAccentRead(LAccentSheet sheet)
    {
        string language = sheet.LAccentSheetLanguage;
        return new CLecternAccent(
            new CRespellingMark(sheet.LAccentSheetRespelled, sheet.LAccentSheetOpener, sheet.LAccentSheetCloser),
            CContour.CContourRead(sheet.LAccentSheetContour, _cDisplayAccentLanguage.LEngineContourScale),
            sheet.LAccentSheetPrimary.LAccentRowText,
            sheet.LAccentSheetSpoken,
            CVariety.CVarietyRead(language, sheet.LAccentSheetPrimary.LAccentRowVariety),
            sheet.LAccentSheetRows.Select(row => CAccent.CAccentRead(language, row)).ToList(),
            sheet.LAccentSheetFlagged);
    }
}
