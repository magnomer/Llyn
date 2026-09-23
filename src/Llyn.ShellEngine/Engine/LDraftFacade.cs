using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LDraftFacade
{
    private readonly LEngine _lDraftFacadeEngine;
    private readonly object _lDraftFacadeGate;

    public LDraftFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lDraftFacadeEngine = engine;
        _lDraftFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LDraftFacadeStaff => _lDraftFacadeEngine.LEngineStaffHeld;

    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)
    {
        return LEntryClerkField.LMarkdownParse(text);
    }

    internal LDraft LEngineDraftStart(string origin, long? entryId)
    {
        lock (_lDraftFacadeGate)
        {
            IReadOnlyList<string> languages = _lDraftFacadeEngine.LEngineLanguage.LEngineLanguageRead();
            string language = languages.Count > 0 ? languages[0] : string.Empty;
            return LDraftFacadeStaff.LEngineStaffCitation.LEntryStart(origin, entryId, language);
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

            LGlyph? glyph = _lDraftFacadeEngine.LEngineEntry.LEngineGlyphRead(draft.LEntryDraftLanguage);
            if (_lDraftFacadeEngine.LEnginePronunciation
                    .LEngineSchemeRead(draft.LEntryDraftLanguage) is [string scheme, ..]
                && !LGlyph.LGlyphOtherCheck(glyph, draft.LEntryDraftTranscriptions))
            {
                requests.Add(new LRequestTranscriptionAddition(id, scheme, 0, true));
            }

            if (glyph is not null && !LGlyph.LGlyphRowCheck(glyph, draft.LEntryDraftTranscriptions))
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
            return LDraftFacadeStaff.LEngineStaffClaim.LDraftRead(id);
        }
    }

    internal IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (_lDraftFacadeGate)
        {
            return LDraftFacadeStaff.LEngineStaffClaim.LDraftScan();
        }
    }

    public void LEngineDraftDelete(long id)
    {
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkDelete(id);
            _lDraftFacadeEngine.LEngineTrove.LTroveClear(id);
        }

        _lDraftFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
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

            LDraft? draft = LDraftFacadeStaff.LEngineStaffClaim.LDraftRead(id);
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
            return LDraftFacadeStaff.LEngineStaffCitation.LCitationDraftCheck(draft);
        }
    }

    internal void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LEngineDraftLoad(id).LDraftNormalize();
            LDraftFacadeStaff.LEngineStaffClaim.LDraftSave(saved);
        }

        _lDraftFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
    }

    internal LOutcome LEngineDraftCommit(long id)
    {
        LOutcome outcome;
        List<long> raised = [];
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            outcome = LDraftFacadeStaff.LEngineStaffOutcome.LOutcomeClerkCommit(id, raised);
            _lDraftFacadeEngine.LEngineTrove.LTroveClear(id);
        }

        foreach (long entryId in raised)
        {
            _lDraftFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectEntry, entryId);
            _lDraftFacadeEngine.LEngineVocabulary.LEngineInflectionStart(entryId);
        }

        return outcome;
    }

    internal void LEngineDraftCancel(long id)
    {
        lock (_lDraftFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            if (_lDraftFacadeEngine.LEngineDraftStale.Remove(id))
            {
                return;
            }

            LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkCancel(id);
            _lDraftFacadeEngine.LEngineTrove.LTroveClear(id);
        }

        _lDraftFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    public void LEngineLeftoverSweep()
    {
        lock (_lDraftFacadeGate)
        {
            LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkSweep();

            foreach (LDraft draft in LDraftFacadeStaff.LEngineStaffClaim.LDraftScan())
            {
                if (LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkHeld.Contains(draft.LDraftId)
                    || LDraftFacadeStaff.LEngineStaffClaim.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                if (draft.LDraftEntryId <= 0)
                {
                    if (!LEngineDraftCheck(draft))
                    {
                        LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (LDraftFacadeStaff.LEngineStaffCitation.LCitationLeftoverCheck(draft))
                {
                    LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkCancel(draft.LDraftId);
                }
            }
        }
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        lock (_lDraftFacadeGate)
        {
            List<LDraft> leftovers = [];
            foreach (LDraft draft in LDraftFacadeStaff.LEngineStaffClaim.LDraftScan())
            {
                if (LDraftFacadeStaff.LEngineStaffClaim.LClaimClerkHeld.Contains(draft.LDraftId)
                    || !LEngineDraftCheck(draft.LDraftId)
                    || LDraftFacadeStaff.LEngineStaffClaim.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                leftovers.Add(draft);
            }

            return leftovers;
        }
    }

    internal void LEngineDraftValidate(long id)
    {
        lock (_lDraftFacadeGate)
        {
            LClaimClerk.LClaimStaleValidate(_lDraftFacadeEngine.LEngineDraftStale, id);
        }
    }

    internal LDraft LEngineDraftLoad(long id)
    {
        lock (_lDraftFacadeGate)
        {
            return LDraftFacadeStaff.LEngineStaffClaim.LDraftLoad(id);
        }
    }
}
