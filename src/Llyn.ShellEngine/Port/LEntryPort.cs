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

    IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft);

    (IReadOnlyList<LTranslationTarget>, bool) LEngineEtymologyRead(LEntryDraft draft);

    LMentionResult LEngineMentionFind(long exampleId, int offset);

    LMentionResult LEngineMentionFind(
        string text, string language, LEntryDraft? shown, int offset, IReadOnlyList<LMention>? mentions);

    IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions);

    LGlyph? LEngineGlyphRead(string language);

    LGlyph? LEngineGlyphRead(LEntryDraft draft);

    IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);

    long LEngineGlyphResolve(string character, string language);

    IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft);

    IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft);

    IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LEngineMeaningRead(
        long entryId, string key);

    IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista);

    LTag LEngineTagCreate(string text);

    IReadOnlyList<LRegister> LEngineRegisterFind(LTenure? held, long card, string query, string language);

    IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista);

    LRegister LEngineRegisterCreate(string name);

    IReadOnlyList<LCatalogSituation> LEngineSituationFind(
        LTenure? held, long card, string query, LCatalogOrder order);

    IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "");

    IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "");

    IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order);

    IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista);

    IReadOnlyList<LCatalogReference> LEngineReferenceFind();

    IReadOnlyList<LCatalogReference> LEngineCitationFind(long draftId, string word);

    long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title);

    IReadOnlyDictionary<long, string> LEngineCitationRead();

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

    IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner);

    string LEngineTallyRead(long? reference);

    IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry);

    IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);

    IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);

    static bool LEngineTextMatch(string field, string shown)
    {
        return LExampleClerk.LExampleTextMatch(field, shown);
    }

    static bool LEngineNarrativeCheck(string text)
    {
        return new LEtymologyDraft(text).LEtymologyDraftNarrated;
    }

    static (LSubject, long)? LEngineChipRead(object? chip, long? link)
    {
        return chip switch
        {
            LSituationDraft { LSituationDraftStored: true } situation
                => (LSubject.LSubjectSituation, situation.LSituationDraftId),
            LRegisterDraft { LRegisterDraftStored: true } register
                => (LSubject.LSubjectRegister, register.LRegisterDraftId),
            LTagDraft { LTagDraftStored: true } tag => (LSubject.LSubjectTag, tag.LTagDraftId),
            _ => link is long id && id != 0 ? (LSubject.LSubjectEntry, id) : null,
        };
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
