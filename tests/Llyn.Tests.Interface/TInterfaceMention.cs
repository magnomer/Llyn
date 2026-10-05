using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceMention
{
    internal static CSentence TSentenceFailCreate(LEngine engine, CDesk desk, CEnvoy envoy)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => throw new InvalidOperationException("no headwords"),
        });
        return new CSentence(
            desk,
            new LPhonologyOutlet(engine),
            drafts,
            new LSettingsOutlet(engine),
            envoy,
            new CLedgerNoticed());
    }

    internal static CSentence TSentenceLineCreate(
        LEngine engine, CDesk desk, CEnvoy envoy, IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>>? lines)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => lines,
        });
        return new CSentence(
            desk,
            new LPhonologyOutlet(engine),
            drafts,
            new LSettingsOutlet(engine),
            envoy,
            new CLedgerNoticed());
    }

    internal static LMentionLabel TMentionLabelCreate(long id, string word, long entry) =>
        new(id, word, entry, word, string.Empty);

    internal static IReadOnlyList<CCatalogExample>? TAnthologyFailRead(LEngine engine, CAtelier atelier, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineExampleFind"] = _ => throw new InvalidOperationException("no examples"),
        });
        CAnthology anthology = new(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            TInterfaceConductDesk.TDeskCreate(engine, "Example", envoy),
            static () => true,
            envoy,
            static _ => true,
            atelier.CAtelierMention);
        anthology.LAnthologyVistaRestore(engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText));
        return anthology.LAnthologyRowsRead();
    }

    internal static CMentionOffer? TAnthologyMentionFind(
        LEngine engine, CAtelier atelier, CEnvoy envoy, long? chosen, IReadOnlyList<long>? found)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionFind"] = _ => found is null
                ? throw new InvalidOperationException("no words")
                : new LMentionResult(2, null, [.. found.Select(id => new LTranslationTarget(id, "cat", "English"))]),
        });
        CAnthology anthology = new(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            TInterfaceConductDesk.TDeskCreate(engine, "Example", envoy),
            static () => true,
            envoy,
            static _ => true,
            atelier.CAtelierMention);
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        anthology.LAnthologyVistaRestore(vista);
        vista.LVistaSelect(chosen);
        return anthology.LAnthologyMentionFind("a cat sat", 3);
    }

    internal static CMentionOffer TMentionResultOpen(CAtelier atelier, CMentionResult result, string text) =>
        atelier.CAtelierMention.LMentionResultOpen(result, text);

    internal static int? TMentionUnitRead(CAtelier atelier, string text, int start, int offset) =>
        atelier.CAtelierMention.LMentionUnitRead(text, start, offset);

    internal static CMentionResult TMentionResultRead(int offset, LMention? stored) =>
        CMention.CMentionResultRead(new LMentionResult(offset, stored, []));

    internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>
        example with { LExampleMention = [mention] };

    internal static CMentionOffer? TDisplayMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy, long shown)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionFind"] = _ => throw new InvalidOperationException("no words"),
        });
        CDisplay display = TDisplayCreate(engine, atelier, envoy, entries, shown);
        return display.CDisplayRoute.CDisplayMentionFind(1, "water", 1);
    }

    internal static int? TDisplayOffsetRead(LEngine engine, CAtelier atelier, long shown, string text, int unit)
    {
        int? heard = null;
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionFind"] = args =>
            {
                heard = (int)args![2]!;
                throw new InvalidOperationException("no words");
            },
        });
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, []);
        CDisplay display = TDisplayCreate(engine, atelier, envoy, entries, shown);
        display.CDisplayRoute.CDisplayMentionFind(1, text, unit);
        return heard;
    }

    private static CDisplay TDisplayCreate(
        LEngine engine, CAtelier atelier, CEnvoy envoy, LEntryPort entries, long shown)
    {
        CDisplay display = new(
            new LDraftOutlet(engine),
            entries,
            new LPhonologyOutlet(engine),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            envoy,
            new CLedgerNoticed());
        display.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        display.LDisplayRule.LDisplaySound.LDisplaySoundShow(shown, engine.TEngineEntryLoad(shown)!);
        return display;
    }

    internal static IReadOnlyList<CMentionLabel> TMentionFailRead(LEngine engine, CDesk desk, CEnvoy envoy)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionResolve"] = _ => throw new InvalidOperationException("no headwords"),
        });
        return CMention.LMentionChipRead(drafts, desk, 0, 0, envoy, new LSettingsOutlet(engine));
    }

    internal static IReadOnlyList<CVistaRow> TQuotationFailRead(LEngine engine, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEntryFind"] = _ => throw new InvalidOperationException("no entries"),
        });
        return new CQuotation(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            envoy,
            static () => true,
            static _ => true,
            static () => true).CQuotationRowsRead();
    }

    internal static IReadOnlyList<CVistaRow> TOccurrenceFailRead(LEngine engine, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineEntryFind"] = _ => throw new InvalidOperationException("no entries"),
        });
        return new COccurrence(
            entries,
            new LPortraitOutlet(engine),
            new LSettingsOutlet(engine),
            envoy,
            static () => true,
            static _ => true,
            static () => true).COccurrenceRowsRead();
    }

    internal static IReadOnlyList<CVistaRow> TFavoriteFailRead(LEngine engine, CEnvoy envoy)
    {
        CAtelier atelier = TInterfaceConduct.TAtelierCreate(
            engine,
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineFavoriteFind"] = _ => throw new InvalidOperationException("no favorites"),
            });
        CFavorite favorite = CFavorite.CFavoriteCreate(atelier, static () => true, envoy, static run => run());
        return favorite.CFavoriteRowsRead();
    }

    internal static CMentionSense? TMentionSenseRead(
        LEngine engine, CDesk desk, CEnvoy envoy, Func<object?[]?, object?> sense)
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineSenseRead"] = sense,
        });
        return CMention.LMentionSenseRead(
            drafts, desk, 0, 0, "she knelt", 0, 3, envoy, new LSettingsOutlet(engine));
    }
}
