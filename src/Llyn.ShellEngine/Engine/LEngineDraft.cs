using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)
    {
        return LEntryClerkField.LMarkdownParse(text);
    }

    internal LDraft LEngineDraftStart(string origin, long? entryId)
    {
        lock (LEngineGate)
        {
            IReadOnlyList<string> languages = LEngineLanguageRead();
            string language = languages.Count > 0 ? languages[0] : string.Empty;
            return _lEngineStaff.LEngineStaffCitation.LEntryStart(origin, entryId, language);
        }
    }

    internal IReadOnlyList<LRequest> LEngineDraftPrepare(long id)
    {
        lock (LEngineGate)
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

            LGlyph? glyph = LEngineGlyphRead(draft.LEntryDraftLanguage);
            if (LEngineSchemeRead(draft.LEntryDraftLanguage) is [string scheme, ..]
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
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            return _lEngineStaff.LEngineStaffClaim.LDraftRead(id);
        }
    }

    internal IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffClaim.LDraftScan();
        }
    }

    public void LEngineDraftDelete(long id)
    {
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            _lEngineStaff.LEngineStaffClaim.LClaimClerkDelete(id);
            LEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    internal bool LEngineDraftCheck(long id)
    {
        return LEngineDraftCheck(id, out _);
    }

    internal bool LEngineDraftCheck(long id, out string? refusal)
    {
        lock (LEngineGate)
        {
            refusal = null;
            if (id == 0)
            {
                return false;
            }

            LEngineDraftValidate(id);

            LDraft? draft = _lEngineStaff.LEngineStaffClaim.LDraftRead(id);
            if (draft is null)
            {
                return false;
            }

            refusal = LClaimClerk.LDraftRefusalRead(draft);
            return LEngineDraftCheck(draft);
        }
    }

    private bool LEngineDraftCheck(LDraft draft)
    {
        return _lEngineStaff.LEngineStaffCitation.LCitationDraftCheck(draft);
    }

    internal void LEngineDraftSweep(long id)
    {
        LDraft saved;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            saved = LEngineDraftLoad(id).LDraftNormalize();
            _lEngineStaff.LEngineStaffClaim.LDraftSave(saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
    }

    internal LOutcome LEngineDraftCommit(long id)
    {
        LOutcome outcome;
        List<long> raised = [];
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            outcome = _lEngineStaff.LEngineStaffOutcome.LOutcomeClerkCommit(id, raised);
            LEngineTrove.LTroveClear(id);
        }

        foreach (long entryId in raised)
        {
            LEngineBulletinRaise(LSubject.LSubjectEntry, entryId);
            LEngineInflectionStart(entryId);
        }

        return outcome;
    }

    internal void LEngineDraftCancel(long id)
    {
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            _lEngineStaff.LEngineStaffClaim.LClaimClerkCancel(id);
            LEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, 0);
    }

    public void LEngineLeftoverSweep()
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffClaim.LClaimClerkSweep();

            foreach (LDraft draft in _lEngineStaff.LEngineStaffClaim.LDraftScan())
            {
                if (_lEngineStaff.LEngineStaffClaim.LClaimClerkHeld.Contains(draft.LDraftId)
                    || _lEngineStaff.LEngineStaffClaim.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                if (draft.LDraftEntryId <= 0)
                {
                    if (!LEngineDraftCheck(draft))
                    {
                        _lEngineStaff.LEngineStaffClaim.LClaimClerkCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (_lEngineStaff.LEngineStaffCitation.LCitationLeftoverCheck(draft))
                {
                    _lEngineStaff.LEngineStaffClaim.LClaimClerkCancel(draft.LDraftId);
                }
            }
        }
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        lock (LEngineGate)
        {
            List<LDraft> leftovers = [];
            foreach (LDraft draft in _lEngineStaff.LEngineStaffClaim.LDraftScan())
            {
                if (_lEngineStaff.LEngineStaffClaim.LClaimClerkHeld.Contains(draft.LDraftId)
                    || !LEngineDraftCheck(draft.LDraftId)
                    || _lEngineStaff.LEngineStaffClaim.LClaimForeignCheck(draft.LDraftId))
                {
                    continue;
                }

                leftovers.Add(draft);
            }

            return leftovers;
        }
    }

    private void LEngineDraftValidate(long id)
    {
        LClaimClerk.LClaimStaleValidate(LEngineDraftStale, id);
    }

    private LDraft LEngineDraftLoad(long id)
    {
        return _lEngineStaff.LEngineStaffClaim.LDraftLoad(id);
    }
}
