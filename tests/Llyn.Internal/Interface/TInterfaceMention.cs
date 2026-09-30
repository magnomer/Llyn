using System;
using System.Collections.Generic;
using System.Linq;
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
        return new CSentence(desk, new LPhonologyOutlet(engine), drafts, new LSettingsOutlet(engine), envoy);
    }

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
            TInterfaceConduct.TDeskCreate(engine, "Example", envoy),
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
            TInterfaceConduct.TDeskCreate(engine, "Example", envoy),
            static () => true,
            envoy,
            static _ => true,
            atelier.CAtelierMention);
        LVista vista = engine.TEngineVistaStart("corpus", LCatalogOrder.LCatalogOrderText);
        anthology.LAnthologyVistaRestore(vista);
        vista.LVistaSelect(chosen);
        return anthology.LAnthologyMentionFind(3);
    }

    internal static CMentionOffer TMentionResultOpen(CAtelier atelier, CMentionResult result) =>
        atelier.CAtelierMention.LMentionResultOpen(result);

    internal static CMentionOffer? TDisplayMentionFind(LEngine engine, CAtelier atelier, CEnvoy envoy)
    {
        LEntryPort entries = TEngineFake.TEngineCreate<LEntryPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMentionFind"] = _ => throw new InvalidOperationException("no words"),
        });
        LDisplay display = new(
            new LDraftOutlet(engine),
            entries,
            new LPhonologyOutlet(engine),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineStubCreate<LMediaPort>(),
            envoy);
        display.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        return display.CDisplayArea.CDisplayMentionFind("water", string.Empty, 1, null);
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
        favorite.CFavoriteVistaRestore();
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
