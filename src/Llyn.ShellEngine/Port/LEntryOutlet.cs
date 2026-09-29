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

    public (bool, string, string) LEngineStampRead(long entryId) =>
        _lEntryOutletEngine.LEngineEntry.LEngineStampRead(entryId);

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

    public LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once) =>
        _lEntryOutletEngine.LEnginePronunciation.LEngineFrequencyResolve(entryId, once);

    public IReadOnlyList<LUsage> LEngineIncomingRead(long entryId) =>
        _lEntryOutletEngine.LEngineCard.LEngineIncomingRead(entryId);

    public IReadOnlyList<LTranslationTarget> LEngineTargetRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineCard.LEngineTargetRead(draft);

    public (IReadOnlyList<LTranslationTarget>, bool) LEngineEtymologyRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineCard.LEngineEtymologyRead(draft);

    public LMentionResult LEngineMentionFind(long exampleId, int offset) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionFind(exampleId, offset);

    public LMentionResult LEngineMentionFind(
        string text,
        string language,
        LEntryDraft? shown,
        int offset,
        IReadOnlyList<LMention>? mentions) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionFind(text, language, shown, offset, mentions);

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions) =>
        _lEntryOutletEngine.LEngineMention.LEngineMentionResolve(text, mentions);

    public LGlyph? LEngineGlyphRead(string language) => _lEntryOutletEngine.LEngineEntry.LEngineGlyphRead(language);

    public IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineEntry.LEngineGlyphRead(draft.LEntryDraftLanguage)?.LGlyphDivide(draft) ?? [];

    public LGlyph? LEngineGlyphRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineEntry.LEngineGlyphRead(draft.LEntryDraftLanguage);

    public long LEngineGlyphResolve(string character, string language) =>
        _lEntryOutletEngine.LEngineEntry.LEngineGlyphResolve(character, language).LEntryId;

    public IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineLanguage.LEngineTranscriptionRead(draft);

    public IReadOnlyList<LReflexDraft> LEngineReflexRead(LEntryDraft draft) =>
        _lEntryOutletEngine.LEngineEntry.LEngineReflexRead(draft);

    public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista) =>
        _lEntryOutletEngine.LEngineCard.LEngineTagFind(vista);

    public LTag LEngineTagCreate(string text) => _lEntryOutletEngine.LEngineCard.LEngineTagCreate(text);

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista) =>
        _lEntryOutletEngine.LEngineCard.LEngineRegisterFind(vista);

    public LRegister LEngineRegisterCreate(string name) =>
        _lEntryOutletEngine.LEngineCard.LEngineRegisterCreate(name);

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

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista) =>
        _lEntryOutletEngine.LEngineReference.LEngineReferenceFind(vista);

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind() =>
        _lEntryOutletEngine.LEngineReference.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);

    public IReadOnlyDictionary<long, string> LEngineCitationRead(LEntryDraft shown) =>
        _lEntryOutletEngine.LEngineReference.LEngineCitationRead(shown);

    public string LEngineCitationRead(LDraft? draft) =>
        _lEntryOutletEngine.LEngineReference.LEngineCitationRead(draft);

    public IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineRollFind(vista);

    public IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineUnionFind(roll, typed);

    public (string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineUnionRead(held, kept);

    public LVita LEngineVitaRead(LVista? roll) => _lEntryOutletEngine.LEngineAuthor.LEngineVitaRead(roll);

    public void LEngineAuthorAbsorb(long kept, long dropped) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineAuthorAbsorb(kept, dropped);

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre) =>
        _lEntryOutletEngine.LEngineAuthor.LEngineOeuvreFind(roll, oeuvre);

    public string LEngineWorkFormat(int count) => _lEntryOutletEngine.LEngineAuthor.LEngineWorkFormat(count);

    public LColophon LEngineColophonRead(LDraft draft) =>
        _lEntryOutletEngine.LEngineReference.LEngineColophonRead(draft);

    public LImprint LEngineImprintRead(LDraft? draft) => LReferenceFacade.LEngineImprintRead(draft);

    public IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held) => LAuthorFacade.LEngineCreditRead(held);

    public string LEngineTallyRead(long? reference) => _lEntryOutletEngine.LEngineEntry.LEngineTallyRead(reference);

    public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels) =>
        _lEntryOutletEngine.LEngineVista.LEngineNameResolve(labels);

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text) =>
        _lEntryOutletEngine.LEngineDraft.LEngineMarkdownParse(text);
}
