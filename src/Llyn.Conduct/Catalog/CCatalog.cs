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

    public CFont CCatalogFontRead(string language, CFontRole role)
    {
        return LCatalogFontRead(_cCatalogAtelier.CAtelierSettingsPort, language, role);
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

    public IReadOnlyList<CFont> CCatalogFontRead(string language, IReadOnlyList<CFontRole> roles)
    {
        ArgumentNullException.ThrowIfNull(roles);

        return roles.Select(role => CCatalogFontRead(language, role)).ToList();
    }

    internal static CSentenceOrder CCatalogOrderRead(LSentenceOrder order)
    {
        return new CSentenceOrder(order.LSentenceOrderParticle, order.LSentenceOrderDependence);
    }

    public IReadOnlyList<string> CCatalogSchemeRead(string language)
    {
        return _cCatalogAtelier.CAtelierPhonologyPort.LEngineSchemeRead(language);
    }

    public IReadOnlyList<CSpeechValue> CCatalogSpeechRead(string language)
    {
        return _cCatalogAtelier.CAtelierPhonologyPort.LEngineSpeechRead(language)
            .Select(static value => new CSpeechValue(value.LSpeechValueId, value.LSpeechValueName))
            .ToList();
    }

    public CGlyph? CCatalogGlyphRead(string language)
    {
        LGlyph? glyph = _cCatalogAtelier.CAtelierEntryPort.LEngineGlyphRead(language);
        return glyph is null ? null : new CGlyph(glyph.LGlyphName, glyph.LGlyphSourced);
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

    public string CCatalogGlossRead()
    {
        return _cCatalogAtelier.CAtelierSettingsPort.LEngineGlossRead();
    }

    public Task CCatalogEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return _cCatalogAtelier.CAtelierSettingsPort.LEngineEnsignLoad(
            (rows, delete) => store(CCatalogEnsignRead(rows), delete));
    }

    public Task CCatalogEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return _cCatalogAtelier.CAtelierSettingsPort.LEngineEnsignLoad(
            language, varieties, (rows, delete) => store(CCatalogEnsignRead(rows), delete));
    }

    public IReadOnlyList<CMeaning>? CCatalogMeaningRead(long entryId, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        try
        {
            return _cCatalogAtelier.CAtelierEntryPort.LEngineMeaningRead(entryId, "Display.Unknown")
                .Select(static row => new CMeaning(row.LMeaningId, row.LMeaningName, row.LMeaningDepth))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, _cCatalogAtelier.CAtelierSettingsPort, "Mention.FindFailed", exception);
            return null;
        }
    }

    public (int, int)? CCatalogEntryLoad(long id)
    {
        LEntryDraft? draft = _cCatalogAtelier.CAtelierEntryPort.LEngineEntryLoad(id);
        if (draft is null)
        {
            return null;
        }

        return (
            CSCustoms.CSCustomsCardScan(draft.LEntryDraftMeanings, static card => card.LCardDraftChild),
            CSCustoms.CSCustomsCardScan(draft.LEntryDraftCollocations, static card => card.LCardDraftChild));
    }

    public IReadOnlyList<long> CCatalogMarkupFind(string headword, string language)
    {
        return _cCatalogAtelier.CAtelierEntryPort.LEngineMarkupFind(new LMarkupEntry(headword, language))
            .Select(static entry => entry.LEntryId)
            .ToList();
    }

    internal static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)
    {
        return rows.Select(static row => new CEnsignRow(row.LEnsignRowKey, row.LEnsignRowPath)).ToList();
    }
}
