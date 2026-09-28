using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

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
        LFont font = _cCatalogAtelier.CAtelierSettingsPort.LEngineFontRead(language, (LFontRole)role);
        return new CFont(font.LFontFamily, font.LFontSized, font.LFontStyle);
    }

    public IReadOnlyList<CReflexRule> CCatalogReflexRead(string language)
    {
        return _cCatalogAtelier.CAtelierPhonologyPort.LEngineReflexRead(language)
            .Select(static rule => new CReflexRule(rule.LReflexRuleLanguage, rule.LReflexRuleFolded))
            .ToList();
    }

    public CSentenceOrder CCatalogOrderRead(string language)
    {
        return CCatalogOrderRead(_cCatalogAtelier.CAtelierPhonologyPort.LEngineOrderRead(language));
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

    public CSpeechValue? CCatalogSpeechAdd(string language, string name)
    {
        LSpeechValue? value = _cCatalogAtelier.CAtelierPhonologyPort.LEngineSpeechAdd(language, name);
        return value is null ? null : new CSpeechValue(value.LSpeechValueId, value.LSpeechValueName);
    }

    public CGlyph? CCatalogGlyphRead(string language)
    {
        LGlyph? glyph = _cCatalogAtelier.CAtelierEntryPort.LEngineGlyphRead(language);
        return glyph is null ? null : new CGlyph(glyph.LGlyphName, glyph.LGlyphSourced);
    }

    public long CCatalogGlyphResolve(string character, string language)
    {
        return _cCatalogAtelier.CAtelierEntryPort.LEngineGlyphResolve(character, language).LEntryId;
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

    public IReadOnlyList<CMeaning> CCatalogMeaningSort(long entryId)
    {
        IReadOnlyList<LMeaning> meanings =
            _cCatalogAtelier.CAtelierEntryPort.LEngineMeaningRead(entryId, LOwner.LOwnerEntry);
        string unknown = _cCatalogAtelier.CAtelierSettingsPort.LEngineTextRead("Display.Unknown");

        Func<long, List<LMeaning>> childrenRead = parent =>
        {
            List<LMeaning> children = meanings
                .Where(meaning => (meaning.LMeaningParentId ?? 0) == parent && meaning.LMeaningId != parent)
                .ToList();
            children.Sort((left, right) => left.LMeaningPosition.CompareTo(right.LMeaningPosition));
            return children;
        };

        List<CMeaning> rows = [];
        Stack<(int, int, List<LMeaning>)> path = [];
        path.Push((0, 0, childrenRead(0)));
        while (path.Count > 0)
        {
            (int depth, int next, List<LMeaning> children) = path.Pop();
            if (next >= children.Count)
            {
                continue;
            }

            LMeaning meaning = children[next];
            path.Push((depth, next + 1, children));
            rows.Add(new CMeaning(
                meaning.LMeaningId, meaning.LMeaningName.Length > 0 ? meaning.LMeaningName : unknown, depth));
            path.Push((depth + 1, 0, childrenRead(meaning.LMeaningId)));
        }

        return rows;
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

    public bool CCatalogAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return _cCatalogAtelier.CAtelierDraftPort.LEngineAnchorMatch(one, other);
    }

    public IReadOnlyList<long> CCatalogMarkupFind(string headword, string language)
    {
        return _cCatalogAtelier.CAtelierEntryPort.LEngineMarkupFind(new LMarkupEntry(headword, language))
            .Select(static entry => entry.LEntryId)
            .ToList();
    }

    private static IReadOnlyList<CEnsignRow> CCatalogEnsignRead(IReadOnlyList<LEnsignRow> rows)
    {
        return rows.Select(static row => new CEnsignRow(row.LEnsignRowKey, row.LEnsignRowPath)).ToList();
    }
}
