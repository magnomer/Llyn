using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LEntryPort
{
    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista);

    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);

    (bool, string, string) LEngineStampRead(long entryId);

    LEntryDraft? LEngineEntryLoad(long id);

    IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista);

    bool LEngineFavoriteCheck(long entryId);

    void LEngineFavoriteSave(long entryId);

    void LEngineFavoriteDelete(long entryId);

    int LEngineGraspStep { get; }

    string LEngineGraspFormat(int step);

    int LEngineGraspRead(long entryId);

    void LEngineGraspSave(long entryId, int grasp);

    LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once);

    IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);

    IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LEngineTranslationRead(LEntryDraft shown);

    LEtymologyResult LEngineEtymologyRead(LEntryDraft draft);

    LMentionResult LEngineMentionFind(long exampleId, int offset);

    LMentionResult LEngineMentionFind(LEntryDraft shown, long sentence, int offset);

    LMentionResult LEngineEtymologyFind(LEntryDraft shown, int offset);

    LGlyph? LEngineGlyphRead(LEntryDraft draft);

    IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);

    long LEngineGlyphResolve(string character, string language);

    IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft);

    IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);

    IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista);

    LTag LEngineTagCreate(string text);

    IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista);

    LRegister LEngineRegisterCreate(string name);

    IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "");

    IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "");

    IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista);

    IReadOnlyList<LCatalogReference> LEngineReferenceFind();

    IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown);

    string LEngineCitationRead(LDraft? draft);

    IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista);

    IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed);

    (string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept);

    LVita LEngineVitaRead(LVista? roll);

    void LEngineAuthorAbsorb(long kept, long dropped);

    IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre);

    string LEngineWorkFormat(int count);

    LColophon LEngineColophonRead(LDraft draft);

    LImprint LEngineImprintRead(LDraft? draft);

    IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held);

    string LEngineTallyRead(long? reference);

    IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);

    IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);

    static string LEngineUnitFormat(LUnit unit)
    {
        return LUnitClerk.LUnitFormat(unit);
    }

    static bool LEngineTextMatch(string field, string shown)
    {
        return LExampleClerk.LExampleTextMatch(field, shown);
    }

    static IReadOnlyList<(string LReferenceKindTag, string LReferenceKindKey)> LEngineKindRead()
    {
        return LReferenceClerk.LReferenceMenuRead();
    }

    static long? LEngineLinkRead(long? link)
    {
        return link is long id && id != 0 ? id : null;
    }

    static (string, IReadOnlyList<LMentionPiece>, string) LEngineLineRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)
    {
        return LExampleClerk.LExampleLineRead(sentence, order, mark, citations);
    }

    static (LOwner, int)? LEngineCardFind(LEntryDraft draft, long id)
    {
        ArgumentNullException.ThrowIfNull(draft);

        for (int index = 0; index < draft.LEntryDraftMeanings.Count; index++)
        {
            if (draft.LEntryDraftMeanings[index].LCardDraftId == id)
            {
                return (LOwner.LOwnerMeaning, index);
            }
        }

        for (int index = 0; index < draft.LEntryDraftCollocations.Count; index++)
        {
            if (draft.LEntryDraftCollocations[index].LCardDraftId == id)
            {
                return (LOwner.LOwnerCollocation, index);
            }
        }

        return null;
    }
}
