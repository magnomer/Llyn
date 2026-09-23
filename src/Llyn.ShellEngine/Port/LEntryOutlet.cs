using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEntryOutlet : LEntryPort
{
    private readonly LEngine _lEntryOutletEngine;

    public LEntryOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lEntryOutletEngine = engine;
    }

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista) => _lEntryOutletEngine.LEngineEntryFind(vista);

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child) =>
        _lEntryOutletEngine.LEngineEntryFind(parent, child);

    public LEntry? LEngineEntryRead(long id) => _lEntryOutletEngine.LEngineEntryRead(id);

    public LEntryDraft? LEngineEntryLoad(long id) => _lEntryOutletEngine.LEngineEntryLoad(id);

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista) => _lEntryOutletEngine.LEngineFavoriteFind(vista);

    public bool LEngineFavoriteCheck(long entryId) => _lEntryOutletEngine.LEngineFavoriteCheck(entryId);

    public void LEngineFavoriteSave(long entryId) => _lEntryOutletEngine.LEngineFavoriteSave(entryId);

    public void LEngineFavoriteDelete(long entryId) => _lEntryOutletEngine.LEngineFavoriteDelete(entryId);

    public int LEngineGraspStep => _lEntryOutletEngine.LEngineGraspStep;

    public string LEngineGraspFormat(int step) => _lEntryOutletEngine.LEngineGraspFormat(step);

    public int LEngineGraspRead(long entryId) => _lEntryOutletEngine.LEngineGraspRead(entryId);

    public void LEngineGraspSave(long entryId, int grasp) => _lEntryOutletEngine.LEngineGraspSave(entryId, grasp);

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId) =>
        _lEntryOutletEngine.LEngineFrequencyRead(entryId);

    public string LEngineEpithetRead(long entryId) => _lEntryOutletEngine.LEngineEpithetRead(entryId);

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId) => _lEntryOutletEngine.LEngineIncomingRead(entryId);

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineTargetRead(draft);

    public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineEtymonRead(draft);

    public LMentionResult LEngineMentionFind(long exampleId, int offset) =>
        _lEntryOutletEngine.LEngineMentionFind(exampleId, offset);

    public LMentionResult LEngineMentionFind(
        string text,
        string language,
        int offset,
        IReadOnlyList<LMention> mentions) =>
        _lEntryOutletEngine.LEngineMentionFind(text, language, offset, mentions);

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions) =>
        _lEntryOutletEngine.LEngineMentionResolve(text, mentions);

    public LGlyph? LEngineGlyphRead(string language) => _lEntryOutletEngine.LEngineGlyphRead(language);

    public LEntry LEngineGlyphResolve(string character, string language) =>
        _lEntryOutletEngine.LEngineGlyphResolve(character, language);

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner) =>
        _lEntryOutletEngine.LEngineMeaningRead(ownerId, owner);

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineTagFind(query, order);

    public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista) => _lEntryOutletEngine.LEngineTagFind(vista);

    public LTag LEngineTagCreate(string text) => _lEntryOutletEngine.LEngineTagCreate(text);

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language) =>
        _lEntryOutletEngine.LEngineRegisterFind(query, language);

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista) =>
        _lEntryOutletEngine.LEngineRegisterFind(vista);

    public LRegister LEngineRegisterCreate(string name) => _lEntryOutletEngine.LEngineRegisterCreate(name);

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineSituationFind(query, order);

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(
        LVista vista,
        string unknown = "",
        string untitled = "") =>
        _lEntryOutletEngine.LEngineSituationFind(vista, unknown, untitled);

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(
        LVista vista,
        string unknown = "",
        string unwritten = "") =>
        _lEntryOutletEngine.LEngineExampleFind(vista, unknown, unwritten);

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineReferenceFind(query, order);

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista) =>
        _lEntryOutletEngine.LEngineReferenceFind(vista);

    public LReference LEngineCitationCreate(string title) => _lEntryOutletEngine.LEngineCitationCreate(title);

    public IReadOnlyDictionary<long, string> LEngineCitationRead() => _lEntryOutletEngine.LEngineCitationRead();

    public LCatalogAuthor? LEngineAuthorFind(long id) => _lEntryOutletEngine.LEngineAuthorFind(id);

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit) =>
        _lEntryOutletEngine.LEngineAuthorFind(query, except, limit);

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited) =>
        _lEntryOutletEngine.LEngineAuthorFind(vista, uncredited);

    public void LEngineAuthorAbsorb(long kept, long dropped) => _lEntryOutletEngine.LEngineAuthorAbsorb(kept, dropped);

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId) => _lEntryOutletEngine.LEngineFellowFind(authorId);

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre) =>
        _lEntryOutletEngine.LEngineOeuvreFind(roll, oeuvre);

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner) => _lEntryOutletEngine.LEngineUsageRead(owner);

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner) =>
        _lEntryOutletEngine.LEngineUsageRead(id, owner);

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry) => _lEntryOutletEngine.LEngineMarkupFind(entry);

    public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels) =>
        _lEntryOutletEngine.LEngineNameResolve(labels);

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text) =>
        _lEntryOutletEngine.LEngineMarkdownParse(text);
}
