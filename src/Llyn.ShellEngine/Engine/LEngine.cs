using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEngine : IDisposable
{
    public LVistaFacade LEngineVista { get; }
    internal LTenureFacade LEngineTenure { get; }
    public LMentionFacade LEngineMention { get; }
    internal LMarkupFacade LEngineMarkup { get; }
    internal LCourierFacade LEngineCourier { get; }
    internal LLiveryFacade LEngineLivery { get; }
    public LWorkspaceFacade LEngineWorkspace { get; }
    public LReflexFacade LEngineReflex { get; }
    internal LPortraitFacade LEnginePortrait { get; }
    public LStemFacade LEngineStem { get; }
    public LFanqieFacade LEngineFanqie { get; }
    public LLanguageFacade LEngineLanguage { get; }
    public LScriptFacade LEngineScript { get; }
    public LVocabularyFacade LEngineVocabulary { get; }
    public LPronunciationFacade LEnginePronunciation { get; }
    public LDraftFacade LEngineDraft { get; }
    internal LSettingsFacade LEngineSettings { get; }
    internal LRequestFacade LEngineRequest { get; }
    public LAuthorFacade LEngineAuthor { get; }
    public LExampleFacade LEngineExample { get; }
    public LReferenceFacade LEngineReference { get; }
    public LSituationFacade LEngineSituation { get; }
    public LCardFacade LEngineCard { get; }
    public LCatalogFacade LEngineCatalog { get; }
    public LEntryFacade LEngineEntry { get; }
    internal LEngineHearth LEngineHearth { get; }
    internal LVistaRowFacade LEngineVistaRow { get; }

    public LEngine(LRig rig, Func<string, LRig> factory, Action<string> pointer)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(pointer);

        LEngineHearth = new LEngineHearth(rig);
        LEngineVistaRow = new LVistaRowFacade(LEngineHearth);
        LEngineCatalog = new LCatalogFacade(LEngineHearth, LEngineVistaRow);
        LEngineSettings = new LSettingsFacade(LEngineHearth);
        LEngineVocabulary = new LVocabularyFacade(LEngineHearth);
        LEngineMention = new LMentionFacade(LEngineHearth);
        LEngineMarkup = new LMarkupFacade(LEngineHearth);
        LEnginePortrait = new LPortraitFacade(LEngineHearth);
        LEngineDraft = new LDraftFacade(LEngineHearth, LEngineVocabulary);
        LEnginePronunciation = new LPronunciationFacade(LEngineHearth, LEngineDraft, LEngineVistaRow);
        LEngineCard = new LCardFacade(LEngineHearth, LEnginePronunciation, LEngineSettings, LEngineVistaRow);
        LEngineEntry = new LEntryFacade(LEngineHearth, LEngineCard, LEngineDraft);
        LEngineExample = new LExampleFacade(LEngineHearth, LEngineDraft);
        LEngineReference = new LReferenceFacade(LEngineHearth, LEngineDraft);
        LEngineSituation = new LSituationFacade(LEngineHearth, LEngineDraft);
        LEngineAuthor = new LAuthorFacade(
            LEngineHearth, LEngineDraft, LEngineEntry, LEngineReference, LEngineSettings);
        LEngineWorkspace = new LWorkspaceFacade(
            LEngineHearth, LEngineDraft, LEnginePronunciation, rig, factory, pointer);
        LEngineVista = new LVistaFacade(
            LEngineHearth,
            LEngineAuthor,
            LEngineCatalog,
            LEngineEntry,
            LEngineExample,
            LEngineReference,
            LEngineSettings,
            LEngineSituation,
            LEngineWorkspace,
            LEngineVistaRow);
        LEngineFanqie = new LFanqieFacade(LEngineHearth, LEngineEntry, LEngineSettings, LEngineVistaRow);
        LEngineReflex = new LReflexFacade(LEngineHearth, LEngineFanqie, LEngineSettings);
        LEngineScript = new LScriptFacade(LEngineHearth);
        LEngineLanguage = new LLanguageFacade(
            LEngineHearth, LEngineFanqie, LEngineReflex, LEngineScript, LEngineSettings, LEngineVocabulary);
        LEngineStem = new LStemFacade(LEngineHearth, LEngineEntry, LEngineLanguage, LEngineVistaRow, LEngineReflex);
        LEngineRequest = new LRequestFacade(LEngineHearth, LEngineDraft, LEngineEntry);
        LEngineLivery = new LLiveryFacade(
            LEngineHearth,
            LEngineCard,
            LEngineCatalog,
            LEngineEntry,
            LEngineFanqie,
            LEngineLanguage,
            LEngineReference,
            LEngineReflex,
            LEngineScript,
            LEngineSettings,
            LEngineVocabulary);
        LEngineCourier = new LCourierFacade(LEngineHearth, LEngineLivery, LEngineSettings);
        LEngineTenure = new LTenureFacade(this);
        LEngineHearth.LEngineWorkspaceOpen();
    }

    public void Dispose()
    {
        LEngineHearth.LEngineStaffClear();
    }

    public void LEngineObserverAttach(Action<LBulletin> observer)
    {
        LEngineHearth.LEngineObserverAttach(observer);
    }

    public void LEngineObserverDetach(Action<LBulletin> observer)
    {
        LEngineHearth.LEngineObserverDetach(observer);
    }
}
