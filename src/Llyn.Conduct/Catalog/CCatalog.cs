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
            return new CFont(null, null, CFontSlant.CFontSlantTheme);
        }

        LFont font = settings.LEngineFontRead(language, LCatalogRoleRead(role));
        return new CFont(
            string.IsNullOrWhiteSpace(font.LFontFamily) ? null : font.LFontFamily,
            font.LFontSized is double size && double.IsFinite(size) && size > 0 ? size : null,
            LCatalogSlantRead(font.LFontStyle));
    }

    private static CFontSlant LCatalogSlantRead(string? style)
    {
        return style?.ToLowerInvariant() switch
        {
            "italic" => CFontSlant.CFontSlantItalic,
            "oblique" => CFontSlant.CFontSlantOblique,
            _ => CFontSlant.CFontSlantTheme,
        };
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
        return order.LSentenceOrderParticle is 0 or 1
            && order.LSentenceOrderDependence == 1 - order.LSentenceOrderParticle
                ? new CSentenceOrder(order.LSentenceOrderParticle, order.LSentenceOrderDependence)
                : new CSentenceOrder(0, 1);
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


    public async Task<IReadOnlyList<string>> CCatalogEnsignLoad(
        CEnvoy envoy,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(store);

        LSettingsPort settings = _cCatalogAtelier.CAtelierSettingsPort;
        try
        {
            return await settings
                .LEngineEnsignLoad((rows, delete) => store(CCatalogEnsignRead(rows), delete))
                .ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Language.LoadFailed", exception);
            return [];
        }
    }

    internal static async Task<CEnsignSheet<LCatalogKind>> LCatalogEnsignLoad<LCatalogKind>(
        CEnvoy envoy,
        LSettingsPort settings,
        string key,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store,
        Func<LCatalogKind> read)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(read);

        IReadOnlyList<string> languages;
        try
        {
            languages = await settings
                .LEngineEnsignLoad((rows, delete) => store(CCatalogEnsignRead(rows), delete))
                .ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, key, exception);
            languages = [];
        }

        return new CEnsignSheet<LCatalogKind>(languages, read());
    }

    internal static IReadOnlyList<CMeaning> LCatalogMeaningRead(
        IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows.Select(static row => new CMeaning(row.LMeaningId, row.LMeaningName, row.LMeaningDepth)).ToList();
    }

    public CArticulationAid CCatalogAidRead()
    {
        LPronunciationPort pronunciation = _cCatalogAtelier.CAtelierEntryBundle.CEntryBundlePronunciation;
        return new CArticulationAid(
            LCatalogArticulationRead(pronunciation.LEngineVowelRead()),
            LCatalogArticulationRead(pronunciation.LEngineConsonantRead()));
    }

    internal static CArticulation LCatalogArticulationRead(LArticulation chart)
    {
        List<string> headers =
            chart.LArticulationHeaders.Select(static name => string.Concat("Articulation.", name)).ToList();
        List<string> sides =
            chart.LArticulationSides.Select(static name => string.Concat("Articulation.", name)).ToList();
        List<IReadOnlyList<IReadOnlyList<string>>> cells = sides
            .Select((_, row) => chart.LArticulationCells.ElementAtOrDefault(row) ?? [])
            .Select(row => (IReadOnlyList<IReadOnlyList<string>>)row.Take(headers.Count).ToList())
            .ToList();
        return new CArticulation(headers, sides, cells);
    }

    internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)
    {
        return rows.Select(static row => new CEnsignRow(row.LEnsignRowKey, row.LEnsignRowPath)).ToList();
    }

    internal static CCatalogOrder LCatalogOrderRead(LCatalogOrder order)
    {
        return order switch
        {
            LCatalogOrder.LCatalogOrderName => CCatalogOrder.CCatalogOrderName,
            LCatalogOrder.LCatalogOrderHeadword => CCatalogOrder.CCatalogOrderHeadword,
            LCatalogOrder.LCatalogOrderReverse => CCatalogOrder.CCatalogOrderReverse,
            LCatalogOrder.LCatalogOrderRecent => CCatalogOrder.CCatalogOrderRecent,
            LCatalogOrder.LCatalogOrderEarliest => CCatalogOrder.CCatalogOrderEarliest,
            LCatalogOrder.LCatalogOrderYear => CCatalogOrder.CCatalogOrderYear,
            LCatalogOrder.LCatalogOrderAuthor => CCatalogOrder.CCatalogOrderAuthor,
            LCatalogOrder.LCatalogOrderUsage => CCatalogOrder.CCatalogOrderUsage,
            LCatalogOrder.LCatalogOrderLanguage => CCatalogOrder.CCatalogOrderLanguage,
            LCatalogOrder.LCatalogOrderMarked => CCatalogOrder.CCatalogOrderMarked,
            LCatalogOrder.LCatalogOrderText => CCatalogOrder.CCatalogOrderText,
            LCatalogOrder.LCatalogOrderSource => CCatalogOrder.CCatalogOrderSource,
            LCatalogOrder.LCatalogOrderKind => CCatalogOrder.CCatalogOrderKind,
            LCatalogOrder.LCatalogOrderSound => CCatalogOrder.CCatalogOrderSound,
            LCatalogOrder.LCatalogOrderPending => CCatalogOrder.CCatalogOrderPending,
            LCatalogOrder.LCatalogOrderWork => CCatalogOrder.CCatalogOrderWork,
            LCatalogOrder.LCatalogOrderGrasp => CCatalogOrder.CCatalogOrderGrasp,
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null),
        };
    }

    internal static LCatalogOrder? LCatalogOrderRead(CCatalogOrder? order)
    {
        return order is CCatalogOrder held ? LCatalogOrderRead(held) : null;
    }

    internal static LCatalogOrder LCatalogOrderRead(CCatalogOrder order)
    {
        return order switch
        {
            CCatalogOrder.CCatalogOrderName => LCatalogOrder.LCatalogOrderName,
            CCatalogOrder.CCatalogOrderHeadword => LCatalogOrder.LCatalogOrderHeadword,
            CCatalogOrder.CCatalogOrderReverse => LCatalogOrder.LCatalogOrderReverse,
            CCatalogOrder.CCatalogOrderRecent => LCatalogOrder.LCatalogOrderRecent,
            CCatalogOrder.CCatalogOrderEarliest => LCatalogOrder.LCatalogOrderEarliest,
            CCatalogOrder.CCatalogOrderYear => LCatalogOrder.LCatalogOrderYear,
            CCatalogOrder.CCatalogOrderAuthor => LCatalogOrder.LCatalogOrderAuthor,
            CCatalogOrder.CCatalogOrderUsage => LCatalogOrder.LCatalogOrderUsage,
            CCatalogOrder.CCatalogOrderLanguage => LCatalogOrder.LCatalogOrderLanguage,
            CCatalogOrder.CCatalogOrderMarked => LCatalogOrder.LCatalogOrderMarked,
            CCatalogOrder.CCatalogOrderText => LCatalogOrder.LCatalogOrderText,
            CCatalogOrder.CCatalogOrderSource => LCatalogOrder.LCatalogOrderSource,
            CCatalogOrder.CCatalogOrderKind => LCatalogOrder.LCatalogOrderKind,
            CCatalogOrder.CCatalogOrderSound => LCatalogOrder.LCatalogOrderSound,
            CCatalogOrder.CCatalogOrderPending => LCatalogOrder.LCatalogOrderPending,
            CCatalogOrder.CCatalogOrderWork => LCatalogOrder.LCatalogOrderWork,
            CCatalogOrder.CCatalogOrderGrasp => LCatalogOrder.LCatalogOrderGrasp,
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null),
        };
    }

    internal static CCatalogFilter LCatalogFilterRead(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return new CCatalogFilter(filter.LCatalogFilterHidden);
    }

    internal static LSubject LCatalogSubjectRead(CSubject subject)
    {
        return subject switch
        {
            CSubject.CSubjectEntry => LSubject.LSubjectEntry,
            CSubject.CSubjectExample => LSubject.LSubjectExample,
            CSubject.CSubjectSituation => LSubject.LSubjectSituation,
            CSubject.CSubjectReference => LSubject.LSubjectReference,
            CSubject.CSubjectAuthor => LSubject.LSubjectAuthor,
            CSubject.CSubjectTag => LSubject.LSubjectTag,
            CSubject.CSubjectRegister => LSubject.LSubjectRegister,
            CSubject.CSubjectFavorite => LSubject.LSubjectFavorite,
            CSubject.CSubjectWorkspace => LSubject.LSubjectWorkspace,
            CSubject.CSubjectDraft => LSubject.LSubjectDraft,
            CSubject.CSubjectFrequency => LSubject.LSubjectFrequency,
            CSubject.CSubjectGrasp => LSubject.LSubjectGrasp,
            CSubject.CSubjectInflection => LSubject.LSubjectInflection,
            CSubject.CSubjectScript => LSubject.LSubjectScript,
            CSubject.CSubjectFanqie => LSubject.LSubjectFanqie,
            CSubject.CSubjectReflex => LSubject.LSubjectReflex,
            CSubject.CSubjectSettings => LSubject.LSubjectSettings,
            CSubject.CSubjectTenure => LSubject.LSubjectTenure,
            CSubject.CSubjectVista => LSubject.LSubjectVista,
            _ => throw new ArgumentOutOfRangeException(nameof(subject), subject, null),
        };
    }

    internal static CVistaRow LCatalogRowRead(LVistaRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CVistaRow(
            row.LVistaRowId,
            row.LVistaRowHeadword,
            row.LVistaRowLanguage,
            row.LVistaRowEpithet,
            row.LVistaRowName,
            row.LVistaRowChosen);
    }
}
