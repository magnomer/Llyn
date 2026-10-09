using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LDraftFacade : LMarkdownPort
{
    private readonly LEngineHearth _lDraftFacadeHearth;
    private readonly LVocabularyFacade _lDraftFacadeVocabulary;
    private readonly object _lDraftFacadeGate;

    internal LDraftFacade(LEngineHearth hearth, LVocabularyFacade vocabulary)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(vocabulary);
        _lDraftFacadeHearth = hearth;
        _lDraftFacadeVocabulary = vocabulary;
        _lDraftFacadeGate = _lDraftFacadeHearth.LEngineGate;
    }

    private LEngineStaff LDraftFacadeStaff => _lDraftFacadeHearth.LEngineStaffHeld;

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)
    {
        return LEntryClerkField.LMarkdownParse(text);
    }

    internal LDraft LEngineDraftStart(string origin, long? entryId)
    {
        lock (_lDraftFacadeGate)
        {
            IReadOnlyList<string> languages =
                LDraftFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageClerkRead();
            string language = languages.Count > 0 ? languages[0] : string.Empty;
            return LDraftFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LEntryStart(origin, entryId, language);
        }
    }

    internal IReadOnlyList<LRequest> LEngineDraftPrepare(long id)
    {
        lock (_lDraftFacadeGate)
        {
            LEntryDraft draft = LEngineDraftLoad(id).LDraftContent;
            List<LRequest> requests = [];
            if (!draft.LEntryDraftDefined)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindMeaning, 0, 0));
            }

            if (!draft.LEntryDraftCollocated)
            {
                requests.Add(new LRequestCardAddition(id, LCardKind.LCardKindCollocation, 0, 0));
            }

            foreach (LCardDraft card in draft.LEntryDraftMeanings)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            foreach (LCardDraft card in draft.LEntryDraftCollocations)
            {
                LEngineSentencePrepare(id, card, requests);
            }

            LLanguageStaff staff = LDraftFacadeStaff.LEngineStaffLanguage;
            LGlyph? glyph = staff.LLanguageStaffLanguage.LLanguageGlyphLoad(draft.LEntryDraftLanguage);
            LGlyphBlock block = staff.LLanguageStaffLanguage.LLanguageGlyphRead(draft);
            if (staff.LLanguageStaffTranscription.LSchemeRead(draft.LEntryDraftLanguage) is [string scheme, ..]
                && block.LGlyphBlockOther.Count == 0)
            {
                requests.Add(new LRequestTranscriptionAddition(id, scheme, 0, true));
            }

            if (glyph is not null && block.LGlyphBlockRows.Count == 0)
            {
                requests.Add(new LRequestTranscriptionAddition(
                    id, glyph.LGlyphName, draft.LEntryDraftTranscriptions.Count, true));
            }

            return requests;
        }
    }

    private static void LEngineSentencePrepare(long id, LCardDraft card, List<LRequest> requests)
    {
        if (!card.LCardDraftExemplified)
        {
            requests.Add(new LRequestSentenceAddition(id, card.LCardDraftId, 0));
        }
    }

    internal LDraft? LEngineDraftRead(long id)
    {
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            return LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LDraftRead(id);
        }
    }

    public void LEngineDraftDelete(long id)
    {
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkDelete(id);
            _lDraftFacadeHearth.LEngineTrove.LTroveClear(id);
        }

        _lDraftFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    internal bool LEngineDraftCheck(long id)
    {
        return LEngineDraftCheck(id, out _);
    }

    internal bool LEngineDraftCheck(long id, out string? refusal)
    {
        lock (_lDraftFacadeGate)
        {
            refusal = null;
            if (id == 0)
            {
                return false;
            }

            LEngineDraftValidate(id);

            LDraft? draft = LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LDraftRead(id);
            if (draft is null)
            {
                return false;
            }

            refusal = LClaimClerk.LDraftRefusalRead(draft);
            return LEngineDraftCheck(draft);
        }
    }

    internal bool LEngineDraftCheck(LDraft draft)
    {
        lock (_lDraftFacadeGate)
        {
            return LDraftFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LCitationDraftCheck(draft);
        }
    }

    internal void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LDraftSweep(id);
        }

        _lDraftFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
    }

    internal LOutcome LEngineDraftCommit(long id)
    {
        LOutcome outcome;
        List<long> raised = [];
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            outcome = LDraftFacadeStaff.LEngineStaffEntry.LEntryStaffOutcome.LOutcomeClerkCommit(id, raised);
            _lDraftFacadeHearth.LEngineTrove.LTroveClear(id);
        }

        foreach (long entryId in raised)
        {
            _lDraftFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectEntry, entryId);
            _lDraftFacadeVocabulary.LEngineInflectionStart(entryId);
        }

        return outcome;
    }

    internal void LEngineDraftCancel(long id)
    {
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            if (_lDraftFacadeHearth.LEngineDraftStale.Remove(id))
            {
                return;
            }

            LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkCancel(id);
            _lDraftFacadeHearth.LEngineTrove.LTroveClear(id);
        }

        _lDraftFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    public void LEngineLeftoverSweep()
    {
        lock (_lDraftFacadeGate)
        {
            _lDraftFacadeHearth.LEngineRevision++;
            LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkSweep();

            foreach (LDraft draft in LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LDraftScan())
            {
                if (LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkHeld.Contains(draft.LDraftId)
                    || LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                if (draft.LDraftEntryId <= 0)
                {
                    if (!LEngineDraftCheck(draft))
                    {
                        LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (LDraftFacadeStaff.LEngineStaffEntry.LEntryStaffCitation.LCitationLeftoverCheck(draft))
                {
                    LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkCancel(draft.LDraftId);
                }
            }
        }
    }

    internal void LEngineDraftValidate(long id)
    {
        lock (_lDraftFacadeGate)
        {
            LClaimClerk.LClaimStaleValidate(_lDraftFacadeHearth.LEngineDraftStale, id);
        }
    }

    internal LDraft LEngineDraftLoad(long id)
    {
        lock (_lDraftFacadeGate)
        {
            return LDraftFacadeStaff.LEngineStaffClaim.LClaimStaffClaim.LDraftLoad(id);
        }
    }
}
