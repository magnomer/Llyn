using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LEntryPort
{
    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista);

    IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);

    LEntry? LEngineEntryRead(long id);

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

    string LEngineEpithetRead(long entryId);

    IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);

    IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft);

    IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft);

    LMentionResult LEngineMentionFind(long exampleId, int offset);

    LMentionResult LEngineMentionFind(string text, string language, int offset, IReadOnlyList<LMention> mentions);

    IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions);

    LGlyph? LEngineGlyphRead(string language);

    IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);

    LEntry LEngineGlyphResolve(string character, string language);

    IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner);

    IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order);

    IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista);

    LTag LEngineTagCreate(string text);

    IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language);

    IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista);

    LRegister LEngineRegisterCreate(string name);

    IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order);

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

    IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner);

    string LEngineTallyRead(long? reference);

    IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry);

    IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);

    IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);

    static bool LEngineTextMatch(string field, string shown)
    {
        return LExampleClerk.LExampleTextMatch(field, shown);
    }
}
