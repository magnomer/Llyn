using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCard
{
    private readonly LDraftPort _lDraftPort;

    private readonly LEntryPort _lEntryPort;

    private readonly LPhonologyPort _lPhonologyPort;

    public LCard(LDraftPort drafts, LEntryPort entries, LPhonologyPort phonology)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);

        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lPhonologyPort = phonology;
    }

    public IReadOnlyList<LVistaRow> LCardProspectFind(string word)
    {
        return _lDraftPort.LEngineProspectFind(word);
    }

    public LEntry? LCardTranslationResolve(string word, long? entryId)
    {
        return _lDraftPort.LEngineTranslationResolve(word, entryId);
    }

    public LCourt LCardCourtStart(long ownerId, string origin, string headword, string language)
    {
        return _lDraftPort.LEngineCourtStart(ownerId, origin, headword, language);
    }

    public LCourt? LCardCourtFind(long ownerId, long targetId)
    {
        return _lDraftPort.LEngineCourtFind(ownerId, targetId);
    }

    public void LCardCourtDelete(long linkId)
    {
        _lDraftPort.LEngineCourtDelete(linkId);
    }

    public void LCardDraftDelete(long id)
    {
        _lDraftPort.LEngineDraftDelete(id);
    }

    public IReadOnlyList<LRegister> LCardRegisterFind(string word, string language)
    {
        return _lEntryPort.LEngineRegisterFind(word, language);
    }

    public IReadOnlyList<LCatalogSituation> LCardSituationFind(string word)
    {
        return _lEntryPort.LEngineSituationFind(word, LCatalogOrder.LCatalogOrderUsage);
    }

    public IReadOnlyList<LCatalogReference> LCardReferenceFind(string word)
    {
        return _lEntryPort.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage);
    }

    public IReadOnlyList<LCatalogReference> LCardReferenceFind()
    {
        return _lEntryPort.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
    }

    public LReference LCardCitationCreate(string title)
    {
        return _lEntryPort.LEngineCitationCreate(title);
    }

    public IReadOnlyList<LTag> LCardTagFind(string word)
    {
        return _lEntryPort.LEngineTagFind(word, LCatalogOrder.LCatalogOrderUsage);
    }

    public LSentenceOrder LCardOrderRead(string language)
    {
        return _lPhonologyPort.LEngineOrderRead(language);
    }

    public IReadOnlyList<string> LCardParticleRead(string language)
    {
        return _lPhonologyPort.LEngineParticleRead(language);
    }

    public IReadOnlyList<string> LCardDependenceRead(string language)
    {
        return _lPhonologyPort.LEngineDependenceRead(language);
    }
}
