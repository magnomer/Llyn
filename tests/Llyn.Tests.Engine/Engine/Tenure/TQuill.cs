using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuill
{
    [Fact]
    public void AuthorSet_FreshAuthor_WritesName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);

        tenure.TQuillAuthorCreate().TQuillAuthorSet("Ada");

        Assert.Equal("Ada", tenure.TTenureRead()!.LDraftAuthorName);
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
    }

    [Fact]
    public void ExampleSet_TextTypedOverUnknown_WritesKnownText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueResolve(null, true));

        tenure.TQuillExampleCreate().TQuillExampleSet("a dog");

        LExample held = tenure.TTenureRead()!.LDraftExample!;
        Assert.Equal("a dog", held.LExampleText.TStateValueShow());
        Assert.False(held.LExampleText.LStateValueUncertain);
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
    }

    [Fact]
    public void SpeakerSet_LanguagePicked_WritesLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueCreate("a cat"));

        tenure.TQuillExampleCreate().TQuillSpeakerSet("French");

        Assert.Equal("French", tenure.TTenureRead()!.LDraftExample!.LExampleLanguage);
    }

    [Fact]
    public void ReferenceSet_ReferencePicked_CitesReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterfaceState.TStateValueCreate("Bestiary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnknown,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueCreate("a cat"));

        tenure.TQuillExampleCreate().TQuillReferenceSet(reference.LReferenceId);

        Assert.Equal(
            reference.LReferenceId, tenure.TTenureRead()!.LDraftExample!.LExampleSource.TStateAnchorShow());
    }

    [Fact]
    public void GlossInsert_PlaceGiven_AddsGlossInTheGlossLanguageAtThePlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueCreate("a cat"));

        tenure.TTenureGlossInsert(0);

        LGloss added = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal(engine.TEngineGlossRead(), added.LGlossLanguage);
    }

    [Fact]
    public void GlossRemove_AddedGloss_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueCreate("a cat"));
        LQuillSentence quill = tenure.TQuillSentenceCreate();

        tenure.TTenureGlossInsert(0);
        quill.TQuillGlossRemove(Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossId);

        Assert.Empty(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
    }

    [Fact]
    public void GlossSet_TextThenLanguage_WritesBoth()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = TQuillExampleStart(engine, TInterfaceState.TStateValueCreate("a cat"));
        LQuillSentence quill = tenure.TQuillSentenceCreate();

        tenure.TTenureGlossInsert(0);
        long gloss = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossId;
        quill.TQuillGlossSet(gloss, null, "un chat");
        quill.TQuillGlossSet(gloss, "French", null);

        LGloss held = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal("French", held.LGlossLanguage);
        Assert.Equal("un chat", held.LGlossText.TStateValueShow());
    }

    [Fact]
    public void SituationSet_UnknownKindLeftEmpty_KeepsKindUnknown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectSituation, stored.LSituationId);

        LQuillSituation quill = tenure.TQuillSituationCreate();
        quill.TQuillTitleSet("Fire");
        quill.TQuillDescriptionSet(string.Empty);
        quill.TQuillKindSet(string.Empty);

        LSituation held = tenure.TTenureRead()!.LDraftSituation!;
        Assert.Equal("Fire", held.LSituationTitle.TStateValueShow());
        Assert.True(held.LSituationKind.LStateValueUncertain);
        Assert.True(held.LSituationDescription.LStateValueEmpty);
    }

    [Fact]
    public void CitationSet_SourcePicked_CitesSentence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        LTenure tenure = TQuillEntryStart(engine);
        LDraft carded = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(tenure.LTenureId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long card = carded.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        LDraft rowed = engine.TEngineRequestApply(TInterface.TSentenceAdditionCreate(tenure.LTenureId, card, 0));
        long sentence = TInterface.TRequestCardFind(rowed.LDraftContent, card).LCardDraftSentence[0].LSentenceDraftId;

        tenure.TQuillSentenceCreate().TQuillCitationSet(card, sentence, notes.LReferenceId);

        LExampleDraft? example = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(notes.LReferenceId, example?.LExampleDraftReference.LStateAnchorShown);
    }

    [Fact]
    public void TranslationRemove_CourtDeleteFails_RaisesTheFailure()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillEntryStart(engine);
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineCourtFind"] = args =>
                TInterface.TCourtCreate(1, (long)args![0]!, (long)args[1]!, "chien", "French"),
            ["LEngineCourtDelete"] = _ => throw new IOException("The court delete failed."),
        });
        LQuillChip chip = tenure.TQuillChipCreate(drafts);

        IOException failure = Assert.Throws<IOException>(() => chip.TQuillTranslationRemove(1, 2));

        Assert.Equal("The court delete failed.", failure.Message);
    }

    [Fact]
    public void ReferenceFind_StaleDraft_ThrowsTheRefusal()
    {
        using TWorkspace first = TWorkspace.TWorkspacePrepare();
        using TWorkspace second = TWorkspace.TWorkspaceCreate();
        using LEngine engine = first.TWorkspaceEngineStart();
        LTenure tenure = TQuillEntryStart(engine);
        LQuillChip chip = tenure.TQuillChipCreate(engine);
        engine.TEngineWorkspaceOpen(second.TWorkspaceFolder);

        LRefusal refusal = Assert.Throws<LRefusal>(() => chip.TQuillReferenceFind("Field"));

        Assert.Equal(LRefusal.LRefusalStale, refusal.LRefusalReason);
    }

    [Fact]
    public void ReferenceFind_EndedTenure_OffersNoneWithoutRefusing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillEntryStart(engine);
        LQuillChip chip = tenure.TQuillChipCreate(engine);
        tenure.TTenureCancel();

        LReferenceOffer offer = chip.TQuillReferenceFind("Field");

        Assert.Empty(offer.LReferenceOfferRows);
    }

    private static LTenure TQuillEntryStart(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        return engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
    }

    private static LTenure TQuillExampleStart(LEngine engine, LStateValue text)
    {
        engine.TEngineDelaySet(0);
        LExample stored = engine.TEngineExampleCreate(
            TInterfaceExample.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        return engine.TEngineTenureStart("test", LSubject.LSubjectExample, stored.LExampleId);
    }

    private static LEntry TQuillEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a small feline", [], [], [], [], [], 1)],
            []));
    }
}
