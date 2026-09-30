using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCatalog
{
    private readonly CAtelier _cCatalogAtelier;

    internal CCatalog(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cCatalogAtelier = atelier;
    }

    internal static CFont LCatalogFontRead(LSettingsPort settings, string language, CFontRole role)
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (string.IsNullOrWhiteSpace(language))
        {
            return new CFont(null, null, null);
        }

        try
        {
            LFont font = settings.LEngineFontRead(language, LCatalogRoleRead(role));
            return new CFont(font.LFontFamily, font.LFontSized, font.LFontStyle);
        }
        catch (Exception)
        {
            return new CFont(null, null, null);
        }
    }

    private static LFontRole LCatalogRoleRead(CFontRole role)
    {
        return role switch
        {
            CFontRole.CFontRoleHeadword => LFontRole.LFontRoleHeadword,
            CFontRole.CFontRoleExample => LFontRole.LFontRoleExample,
            CFontRole.CFontRoleGloss => LFontRole.LFontRoleGloss,
            CFontRole.CFontRoleGlyph => LFontRole.LFontRoleGlyph,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };
    }

    internal static CSentenceOrder CCatalogOrderRead(LSentenceOrder order)
    {
        return new CSentenceOrder(order.LSentenceOrderParticle, order.LSentenceOrderDependence);
    }

    internal static long? LCatalogGlyphOpen(CEnvoy envoy, LSettingsPort settings, Func<long> resolve)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(resolve);

        try
        {
            return resolve();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Glyph.OpenFailed", exception);
            return null;
        }
    }

    public IReadOnlyList<string> CCatalogLanguageRead()
    {
        return _cCatalogAtelier.CAtelierSettingsPort.LEngineLanguageRead();
    }


    public Task<IReadOnlyList<string>> CCatalogEnsignLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return _cCatalogAtelier.CAtelierSettingsPort.LEngineEnsignLoad(
            (rows, delete) => store(CCatalogEnsignRead(rows), delete));
    }

    internal static async Task<CEnsignSheet<LCatalogKind>> LCatalogEnsignLoad<LCatalogKind>(
        LSettingsPort settings,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store,
        Func<LCatalogKind> read)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(read);

        IReadOnlyList<string> languages;
        try
        {
            languages = await settings.LEngineEnsignLoad((rows, delete) => store(CCatalogEnsignRead(rows), delete))
                .ConfigureAwait(true);
        }
        catch (Exception)
        {
            languages = settings.LEngineLanguageRead();
        }

        return new CEnsignSheet<LCatalogKind>(languages, read());
    }

    internal static IReadOnlyList<CMeaning> LCatalogMeaningRead(
        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows.Select(static row => new CMeaning(row.LMeaningId, row.LMeaningName, row.LMeaningDepth)).ToList();
    }

    public static CArticulation CCatalogConsonantRead()
    {
        return LCatalogArticulationRead(LPhonologyPort.LEngineConsonantRead());
    }

    public static CArticulation CCatalogVowelRead()
    {
        return LCatalogArticulationRead(LPhonologyPort.LEngineVowelRead());
    }

    private static CArticulation LCatalogArticulationRead(LArticulation chart)
    {
        return new CArticulation(
            chart.LArticulationHeaders.Select(static name => string.Concat("Articulation.", name)).ToList(),
            chart.LArticulationSides.Select(static name => string.Concat("Articulation.", name)).ToList(),
            chart.LArticulationCells);
    }

    internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)
    {
        return rows.Select(static row => new CEnsignRow(row.LEnsignRowKey, row.LEnsignRowPath)).ToList();
    }
}
