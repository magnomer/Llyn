using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TFaultList
{
    internal static IReadOnlyList<TFaultRow> TFaultListRows =>
    [
        new(
            "CCorpus.CCorpusRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Example.LoadFailed",
            static stage =>
            {
                CCorpus corpus = CCorpus.CCorpusCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => corpus.CCorpusRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CFavorite.CFavoriteRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Favorite.LoadFailed",
            static stage =>
            {
                CFavorite favorite = CFavorite.CFavoriteCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => favorite.CFavoriteRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CFootnote.CFootnoteRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "List.LoadFailed",
            static stage =>
            {
                CShelf shelf = CShelf.CShelfCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => shelf.CShelfFootnote.CFootnoteRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CLibrary.CLibraryRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "List.LoadFailed",
            static stage =>
            {
                CLibrary library = CLibrary.CLibraryCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => library.CLibraryRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "COccurrence.COccurrenceRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Situation.LoadFailed",
            static stage =>
            {
                CRepertoire repertoire = CRepertoire.CRepertoireCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => repertoire.CRepertoireOccurrence.COccurrenceRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CPhonology.CPhonologyRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Sound.LoadFailed",
            static stage =>
            {
                CPhonology phonology = CPhonology.CPhonologyCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => phonology.CPhonologyRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CQuotation.CQuotationRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Example.LoadFailed",
            static stage =>
            {
                CCorpus corpus = CCorpus.CCorpusCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => corpus.CCorpusQuotation.CQuotationRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CRepertoire.CRepertoireRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Situation.LoadFailed",
            static stage =>
            {
                CRepertoire repertoire = CRepertoire.CRepertoireCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => repertoire.CRepertoireRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CShelf.CShelfRollLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Source.LoadFailed",
            static stage =>
            {
                CShelf shelf = CShelf.CShelfCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => shelf.CShelfRollLoad(static (_, _) => static () => { }));
            }),
        new(
            "CTaxonomy.CTaxonomyRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Tag.LoadFailed",
            static stage =>
            {
                CTaxonomy taxonomy = CTaxonomy.CTaxonomyCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => taxonomy.CTaxonomyRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CTenor.CTenorRowsLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Register.LoadFailed",
            static stage =>
            {
                CTenor tenor = CTenor.CTenorCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => tenor.CTenorRowsLoad(static (_, _) => static () => { }));
            }),
        new(
            "CEntryList.CEntryListLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Xiesheng.LoadFailed",
            static stage =>
            {
                CXiesheng xiesheng = CXiesheng.CXieshengCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => xiesheng.CXieshengKindred.CEntryListLoad(static (_, _) => static () => { }));
            }),
        new(
            "CYunjing.CYunjingXiaoyun.CEntryListLoad",
            "LSettingsPort.LEngineEnsignLoad",
            "Yunjing.LoadFailed",
            static stage =>
            {
                CYunjing yunjing = CYunjing.CYunjingCreate(
                    TFault.TFaultAtelierStart(stage),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                return Task.FromResult<Func<Task>>(
                    () => yunjing.CYunjingXiaoyun.CEntryListLoad(static (_, _) => static () => { }));
            }),
    ];
}
