using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCard
{
    private readonly CDesk _cCardDesk;

    private readonly LReferencePort _cCardReferencePort;

    private readonly LSettingsPort _cCardSettingsPort;

    private readonly CEnvoy _cCardEnvoy;

    private readonly CLedgerNoticed _cCardNoticed;

    internal CCard(
        CDesk desk, LReferencePort references, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cCardDesk = desk;
        _cCardReferencePort = references;
        _cCardSettingsPort = settings;
        _cCardEnvoy = envoy;
        _cCardNoticed = noticed;
    }

    public CProspect CCardMentionRead(string word)
    {
        return CMention.LMentionProspectRead(_cCardDesk, word, _cCardEnvoy, _cCardSettingsPort);
    }

    internal CProspect CCardTranslationAdd(bool collocated, long cardId, string text, int position)
    {
        ArgumentNullException.ThrowIfNull(text);

        try
        {
            return _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillTranslationAdd(
                    collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning,
                    cardId,
                    text,
                    position) is LTranslationOffer offer
                ? LCardProspectRead(offer)
                : new CProspect(text, string.Empty, [], false, false, []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false, []);
        }
    }

    internal CProspect CCardTranslationResolve(
        bool collocated, long cardId, string text, int position, bool offered)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        ArgumentNullException.ThrowIfNull(text);

        if (_cCardDesk.CDeskDraft.CDeskDraftChip is not LQuillChip chip)
        {
            return new CProspect(text, string.Empty, [], false, false, []);
        }

        try
        {
            return offered
                ? LCardProspectRead(chip.LQuillTranslationFind(text))
                : new CProspect(
                    chip.LQuillTranslationResolve(kind, cardId, text, position),
                    string.Empty,
                    [],
                    false,
                    false,
                    []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
            return new CProspect(text, string.Empty, [], false, false, []);
        }
    }

    internal void CCardTranslationInsert(
        bool collocated, long cardId, long? entryId, string headword, string language, int position)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        try
        {
            if (entryId is long stored)
            {
                _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillTranslationInsert(kind, cardId, stored, position);
                return;
            }

            _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillTranslationStart(
                kind, cardId, 0, headword, language, position);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
        }
    }

    public void CCardTranslationRemove(bool collocated, long cardId, long entryId)
    {
        CCardTranslationRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, entryId);
    }

    internal void CCardTranslationRemove(LCardKind kind, long cardId, long entryId)
    {
        try
        {
            _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillTranslationRemove(kind, cardId, entryId);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Input.TranslationFailed", exception);
        }
    }

    internal static CProspect LCardProspectRead(LTranslationOffer offer)
    {
        return new CProspect(
            offer.LTranslationOfferText,
            offer.LTranslationOfferWord,
            offer.LTranslationOfferRows.Select(CCatalog.LCatalogRowRead).ToList(),
            offer.LTranslationOfferShown,
            offer.LTranslationOfferChosen,
            offer.LTranslationOfferLanguages);
    }

    internal static CRegister LCardRegisterRead(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);

        return new CRegister(register.LRegisterId, CFolio.CFolioStateRead(register.LRegisterName).CStateValueText);
    }

    public CProffer CCardReferenceFind(bool collocated, long cardId, long sentenceId, string text)
    {
        LCardKind kind = collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning;
        return _cCardDesk.LDeskRepaintRead(
            _cCardNoticed,
            () => _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillReferenceFind(
                    kind, cardId, sentenceId, text) is LReferenceOffer offer
                ? LCardProfferRead(offer)
                : new CProffer(text, [], false),
            new CProffer(text, [], false),
            ".FindFailed");
    }

    internal static CProffer LCardProfferRead(LReferenceOffer offer)
    {
        return new CProffer(
            offer.LReferenceOfferText,
            offer.LReferenceOfferRows
                .Select(static row => new CProfferRow(
                    row.LReferenceRowId,
                    row.LReferenceRowLead,
                    row.LReferenceRowMark,
                    row.LReferenceRowTail,
                    row.LReferenceRowCount))
                .ToList(),
            offer.LReferenceOfferShown);
    }

    public IReadOnlyList<CCatalogReference> CCardReferenceRead()
    {
        try
        {
            return COeuvre.COeuvreReferenceRead(_cCardReferencePort.LEngineReferenceFind());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Reference.LoadFailed", exception);
            return [];
        }
    }

    internal static CTag LCardTagRead(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);

        return new CTag(tag.LTagId, tag.LTagText);
    }

    public void CCardTagRemove(bool collocated, long cardId, long tagId)
    {
        _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillTagRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, tagId);
    }

    public void CCardSituationRemove(bool collocated, long cardId, long situationId)
    {
        _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillSituationRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, situationId);
    }

    public void CCardRegisterRemove(bool collocated, long cardId, long registerId)
    {
        _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillRegisterRemove(
            collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, registerId);
    }

    public void CCardCitationSet(bool collocated, long cardId, long sentenceId, string title)
    {
        try
        {
            _cCardDesk.CDeskDraft.CDeskDraftChip?.LQuillCitationResolve(
                collocated ? LCardKind.LCardKindCollocation : LCardKind.LCardKindMeaning, cardId, sentenceId, title);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCardEnvoy, _cCardSettingsPort, "Reference.CreateFailed", exception);
        }
    }
}
