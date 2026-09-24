using System.Collections.Generic;
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

    IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId);

    string LEngineEpithetRead(long entryId);

    IReadOnlyList<LUsage> LEngineIncomingRead(long entryId);

    IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft);

    IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft);

    LMentionResult LEngineMentionFind(long exampleId, int offset);

    LMentionResult LEngineMentionFind(string text, string language, int offset, IReadOnlyList<LMention> mentions);

    IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions);

    LGlyph? LEngineGlyphRead(string language);

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

    long LEngineCitationResolve(long draftId, long cardId, long sentenceId, string title);

    IReadOnlyDictionary<long, string> LEngineCitationRead();

    LCatalogAuthor? LEngineAuthorFind(long id);

    IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit);

    IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited);

    void LEngineAuthorAbsorb(long kept, long dropped);

    IReadOnlyList<LFellow> LEngineFellowFind(long authorId);

    IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre);

    IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner);

    IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner);

    IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry);

    IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels);

    IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text);
}
