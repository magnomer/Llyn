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

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista) =>
        _lEntryOutletEngine.LEngineVista.LEngineEntryFind(vista);

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child) =>
        _lEntryOutletEngine.LEngineVista.LEngineEntryFind(parent, child);

    public LEntry? LEngineEntryRead(long id) => _lEntryOutletEngine.LEngineEntry.LEngineEntryRead(id);

    public LEntryDraft? LEngineEntryLoad(long id) => _lEntryOutletEngine.LEngineEntry.LEngineEntryLoad(id);

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista) =>
        _lEntryOutletEngine.LEngineVista.LEngineFavoriteFind(vista);

    public bool LEngineFavoriteCheck(long entryId) => _lEntryOutletEngine.LEngineVista.LEngineFavoriteCheck(entryId);

    public void LEngineFavoriteSave(long entryId) => _lEntryOutletEngine.LEngineVista.LEngineFavoriteSave(entryId);

    public void LEngineFavoriteDelete(long entryId) => _lEntryOutletEngine.LEngineVista.LEngineFavoriteDelete(entryId);

    public int LEngineGraspStep => _lEntryOutletEngine.LEngineEntry.LEngineGraspStep;

    public string LEngineGraspFormat(int step) => _lEntryOutletEngine.LEngineEntry.LEngineGraspFormat(step);

    public int LEngineGraspRead(long entryId) => _lEntryOutletEngine.LEngineEntry.LEngineGraspRead(entryId);

    public void LEngineGraspSave(long entryId, int grasp)
    {
        _lEntryOutletEngine.LEngineEntry.LEngineGraspSave(entryId, grasp);
    }

    public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId) =>
        _lEntryOutletEngine.LEnginePronunciation.LEngineFrequencyRead(entryId);

    public string LEngineEpithetRead(long entryId) => _lEntryOutletEngine.LEngineEntry.LEngineEpithetRead(entryId);

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId) =>
        _lEntryOutletEngine.LEngineCard.LEngineIncomingRead(entryId);

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineCard.LEngineTargetRead(draft);

    public IReadOnlyList<LTranslationTarget> LEngineEtymonRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineCard.LEngineEtymonRead(draft);

    public LMentionResult LEngineMentionFind(long exampleId, int offset) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionFind(exampleId, offset);

    public LMentionResult LEngineMentionFind(
        string text,
        string language,
        int offset,
        IReadOnlyList<LMention> mentions) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionFind(text, language, offset, mentions);

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionResolve(text, mentions);

    public LGlyph? LEngineGlyphRead(string language) => _lEntryOutletEngine.LEngineEntry.LEngineGlyphRead(language);

    public LEntry LEngineGlyphResolve(string character, string language) =>
        _lEntryOutletEngine.LEngineEntry.LEngineGlyphResolve(character, language);

    public IReadOnlyList<LMeaning> LEngineMeaningRead(long ownerId, LOwner owner) =>
        _lEntryOutletEngine.LEngineCard.LEngineMeaningRead(ownerId, owner);

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineCard.LEngineTagFind(query, order);

    public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista) =>
        _lEntryOutletEngine.LEngineCard.LEngineTagFind(vista);

    public LTag LEngineTagCreate(string text) => _lEntryOutletEngine.LEngineCard.LEngineTagCreate(text);

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language) =>
        _lEntryOutletEngine.LEngineCard.LEngineRegisterFind(query, language);

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista) =>
        _lEntryOutletEngine.LEngineCard.LEngineRegisterFind(vista);

    public LRegister LEngineRegisterCreate(string name) =>
        _lEntryOutletEngine.LEngineCard.LEngineRegisterCreate(name);

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineSituation.LEngineSituationFind(query, order);

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(
        LVista vista,
        string unknown = "",
        string untitled = "") =>
        _lEntryOutletEngine.LEngineSituation.LEngineSituationFind(vista, unknown, untitled);

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(
        LVista vista,
        string unknown = "",
        string unwritten = "") =>
        _lEntryOutletEngine.LEngineExample.LEngineExampleFind(vista, unknown, unwritten);

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order) =>
        _lEntryOutletEngine.LEngineReference.LEngineReferenceFind(query, order);

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista) =>
        _lEntryOutletEngine.LEngineReference.LEngineReferenceFind(vista);

    public LReference LEngineCitationCreate(string title) =>
        _lEntryOutletEngine.LEngineReference.LEngineCitationCreate(title);

    public IReadOnlyDictionary<long, string> LEngineCitationRead() =>
        _lEntryOutletEngine.LEngineReference.LEngineCitationRead();

    public LCatalogAuthor? LEngineAuthorFind(long id) => _lEntryOutletEngine.LEngineAuthor.LEngineAuthorFind(id);

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineAuthorFind(query, except, limit);

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineAuthorFind(vista, uncredited);

    public void LEngineAuthorAbsorb(long kept, long dropped) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineAuthorAbsorb(kept, dropped);

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineFellowFind(authorId);

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineOeuvreFind(roll, oeuvre);

    public IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner)
    {
        return _lEntryOutletEngine.LEngineEntry.LEngineUsageRead(owner);
    }

    public IReadOnlyList<LUsage> LEngineUsageRead(long id, LOwner owner) =>
        _lEntryOutletEngine.LEngineEntry.LEngineUsageRead(id, owner);

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry) =>
        _lEntryOutletEngine.LEngineMarkup.LEngineMarkupFind(entry);

    public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels) =>
        _lEntryOutletEngine.LEngineVista.LEngineNameResolve(labels);

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text) =>
        _lEntryOutletEngine.LEngineDraft.LEngineMarkdownParse(text);
}
